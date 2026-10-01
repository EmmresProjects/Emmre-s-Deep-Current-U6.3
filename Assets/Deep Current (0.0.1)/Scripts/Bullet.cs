using System.Collections.Generic;
using UnityEngine;

namespace DeepCurrent
{
    //  Bullet — Player bolt.  Same as the JS Version, straight travel at 640 u/s, 0.9 s lifetime,
    //  kills the first enemy it touches (radius enemy.r + 5), dies 60 units past the arena.
    public class Bullet : MonoBehaviour
    {
        Vector2 vel;
        float life;

        public void Setup(Vector2 pos, Vector2 v, float ang)
        {
            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, ang * Mathf.Rad2Deg);
            vel = v;
            life = DC.BulletLife;
        }

        public bool UpdateBullet(float dt, bool live, List<Enemy> enemies)
        {
            Vector2 pos = (Vector2)transform.localPosition + vel * dt;
            life -= dt;

            bool dead = life <= 0f
                || pos.x < -Arena.HalfW - 60f || pos.x > Arena.HalfW + 60f
                || pos.y < -Arena.HalfH - 60f || pos.y > Arena.HalfH + 60f;

            if (!dead && live)
            {
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var e = enemies[j];
                    float rr = e.radius + 5f;
                    Vector2 ep = e.Pos;
                    float dx = pos.x - ep.x, dy = pos.y - ep.y;
                    if (dx * dx + dy * dy < rr * rr)
                    {
                        e.TakeHit();
                        dead = true;
                        break;
                    }
                }
            }

            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            return dead;
        }
    }
}
