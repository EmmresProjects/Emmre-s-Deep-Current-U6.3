using System;
using System.Collections.Generic;

namespace DeepCurrent
{
    // Moves a run through its wave announcement, enemy spawn, and clear phases.
    public sealed class WaveDirector
    {
        const float FirstSpawnDelay = 0.3f;

        enum Phase { Idle, Announce, Active, Clear }

        readonly GameSettings settings;
        readonly WaveQueueBuilder queueBuilder;
        readonly List<EnemyKind> queue = new List<EnemyKind>();
        Phase phase = Phase.Idle;
        float timer;

        public int WaveNum { get; private set; }
        public int QueuedCount => queue.Count;

        public event Action<EnemyKind> OnSpawn;
        public event Action<string> OnBanner;
        public event Action<int> OnWaveClearBonus;

        public WaveDirector(GameSettings gameSettings)
        {
            settings = gameSettings;
            queueBuilder = new WaveQueueBuilder(gameSettings);
        }

        public void Reset()
        {
            phase = Phase.Idle;
            queue.Clear();
            WaveNum = 0;
        }

        public void StartWave(int waveNumber)
        {
            WaveNum = waveNumber;
            phase = Phase.Announce;
            timer = settings.waveAnnounceTime;
            queueBuilder.Fill(queue, waveNumber);
            OnBanner?.Invoke("WAVE " + waveNumber + " - ENEMIES INCOMING");
        }

        public void Update(float deltaTime, int aliveEnemies)
        {
            switch (phase)
            {
                case Phase.Announce:
                    UpdateAnnouncement(deltaTime);
                    break;
                case Phase.Active:
                    UpdateSpawning(deltaTime, aliveEnemies);
                    break;
                case Phase.Clear:
                    UpdateClearDelay(deltaTime);
                    break;
            }
        }

        void UpdateAnnouncement(float deltaTime)
        {
            timer -= deltaTime;
            if (timer > 0f) return;

            phase = Phase.Active;
            timer = FirstSpawnDelay;
        }

        void UpdateSpawning(float deltaTime, int aliveEnemies)
        {
            if (queue.Count > 0)
            {
                timer -= deltaTime;
                if (timer > 0f) return;

                timer = Math.Max(settings.spawnIntervalMin,
                    settings.spawnInterval - WaveNum * settings.spawnIntervalShrinkPerWave);
                EnemyKind next = queue[0];
                queue.RemoveAt(0);
                OnSpawn?.Invoke(next);
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

        void UpdateClearDelay(float deltaTime)
        {
            timer -= deltaTime;
            if (timer <= 0f) StartWave(WaveNum + 1);
        }
    }
}
