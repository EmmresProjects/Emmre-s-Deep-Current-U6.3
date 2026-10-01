using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace DeepCurrent
{
    // Plain screen, don't touch unless needed.
    [DefaultExecutionOrder(-60)]
    public class UIManager : MonoBehaviour
    {
        Canvas canvas;
        GameObject hudRoot;
        Image hullFill;
        TextMeshProUGUI scoreVal, bestVal, waveVal, contactVal, bannerText;
        GameObject titleScreen, pauseScreen, overScreen;
        TextMeshProUGUI titleBest, finalScore, finalWave, finalKills, overLine;
        float bannerT;

        void Awake()
        {
            DC.UI = this;
            UIFactory.LoadSprites();
            BuildCanvas();
            BuildHUD();
            BuildTitle();
            // Pause and Game Over are built when first needed, during play.
            HideAllScreens();
        }

        void BuildCanvas()
        {
            var go = new GameObject("Canvas", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                es.AddComponent<StandaloneInputModule>();
#endif
            }
        }

        RectTransform NewLayer(string name)
        {
            return UIFactory.Stretch(UIFactory.Rect(name, canvas.transform));
        }

        void BuildHUD()
        {
            var hud = NewLayer("HUD");
            hudRoot = hud.gameObject;
            var hpPanel = MakeHudPanel(hud, "HullPanel", 250f);
            SetTL(hpPanel, 16f, -12f, 250f, 66f);
            var hullLbl = UIFactory.Text("Lbl", hpPanel.transform, "HULL / HEALTH", FontLib.ChakraBold, 18f, DC.Ice);
            SetTL(hullLbl, 12f, -8f, 226f, 24f);
            var barBG = UIFactory.Img("HullBar", hpPanel.transform, UIFactory.WhiteSprite, DC.Abyss);
            SetTL(barBG, 12f, -40f, 226f, 14f);
            hullFill = UIFactory.Img("HullFill", barBG.transform, UIFactory.WhiteSprite, DC.Cyan);
            UIFactory.Stretch((RectTransform)hullFill.transform);

            var scPanel = MakeHudPanel(hud, "ScorePanel", 200f);
            SetTL(scPanel, 278f, -12f, 200f, 66f);
            scoreVal = UIFactory.Text("Score", scPanel.transform, "SCORE 0", FontLib.ChakraBold, 23f, DC.Ice);
            SetTL(scoreVal, 12f, -8f, 176f, 29f);
            bestVal = UIFactory.Text("Best", scPanel.transform, "BEST 0", FontLib.Chakra, 17f, DC.Ice);
            SetTL(bestVal, 12f, -38f, 176f, 24f);

            var wvPanel = MakeHudPanel(hud, "WavePanel", 230f);
            SetTL(wvPanel, 490f, -12f, 230f, 66f);
            waveVal = UIFactory.Text("Wave", wvPanel.transform, "WAVE 1", FontLib.ChakraBold, 23f, DC.Ice);
            SetTL(waveVal, 12f, -8f, 206f, 29f);
            contactVal = UIFactory.Text("Contacts", wvPanel.transform, "ENEMIES 0", FontLib.Chakra, 17f, DC.Ice);
            SetTL(contactVal, 12f, -38f, 206f, 24f);

            // Choose the steps and values for a Pause button.
            var pauseBtn = new UIFactory()
                .Panel("PauseBtn", hud, UIFactory.Ghost)
                .Kicker("PAUSE", 20f)
                .Fixed(116f, 56f)
                .MakeButton(() =>
                {
                    if (DC.Main.Mode == GameMode.Playing) DC.Main.Pause();
                    else if (DC.Main.Mode == GameMode.Paused) DC.Main.Resume();
                });
            SetTL(pauseBtn, 732f, -12f, 116f, 56f);

            bannerText = UIFactory.Text("Banner", hud, "", FontLib.ChakraBold, 26f,
                DC.Ice, TextAlignmentOptions.Center);
            var brt = (RectTransform)bannerText.transform;
            brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 1f);
            brt.pivot = new Vector2(0.5f, 1f);
            brt.anchoredPosition = new Vector2(0f, -92f);
            brt.sizeDelta = new Vector2(900f, 42f);
        }

        Image MakeHudPanel(Transform parent, string name, float width)
        {
            var p = UIFactory.Img(name, parent, UIFactory.WhiteSprite, DC.Panel);
            ((RectTransform)p.transform).sizeDelta = new Vector2(width, 66f);
            return p;
        }

        static void SetTL(Component c, float x, float y, float w, float h)
        {
            var rt = (RectTransform)c.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
        }

        GameObject MakeScreen(string name, float dossierW, float dossierH, out RectTransform content)
        {
            var layer = NewLayer(name);
            var dim = layer.gameObject.AddComponent<Image>();
            dim.sprite = UIFactory.WhiteSprite;
            dim.color = new Color(0f, 0f, 0f, 0.8f);
            dim.raycastTarget = true;
            var dossier = UIFactory.Img("Dossier", layer, UIFactory.WhiteSprite, DC.Panel);
            dossier.raycastTarget = true;
            content = (RectTransform)dossier.transform;
            content.sizeDelta = new Vector2(dossierW, dossierH);
            return layer.gameObject;
        }

        void BuildTitle()
        {
            RectTransform d;
            titleScreen = MakeScreen("TitleScreen", 760f, 480f, out d);
            var logo = UIFactory.Text("Logo", d, "DEEP CURRENT", FontLib.Bungee, 50f, DC.Cyan);
            SetTL(logo, 30f, -26f, 700f, 68f);
            var tag = UIFactory.Text("Tag", d, "Basic version: move, shoot, and clear each wave.", FontLib.Chakra, 22f, DC.Ice);
            SetTL(tag, 34f, -108f, 692f, 32f);
            var ctl = UIFactory.Text("Controls", d,
                "Move: WASD / Arrow keys\nAim: Mouse\nShoot: Left click / Space / J\nPause: P / Esc\nStart or retry: Enter",
                FontLib.Chakra, 24f, DC.Ice);
            SetTL(ctl, 34f, -156f, 692f, 158f);

            var dive = new UIFactory()
                .Panel("DiveBtn", d, UIFactory.Primary)
                .Kicker("DIVE", 24f)
                .Fixed(260f, 58f)
                .MakeButton(() => DC.Main.StartGame());
            SetBtn(dive, 34f, -330f, 260f, 58f);
            var quit = new UIFactory()
                .Panel("QuitBtn", d, UIFactory.Exit)
                .Kicker("QUIT", 24f)
                .Fixed(210f, 58f)
                .MakeButton(() => DC.Main.QuitGame());
            SetBtn(quit, 312f, -330f, 210f, 58f);
            titleBest = UIFactory.Text("BestChip", d, "BEST SCORE 0", FontLib.ChakraBold, 22f, DC.Amber);
            SetTL(titleBest, 34f, -414f, 692f, 32f);
        }

        static void SetBtn(Button b, float x, float y, float w, float h)
        {
            SetTL(b, x, y, w, h);
        }

        void BuildPause()
        {
            RectTransform d;
            pauseScreen = MakeScreen("PauseScreen", 620f, 350f, out d);
            var h = UIFactory.Text("H", d, "PAUSED", FontLib.Bungee, 44f, DC.Ice);
            SetTL(h, 30f, -28f, 560f, 60f);
            var tag = UIFactory.Text("Tag", d, "Press P / Esc or use the buttons below.", FontLib.Chakra, 22f, DC.Ice);
            SetTL(tag, 34f, -98f, 552f, 34f);
            var resume = new UIFactory()
                .Panel("ResumeBtn", d, UIFactory.Primary).Kicker("RESUME", 23f)
                .Fixed(260f, 56f).MakeButton(() => DC.Main.Resume());
            SetBtn(resume, 34f, -160f, 260f, 56f);
            var restart = new UIFactory()
                .Panel("RestartBtn", d, UIFactory.Ghost).Kicker("RESTART", 23f)
                .Fixed(260f, 56f).MakeButton(() => DC.Main.StartGame());
            SetBtn(restart, 310f, -160f, 260f, 56f);
            var exit = new UIFactory()
                .Panel("ExitBtn", d, UIFactory.Exit).Kicker("TITLE", 23f)
                .Fixed(260f, 56f).MakeButton(() => DC.Main.GoToTitle());
            SetBtn(exit, 34f, -238f, 260f, 56f);
            var quit = new UIFactory()
                .Panel("QuitBtn", d, UIFactory.Exit).Kicker("QUIT", 23f)
                .Fixed(260f, 56f).MakeButton(() => DC.Main.QuitGame());
            SetBtn(quit, 310f, -238f, 260f, 56f);
        }

        void BuildGameOver()
        {
            RectTransform d;
            overScreen = MakeScreen("OverScreen", 660f, 460f, out d);
            var h = UIFactory.Text("H", d, "GAME OVER", FontLib.Bungee, 46f, DC.Coral);
            SetTL(h, 30f, -26f, 600f, 66f);
            finalScore = UIFactory.Text("Score", d, "SCORE 0", FontLib.ChakraBold, 30f, DC.Ice);
            SetTL(finalScore, 34f, -124f, 592f, 42f);
            finalWave = UIFactory.Text("Wave", d, "WAVE 1", FontLib.Chakra, 26f, DC.Ice);
            SetTL(finalWave, 34f, -174f, 592f, 36f);
            finalKills = UIFactory.Text("Kills", d, "KILLS 0", FontLib.Chakra, 26f, DC.Ice);
            SetTL(finalKills, 34f, -218f, 592f, 36f);
            overLine = UIFactory.Text("OverLine", d, "BEST 0", FontLib.ChakraBold, 22f, DC.Amber);
            SetTL(overLine, 34f, -274f, 592f, 34f);
            var again = new UIFactory()
                .Panel("AgainBtn", d, UIFactory.Primary).Kicker("RETRY", 24f)
                .Fixed(260f, 58f).MakeButton(() => DC.Main.StartGame());
            SetBtn(again, 34f, -350f, 260f, 58f);
            var exit = new UIFactory()
                .Panel("ExitBtn", d, UIFactory.Exit).Kicker("TITLE", 24f)
                .Fixed(260f, 58f).MakeButton(() => DC.Main.GoToTitle());
            SetBtn(exit, 310f, -350f, 260f, 58f);
        }

        void HideAllScreens()
        {
            Show(titleScreen, false);
            Show(pauseScreen, false);
            Show(overScreen, false);
            hudRoot.SetActive(false);
        }

        void Show(GameObject screen, bool on)
        {
            if (screen != null) screen.SetActive(on);
        }

        public void ShowTitle(int best)
        {
            HideAllScreens();
            Show(titleScreen, true);
            titleBest.text = "BEST SCORE " + best.ToString("N0");
        }

        public void ShowGameplay()
        {
            HideAllScreens();
            hudRoot.SetActive(true);
            bannerText.text = "";
            bannerT = 0f;
        }

        public void ShowPause(bool on)
        {
            if (on && pauseScreen == null) BuildPause();
            Show(pauseScreen, on);
        }

        public void ShowGameOver(int score, int wave, int kills, int best, bool isRecord)
        {
            if (overScreen == null) BuildGameOver();
            HideAllScreens();
            finalScore.text = "SCORE " + score.ToString("N0");
            finalWave.text = "WAVE " + wave;
            finalKills.text = "KILLS " + kills;
            overLine.text = (isRecord ? "NEW BEST " : "BEST ") + best.ToString("N0");
            Show(overScreen, true);
        }

        public void Banner(string txt)
        {
            bannerText.text = txt;
            bannerT = DC.WaveAnnounceTime;
        }

        public void UpdateHUD(float dt)
        {
            var g = DC.Main;
            if (g == null || !hudRoot.activeSelf) return;
            scoreVal.text = "SCORE " + g.Score.ToString("N0");
            bestVal.text = "BEST " + g.Best.ToString("N0");
            waveVal.text = "WAVE " + Mathf.Max(1, g.WaveNum);
            contactVal.text = "ENEMIES " + g.ContactCount;
            var hfrt = (RectTransform)hullFill.transform;
            hfrt.anchorMax = new Vector2(g.HullPct, 1f);
            hullFill.color = g.HullPct < 0.3f ? DC.Coral : DC.Cyan;
            if (g.Mode != GameMode.Paused)
            {
                bannerT -= dt;
                if (bannerT <= 0f) bannerText.text = "";
            }
        }
    }
}
