namespace DeepCurrent
{
    // STATE PATTERN: the base class of every game state.
    // The game is always in exactly one state. Each state decides what a key press, a frame update, or a focus change means.
    // GameRoot only forwards events to the current state, so it never needs a switch on the state.
    // Enter runs once when the state starts, Exit runs once when it ends.
    public abstract class GameState
    {
        public virtual bool IsPlaying => false;
        public virtual bool IsPaused => false;

        public virtual void Enter(GameRoot game) { }
        public virtual void Exit(GameRoot game) { }
        public virtual void Tick(GameRoot game, float dt) { }

        // Pause key pressed.
        public virtual void OnPauseRequested(GameRoot game) { }

        // Enter key pressed.
        public virtual void OnStartRequested(GameRoot game) { }

        // The window lost focus.
        public virtual void OnFocusLost(GameRoot game) { }
    }
}
