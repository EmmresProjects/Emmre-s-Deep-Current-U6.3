using UnityEngine;

namespace DeepCurrent
{
    // Angler-like: fast chase with a sideways sine strafe.
    public class PursueMovement : IMovementStrategy
    {
        const float BaseSpeed = 150f;
        const float SpeedPerWave = 8f;
        const float MaxSpeed = 260f;
        const float StrafeRate = 2f;
        const float StrafeWidth = 40f;
        const float StrafeShare = 0.4f;
        const float Responsiveness = 2.6f;

        readonly float speed;
        readonly float phase;

        public PursueMovement(int wave, float phase)
        {
            speed = Mathf.Min(BaseSpeed + wave * SpeedPerWave, MaxSpeed);
            this.phase = phase;
        }

        public Vector2 ComputeVelocity(in MovementContext ctx)
        {
            float strafe = Mathf.Sin(ctx.time * StrafeRate + phase) * StrafeWidth;
            Vector2 side = new Vector2(-ctx.toPlayer.y, ctx.toPlayer.x);
            Vector2 target = ctx.toPlayer * speed + side * (strafe * StrafeShare);

            float k = 1f - Mathf.Exp(-Responsiveness * ctx.dt);
            return ctx.velocity + (target - ctx.velocity) * k;
        }
    }
}
