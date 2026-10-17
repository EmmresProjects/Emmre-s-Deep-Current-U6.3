using UnityEngine;

namespace DeepCurrent
{
    // Scriptable Object with every balance number in the game.
    // "If you want to change the player speed, wave pacing or a rule, edit the GameSettings asset, not the code.
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Deep Current/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Player")]
        public float playerMaxHp = 100f;
        public float playerMaxSpeed = 330f;
        public float playerAccel = 8f;
        public float playerRadius = 15f;
        public float iframeTime = 0.9f;
        public float arenaPadding = 20f;

        [Header("Shooting")]
        public float fireCooldown = 0.16f;
        public float bulletSpeed = 640f;
        public float bulletLife = 0.9f;

        [Header("Enemy Bolts")]
        public float enemyBulletDamage = 10f;

        [Header("Waves")]
        public float waveAnnounceTime = 2.4f;
        public float waveClearTime = 2.1f;
        public float deathDelay = 1.7f;
        public float spawnInterval = 1.15f;
        public float spawnIntervalShrinkPerWave = 0.05f;
        public float spawnIntervalMin = 0.35f;
        public int enemiesBase = 6;
        public int enemiesPerWave = 2;
        public int maxEnemiesPerWave = 24;
        public int waveClearBonusBase = 100;
        public int waveClearBonusPerWave = 50;

        [Header("Unpredictable Enemies")]
        // From this wave on, enemies may randomly swap their movement strategy.
        public int randomMovementWave = 4;
    }
}
