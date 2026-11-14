using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeepCurrent
{
    // Builds the enemy order for one wave and keeps its first two enemies predictable.
    public sealed class WaveQueueBuilder
    {
        const int SpitterFromWave = 3;
        const float SpitterChance = 0.22f;
        const float JellyChance = 0.62f;
        const int EliteFromWave = 4;
        const int WavesPerElite = 4;

        readonly GameSettings settings;

        public WaveQueueBuilder(GameSettings gameSettings)
        {
            settings = gameSettings;
        }

        public void Fill(List<EnemyKind> queue, int waveNumber)
        {
            queue.Clear();
            int count = Mathf.Max(2, Mathf.Min(
                settings.enemiesBase + waveNumber * settings.enemiesPerWave,
                settings.maxEnemiesPerWave));

            for (int i = 0; i < count; i++)
                queue.Add(ChooseEnemy(waveNumber));

            queue[0] = EnemyKind.Jelly;
            queue[1] = EnemyKind.Angler;
            AddElites(queue, waveNumber);
            ShuffleOptionalEnemies(queue);
        }

        EnemyKind ChooseEnemy(int waveNumber)
        {
            float roll = Random.value;
            if (waveNumber >= SpitterFromWave && roll < SpitterChance)
                return EnemyKind.Spitter;
            return roll < JellyChance ? EnemyKind.Jelly : EnemyKind.Angler;
        }

        static void AddElites(List<EnemyKind> queue, int waveNumber)
        {
            int eliteCount = waveNumber >= EliteFromWave ? waveNumber / WavesPerElite : 0;
            for (int i = 0; i < eliteCount; i++)
                queue.Add(EnemyKind.Elite);
        }

        static void ShuffleOptionalEnemies(List<EnemyKind> queue)
        {
            for (int i = queue.Count - 1; i > 2; i--)
            {
                int other = Random.Range(2, i + 1);
                (queue[i], queue[other]) = (queue[other], queue[i]);
            }
        }
    }
}
