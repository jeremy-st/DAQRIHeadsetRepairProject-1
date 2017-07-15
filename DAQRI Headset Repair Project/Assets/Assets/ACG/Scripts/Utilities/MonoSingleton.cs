using UnityEngine;

namespace DAQRI.ACG.Scripts.Utilities
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T instance = null;
        public static T Instance
        {
            get { return instance != null ? instance : (instance = FindObjectOfType(typeof(T)) as T); }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Debug.LogError("Another instance of " + GetType() + " already exist!");
                DestroyImmediate(this);
                return;
            }

            Init();
        }

        protected virtual void Init() { }

        private void OnApplicationQuit()
        {
            instance = null;
        }
    }
}