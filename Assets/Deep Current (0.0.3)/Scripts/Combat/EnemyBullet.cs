using UnityEngine;

namespace DeepCurrent
{
    // Enemy bolt (ink bolt and elite shard). It hits the player once and then disappears.
    // Damage is set by ProjectileSpawner from the GameSettings asset.
    public class EnemyBullet : Projectile
    {
        const float BoltExitMargin = 80f;

        float damage;
        PlayerController target;
        float targetRadius;

        protected override float ExitMargin => BoltExitMargin;

        // Called by the spawner. The bolt only needs a target and a damage value.
        public void SetTarget(PlayerController player, float playerRadius, float dmg)
        {
            target = player;
            targetRadius = playerRadius;
            damage = dmg;
        }

        protected override bool OnStep(Vector2 pos)
        {
            if (target == null || target.Dead || target.Iframes > 0f) return false;

            // Hit when the bolt is close enough to the player body.
            float reach = Radius + targetRadius;
            if ((pos - target.Pos).sqrMagnitude >= reach * reach) return false;

            target.Damage(damage);
            return true;
        }
    }
}
