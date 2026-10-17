using UnityEngine;
using Random = UnityEngine.Random;

namespace DeepCurrent
{
    // The movement component of one enemy.
    // It holds the current strategy (STRATEGY PATTERN).
    // It decides when a random swap happens, and moves the body while keeping it inside the arena.
    // It does not know which enemy owns it, what that enemy attacks, or how much health it has.
    // The enemy only calls Step, Coast or Knockback.
    public class EnemyMovement
    {
        // Free space kept between the body and the arena edge, on top of the body radius.
        const float EdgeMargin = 15f;

        // The margin never takes more than this share of the arena, so small arenas still work.
        const float MaxEdgeShare = 0.35f;

        IMovementStrategy strategy;
        MovementKind kind;
        float phase;
        float switchInterval;
        float switchTimer;
        int swapFromWave;

        public Vector2 Velocity { get; private set; }
        public MovementKind Kind => kind;

        // Call once when the enemy leaves the pool.
        public void Setup(MovementKind startKind, float interval, int wave, int swapWave)
        {
            phase = Random.Range(0f, Mathf.PI * 2f);
            switchInterval = interval;
            switchTimer = interval;
            swapFromWave = swapWave;
            Velocity = Vector2.zero;
            UseKind(startKind, wave);
        }

        // Works out the new velocity for this frame and returns the new position.
        public Vector2 Step(MovementContext ctx)
        {
            TrySwapKind(ctx);

            ctx.time = Time.time;
            ctx.velocity = Velocity;
            Velocity = strategy.ComputeVelocity(ctx);

            return Integrate(ctx.dt, ctx.position, ctx.arena, ctx.bodyRadius);
        }

        // Keeps sliding with the current velocity. Used when the player is down.
        public Vector2 Coast(float dt, Vector2 pos, ArenaBounds arena, float bodyRadius)
        {
            return Integrate(dt, pos, arena, bodyRadius);
        }

        // Replaces the velocity with a push, for example after a contact hit.
        public void Knockback(Vector2 velocity)
        {
            Velocity = velocity;
        }

        // Random swap on later waves: the same enemy becomes harder to predict.
        void TrySwapKind(MovementContext ctx)
        {
            if (switchInterval <= 0f) return;
            if (ctx.wave < swapFromWave) return;

            switchTimer -= ctx.dt;
            if (switchTimer > 0f) return;

            switchTimer = switchInterval * Random.Range(0.75f, 1.25f);
            var kinds = (MovementKind[])System.Enum.GetValues(typeof(MovementKind));
            UseKind(kinds[Random.Range(0, kinds.Length)], ctx.wave);
        }

        void UseKind(MovementKind next, int wave)
        {
            kind = next;
            strategy = MovementStrategies.Create(next, wave, phase);
        }

        Vector2 Integrate(float dt, Vector2 pos, ArenaBounds arena, float bodyRadius)
        {
            Vector2 next = pos + Velocity * dt;

            float reach = bodyRadius + EdgeMargin;
            var margin = new Vector2(
                Mathf.Min(reach, arena.Half.x * 2f * MaxEdgeShare),
                Mathf.Min(reach, arena.Half.y * 2f * MaxEdgeShare));
            return arena.Clamp(next, margin);
        }
    }
}
