using UnityEngine;

namespace DeepCurrent
{
    // Spawns Jellies from the jelly prefab.
    public class JellyFactory : EnemyFactory
    {
        public JellyFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }

        public override EnemyKind Kind => EnemyKind.Jelly;
    }
}
