using UnityEngine;

namespace DeepCurrent
{
    // Only aiming and firing. Movement and health live in PlayerController.
    public class PlayerShooting : MonoBehaviour
    {
        const float MuzzleOffset = 24f;
        const float MomentumShare = 0.3f;

        public float Aim { get; private set; } = -Mathf.PI * 0.5f;

        float fireCooldown;
        PlayerController player;

        void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        public void ResetShooting()
        {
            Aim = -Mathf.PI * 0.5f;
            fireCooldown = 0f;
            transform.localRotation = Quaternion.Euler(0f, 0f, Aim * Mathf.Rad2Deg);
        }

        void Update()
        {
            fireCooldown -= Time.deltaTime;
            if (player.Dead || !GameRoot.Instance.IsPlaying) return;

            var input = InputBridge.Instance;

            // Aim at the mouse, in world space.
            Vector3 mouseWorld = CameraFitter.Instance.ScreenToWorld(input.MouseScreenPos);
            Aim = Mathf.Atan2(mouseWorld.y - player.Pos.y, mouseWorld.x - player.Pos.x);
            transform.localRotation = Quaternion.Euler(0f, 0f, Aim * Mathf.Rad2Deg);

            bool firing = input.KeyFireHeld || (input.MouseHeld && !input.PointerOverUI());
            if (firing && fireCooldown <= 0f) Fire();
        }

        void Fire()
        {
            var s = GameRoot.Instance.settings;
            fireCooldown = s.fireCooldown;

            Vector2 dir = new Vector2(Mathf.Cos(Aim), Mathf.Sin(Aim));
            GameRoot.Instance.Projectiles.FirePlayerBolt(
                player.Pos + dir * MuzzleOffset,
                dir * s.bulletSpeed + player.Velocity * MomentumShare,
                Aim);
        }
    }
}
