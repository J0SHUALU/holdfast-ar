using System;
using System.Collections;
using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    [RequireComponent(typeof(Collider))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int AttackParam = Animator.StringToHash("Attack");
        private static readonly int HitParam = Animator.StringToHash("Hit");
        private static readonly int DieParam = Animator.StringToHash("Die");

        [Header("Stats")]
        [SerializeField] protected float maxHealth = 3f;
        [SerializeField] protected float moveSpeed = 0.35f;
        [SerializeField] protected float attackRange = 0.5f;
        [SerializeField] protected float attackDamage = 10f;
        [SerializeField] protected float attackCooldown = 1.2f;
        [SerializeField] protected int scoreValue = 100;

        [Header("Presentation")]
        [SerializeField] protected Transform model;
        [SerializeField] protected Animator animator;
        [SerializeField] private Color hitFlashColor = Color.white;
        [SerializeField] private float turnSpeed = 8f;
        [SerializeField] private float deathAnimationTime = 0.7f;

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

        public event Action<Enemy> Removed;

        public abstract EnemyType Type { get; }
        public Team Team => Team.Enemy;
        public bool IsAlive => !_dying && _health > 0f;
        public float HealthFraction => maxHealth > 0f ? _health / maxHealth : 0f;

        protected abstract float StoppingDistance { get; }

        protected abstract void Attack(float distanceToTarget);

        protected virtual void Reset() { }

        protected virtual void Awake()
        {
            if (model == null) model = transform;
            if (animator == null) animator = GetComponentInChildren<Animator>();
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
            _nextAttackTime = Time.time + attackCooldown * 0.75f;
            if (animator != null) animator.speed = Mathf.Max(0.5f, SpeedMultiplier);
            StartCoroutine(SpawnRoutine());
        }

        private void Update()
        {
            if (!IsAlive || Context.Target == null) return;

            float distance = FlatDistanceToTarget();
            FaceTarget();
            bool moving = distance > StoppingDistance;
            if (moving) MoveTowardTarget();
            if (animator != null) animator.SetFloat(SpeedParam, moving ? 1f : 0f);
            Animate(distance);

            if (distance <= attackRange && Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + attackCooldown;
                Attack(distance);
            }
        }

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
            transform.position += FlatDirectionToTarget() * (moveSpeed * SpeedMultiplier * Time.deltaTime);
        }

        protected virtual void Animate(float distanceToTarget) { }

        protected void PlayAttackAnimation()
        {
            if (animator != null) animator.SetTrigger(AttackParam);
        }

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
            if (animator != null) animator.SetTrigger(HitParam);
            if (_hitRoutine != null) StopCoroutine(_hitRoutine);
            _hitRoutine = StartCoroutine(HitFlashRoutine());
        }

        protected virtual void Die()
        {
            GameEvents.RaiseEnemyKilled(scoreValue, transform.position);
            BeginDeath();
        }

        protected void SelfDestruct() => BeginDeath();

        private void BeginDeath()
        {
            _dying = true;
            AudioManager.Instance?.PlayAt(SoundId.EnemyDeath, transform.position);
            StopAllCoroutines();
            SetTint(null);
            model.localScale = _modelBaseScale;
            foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
            if (animator != null)
            {
                animator.speed = 1f;
                animator.SetFloat(SpeedParam, 0f);
                animator.SetTrigger(DieParam);
            }
            StartCoroutine(DeathRoutine());
        }

        public void Despawn()
        {
            _dying = true;
            Destroy(gameObject);
        }

        private void OnDestroy() => Removed?.Invoke(this);

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
            model.localScale = _modelBaseScale * 1.12f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.12f;
                model.localScale = Vector3.Lerp(_modelBaseScale * 1.12f, _modelBaseScale, t);
                yield return null;
            }
            SetTint(null);
            _hitRoutine = null;
        }

        private IEnumerator DeathRoutine()
        {
            if (animator != null) yield return new WaitForSeconds(deathAnimationTime);
            Vector3 start = model.localScale;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.25f;
                model.localScale = Vector3.Lerp(start, Vector3.zero, t);
                yield return null;
            }
            Destroy(gameObject);
        }

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
