using UnityEngine;
using Random = UnityEngine.Random;

namespace DeepCurrent
{
    // Owns the capsules on the field.
    // A kill can drop one: elites always drop, other kills roll the drop chance.
    // Capsules reuse each other through one object pool.
    public class CapsuleSpawner
    {
        static readonly CapsuleKind[] Kinds =
        {
            CapsuleKind.Repair, CapsuleKind.Shield, CapsuleKind.Rapid, CapsuleKind.Spread
        };

        readonly ObjectPool pool;
        readonly Sprite[] sprites = new Sprite[Kinds.Length];
        readonly GameSettings settings;

        public CapsuleSpawner(GameObject prefab, Transform parent, GameSettings settings)
        {
            this.settings = settings;
            pool = new ObjectPool(prefab, parent);
            for (int i = 0; i < Kinds.Length; i++)
            {
                sprites[i] = Resources.Load<Sprite>(SpritePath(Kinds[i]));
                if (sprites[i] == null)
                    Debug.LogWarning("[DeepCurrent] Missing sprite: " + SpritePath(Kinds[i]));
            }
        }

        static string SpritePath(CapsuleKind kind)
        {
            switch (kind)
            {
                case CapsuleKind.Shield: return "Sprites/spr_capsule_shield";
                case CapsuleKind.Rapid:  return "Sprites/spr_capsule_rapid";
                case CapsuleKind.Spread: return "Sprites/spr_capsule_spread";
                default:                 return "Sprites/spr_capsule_repair";
            }
        }

        // Rolls the drop table for a kill and spawns a capsule when the roll hits.
        public void TryDrop(Vector2 pos, bool fromElite)
        {
            if (!fromElite && Random.value > settings.capsuleDropChance) return;

            var kind = Kinds[Random.Range(0, Kinds.Length)];
            var capsule = pool.Get().GetComponent<SalvageCapsule>();
            capsule.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            capsule.Setup(kind, sprites[(int)kind], settings.capsuleLife, pool);
        }

        // Gives every live capsule back to the pool (used when a run restarts).
        public void ClearAll()
        {
            pool.ReleaseAll();
        }
    }
}
