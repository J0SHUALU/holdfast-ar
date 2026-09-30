using HoldfastAR.Core;
using HoldfastAR.Pooling;
using UnityEngine;

namespace HoldfastAR.Combat
{
    /// <summary>
    /// A pooled bullet used by both the player and the Shooter enemy.
    /// Movement is swept with a sphere cast every frame so fast bullets never tunnel
    /// through small AR-scale targets.
    /// </summary>
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float radius = 0.02f;
        [SerializeField] private float lifetime = 3f;

        private static readonly RaycastHit[] HitBuffer = new RaycastHit[16];

        private ProjectilePool _owner;
        private TrailRenderer _trail;
        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private float _age;
        private Team _team;
        private bool _inFlight;

        private void Awake()
        {
            _trail = GetComponentInChildren<TrailRenderer>();
        }

        public void SetOwnerPool(ProjectilePool pool) => _owner = pool;

        public void Launch(Vector3 direction, float speed, float damage, Team team)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _team = team;
            _inFlight = true;
            transform.rotation = Quaternion.LookRotation(_direction);
        }

        private void Update()
        {
            if (!_inFlight) return;

            float step = _speed * Time.deltaTime;
            if (TryHit(step)) return;

            transform.position += _direction * step;
            _age += Time.deltaTime;
            if (_age >= lifetime) Despawn();
        }

        private bool TryHit(float distance)
        {
            int count = Physics.SphereCastNonAlloc(transform.position, radius, _direction, HitBuffer,
                distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

            IDamageable best = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                IDamageable target = HitBuffer[i].collider.GetComponentInParent<IDamageable>();
                if (target == null || target.Team == _team || !target.IsAlive) continue;
                if (HitBuffer[i].distance < bestDistance)
                {
                    bestDistance = HitBuffer[i].distance;
                    best = target;
                }
            }

            if (best == null) return false;
            best.TakeDamage(_damage);
            Despawn();
            return true;
        }

        private void Despawn()
        {
            _inFlight = false;
            if (_owner != null) _owner.Release(this);
            else gameObject.SetActive(false);
        }

        // ---- IPoolable ---------------------------------------------------

        public void OnTakenFromPool()
        {
            _age = 0f;
            _inFlight = false;
            if (_trail != null)
            {
                _trail.Clear();
                _trail.emitting = true;
            }
        }

        public void OnReturnedToPool()
        {
            _inFlight = false;
            _speed = 0f;
            _damage = 0f;
            _direction = Vector3.zero;
            if (_trail != null)
            {
                _trail.emitting = false;
                _trail.Clear();
            }
        }
    }
}
