using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Player
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float invulnerabilityAfterHit = 0.25f;

        private float _lastHitTime = -10f;

        public Team Team => Team.Player;
        public float MaxHealth { get; private set; } = 100f;
        public float Current { get; private set; } = 100f;
        public bool IsAlive => Current > 0f;
        public bool Invincible { get; set; } = true;

        private void Reset()
        {
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.15f;
        }

        public void ResetHealth(float max)
        {
            MaxHealth = max;
            Current = max;
            Invincible = false;
            GameEvents.RaisePlayerHealthChanged(Current, MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (Invincible || !IsAlive) return;
            if (Time.time - _lastHitTime < invulnerabilityAfterHit) return;
            _lastHitTime = Time.time;

            Current = Mathf.Max(0f, Current - amount);
            GameEvents.RaisePlayerHealthChanged(Current, MaxHealth);
            GameEvents.RaisePlayerDamaged(amount);

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
            if (Current > 0f)
            {
                AudioManager.Instance?.Play(SoundId.PlayerHurt, 0.08f);
                return;
            }

            Invincible = true;
            AudioManager.Instance?.Play(SoundId.PlayerDeath);
            GameEvents.RaisePlayerDied();
        }
    }
}
