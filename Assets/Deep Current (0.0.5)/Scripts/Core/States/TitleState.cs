namespace DeepCurrent
{
    // STATE: Title. The start screen with the best score and the Diver Bay.
    // Enter starts a run.
    public class TitleState : GameState
    {
        public override void Enter(GameRoot game)
        {
            game.ClearRun();
            UIManager.Instance.ShowTitle();
        }

        public override void OnStartRequested(GameRoot game)
        {
            game.StartRun();
        }
    }
}
