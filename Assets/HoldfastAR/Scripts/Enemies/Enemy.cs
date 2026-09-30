using System;
using System.Collections;
using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    /// <summary>
    /// Abstract base for every enemy (Inheritance + Abstraction).
    /// Holds shared behaviour: health, chasing the player, facing, cooldown-gated attacks,
    /// hit feedback, scoring on death and clean removal. Subclasses only decide
    /// WHERE to stop (StoppingDistance) and HOW to attack (Attack) - Polymorphism.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [Header("Stats")]
        [SerializeField] protected float maxHealth = 3f;       // number of player bullets to kill
        [SerializeField] protected float moveSpeed = 0.35f;    // m/s
        [SerializeField] protected float attackRange = 0.5f;   // m, horizontal
        [SerializeField] protected float attackDamage = 10f;
        [SerializeField] protected float attackCooldown = 1.2f;
        [SerializeField] protected int scoreValue = 100;

        [Header("Presentation")]
        [SerializeField] protected Transform model;
        [SerializeField] private Color hitFlashColor = Color.white;
        [SerializeField] private float turnSpeed = 8f;

        protected EnemyContext Context;
        protected float SpeedMultiplier = 1f;
        protected float DamageMultiplier = 1f;

        private float _health;
        private float _nextAttackTime;
        private bool _dying;
        private Renderer[] _renderers;
        private MaterialPropertyBlock _block;
        private Vector3 _modelBaseScale = Vector3.one;
        private Coroutine _hitRoutine;

        /// <summary>Raised once when the enemy leaves play (killed or wiped).</summary>
        public event Action<Enemy> Removed;

        public abstract EnemyType Type { get; }
        public Team Team => Team.Enemy;
        public bool IsAlive => !_dying && _health > 0f;
        public float HealthFraction => maxHealth > 0f ? _health / maxHealth : 0f;

        /// <summary>How close the enemy walks before it stops moving.</summary>
        protected abstract float StoppingDistance { get; }

        /// <summary>Performs this enemy's attack. Only called when in range and off cooldown.</summary>
        protected abstract void Attack(float distanceToTarget);

        /// <summary>Editor hook: subclasses set their own default stats here.</summary>
        protected virtual void Reset() { }

        protected virtual void Awake()
        {
            if (model == null) model = transform;
            _renderers = GetComponentsInChildren<Renderer>();
            _block = new MaterialPropertyBlock();
            _modelBaseScale = model.localScale;
        }

        public virtual void Initialize(EnemyContext context)
        {
            Context = context;
            SpeedMultiplier = context.Difficulty != null ? context.Difficulty.enemySpeedMultiplier : 1f;
            DamageMultiplier = context.Difficulty != null ? context.Difficulty.enemyDamageMultiplier : 1f;
            _health = maxHealth;
            _nextAttackTime = Time.time + attackCooldown * 0.75f; // grace period after spawning
            StartCoroutine(SpawnRoutine());
        }

        private void Update()
        {
            if (!IsAlive || Context.Target == null) return;

            float distance = FlatDistanceToTarget();
            FaceTarget();
            if (distance > StoppingDistance) MoveTowardTarget();
            Animate(distance);

            if (distance <= attackRange && Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + attackCooldown;
                Attack(distance);
            }
        }

        // ---- Movement helpers -------------------------------------------------

        protected Vector3 FlatDirectionToTarget()
        {
            Vector3 d = Context.Target.position - transform.position;
            d.y = 0f;
            return d.sqrMagnitude > 0.0001f ? d.normalized : transform.forward;
        }

        protected float FlatDistanceToTarget()
        {
            Vector3 d = Context.Target.position - transform.position;
            d.y = 0f;
            return d.magnitude;
        }

        private void FaceTarget()
        {
            Quaternion look = Quaternion.LookRotation(FlatDirectionToTarget(), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        private void MoveTowardTarget()
        {
            // Moves along the floor plane only; the enemy keeps the height of the plane it spawned on.
            transform.position += FlatDirectionToTarget() * (moveSpeed * SpeedMultiplier * Time.deltaTime);
        }

        /// <summary>Optional per-type idle/move animation (bobbing, waddling...).</summary>
        protected virtual void Animate(float distanceToTarget) { }

        // ---- Damage -------------------------------------------------------------

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            _health -= amount;

            if (_health <= 0f)
            {
                Die();
                return;
            }

            AudioManager.Instance?.PlayAt(SoundId.EnemyHit, transform.position);
            if (_hitRoutine != null) StopCoroutine(_hitRoutine);
            _hitRoutine = StartCoroutine(HitFlashRoutine());
        }

        protected virtual void Die()
        {
            _dying = true;
            GameEvents.RaiseEnemyKilled(scoreValue, transform.position);
            AudioManager.Instance?.PlayAt(SoundId.EnemyDeath, transform.position);
            StopAllCoroutines();
            SetTint(null);
            StartCoroutine(DeathRoutine());
        }

        /// <summary>Removes the enemy immediately, without awarding score (used when a match ends).</summary>
        public void Despawn()
        {
            _dying = true;
            Destroy(gameObject);
        }

        private void OnDestroy() => Removed?.Invoke(this);

        // ---- Feedback ---------------------------------------------------------------

        private IEnumerator SpawnRoutine()
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.35f;
                float s = Mathf.SmoothStep(0f, 1f, t) * (1f + 0.15f * Mathf.Sin(t * Mathf.PI));
                model.localScale = _modelBaseScale * s;
                yield return null;
            }
            model.localScale = _modelBaseScale;
        }

        private IEnumerator HitFlashRoutine()
        {
            SetTint(hitFlashColor);
            model.localScale = _modelBaseScale * 1.15f;   // punch
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.12f;
                model.localScale = Vector3.Lerp(_modelBaseScale * 1.15f, _modelBaseScale, t);
                yield return null;
            }
            SetTint(null);
            _hitRoutine = null;
        }

        private IEnumerator DeathRoutine()
        {
            Vector3 start = model.localScale;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.25f;
                model.localScale = Vector3.Lerp(start * 1.2f, Vector3.zero, t);
                model.Rotate(0f, 720f * Time.deltaTime, 0f, Space.Self);
                yield return null;
            }
            Destroy(gameObject);
        }

        /// <summary>Tints every renderer (works with both URP Lit and Built-in Standard shaders).</summary>
        private void SetTint(Color? color)
        {
            foreach (Renderer r in _renderers)
            {
                if (r == null) continue;
                if (color.HasValue)
                {
                    r.GetPropertyBlock(_block);
                    _block.SetColor(BaseColorId, color.Value);
                    _block.SetColor(ColorId, color.Value);
                    r.SetPropertyBlock(_block);
                }
                else
                {
                    r.SetPropertyBlock(null);
                }
            }
        }
    }
}
