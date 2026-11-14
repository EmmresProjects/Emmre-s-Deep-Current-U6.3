using UnityEngine;

namespace DeepCurrent
{
    // Elite: It's a tanky stalker that spins and erupts in radial bullet bursts.
    // It decides WHEN to burst and WHERE the shards start. ProjectileSpawner creates them.
    public class EnemyElite : Enemy
    {
        const float SpinRate = 0.5f;
        const float CoreCounterSpin = 2f;
        const float BurstOffset = 36f;
        const float ShardSpeed = 165f;
        const float ShardRadius = 5f;
        const float ShardLife = 5f;

        [Header("Elite attack")]
        public float burstInterval = 3.4f;
        public int burstShots = 12;

        float spin;
        float burstTimer;

        public override EnemyKind Kind => EnemyKind.Elite;

        // Elites grow with the wave: base hp + one per wave.
        protected override int ExtraHpPerWave(int wave) => wave;

        public override void OnSpawn(int wave)
        {
            base.OnSpawn(wave);
            spin = 0f;
            burstTimer = burstInterval;
        }

        protected override void Attack(GameRoot game, float dt)
        {
            spin += dt * SpinRate;
            burstTimer -= dt;
            if (burstTimer > 0f) return;

            burstTimer = burstInterval;
            for (int a = 0; a < burstShots; a++)
            {
                float angle = a / (float)burstShots * Mathf.PI * 2f + spin;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                game.Projectiles.FireEnemyBolt(Pos + dir * BurstOffset, dir * ShardSpeed, ShardRadius, ShardLife, game.Player);
            }
        }

        protected override void UpdateVisuals(float dt)
        {
            // The whole hull slowly rotates. The inner Core child turns the other way for a simple mechanical look.
            transform.localRotation = Quaternion.Euler(0f, 0f, spin * Mathf.Rad2Deg);
            var core = transform.childCount > 0 ? transform.GetChild(0) : null;
            if (core != null) core.localRotation = Quaternion.Euler(0f, 0f, -spin * CoreCounterSpin * Mathf.Rad2Deg);
        }
    }
}
