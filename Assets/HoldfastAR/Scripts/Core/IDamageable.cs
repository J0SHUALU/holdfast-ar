namespace HoldfastAR.Core
{
    public interface IDamageable
    {
        Team Team { get; }
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
