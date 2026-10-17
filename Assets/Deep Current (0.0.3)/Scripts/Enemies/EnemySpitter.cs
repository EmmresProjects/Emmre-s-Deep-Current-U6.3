using UnityEngine;

namespace DeepCurrent
{
    // Spitter: keeps its distance and fires aimed ink bolts.
    // It only decides when to shoot and where the bolt starts. The bolt itself is created by ProjectileSpawner.
    public class EnemySpitter : Enemy
    {
        const float FirstShotMin = 1f;
        const float FirstShotMax = 2f;
        const float ReloadMin = 1.4f;
        const float ReloadRandom = 0.9f;
        const float MuzzleOffset = 14f;
        const float BoltSpeed = 240f;
        const float BoltRadius = 5f;
        const float BoltLife = 4f;

        [Header("Spitter attack")]
        public float shotRange = 540f;

        float reloadTimer;

        public override EnemyKind Kind => EnemyKind.Spitter;

        public override void OnSpawn(int wave)
        {
            base.OnSpawn(wave);
            reloadTimer = Random.Range(FirstShotMin, FirstShotMax);
        }

        protected override void Attack(GameRoot game, float dt)
        {
            reloadTimer -= dt;
            if (reloadTimer > 0f || DistanceToPlayer >= shotRange) return;

            reloadTimer = ReloadMin + Random.value * ReloadRandom;
            game.Projectiles.FireEnemyBolt(Pos + ToPlayer * MuzzleOffset, ToPlayer * BoltSpeed, BoltRadius, BoltLife, game.Player);
        }
    }
}
