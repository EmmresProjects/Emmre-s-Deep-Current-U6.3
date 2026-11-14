using UnityEngine;

namespace DeepCurrent
{
    // Keeps the numbers of one run (score and kills) and the best score saved on disk.
    // It does not draw anything and does not spawn anything. The UI reads it, and the game states write to it.
    public class ScoreKeeper
    {
        const string BestKey = "deepcurrent_best";

        public int Score { get; private set; }
        public int Kills { get; private set; }
        public int Best { get; private set; }

        public ScoreKeeper()
        {
            Best = PlayerPrefs.GetInt(BestKey, 0);
        }

        public void ResetRun()
        {
            Score = 0;
            Kills = 0;
        }

        public void AddKill(int points)
        {
            Score += points;
            Kills++;
        }

        // Points that are not tied to one kill, such as a wave clear bonus.
        public void AddBonus(int points)
        {
            Score += points;
        }

        // Saves the run when it beats the best score. Returns true for a new record.
        public bool SaveIfRecord()
        {
            if (Score <= Best) return false;

            Best = Score;
            PlayerPrefs.SetInt(BestKey, Best);
            PlayerPrefs.Save();
            return true;
        }
    }
}
