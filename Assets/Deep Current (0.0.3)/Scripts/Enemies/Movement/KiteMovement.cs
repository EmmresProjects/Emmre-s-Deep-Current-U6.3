using UnityEngine;

namespace DeepCurrent
{
    // Spitter-like: approaches until close enough, then backs off and circles, staying near its range so it can shoot from a safe spot.
    public class KiteMovement : IMovementStrategy
    {
        const float PreferredDistance = 300f;
        const float EdgeBand = 75f;
        const float ApproachSpeed = 110f;
        const float StrafeSpeed = 70f;
        const float StrafeRate = 0.9f;
        const float StrafeAmount = 0.9f;
        const float Responsiveness = 3f;

        readonly float phase;

        public KiteMovement(float phase)
        {
            this.phase = phase;
        }

        public Vector2 ComputeVelocity(in MovementContext ctx)
        {
            // Move toward the player when far away or near a wall, otherwise back off.
            bool moveIn = ctx.distance > PreferredDistance || ctx.arena.IsNearEdge(ctx.position, EdgeBand);
            float want = moveIn ? 1f : -1f;
            float strafe = Mathf.Sin(ctx.time * StrafeRate + phase) * StrafeAmount;

            Vector2 side = new Vector2(-ctx.toPlayer.y, ctx.toPlayer.x);
            Vector2 target = ctx.toPlayer * (ApproachSpeed * want) + side * (StrafeSpeed * strafe);

            float k = 1f - Mathf.Exp(-Responsiveness * ctx.dt);
            return ctx.velocity + (target - ctx.velocity) * k;
        }
    }
}
