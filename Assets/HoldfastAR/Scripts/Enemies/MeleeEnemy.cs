using HoldfastAR.Audio;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    public class MeleeEnemy : Enemy
    {
        public override EnemyType Type => EnemyType.Melee;

        protected override float StoppingDistance => attackRange * 0.7f;

        protected override void Reset()
        {
            maxHealth = 3f;
            moveSpeed = 0.4f;
            attackRange = 0.5f;
            attackDamage = 15f;
            attackCooldown = 1.1f;
            scoreValue = 100;
        }

        protected override void Attack(float distanceToTarget)
        {
            if (distanceToTarget > attackRange || Context.TargetHealth == null) return;

            PlayAttackAnimation();
            Context.TargetHealth.TakeDamage(attackDamage * DamageMultiplier);
            AudioManager.Instance?.PlayAt(SoundId.MeleeAttack, transform.position);
        }
    }
}
