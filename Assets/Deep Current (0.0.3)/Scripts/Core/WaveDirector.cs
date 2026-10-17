using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeepCurrent
{
    // It decides WHAT spawns and WHEN.
    // If waves feel wrong, check here.
    // "If you want to change the enemy mix, check BuildQueue().
    public class WaveDirector
    {
        // Tuning values that are not in the GameSettings asset.
        const float FirstSpawnDelay = 0.3f;
        const int SpitterFromWave = 3;
        const float SpitterChance = 0.22f;
        const float JellyChance = 0.62f;
        const int EliteFromWave = 4;
        const int WavesPerElite = 4;

        public int WaveNum { get; private set; }
        public int QueuedCount => queue.Count;

        // GameRoot listens to these instead of the director knowing about spawning or UI.
        public Action<EnemyKind> OnSpawn;
        public Action<string> OnBanner;
        public Action<int> OnWaveClearBonus;

        enum Phase { Idle, Announce, Active, Clear }
        Phase phase = Phase.Idle;
        float timer;
        readonly List<EnemyKind> queue = new List<EnemyKind>();
        readonly GameSettings settings;

        public WaveDirector(GameSettings settings)
        {
            this.settings = settings;
        }

        public void Reset()
        {
            phase = Phase.Idle;
            queue.Clear();
            WaveNum = 0;
        }

        public void StartWave(int n)
        {
            WaveNum = n;
            phase = Phase.Announce;
            timer = settings.waveAnnounceTime;
            BuildQueue(n);
            OnBanner?.Invoke("WAVE " + n + " - ENEMIES INCOMING");
        }

        // aliveEnemies is reported by GameRoot each frame.
        public void Update(float dt, int aliveEnemies)
        {
            switch (phase)
            {
                case Phase.Announce:
                    timer -= dt;
                    if (timer <= 0f)
                    {
                        phase = Phase.Active;
                        timer = FirstSpawnDelay;
                    }
                    break;

                case Phase.Active:
                    UpdateActive(dt, aliveEnemies);
                    break;

                case Phase.Clear:
                    timer -= dt;
                    if (timer <= 0f) StartWave(WaveNum + 1);
                    break;
            }
        }

        void UpdateActive(float dt, int aliveEnemies)
        {
            if (queue.Count > 0)
            {
                timer -= dt;
                if (timer > 0f) return;

                timer = Mathf.Max(settings.spawnIntervalMin,
                    settings.spawnInterval - WaveNum * settings.spawnIntervalShrinkPerWave);
                var kind = queue[0];
                queue.RemoveAt(0);
                OnSpawn?.Invoke(kind);
                return;
            }

            if (aliveEnemies == 0) FinishWave();
        }

        void FinishWave()
        {
            phase = Phase.Clear;
            timer = settings.waveClearTime;
            int bonus = settings.waveClearBonusBase + WaveNum * settings.waveClearBonusPerWave;
            OnBanner?.Invoke("WAVE CLEAR - BONUS " + bonus);
            OnWaveClearBonus?.Invoke(bonus);
        }

        // The enemy mix for one wave. The first two slots are always a Jelly and an Angler, so both kinds are visible right away.
        void BuildQueue(int w)
        {
            queue.Clear();
            int count = Mathf.Min(settings.enemiesBase + w * settings.enemiesPerWave, settings.maxEnemiesPerWave);
            for (int i = 0; i < count; i++)
            {
                float r = Random.value;
                EnemyKind k = EnemyKind.Angler;
                if (w >= SpitterFromWave && r < SpitterChance) k = EnemyKind.Spitter;
                else if (r < JellyChance) k = EnemyKind.Jelly;
                queue.Add(k);
            }
            queue[0] = EnemyKind.Jelly;
            queue[1] = EnemyKind.Angler;

            int elites = w >= EliteFromWave ? w / WavesPerElite : 0;
            for (int i = 0; i < elites; i++) queue.Add(EnemyKind.Elite);

            ShuffleAfterGuaranteedSlots();
        }

        void ShuffleAfterGuaranteedSlots()
        {
            for (int i = queue.Count - 1; i > 2; i--)
            {
                int j = Random.Range(2, i + 1);
                (queue[i], queue[j]) = (queue[j], queue[i]);
            }
        }
    }
}
