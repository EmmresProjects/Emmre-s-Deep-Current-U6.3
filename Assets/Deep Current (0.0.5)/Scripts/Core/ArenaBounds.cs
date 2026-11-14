using UnityEngine;

namespace DeepCurrent
{
    // The playable rectangle around the screen center.
    // Any script that must stay inside the screen asks this struct, so no script needs to read the camera on its own.
    public readonly struct ArenaBounds
    {
        public readonly Vector2 Half;

        public ArenaBounds(Vector2 half)
        {
            Half = half;
        }

        // Keeps a point inside the arena, leaving `margin` free space from each edge.
        public Vector2 Clamp(Vector2 p, Vector2 margin)
        {
            p.x = Mathf.Clamp(p.x, -Half.x + margin.x, Half.x - margin.x);
            p.y = Mathf.Clamp(p.y, -Half.y + margin.y, Half.y - margin.y);
            return p;
        }

        // True when a point is beyond every edge by more than `margin`.
        public bool IsOutside(Vector2 p, float margin)
        {
            return p.x < -Half.x - margin || p.x > Half.x + margin
                || p.y < -Half.y - margin || p.y > Half.y + margin;
        }

        // True when a point sits within `band` units of any edge.
        public bool IsNearEdge(Vector2 p, float band)
        {
            return Mathf.Abs(p.x) > Half.x - band || Mathf.Abs(p.y) > Half.y - band;
        }
    }
}
