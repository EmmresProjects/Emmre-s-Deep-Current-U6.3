namespace DeepCurrent
{
    // Strategy Pattern: The common contract for every movement behavior.
    // A movement is an interchangeable object.
    // Any enemy can use any strategy, and an enemy can swap strategies in the middle of a wave.
    // Strategies only compute velocity. They never move the enemy themselves.
    public interface IMovementStrategy
    {
        UnityEngine.Vector2 ComputeVelocity(in MovementContext ctx);
    }
}
