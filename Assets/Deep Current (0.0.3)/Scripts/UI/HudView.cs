using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeepCurrent
{
    // The in-game HUD: hull bar, score, wave, enemy count and the wave banner.
    // It only reads game values. The pause button asks GameRoot to toggle pause.
    public class HudView
    {
        const float PanelHeight = 66f;

        readonly GameObject root;
        readonly Image hullFill;
        readonly TextMeshProUGUI scoreVal, bestVal, waveVal, contactVal, bannerText;
        readonly Theme theme;
        float bannerTimer;

        public HudView(UIManager ui, RectTransform layer)
        {
            root = layer.gameObject;
            theme = ui.Theme;
            var t = theme;

            var hpPanel = Panel(layer, "HullPanel", 250f);
            UIWidgets.SetTopLeft(hpPanel, 16f, -12f, 250f, PanelHeight);
            var lbl = UIWidgets.Text("Lbl", hpPanel.transform, "HULL / HEALTH", FontLib.ChakraBold, 18f, t.ice);
            UIWidgets.SetTopLeft(lbl, 12f, -8f, 226f, 24f);
            var barBG = UIWidgets.Img("HullBar", hpPanel.transform, UIWidgets.WhiteSprite, t.abyss);
            UIWidgets.SetTopLeft(barBG, 12f, -40f, 226f, 14f);
            hullFill = UIWidgets.Img("HullFill", barBG.transform, UIWidgets.WhiteSprite, t.cyan);
            UIWidgets.Stretch((RectTransform)hullFill.transform);

            var scPanel = Panel(layer, "ScorePanel", 200f);
            UIWidgets.SetTopLeft(scPanel, 278f, -12f, 200f, PanelHeight);
            scoreVal = UIWidgets.Text("Score", scPanel.transform, "SCORE 0", FontLib.ChakraBold, 23f, t.ice);
            UIWidgets.SetTopLeft(scoreVal, 12f, -8f, 176f, 29f);
            bestVal = UIWidgets.Text("Best", scPanel.transform, "BEST 0", FontLib.Chakra, 17f, t.ice);
            UIWidgets.SetTopLeft(bestVal, 12f, -38f, 176f, 24f);

            var wvPanel = Panel(layer, "WavePanel", 230f);
            UIWidgets.SetTopLeft(wvPanel, 490f, -12f, 230f, PanelHeight);
            waveVal = UIWidgets.Text("Wave", wvPanel.transform, "WAVE 1", FontLib.ChakraBold, 23f, t.ice);
            UIWidgets.SetTopLeft(waveVal, 12f, -8f, 206f, 29f);
            contactVal = UIWidgets.Text("Contacts", wvPanel.transform, "ENEMIES 0", FontLib.Chakra, 17f, t.ice);
            UIWidgets.SetTopLeft(contactVal, 12f, -38f, 206f, 24f);

            var pauseBtn = new UIBuilder()
                .Panel("PauseBtn", layer, ui.Ghost)
                .Kicker("PAUSE", 20f)
                .Sized(116f, 56f)
                .FinishButton(() => GameRoot.Instance.TogglePause());
            UIWidgets.SetTopLeft(pauseBtn, 732f, -12f, 116f, 56f);

            bannerText = UIWidgets.Text("Banner", layer, "", FontLib.ChakraBold, 26f, t.ice,
                TextAlignmentOptions.Center);
            var brt = (RectTransform)bannerText.transform;
            brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 1f);
            brt.pivot = new Vector2(0.5f, 1f);
            brt.anchoredPosition = new Vector2(0f, -92f);
            brt.sizeDelta = new Vector2(900f, 42f);
        }

        Image Panel(RectTransform parent, string name, float width)
        {
            var img = UIWidgets.Img(name, parent, UIWidgets.WhiteSprite, theme.panel);
            ((RectTransform)img.transform).sizeDelta = new Vector2(width, PanelHeight);
            return img;
        }

        public void Show(bool on) => root.SetActive(on);

        public void Banner(string txt)
        {
            bannerText.text = txt;
            bannerTimer = GameRoot.Instance.settings.waveAnnounceTime;
        }

        public void ClearBanner()
        {
            bannerText.text = "";
            bannerTimer = 0f;
        }

        public void Refresh(float dt)
        {
            if (!root.activeSelf) return;
            var g = GameRoot.Instance;
            if (g == null) return;

            scoreVal.text = "SCORE " + g.Scores.Score.ToString("N0");
            bestVal.text = "BEST " + g.Scores.Best.ToString("N0");
            waveVal.text = "WAVE " + Mathf.Max(1, g.WaveNum);
            contactVal.text = "ENEMIES " + g.ContactCount;

            var hfrt = (RectTransform)hullFill.transform;
            hfrt.anchorMax = new Vector2(g.HullPct, 1f);
            hullFill.color = g.HullPct < 0.3f ? theme.coral : theme.cyan;

            // The banner stays up while the game is paused, and fades out while it runs.
            if (!g.IsPaused)
            {
                bannerTimer -= dt;
                if (bannerTimer <= 0f) bannerText.text = "";
            }
        }
    }
}
