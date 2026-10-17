using UnityEngine;

namespace DeepCurrent
{
    // Base class for every enemy. An enemy knows its own stats, how it touches the player, and how it looks.
    // It does not know how it moves, and it does not know the rules of bolts or the score.
    //  If you want to change an enemy's numbers, change its prefab (serialized fields below).
    //  If you want to change how it moves, change EnemyMovement and the Movement folder.
    //  If you want to change how it attacks, change the Attack method of that enemy's subclass.
    public abstract class Enemy : MonoBehaviour
    {
        const float MaxFrameTime = 0.05f;
        const float KnockbackSpeed = 190f;   // How fast the enemy bounces back after touching the player
        const float PlayerPushSpeed = 170f;  // How hard the enemy shoves the player

        public abstract EnemyKind Kind { get; }

        [Header("Stats")]
        public float maxHp = 1f;
        public float radius = 15f;
        public int score = 40;
        public float contactDamage = 10f;
        public bool explodesOnContact = false;
        public bool facesTarget = false;

        [Header("Movement (Strategy pattern)")]
        public MovementKind movement = MovementKind.Drift;
        [Tooltip("Seconds between random movement swaps on later waves. 0 = never swap.")]
        public float switchInterval = 0f;

        public Vector2 Pos => transform.localPosition;

        // Unit direction from this enemy to the player, and the distance between them.
        // Set every frame before Attack and UpdateVisuals run.
        protected Vector2 ToPlayer = Vector2.up;
        protected float DistanceToPlayer;

        float hp;
        readonly EnemyMovement mover = new EnemyMovement();

        // Called by the factory right after the object leaves the pool.
        public virtual void OnSpawn(int wave)
        {
            hp = maxHp + ExtraHpPerWave(wave);
            mover.Setup(movement, switchInterval, wave, GameRoot.Instance.settings.randomMovementWave);
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;
        }

        protected virtual int ExtraHpPerWave(int wave) => 0;
        protected virtual void Attack(GameRoot game, float dt) { }
        protected virtual void UpdateVisuals(float dt)
        {
            if (facesTarget) FaceTarget();
        }

        void Update()
        {
            var game = GameRoot.Instance;
            if (game == null || !game.IsPlaying) return;

            float dt = Mathf.Min(Time.deltaTime, MaxFrameTime);
            var player = game.Player;
            UpdateAimAt(player.Pos);

            if (player.Dead)
            {
                transform.localPosition = mover.Coast(dt, Pos, Arena, radius);
            }
            else
            {
                Attack(game, dt);
                transform.localPosition = mover.Step(BuildContext(dt, game.WaveNum));
                TouchPlayer(player);
            }

            UpdateVisuals(dt);
        }

        void UpdateAimAt(Vector2 target)
        {
            Vector2 d = target - Pos;
            float dist = d.magnitude;
            if (dist < 0.0001f) dist = 1f;

            ToPlayer = d / dist;
            DistanceToPlayer = dist;
        }

        MovementContext BuildContext(float dt, int wave)
        {
            return new MovementContext
            {
                position = Pos,
                toPlayer = ToPlayer,
                distance = DistanceToPlayer,
                dt = dt,
                arena = Arena,
                wave = wave,
                bodyRadius = radius
            };
        }

        static ArenaBounds Arena => CameraFitter.Instance.Bounds;

        void TouchPlayer(PlayerController player)
        {
            float reach = radius + GameRoot.Instance.settings.playerRadius - 2f;
            if ((player.Pos - Pos).sqrMagnitude >= reach * reach) return;

            player.Damage(contactDamage);
            if (explodesOnContact)
            {
                GameRoot.Instance.KillEnemy(this, false);
                return;
            }

            mover.Knockback(-ToPlayer * KnockbackSpeed);
            player.AddImpulse(ToPlayer * PlayerPushSpeed);
        }

        public void TakeHit()
        {
            hp--;
            if (hp <= 0f) GameRoot.Instance.KillEnemy(this, true);
        }

        protected void FaceTarget()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(ToPlayer.y, ToPlayer.x) * Mathf.Rad2Deg);
        }
    }
}
