using System.Collections.Generic;
using UnityEngine;

namespace DeepCurrent
{
    // Each instance makes objects from one prefab and keeps them for later use.
    // GameRoot chooses which of these prefab factories to use for an enemy.

    public class GOPool
    {
        readonly GameObject prefab;
        readonly Transform parent;
        readonly string label;
        readonly Stack<GameObject> free = new Stack<GameObject>();
        public int ActiveCount { get; private set; }

        public GOPool(GameObject prefab, Transform parent, string label, int prewarm)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.label = label;
            for (int i = 0; i < prewarm; i++)
            {
                var go = Object.Instantiate(prefab, parent);
                go.name = label + "_" + i;
                go.SetActive(false);
                free.Push(go);
            }
        }

        public GameObject Get()
        {
            GameObject go;
            if (free.Count > 0) go = free.Pop();
            else
            {
                go = Object.Instantiate(prefab, parent);
                go.name = label;
                if (go.GetComponent<Enemy>() != null)
                    Debug.Log("[Factory] Created new enemy object: " + label);
            }
            go.SetActive(true);
            ActiveCount++;
            return go;
        }

        public void Release(GameObject go)
        {
            if (go == null) return;
            go.SetActive(false);
            free.Push(go);
            ActiveCount--;
        }
    }
}
