using UnityEngine;

namespace DeepCurrent
{
    // Player bolt. It flies straight and pops the first enemy it touches.
    public class PlayerBullet : Projectile
    {
        // Extra reach added to the enemy radius, so near misses still count.
        const float HitPadding = 5f;

        public void SetAngle(float ang)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, ang * Mathf.Rad2Deg);
        }

        protected override bool OnStep(Vector2 pos)
        {
            var enemies = GameRoot.Instance.Spawner.Alive;
            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                var e = enemies[j];
                float reach = e.radius + HitPadding;
                if ((pos - e.Pos).sqrMagnitude < reach * reach)
                {
                    e.TakeHit();
                    return true;
                }
            }
            return false;
        }
    }
}
