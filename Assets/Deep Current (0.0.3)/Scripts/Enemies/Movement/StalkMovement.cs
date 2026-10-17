using UnityEngine;

namespace DeepCurrent
{
    // Elite-like: slow, heavy and relentless advance.
    public class StalkMovement : IMovementStrategy
    {
        const float AdvanceSpeed = 46f;

        public Vector2 ComputeVelocity(in MovementContext ctx)
        {
            Vector2 target = ctx.toPlayer * AdvanceSpeed;
            return ctx.velocity + (target - ctx.velocity) * ctx.dt;
        }
    }
}
