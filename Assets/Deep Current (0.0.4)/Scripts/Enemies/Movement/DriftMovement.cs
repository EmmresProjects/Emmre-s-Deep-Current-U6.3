using UnityEngine;

namespace DeepCurrent
{
    // Jellyfish-like: slowly drifts toward the player and never moves fast.
    public class DriftMovement : IMovementStrategy
    {
        const float BaseSpeed = 14f;
        const float SpeedPerWave = 1.2f;
        const float Responsiveness = 1.2f;

        readonly float speed;

        public DriftMovement(int wave)
        {
            speed = BaseSpeed + wave * SpeedPerWave;
        }

        public Vector2 ComputeVelocity(in MovementContext ctx)
        {
            Vector2 target = ctx.toPlayer * speed;
            return ctx.velocity + (target - ctx.velocity) * (ctx.dt * Responsiveness);
        }
    }
}
