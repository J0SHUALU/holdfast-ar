using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    /// <summary>
    /// "Sentinel": blue hovering drone with a cannon. Approaches until it reaches its
    /// shooting distance, stops, and fires pooled projectiles at the player from a
    /// longer range than the melee enemy can reach. Takes 5 player bullets to destroy.
    /// </summary>
    public class ShooterEnemy : Enemy
    {
        [SerializeField] private float shootingDistance = 1.4f;
        [SerializeField] private float projectileSpeed = 2.2f;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float hoverHeight = 0.05f;

        private float _hoverPhase;
        private Vector3 _modelRest;

        public override EnemyType Type => EnemyType.Shooter;

        protected override float StoppingDistance => shootingDistance;

        protected override void Reset()
        {
            maxHealth = 5f;
            moveSpeed = 0.3f;
            attackRange = 2.2f;     // longer reach than the melee brute
            attackDamage = 8f;
            attackCooldown = 1.6f;
            scoreValue = 150;
        }

        protected override void Awake()
        {
            base.Awake();
            _modelRest = model.localPosition;
            _hoverPhase = Random.value * Mathf.PI * 2f;
            if (attackRange < shootingDistance) attackRange = shootingDistance + 0.5f;
        }

        protected override void Attack(float distanceToTarget)
        {
            if (Context.EnemyProjectiles == null || Context.Target == null) return;

            Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up * 0.2f;
            Vector3 direction = Context.Target.position - origin;
            Context.EnemyProjectiles.Fire(origin, direction, projectileSpeed, attackDamage * DamageMultiplier, Team.Enemy);
            AudioManager.Instance?.PlayAt(SoundId.EnemyShoot, origin);
        }

        protected override void Animate(float distanceToTarget)
        {
            _hoverPhase += Time.deltaTime * 3f;
            model.localPosition = _modelRest + Vector3.up * (Mathf.Sin(_hoverPhase) * hoverHeight);

            // Tilt the cannon up toward the player's phone.
            if (muzzle != null && Context.Target != null)
            {
                Vector3 toTarget = Context.Target.position - muzzle.position;
                muzzle.rotation = Quaternion.Slerp(muzzle.rotation, Quaternion.LookRotation(toTarget), 6f * Time.deltaTime);
            }
        }
    }
}
