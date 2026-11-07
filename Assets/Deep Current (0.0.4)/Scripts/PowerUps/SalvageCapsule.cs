using UnityEngine;

namespace DeepCurrent
{
    // One floating salvage capsule dropped by a kill.
    // It pulls toward the player when close, blinks when it is about to expire, and releases itself on pickup.
    // Pickup builds the command for its kind and lets the invoker run it.
    public class SalvageCapsule : MonoBehaviour
    {
        // Blink rate and the last seconds of life when the blink starts.
        const float BlinkFrom = 3f;
        const float BlinkRate = 8f;
        const float MinBlinkOn = 0.5f;

        CapsuleKind kind;
        SpriteRenderer body;
        ObjectPool home;
        float lifeLeft;
        float pulse;

        void Awake()
        {
            body = GetComponent<SpriteRenderer>();
        }

        // Called by CapsuleSpawner right after the capsule leaves the pool.
        public void Setup(CapsuleKind newKind, Sprite sprite, float lifeTime, ObjectPool owner)
        {
            kind = newKind;
            lifeLeft = lifeTime;
            pulse = Random.Range(0f, Mathf.PI * 2f);
            home = owner;

            if (sprite != null) body.sprite = sprite;
            body.color = Color.white;
            transform.localScale = Vector3.one;
        }

        void Update()
        {
            var game = GameRoot.Instance;
            if (game == null || !game.IsPlaying) return;

            float dt = SimTime.Delta;
            lifeLeft -= dt;
            if (lifeLeft <= 0f)
            {
                home.Release(gameObject);
                return;
            }

            pulse += dt;
            float breath = 1f + Mathf.Sin(pulse * 2.4f) * 0.08f;
            transform.localScale = new Vector3(breath, breath, 1f);

            MoveAndCollect(game, dt);
            BlinkWhenExpiring();
        }

        // Drift toward the player inside the magnet range, and collect inside the touch range.
        void MoveAndCollect(GameRoot game, float dt)
        {
            var settings = game.settings;
            var player = game.Player;
            if (player == null || player.Dead) return;

            Vector3 p = transform.localPosition;
            Vector2 to = player.Pos - (Vector2)p;
            float distSq = to.sqrMagnitude;

            if (distSq < settings.capsuleCollectRange * settings.capsuleCollectRange)
            {
                Collect(game);
                return;
            }

            if (distSq < settings.capsuleMagnetRange * settings.capsuleMagnetRange && distSq > 0.0001f)
            {
                p += (Vector3)(to.normalized * (settings.capsuleMagnetSpeed * dt));
                transform.localPosition = p;
            }
        }

        void BlinkWhenExpiring()
        {
            if (lifeLeft > BlinkFrom)
            {
                body.color = Color.white;
                return;
            }

            float t = Mathf.FloorToInt(lifeLeft * BlinkRate) % 2 == 0 ? 1f : MinBlinkOn;
            body.color = new Color(1f, 1f, 1f, t);
        }

        void Collect(GameRoot game)
        {
            var command = CapsuleCommands.Create(kind, game.Player, game.Player.Powerups, game.settings);
            game.Commands.Execute(command);
            home.Release(gameObject);
        }
    }
}
