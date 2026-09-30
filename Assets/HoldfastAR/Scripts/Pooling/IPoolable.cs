namespace HoldfastAR.Pooling
{
    /// <summary>Contract for objects that live inside an ObjectPool.</summary>
    public interface IPoolable
    {
        /// <summary>Called when the object is taken out of the pool. Reset all runtime state here.</summary>
        void OnTakenFromPool();

        /// <summary>Called when the object goes back into the pool. Stop effects, clear references.</summary>
        void OnReturnedToPool();
    }
}
