using UnityEngine;

namespace HoldfastAR.Core
{
    /// <summary>
    /// Generic MonoBehaviour singleton (Singleton pattern).
    /// Derived managers (GameManager, AudioManager) get a single global access point
    /// while keeping their own state encapsulated.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
