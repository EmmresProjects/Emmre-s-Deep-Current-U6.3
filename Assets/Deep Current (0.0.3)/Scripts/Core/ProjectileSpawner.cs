using UnityEngine;

namespace DeepCurrent
{
    // Hands out player bolts and enemy bolts from two object pools.
    // Shooting code asks this class for a bolt. Each bolt returns to its own pool when it is finished.
    public class ProjectileSpawner
    {
        const float PlayerBoltRadius = 5f;

        readonly ObjectPool playerPool;
        readonly ObjectPool enemyPool;
        readonly GameSettings settings;

        public ProjectileSpawner(GameObject playerBoltPrefab, GameObject enemyBoltPrefab,
            Transform parent, GameSettings settings)
        {
            playerPool = new ObjectPool(playerBoltPrefab, parent);
            enemyPool = new ObjectPool(enemyBoltPrefab, parent);
            this.settings = settings;
        }

        public void FirePlayerBolt(Vector2 pos, Vector2 vel, float angle)
        {
            var bolt = playerPool.Get().GetComponent<PlayerBullet>();
            bolt.Launch(pos, vel, settings.bulletLife, PlayerBoltRadius, playerPool);
            bolt.SetAngle(angle);
        }

        public void FireEnemyBolt(Vector2 pos, Vector2 vel, float radius, float life, PlayerController target)
        {
            var bolt = enemyPool.Get().GetComponent<EnemyBullet>();
            bolt.Launch(pos, vel, life, radius, enemyPool);
            bolt.SetTarget(target, settings.playerRadius, settings.enemyBulletDamage);
        }

        public void ClearAll()
        {
            playerPool.ReleaseAll();
            enemyPool.ReleaseAll();
        }
    }
}
