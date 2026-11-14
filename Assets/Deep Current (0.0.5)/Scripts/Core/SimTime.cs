using UnityEngine;

namespace DeepCurrent
{
    // One shared cap for the physics step.
    // Long frames are cut short, so a lag spike does not make bolts tunnel through enemies.
    public static class SimTime
    {
        public const float MaxStep = 0.05f;

        public static float Delta => Mathf.Min(Time.deltaTime, MaxStep);
    }
}
