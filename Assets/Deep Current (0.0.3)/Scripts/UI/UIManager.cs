using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DeepCurrent
{
    // Owns the canvas and one object per screen.
    // Each screen builds itself, so this class only shows the right screen for each game state.
    [DefaultExecutionOrder(-40)]
    public class UIManager : Singleton<UIManager>
    {
        public Theme Theme => GameRoot.Instance.theme;
        public ButtonStyle Primary { get; private set; }
        public ButtonStyle Ghost { get; private set; }
        public ButtonStyle Exit { get; private set; }

        Canvas canvas;
        HudView hud;
        TitleScreen title;
        CustomizeScreen customize;
        PauseScreen pause;
        GameOverScreen over;

        protected override void Awake()
        {
            base.Awake();
            UIWidgets.WhiteSprite = Resources.Load<Sprite>("Sprites/UI/ui_white");
            if (UIWidgets.WhiteSprite == null)
                Debug.LogWarning("[DeepCurrent] Missing sprite: Sprites/UI/ui_white");

            BuildCanvas();

            var t = Theme;
            Primary = new ButtonStyle(t.amber, t.abyss, new Color(1.12f, 1.12f, 1.12f, 1f));
            Ghost = new ButtonStyle(t.panel, t.ice, new Color(1.2f, 1.2f, 1.2f, 1f));
            Exit = new ButtonStyle(t.panel, t.coral, new Color(1.2f, 1.2f, 1.2f, 1f));

            hud = new HudView(this, NewLayer("HUD"));
            title = new TitleScreen(this, NewLayer("TitleScreen"));
            customize = new CustomizeScreen(this, NewLayer("CustomizeScreen"));
            pause = new PauseScreen(this, NewLayer("PauseScreen"));
            over = new GameOverScreen(this, NewLayer("OverScreen"));
            HideAll();
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
            return UIWidgets.Stretch(UIWidgets.Rect(name, canvas.transform));
        }

        void HideAll()
        {
            hud.Show(false);
            title.Hide();
            customize.Hide();
            pause.Hide();
            over.Hide();
        }

        public void ShowTitle(int best) { HideAll(); title.Show(best); }
        public void ShowCustomize() { HideAll(); customize.Show(); }
        public void ShowGameplay() { HideAll(); hud.Show(true); hud.ClearBanner(); }
        public void ShowPause(bool on) { pause.Show(on); }

        public void ShowGameOver(int score, int wave, int kills, int best, bool isRecord)
        {
            HideAll();
            over.Show(score, wave, kills, best, isRecord);
        }

        public void Banner(string txt) => hud.Banner(txt);

        void Update()
        {
            if (hud != null) hud.Refresh(Time.unscaledDeltaTime);
        }
    }
}
