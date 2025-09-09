using UnityEngine;

namespace CustomAssets.InteractionSystem
{
    using UnityEngine;

    /// <summary>
    /// Thread-safe, persistent Singleton base class for Unity.
    /// Designed for AAA-style projects.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _shuttingDown = false;

        public static T Instance
        {
            get
            {
                if (_shuttingDown) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Try find existing instance
                        _instance = (T)FindFirstObjectByType(typeof(T));

                        // Create if not found
                        if (_instance == null)
                        {
                            var singletonObject = new GameObject(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Override Awake if needed, but always call base.Awake()
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // Kill duplicate
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _shuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _shuttingDown = true;
        }
    }
}