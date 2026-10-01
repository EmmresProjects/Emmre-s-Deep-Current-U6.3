using UnityEngine;

namespace DeepCurrent
{
    public enum GameMode { Title, Playing, Paused, Over }
    public enum EnemyKind { Jelly = 0, Angler = 1, Spitter = 2, Elite = 3 }

    // Shared values and links.
    public static class DC
    {
        public static GameRoot Main;
        public static UIManager UI;
        public static CameraRig Cam;
        public static InputBridge Input;

        public const float WorldHeight = 810f;
        public const float HalfHeight = WorldHeight * 0.5f;

        public const float PlayerRadius = 15f;
        public const float PlayerMaxHp = 100f;
        public const float PlayerMaxSpeed = 330f;
        public const float PlayerAccelK = 8f;
        public const float FireCooldown = 0.16f;
        public const float BulletSpeed = 640f;
        public const float BulletLife = 0.9f;
        public const float IFrameTime = 0.9f;
        public const float PlayerClamp = 20f;

        public const float WaveAnnounceTime = 2.4f;
        public const float WaveClearTime = 2.1f;
        public const float DeathDelay = 1.7f;

        public static Color Hex(int rgb, float a = 1f)
        {
            return new Color(((rgb >> 16) & 0xFF) / 255f,
                ((rgb >> 8) & 0xFF) / 255f, (rgb & 0xFF) / 255f, a);
        }

        public static readonly Color Abyss = Hex(0x020A10);
        public static readonly Color Panel = Hex(0x10222B);
        public static readonly Color Cyan = Hex(0x3DF2CF);
        public static readonly Color Ice = Hex(0xD9F6F0);
        public static readonly Color Amber = Hex(0xFFC24B);
        public static readonly Color Coral = Hex(0xFF6A4D);

        public static int EnemyScore(EnemyKind k)
        {
            switch (k)
            {
                case EnemyKind.Jelly: return 40;
                case EnemyKind.Angler: return 80;
                case EnemyKind.Spitter: return 120;
                default: return 400;
            }
        }

        public static float EnemyRadius(EnemyKind k)
        {
            switch (k)
            {
                case EnemyKind.Jelly: return 15f;
                case EnemyKind.Angler: return 14f;
                case EnemyKind.Spitter: return 15f;
                default: return 32f;
            }
        }

        public static int EnemyBaseHp(EnemyKind k)
        {
            switch (k)
            {
                case EnemyKind.Jelly: return 1;
                case EnemyKind.Angler: return 3;
                case EnemyKind.Spitter: return 4;
                default: return 16;
            }
        }
    }

    public static class Arena
    {
        public static float HalfW = 720f;
        public static float HalfH = DC.HalfHeight;
        public static float W { get { return HalfW * 2f; } }
        public static float H { get { return HalfH * 2f; } }
    }

    public static class Res
    {
        public static T Load<T>(string path) where T : Object
        {
            T o = Resources.Load<T>(path);
            if (o == null) Debug.LogWarning("[DEEPCURRENT] Missing resource: " + path);
            return o;
        }
    }
}
