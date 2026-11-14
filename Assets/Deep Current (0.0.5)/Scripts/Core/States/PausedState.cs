using UnityEngine;

namespace DeepCurrent
{
    // STATE: Paused. Time stops and the pause overlay is shown.
    // Resuming returns to the same PlayingState object that was paused,
    // so the run keeps its exact timers.
    public class PausedState : GameState
    {
        readonly PlayingState resumeTo;

        public PausedState(PlayingState resumeTo)
        {
            this.resumeTo = resumeTo;
        }

        public override bool IsPaused => true;

        public override void Enter(GameRoot game)
        {
            Time.timeScale = 0f;
            UIManager.Instance.ShowPause(true);
        }

        public override void Exit(GameRoot game)
        {
            Time.timeScale = 1f;
            UIManager.Instance.ShowPause(false);
        }

        public override void OnPauseRequested(GameRoot game)
        {
            game.ChangeState(resumeTo);
        }
    }
}
