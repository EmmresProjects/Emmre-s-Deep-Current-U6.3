using UnityEngine;

namespace DeepCurrent
{
    // Everything a movement strategy needs to decide one frame of velocity.
    // The enemy fills in where it is and where the player is.
    // EnemyMovement fills in the current velocity and the time before it calls the strategy.
    public struct MovementContext
    {
        public Vector2 position;
        public Vector2 toPlayer;     // normalized direction to the player
        public float distance;       // distance to the player
        public float time;           // game time in seconds
        public float dt;             // length of this frame
        public Vector2 velocity;     // velocity from the previous frame
        public ArenaBounds arena;    // the playable area
        public int wave;             // the current wave number
        public float bodyRadius;     // size of the enemy, used to keep it off the walls
    }
}
