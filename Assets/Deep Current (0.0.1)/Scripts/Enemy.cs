using UnityEngine;

namespace DeepCurrent
{
    // One Enemy script is kept for the four existing prefab types.
    public class Enemy : MonoBehaviour
    {
        public int kind;
        public EnemyKind Kind { get { return (EnemyKind)kind; } }
        [HideInInspector] public float radius = 15f;
        [HideInInspector] public int hp = 1;

        float vx, vy, phase, ang, fireT, burstT;
        SpriteRenderer mainSR;
        public Vector2 Pos { get { return transform.localPosition; } }

        void Awake()
        {
            mainSR = GetComponent<SpriteRenderer>();
        }

        public void Setup(int wave)
        {
            radius = DC.EnemyRadius(Kind);
            hp = DC.EnemyBaseHp(Kind)
               + ((Kind == EnemyKind.Angler || Kind == EnemyKind.Spitter) ? Mathf.FloorToInt(wave / 3f) : 0)
               + (Kind == EnemyKind.Elite ? wave : 0);
            phase = Random.Range(0f, Mathf.PI * 2f);
            ang = Random.Range(0f, Mathf.PI * 2f);
            fireT = Random.Range(1f, 2f);
            burstT = 3f;
            vx = 0f;
            vy = 0f;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            if (mainSR != null) mainSR.color = Color.white;
        }

        public void TakeHit()
        {
            hp--;
            if (hp <= 0) DC.Main.KillEnemy(this, true);
        }

        public void UpdateEnemy(float dt, float wallT, bool live, PlayerController player, int wave)
        {
            Vector2 pos = transform.localPosition;
            Vector2 pp = player.Pos;
            float dx = pp.x - pos.x, dy = pp.y - pos.y;
            float d = Mathf.Sqrt(dx * dx + dy * dy);
            if (d < 0.0001f) d = 1f;
            float nx = dx / d, ny = dy / d;
            bool pdead = player.dead;
            switch (Kind)
            {
                case EnemyKind.Jelly:
                {
                    float sp = 14f + wave * 1.2f;
                    vx += (nx * sp - vx) * dt * 1.2f;
                    vy += (ny * sp - vy) * dt * 1.2f;
                    break;
                }
                case EnemyKind.Angler:
                {
                    float sp = Mathf.Min(150f + wave * 8f, 260f);
                    float k = 1f - Mathf.Exp(-2.6f * dt);
                    float strafe = Mathf.Sin(wallT * 2f + phase) * 40f;
                    vx += ((nx * sp - ny * strafe * 0.4f) - vx) * k;
                    vy += ((ny * sp + nx * strafe * 0.4f) - vy) * k;
                    ang = Mathf.Atan2(dy, dx);
                    break;
                }
                case EnemyKind.Spitter:
                {
                    bool nearEdge = pos.x < -Arena.HalfW + 75f || pos.x > Arena.HalfW - 75f
                                 || pos.y < -Arena.HalfH + 75f || pos.y > Arena.HalfH - 75f;
                    float want = (d > 300f || nearEdge) ? 1f : -1f;
                    float k = 1f - Mathf.Exp(-3f * dt);
                    float strafe = Mathf.Sin(wallT * 0.9f + phase) * 0.9f;
                    vx += ((nx * 110f * want - ny * 70f * strafe) - vx) * k;
                    vy += ((ny * 110f * want + nx * 70f * strafe) - vy) * k;
                    ang = Mathf.Atan2(dy, dx);
                    if (live && !pdead)
                    {
                        fireT -= dt;
                        if (fireT <= 0f && d < 540f)
                        {
                            fireT = 1.4f + Random.value * 0.9f;
                            Vector2 dir = new Vector2(nx, ny);
                            DC.Main.SpawnEnemyBullet(pos + dir * 14f, dir * 240f, 5f, 4f);
                        }
                    }
                    break;
                }
                case EnemyKind.Elite:
                {
                    vx += (nx * 46f - vx) * dt;
                    vy += (ny * 46f - vy) * dt;
                    ang += dt * 0.5f;
                    if (live && !pdead)
                    {
                        burstT -= dt;
                        if (burstT <= 0f)
                        {
                            burstT = 3.4f;
                            for (int a = 0; a < 12; a++)
                            {
                                float an = a / 12f * Mathf.PI * 2f + ang;
                                Vector2 dir = new Vector2(Mathf.Cos(an), Mathf.Sin(an));
                                DC.Main.SpawnEnemyBullet(pos + dir * 36f, dir * 165f, 5f, 5f);
                            }
                        }
                    }
                    break;
                }
            }
            pos.x += vx * dt;
            pos.y += vy * dt;
            float safeX = Mathf.Min(radius + 15f, Arena.W * 0.35f);
            float safeY = Mathf.Min(radius + 15f, Arena.H * 0.35f);
            pos.x = Mathf.Clamp(pos.x, -Arena.HalfW + safeX, Arena.HalfW - safeX);
            pos.y = Mathf.Clamp(pos.y, -Arena.HalfH + safeY, Arena.HalfH - safeY);
            transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            if (live && !pdead)
            {
                float rr = radius + DC.PlayerRadius - 2f;
                if (dx * dx + dy * dy < rr * rr)
                {
                    if (Kind == EnemyKind.Jelly)
                    {
                        player.Damage(26f);
                        DC.Main.KillEnemy(this, false);
                        return;
                    }
                    float dmg = Kind == EnemyKind.Angler ? 14f : Kind == EnemyKind.Spitter ? 12f : 22f;
                    player.Damage(dmg);
                    vx = -nx * 190f;
                    vy = -ny * 190f;
                    player.AddImpulse(new Vector2(nx * 170f, ny * 170f));
                }
            }
        }

        public void UpdateVisuals(float dt, float wallT)
        {
            // Facing direction is kept, extra flashes, tails and glow are not.
            if (Kind != EnemyKind.Jelly)
                transform.localRotation = Quaternion.Euler(0f, 0f, ang * Mathf.Rad2Deg);
        }
    }
}
