using UnityEngine;

namespace DeepCurrent
{
    // Spawns Anglers from the angler prefab.
    public class AnglerFactory : EnemyFactory
    {
        public AnglerFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }

        public override EnemyKind Kind => EnemyKind.Angler;
    }
}
