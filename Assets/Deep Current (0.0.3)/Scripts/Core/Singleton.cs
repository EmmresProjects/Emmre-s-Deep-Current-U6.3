using UnityEngine;

namespace DeepCurrent
{
    // SINGLETON PATTERN: one shared instance of a system that any script can reach as `ClassName.Instance`.
    // The class itself owns its only instance and destroys extra copies.
    // Remember: public class GameRoot : Singleton<GameRoot> { protected override void Awake() { base.Awake(); ... } }
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[DeepCurrent] Duplicate " + typeof(T).Name + " destroyed.", gameObject);
                Destroy(gameObject);
                return;
            }
            Instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}
