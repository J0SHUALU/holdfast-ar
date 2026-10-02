using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    public class ShooterEnemy : Enemy
    {
        [SerializeField] private float shootingDistance = 1.4f;
        [SerializeField] private float projectileSpeed = 2.2f;
        [SerializeField] private Transform muzzle;

        public override EnemyType Type => EnemyType.Shooter;

        protected override float StoppingDistance => shootingDistance;

        protected override void Reset()
        {
            maxHealth = 5f;
            moveSpeed = 0.3f;
            attackRange = 2.2f;
            attackDamage = 8f;
            attackCooldown = 1.6f;
            scoreValue = 150;
        }

        protected override void Awake()
        {
            base.Awake();
            if (attackRange < shootingDistance) attackRange = shootingDistance + 0.5f;
        }

        protected override void Attack(float distanceToTarget)
        {
            if (Context.EnemyProjectiles == null || Context.Target == null) return;

            PlayAttackAnimation();
            Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up * 0.3f;
            Vector3 direction = Context.Target.position - origin;
            Context.EnemyProjectiles.Fire(origin, direction, projectileSpeed, attackDamage * DamageMultiplier, Team.Enemy);
            AudioManager.Instance?.PlayAt(SoundId.EnemyShoot, origin);
        }
    }
}
