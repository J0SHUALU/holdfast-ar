namespace HoldfastAR.Core
{
    /// <summary>
    /// Anything that can be hit by a projectile or melee attack.
    /// Projectiles only know this abstraction, never the concrete Player/Enemy classes.
    /// </summary>
    public interface IDamageable
    {
        Team Team { get; }
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
