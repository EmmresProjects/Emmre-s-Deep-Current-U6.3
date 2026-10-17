using System.Collections.Generic;
using UnityEngine;

namespace DeepCurrent
{
    // It reuses GameObjects so the game does not create and destroy objects every time a bullet or enemy appears.
    // One pool holds one prefab. Whoever owns the pool gets the objects back with Release.
    public class ObjectPool
    {
        readonly GameObject prefab;
        readonly Transform parent;
        readonly Stack<GameObject> free = new Stack<GameObject>();
        readonly List<GameObject> active = new List<GameObject>();

        public ObjectPool(GameObject prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        public GameObject Get()
        {
            var go = free.Count > 0 ? free.Pop() : Object.Instantiate(prefab, parent);
            go.SetActive(true);
            active.Add(go);
            return go;
        }

        public void Release(GameObject go)
        {
            if (go == null || !active.Remove(go)) return;

            go.SetActive(false);
            free.Push(go);
        }

        // Gives back every object that is still in use (used when a run restarts).
        public void ReleaseAll()
        {
            while (active.Count > 0) Release(active[active.Count - 1]);
        }
    }
}
