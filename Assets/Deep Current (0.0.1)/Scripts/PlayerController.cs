using UnityEngine;

namespace DeepCurrent
{
    // Movement, aim, one shot at a time, health and death.
    public class PlayerController : MonoBehaviour
    {
        public float hp = DC.PlayerMaxHp;
        public float iframes;
        public float fireCd;
        public bool dead;
        public float aim = -Mathf.PI * 0.5f;

        Vector2 pos;
        Vector2 vel;
        SpriteRenderer body;
        public Vector2 Pos { get { return pos; } }

        void Awake()
        {
            body = GetComponent<SpriteRenderer>();
        }

        public void ResetPlayer()
        {
            pos = Vector2.zero;
            vel = Vector2.zero;
            aim = -Mathf.PI * 0.5f;
            hp = DC.PlayerMaxHp;
            fireCd = 0f;
            iframes = 0f;
            dead = false;
            ApplyTransform();
            body.enabled = true;
            body.color = Color.white;
        }

        public void UpdatePlayer(float dt, float wallT, bool live)
        {
            iframes = Mathf.Max(0f, iframes - dt);
            fireCd -= dt;
            float tx = 0f, ty = 0f;
            if (!dead && live && DC.Input != null)
            {
                Vector2 mv = DC.Input.MoveAxis;
                tx = mv.x;
                ty = mv.y;
                float m = Mathf.Sqrt(tx * tx + ty * ty);
                if (m > 1f) { tx /= m; ty /= m; }
                Vector3 mw = DC.Cam.ScreenToWorld(DC.Input.MouseScreenPos);
                aim = Mathf.Atan2(mw.y - pos.y, mw.x - pos.x);
                bool firing = DC.Input.KeyFireHeld || (DC.Input.MouseHeld && !DC.Input.PointerOverUI());
                if (firing && fireCd <= 0f) Fire();
            }
            float k = 1f - Mathf.Exp(-DC.PlayerAccelK * dt);
            vel.x += (tx * DC.PlayerMaxSpeed - vel.x) * k;
            vel.y += (ty * DC.PlayerMaxSpeed - vel.y) * k;
            pos += vel * dt;
            pos.x = Mathf.Clamp(pos.x, -Arena.HalfW + DC.PlayerClamp, Arena.HalfW - DC.PlayerClamp);
            pos.y = Mathf.Clamp(pos.y, -Arena.HalfH + DC.PlayerClamp, Arena.HalfH - DC.PlayerClamp);
            ApplyTransform();
        }

        void ApplyTransform()
        {
            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, aim * Mathf.Rad2Deg);
        }

        public static Vector2 Dir(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        void Fire()
        {
            fireCd = DC.FireCooldown;
            Vector2 d = Dir(aim);
            DC.Main.SpawnPlayerBullet(pos + d * 24f, d * DC.BulletSpeed + vel * 0.3f, aim);
        }

        public void Damage(float d)
        {
            if (dead || iframes > 0f) return;
            hp = Mathf.Max(0f, hp - d);
            iframes = DC.IFrameTime;
            if (hp <= 0f) KillPlayer();
        }

        void KillPlayer()
        {
            dead = true;
            body.enabled = false;
            DC.Main.OnPlayerDied();
        }

        public void AddImpulse(Vector2 v)
        {
            vel += v;
        }
    }
}
