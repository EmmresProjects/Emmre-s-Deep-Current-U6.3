namespace DeepCurrent
{
    // Sends input and state changes to the active game state.
    public sealed class GameStateController
    {
        readonly GameRoot game;

        public GameState Current { get; private set; }

        public GameStateController(GameRoot gameRoot)
        {
            game = gameRoot;
        }

        public void Tick(float deltaTime)
        {
            var input = InputBridge.Instance;
            if (input != null)
            {
                if (input.PausePressed) Current?.OnPauseRequested(game);
                if (input.EnterPressed) Current?.OnStartRequested(game);
            }

            Current?.Tick(game, deltaTime);
        }

        public void ChangeState(GameState next)
        {
            Current?.Exit(game);
            Current = next;
            Current?.Enter(game);
        }

        public void StartRun() => ChangeState(new PlayingState(true));
        public void TogglePause() => Current?.OnPauseRequested(game);
        public void ReturnToTitle() => ChangeState(new TitleState());
        public void OnFocusLost() => Current?.OnFocusLost(game);
    }
}
