using System.Collections.Generic;
using UnityEngine;

namespace DeepCurrent
{
    // Owns the enemies that are alive right now and decides where new ones appear.
    // The factories (one per EnemyKind) do the actual creating and recycling.
    // It does not score kills and does not run enemy rules. GameRoot does those.
    public class EnemySpawner
    {
        const float MinEdgeInset = 45f;
        const float MaxEdgeInset = 85f;
        const float MinDistanceFromPlayer = 160f;
        const int EdgePickTries = 20;

        readonly Dictionary<EnemyKind, EnemyFactory> factories = new Dictionary<EnemyKind, EnemyFactory>();
        readonly List<Enemy> alive = new List<Enemy>();

        public IReadOnlyList<Enemy> Alive => alive;
        public int Count => alive.Count;

        // Factory Method: one factory per enemy kind.
        // Adding an enemy = new prefab + new Enemy subclass + one factory line here.
        public EnemySpawner(GameObject jellyPrefab, GameObject anglerPrefab,
            GameObject spitterPrefab, GameObject elitePrefab, Transform parent)
        {
            Register(new JellyFactory(jellyPrefab, parent));
            Register(new AnglerFactory(anglerPrefab, parent));
            Register(new SpitterFactory(spitterPrefab, parent));
            Register(new EliteFactory(elitePrefab, parent));
        }

        void Register(EnemyFactory factory)
        {
            factories[factory.Kind] = factory;
        }

        // Spawns one enemy on a random arena edge, never too close to the player.
        public Enemy SpawnAtEdge(EnemyKind kind, int wave, Vector2 playerPos)
        {
            var e = factories[kind].Spawn(PickEdgePoint(playerPos), wave);
            alive.Add(e);
            return e;
        }

        // Removes the enemy from the alive list and returns it to its pool.
        public bool Despawn(Enemy e)
        {
            if (!alive.Remove(e)) return false;

            factories[e.Kind].Despawn(e);
            return true;
        }

        public void DespawnAll()
        {
            foreach (var e in alive) factories[e.Kind].Despawn(e);
            alive.Clear();
        }

        Vector2 PickEdgePoint(Vector2 playerPos)
        {
            var arena = CameraFitter.Instance.Bounds;
            float mx = Mathf.Clamp(arena.Half.x * 0.08f, MinEdgeInset, MaxEdgeInset);
            float my = Mathf.Clamp(arena.Half.y * 0.08f, MinEdgeInset, MaxEdgeInset);

            Vector2 p = Vector2.zero;
            for (int i = 0; i < EdgePickTries; i++)
            {
                switch (Random.Range(0, 4))
                {
                    case 0: p = new Vector2(Random.Range(-arena.Half.x + mx, arena.Half.x - mx), arena.Half.y - my); break;
                    case 1: p = new Vector2(arena.Half.x - mx, Random.Range(-arena.Half.y + my, arena.Half.y - my)); break;
                    case 2: p = new Vector2(Random.Range(-arena.Half.x + mx, arena.Half.x - mx), -arena.Half.y + my); break;
                    default: p = new Vector2(-arena.Half.x + mx, Random.Range(-arena.Half.y + my, arena.Half.y - my)); break;
                }
                if ((p - playerPos).sqrMagnitude > MinDistanceFromPlayer * MinDistanceFromPlayer) break;
            }
            return p;
        }
    }
}
