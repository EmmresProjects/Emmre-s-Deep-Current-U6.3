using UnityEngine;
using TMPro;

namespace DeepCurrent
{
    // The first screen: logo, controls, best score, and the way into the game or into the Diver Bay (customization).
    public class TitleScreen
    {
        readonly GameObject root;
        readonly TextMeshProUGUI titleBest;

        public TitleScreen(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            var t = ui.Theme;

            UIWidgets.Dimmer(layer);
            var card = UIWidgets.Dossier(layer, t.panel, 760f, 520f);

            var logo = UIWidgets.Text("Logo", card, "DEEP CURRENT", FontLib.Bungee, 50f, t.cyan);
            UIWidgets.SetTopLeft(logo, 30f, -26f, 700f, 68f);

            var tag = UIWidgets.Text("Tag", card,
                "v" + GameRoot.Version + " - move, shoot, clear waves, customize your diver.",
                FontLib.Chakra, 22f, t.ice);
            UIWidgets.SetTopLeft(tag, 34f, -108f, 692f, 32f);

            var controls = UIWidgets.Text("Controls", card,
                "Move: WASD / Arrow keys\nAim: Mouse\nShoot: Left click / Space / J\nPause: P / Esc\nStart or retry: Enter",
                FontLib.Chakra, 24f, t.ice);
            UIWidgets.SetTopLeft(controls, 34f, -152f, 692f, 158f);

            var dive = new UIBuilder().Panel("DiveBtn", card, ui.Primary)
                .Kicker("DIVE", 24f).Sized(220f, 58f)
                .FinishButton(() => GameRoot.Instance.StartRun());
            UIWidgets.SetTopLeft(dive, 34f, -330f, 220f, 58f);

            var diverBay = new UIBuilder().Panel("CustomizeBtn", card, ui.Ghost)
                .Kicker("DIVER BAY", 24f).Sized(250f, 58f)
                .FinishButton(() => UIManager.Instance.ShowCustomize());
            UIWidgets.SetTopLeft(diverBay, 272f, -330f, 250f, 58f);

            var quit = new UIBuilder().Panel("QuitBtn", card, ui.Exit)
                .Kicker("QUIT", 24f).Sized(180f, 58f)
                .FinishButton(() => GameRoot.Instance.QuitGame());
            UIWidgets.SetTopLeft(quit, 540f, -330f, 180f, 58f);

            titleBest = UIWidgets.Text("BestChip", card, "BEST SCORE 0", FontLib.ChakraBold, 22f, t.amber);
            UIWidgets.SetTopLeft(titleBest, 34f, -452f, 692f, 32f);
        }

        public void Show(int best)
        {
            root.SetActive(true);
            titleBest.text = "BEST SCORE " + best.ToString("N0");
        }

        public void Hide() => root.SetActive(false);
    }
}
