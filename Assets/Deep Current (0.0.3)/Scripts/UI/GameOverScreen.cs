using UnityEngine;
using TMPro;

namespace DeepCurrent
{
    // The final tally after a run ends.
    public class GameOverScreen
    {
        readonly GameObject root;
        readonly TextMeshProUGUI finalScore, finalWave, finalKills, bestLine;

        public GameOverScreen(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            var t = ui.Theme;

            UIWidgets.Dimmer(layer);
            var card = UIWidgets.Dossier(layer, t.panel, 660f, 460f);

            var heading = UIWidgets.Text("Heading", card, "GAME OVER", FontLib.Bungee, 46f, t.coral);
            UIWidgets.SetTopLeft(heading, 30f, -26f, 600f, 66f);
            finalScore = UIWidgets.Text("Score", card, "SCORE 0", FontLib.ChakraBold, 30f, t.ice);
            UIWidgets.SetTopLeft(finalScore, 34f, -124f, 592f, 42f);
            finalWave = UIWidgets.Text("Wave", card, "WAVE 1", FontLib.Chakra, 26f, t.ice);
            UIWidgets.SetTopLeft(finalWave, 34f, -174f, 592f, 36f);
            finalKills = UIWidgets.Text("Kills", card, "KILLS 0", FontLib.Chakra, 26f, t.ice);
            UIWidgets.SetTopLeft(finalKills, 34f, -218f, 592f, 36f);
            bestLine = UIWidgets.Text("BestLine", card, "BEST 0", FontLib.ChakraBold, 22f, t.amber);
            UIWidgets.SetTopLeft(bestLine, 34f, -274f, 592f, 34f);

            var retry = new UIBuilder().Panel("RetryBtn", card, ui.Primary)
                .Kicker("RETRY", 24f).Sized(260f, 58f)
                .FinishButton(() => GameRoot.Instance.StartRun());
            UIWidgets.SetTopLeft(retry, 34f, -350f, 260f, 58f);

            var title = new UIBuilder().Panel("TitleBtn", card, ui.Exit)
                .Kicker("TITLE", 24f).Sized(260f, 58f)
                .FinishButton(() => GameRoot.Instance.ReturnToTitle());
            UIWidgets.SetTopLeft(title, 310f, -350f, 260f, 58f);
        }

        public void Show(int score, int wave, int kills, int best, bool isRecord)
        {
            root.SetActive(true);
            finalScore.text = "SCORE " + score.ToString("N0");
            finalWave.text = "WAVE " + wave;
            finalKills.text = "KILLS " + kills;
            bestLine.text = (isRecord ? "NEW BEST " : "BEST ") + best.ToString("N0");
        }

        public void Hide() => root.SetActive(false);
    }
}
