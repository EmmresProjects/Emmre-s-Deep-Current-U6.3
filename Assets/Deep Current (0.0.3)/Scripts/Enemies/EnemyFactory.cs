using UnityEngine;

namespace DeepCurrent
{
    // Factory Method: The base class knows HOW to spawn an enemy (pool and setup).
    // Each concrete factory knows what it spawns, which is its EnemyKind and prefab.
    // If you want to add an enemy, add a small subclass + one prefab.
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

    public class JellyFactory : EnemyFactory
    {
        public JellyFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }
        public override EnemyKind Kind => EnemyKind.Jelly;
    }

    public class AnglerFactory : EnemyFactory
    {
        public AnglerFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }
        public override EnemyKind Kind => EnemyKind.Angler;
    }

    public class SpitterFactory : EnemyFactory
    {
        public SpitterFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }
        public override EnemyKind Kind => EnemyKind.Spitter;
    }

    public class EliteFactory : EnemyFactory
    {
        public EliteFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }
        public override EnemyKind Kind => EnemyKind.Elite;
    }
}
