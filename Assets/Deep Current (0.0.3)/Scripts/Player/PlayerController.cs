using UnityEngine;

namespace DeepCurrent
{
    // Only movement, health and death. Aiming and shooting live in PlayerShooting.
    // Looks live in DiverBuilder. (Single Responsibility: SOLID)
    [DefaultExecutionOrder(-10)]
    public class PlayerController : MonoBehaviour
    {
        public float Hp { get; private set; }
        public float Iframes { get; private set; }
        public bool Dead { get; private set; }
        public Vector2 Pos { get; private set; }
        public Vector2 Velocity { get; private set; }

        SpriteRenderer body;
        PlayerShooting shooting;

        void Awake()
        {
            body = GetComponent<SpriteRenderer>();
            shooting = GetComponent<PlayerShooting>();
        }

        public void ResetPlayer()
        {
            Pos = Vector2.zero;
            Velocity = Vector2.zero;
            Hp = GameRoot.Instance.settings.playerMaxHp;
            Iframes = 0f;
            Dead = false;
            body.enabled = true;
            transform.localPosition = new Vector3(Pos.x, Pos.y, 0f);
            if (shooting != null) shooting.ResetShooting();
        }

        void Update()
        {
            var s = GameRoot.Instance.settings;
            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            Iframes = Mathf.Max(0f, Iframes - dt);

            Vector2 target = ReadMoveInput();

            // Smooth acceleration toward the target speed (frame-rate independent).
            float k = 1f - Mathf.Exp(-s.playerAccel * dt);
            Velocity += (target * s.playerMaxSpeed - Velocity) * k;

            // Stay inside what the camera sees.
            Vector2 next = CameraFitter.Instance.Bounds.Clamp(
                Pos + Velocity * dt,
                new Vector2(s.arenaPadding, s.arenaPadding));
            Pos = next;
            transform.localPosition = new Vector3(next.x, next.y, 0f);
        }

        // The wanted direction from input, normalized. Zero when the player cannot move.
        Vector2 ReadMoveInput()
        {
            if (Dead || !GameRoot.Instance.IsPlaying) return Vector2.zero;

            Vector2 target = InputBridge.Instance.MoveAxis;
            if (target.sqrMagnitude > 1f) target.Normalize();
            return target;
        }

        // Hurts the player. Hits during invincibility frames are ignored.
        public void Damage(float amount)
        {
            if (Dead || Iframes > 0f) return;

            Hp = Mathf.Max(0f, Hp - amount);
            Iframes = GameRoot.Instance.settings.iframeTime;
            if (Hp <= 0f)
            {
                Dead = true;
                body.enabled = false;
            }
        }

        public void AddImpulse(Vector2 v)
        {
            Velocity += v;
        }

        // Called by DiverBuilder when the loadout (skin and paint) is applied.
        public void ApplyLook(Sprite skin, Color paint)
        {
            if (skin != null) body.sprite = skin;
            body.color = paint;
        }
    }
}
