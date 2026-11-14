using UnityEngine;

namespace DeepCurrent
{
    // Shown on top of the HUD while the game is paused (Time.timeScale is 0).
    public class PauseScreen
    {
        readonly GameObject root;

        public PauseScreen(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            var t = ui.Theme;

            UIWidgets.Dimmer(layer);
            var card = UIWidgets.Dossier(layer, t.panel, 620f, 350f);

            var heading = UIWidgets.Text("Heading", card, "PAUSED", FontLib.Bungee, 44f, t.ice);
            UIWidgets.SetTopLeft(heading, 30f, -28f, 560f, 60f);
            var hint = UIWidgets.Text("Hint", card, "Press P / Esc or use the buttons below.",
                FontLib.Chakra, 22f, t.ice);
            UIWidgets.SetTopLeft(hint, 34f, -98f, 552f, 34f);

            var resume = new UIBuilder().Panel("ResumeBtn", card, ui.Primary)
                .Kicker("RESUME", 23f).Sized(260f, 56f)
                .FinishButton(() => GameRoot.Instance.TogglePause());
            UIWidgets.SetTopLeft(resume, 34f, -160f, 260f, 56f);

            var restart = new UIBuilder().Panel("RestartBtn", card, ui.Ghost)
                .Kicker("RESTART", 23f).Sized(260f, 56f)
                .FinishButton(() => GameRoot.Instance.StartRun());
            UIWidgets.SetTopLeft(restart, 310f, -160f, 260f, 56f);

            var title = new UIBuilder().Panel("TitleBtn", card, ui.Exit)
                .Kicker("TITLE", 23f).Sized(260f, 56f)
                .FinishButton(() => GameRoot.Instance.ReturnToTitle());
            UIWidgets.SetTopLeft(title, 34f, -238f, 260f, 56f);

            var quit = new UIBuilder().Panel("QuitBtn", card, ui.Exit)
                .Kicker("QUIT", 23f).Sized(260f, 56f)
                .FinishButton(() => GameRoot.Instance.QuitGame());
            UIWidgets.SetTopLeft(quit, 310f, -238f, 260f, 56f);
        }

        public void Show(bool on) => root.SetActive(on);
        public void Hide() => root.SetActive(false);
    }
}
