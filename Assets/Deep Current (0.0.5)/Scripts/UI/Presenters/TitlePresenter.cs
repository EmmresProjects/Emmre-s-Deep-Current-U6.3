using System;

namespace DeepCurrent
{
    // MVP Pattern: handles title screen actions and leaves display work to the view.
    public sealed class TitlePresenter
    {
        readonly ITitleView view;
        readonly GameRoot game;
        readonly Action showCustomize;

        public TitlePresenter(ITitleView titleView, GameRoot gameRoot, Action openCustomize)
        {
            view = titleView;
            game = gameRoot;
            showCustomize = openCustomize;

            view.DiveRequested += StartRun;
            view.CustomizeRequested += OpenCustomize;
            view.QuitRequested += QuitGame;
        }

        public void Show() => view.Show(game.Scores.Best);
        public void Hide() => view.Hide();

        void StartRun() => game.StartRun();
        void OpenCustomize() => showCustomize();
        void QuitGame() => game.QuitGame();
    }
}
