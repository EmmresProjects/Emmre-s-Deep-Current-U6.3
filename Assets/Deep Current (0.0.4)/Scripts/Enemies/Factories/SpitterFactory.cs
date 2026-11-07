using UnityEngine;

namespace DeepCurrent
{
    // Spawns Spitters from the spitter prefab.
    public class SpitterFactory : EnemyFactory
    {
        public SpitterFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }

        public override EnemyKind Kind => EnemyKind.Spitter;
    }
}
