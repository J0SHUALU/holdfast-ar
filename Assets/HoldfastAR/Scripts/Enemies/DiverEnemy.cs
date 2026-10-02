using HoldfastAR.Audio;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    public class DiverEnemy : Enemy
    {
        [SerializeField] private float cruiseHeight = 0.35f;
        [SerializeField] private float diveStartDistance = 0.9f;
        [SerializeField] private float targetHeightOffset = -0.12f;
        [SerializeField] private float bobAmplitude = 0.04f;
        [SerializeField] private float climbSpeed = 1.2f;

        private float _groundY;
        private float _bobPhase;

        public override EnemyType Type => EnemyType.Diver;

        protected override float StoppingDistance => 0f;

        protected override void Reset()
        {
            maxHealth = 1f;
            moveSpeed = 0.7f;
            attackRange = 0.22f;
            attackDamage = 20f;
            attackCooldown = 0.2f;
            scoreValue = 75;
        }

        public override void Initialize(EnemyContext context)
        {
            _groundY = transform.position.y;
            _bobPhase = Random.value * Mathf.PI * 2f;
            transform.position += Vector3.up * cruiseHeight;
            base.Initialize(context);
        }

        protected override void Animate(float distanceToTarget)
        {
            float cruise = _groundY + cruiseHeight + Mathf.Sin(Time.time * 4f + _bobPhase) * bobAmplitude;
            float strike = Context.Target.position.y + targetHeightOffset;
            float dive = Mathf.InverseLerp(diveStartDistance, attackRange, distanceToTarget);
            float desired = Mathf.Lerp(cruise, strike, dive);

            Vector3 p = transform.position;
            p.y = Mathf.MoveTowards(p.y, desired, climbSpeed * SpeedMultiplier * Time.deltaTime);
            transform.position = p;
        }

        protected override void Attack(float distanceToTarget)
        {
            if (distanceToTarget > attackRange || Context.TargetHealth == null) return;

            PlayAttackAnimation();
            Context.TargetHealth.TakeDamage(attackDamage * DamageMultiplier);
            AudioManager.Instance?.PlayAt(SoundId.MeleeAttack, transform.position);
            SelfDestruct();
        }
    }
}
