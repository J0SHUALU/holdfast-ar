using HoldfastAR.Core;
using HoldfastAR.Pooling;
using UnityEngine;

namespace HoldfastAR.Combat
{
    /// <summary>
    /// Scene component that owns one ObjectPool of projectiles.
    /// The scene has two of these: one for player bullets and one for Shooter enemy bullets.
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile prefab;
        [SerializeField] private int initialSize = 40;

        private ObjectPool<Projectile> _pool;

        public int ActiveCount => _pool?.CountActive ?? 0;

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError($"{name}: no projectile prefab assigned.", this);
                return;
            }

            _pool = new ObjectPool<Projectile>(prefab, initialSize, transform);
            // Every instance learns which pool to return to once, up front.
            foreach (Projectile p in GetComponentsInChildren<Projectile>(true)) p.SetOwnerPool(this);
        }

        public Projectile Fire(Vector3 origin, Vector3 direction, float speed, float damage, Team team)
        {
            if (_pool == null) return null;
            Projectile p = _pool.Get(origin, Quaternion.LookRotation(direction));
            p.Launch(direction, speed, damage, team);
            return p;
        }

        public void Release(Projectile projectile) => _pool?.Release(projectile);

        public void ReleaseAll() => _pool?.ReleaseAll();
    }
}
