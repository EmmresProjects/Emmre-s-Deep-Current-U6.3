namespace DeepCurrent
{
    // STATE: Playing. Waves run, enemies move and the player can shoot.
    // The pause key or losing window focus moves the game to PausedState.
    // When the hull is gone, the state waits for the death delay and then moves to OverState.
    public class PlayingState : GameState
    {
        // True only for a brand new run. Resuming from pause reuses this same
        // object, so the run must not restart.
        bool needsStart;
        float deathTimer = -1f;

        public PlayingState(bool needsStart)
        {
            this.needsStart = needsStart;
        }

        public override bool IsPlaying => true;

        public override void Enter(GameRoot game)
        {
            if (!needsStart) return;

            needsStart = false;
            game.BeginRun();
        }

        public override void Tick(GameRoot game, float dt)
        {
            if (!game.Player.Dead)
            {
                game.Director.Update(dt, game.Spawner.Count);
                return;
            }

            if (deathTimer < 0f) deathTimer = game.settings.deathDelay;
            deathTimer -= dt;
            if (deathTimer <= 0f) game.ChangeState(new OverState());
        }

        public override void OnPauseRequested(GameRoot game)
        {
            game.ChangeState(new PausedState(this));
        }

        public override void OnFocusLost(GameRoot game)
        {
            game.ChangeState(new PausedState(this));
        }
    }
}
