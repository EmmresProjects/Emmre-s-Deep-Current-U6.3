using UnityEngine;

namespace DeepCurrent
{
    // Loads a Resources prefab only when no prefab was assigned in the scene.
    public static class GamePrefabLoader
    {
        public static void LoadMissing(GameRoot game)
        {
            Load(ref game.playerPrefab, "Prefabs/Player");
            Load(ref game.bulletPrefab, "Prefabs/Bullet");
            Load(ref game.enemyBulletPrefab, "Prefabs/EnemyBullet");
            Load(ref game.jellyPrefab, "Prefabs/EnemyJelly");
            Load(ref game.anglerPrefab, "Prefabs/EnemyAngler");
            Load(ref game.spitterPrefab, "Prefabs/EnemySpitter");
            Load(ref game.elitePrefab, "Prefabs/EnemyElite");
            Load(ref game.capsulePrefab, "Prefabs/Capsule");
        }

        static void Load(ref GameObject prefab, string path)
        {
            if (prefab != null) return;

            prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
                Debug.LogWarning("[DeepCurrent] Missing prefab: " + path);
        }
    }
}
