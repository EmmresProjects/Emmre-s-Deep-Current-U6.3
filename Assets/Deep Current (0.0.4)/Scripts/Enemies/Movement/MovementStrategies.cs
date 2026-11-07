namespace DeepCurrent
{
    // Simple Factory: It creates the strategy object that matches a MovementKind.
    // Callers ask for a kind and never write `new` for a strategy themselves.
    public static class MovementStrategies
    {
        public static IMovementStrategy Create(MovementKind kind, int wave, float phase)
        {
            switch (kind)
            {
                case MovementKind.Pursue: return new PursueMovement(wave, phase);
                case MovementKind.Kite:   return new KiteMovement(phase);
                case MovementKind.Stalk:  return new StalkMovement();
                default:                  return new DriftMovement(wave);
            }
        }
    }
}
