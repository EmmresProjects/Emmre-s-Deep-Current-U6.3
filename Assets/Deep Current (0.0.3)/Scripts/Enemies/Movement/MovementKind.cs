namespace DeepCurrent
{
    // The four movement behaviors any enemy can use.
    public enum MovementKind
    {
        Drift,   // slow homing wobble (jellyfish-like)
        Pursue,  // fast chase with strafing (angler-like)
        Kite,    // keeps its distance and circles (spitter-like)
        Stalk    // slow relentless advance (elite-like)
    }
}
