using System.Collections;
using HoldfastAR.Audio;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    /// <summary>
    /// "Brute": red horned walker. Rushes the player and only deals damage
    /// when it is within a short melee range, with a cooldown between swings.
    /// Takes 3 player bullets to destroy (default stats).
    /// </summary>
    public class MeleeEnemy : Enemy
    {
        [SerializeField] private float lungeDistance = 0.08f;

        private float _walkCycle;

        public override EnemyType Type => EnemyType.Melee;

        // Walk right up to the player, just inside the swing range.
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
            // Close-proximity check: melee never hurts from further away than attackRange.
            if (distanceToTarget > attackRange || Context.TargetHealth == null) return;

            Context.TargetHealth.TakeDamage(attackDamage * DamageMultiplier);
            AudioManager.Instance?.PlayAt(SoundId.MeleeAttack, transform.position);
            StartCoroutine(LungeRoutine());
        }

        protected override void Animate(float distanceToTarget)
        {
            // Side-to-side waddle while walking.
            _walkCycle += Time.deltaTime * 10f * SpeedMultiplier;
            float waddle = distanceToTarget > StoppingDistance ? Mathf.Sin(_walkCycle) * 8f : 0f;
            model.localRotation = Quaternion.Euler(0f, 0f, waddle);
        }

        private IEnumerator LungeRoutine()
        {
            Vector3 rest = model.localPosition;
            Vector3 forward = rest + Vector3.forward * lungeDistance;
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.08f)
            {
                model.localPosition = Vector3.Lerp(rest, forward, t);
                yield return null;
            }
            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.2f)
            {
                model.localPosition = Vector3.Lerp(forward, rest, t);
                yield return null;
            }
            model.localPosition = rest;
        }
    }
}
