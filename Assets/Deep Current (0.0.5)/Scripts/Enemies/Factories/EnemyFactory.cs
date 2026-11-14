using UnityEngine;

namespace DeepCurrent
{
    // Factory Method: the base class knows HOW to spawn an enemy (pool and setup).
    // Each concrete factory knows what it spawns, which is its EnemyKind and prefab.
    // If you want to add an enemy, add one small subclass and one prefab.
    public abstract class EnemyFactory
    {
        readonly ObjectPool pool;

        public abstract EnemyKind Kind { get; }

        protected EnemyFactory(GameObject prefab, Transform parent)
        {
            pool = new ObjectPool(prefab, parent);
        }

        public Enemy Spawn(Vector2 pos, int wave)
        {
            var go = pool.Get();
            var e = go.GetComponent<Enemy>();
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            e.OnSpawn(wave);
            return e;
        }

        public void Despawn(Enemy e)
        {
            pool.Release(e.gameObject);
        }
    }
}
