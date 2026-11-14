namespace DeepCurrent
{
    // STATE: Game Over. Shows the final tally and saves a new best score if one was earned.
    // Enter starts a new run again.
    public class OverState : GameState
    {
        public override void Enter(GameRoot game)
        {
            bool isRecord = game.Scores.SaveIfRecord();
            UIManager.Instance.ShowGameOver(game.Scores.Score, game.WaveNum,
                game.Scores.Kills, game.Scores.Best, isRecord);
        }

        public override void OnStartRequested(GameRoot game)
        {
            game.StartRun();
        }
    }
}
