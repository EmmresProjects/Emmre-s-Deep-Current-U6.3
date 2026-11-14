using UnityEngine;

namespace DeepCurrent
{
    // Spawns Elites from the elite prefab.
    public class EliteFactory : EnemyFactory
    {
        public EliteFactory(GameObject prefab, Transform parent) : base(prefab, parent) { }

        public override EnemyKind Kind => EnemyKind.Elite;
    }
}
