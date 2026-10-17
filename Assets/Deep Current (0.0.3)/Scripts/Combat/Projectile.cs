using UnityEngine;

namespace DeepCurrent
{
    // Base class for every bolt. It handles straight travel, lifetime, leaving the arena and returning to its own pool.
    // Subclasses only decide what they hit (OnStep).
    public abstract class Projectile : MonoBehaviour
    {
        const float DefaultExitMargin = 60f;
        const float MaxFrameTime = 0.05f;

        protected Vector2 Velocity;
        protected float LifeLeft;
        protected float Radius = 5f;
        ObjectPool home;

        // How far outside the screen the bolt may fly before it is removed.
        protected virtual float ExitMargin => DefaultExitMargin;

        public void Launch(Vector2 pos, Vector2 vel, float life, float radius, ObjectPool owner)
        {
            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            Velocity = vel;
            LifeLeft = life;
            Radius = radius;
            home = owner;
        }

        void Update()
        {
            float dt = Mathf.Min(Time.deltaTime, MaxFrameTime);
            Vector2 pos = (Vector2)transform.localPosition + Velocity * dt;
            LifeLeft -= dt;

            bool done = LifeLeft <= 0f || CameraFitter.Instance.Bounds.IsOutside(pos, ExitMargin);
            if (!done) done = OnStep(pos);

            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            if (done) home.Release(gameObject);
        }

        // Return true when this bolt should disappear this frame.
        protected abstract bool OnStep(Vector2 pos);
    }
}
