using UnityEngine;

namespace DeepCurrent
{
    //  Things to remember:
    //  EnemyBullet — Ink bolts and Leviathan radial shots.
    //  Dies 80 units past the arena or after its lifetime; hits the player at r + 15 for 10 damage (i-frame gated).
    public class EnemyBullet : MonoBehaviour
    {
        Vector2 vel;
        float life;
        float radius = 5f;

        public void Setup(Vector2 pos, Vector2 v, float r, float lifeT)
        {
            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            transform.localRotation = Quaternion.identity;
            vel = v;
            radius = r;
            life = lifeT;
        }

        public bool UpdateEnemyBullet(float dt, bool live, PlayerController player)
        {
            Vector2 pos = (Vector2)transform.localPosition + vel * dt;
            life -= dt;

            bool dead = life <= 0f
                || pos.x < -Arena.HalfW - 80f || pos.x > Arena.HalfW + 80f
                || pos.y < -Arena.HalfH - 80f || pos.y > Arena.HalfH + 80f;

            if (!dead && live && !player.dead && player.iframes <= 0f)
            {
                Vector2 pp = player.Pos;
                float rr = radius + DC.PlayerRadius;
                float dx = pos.x - pp.x, dy = pos.y - pp.y;
                if (dx * dx + dy * dy < rr * rr)
                {
                    player.Damage(10f);
                    dead = true;
                }
            }

            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            return dead;
        }
    }
}
