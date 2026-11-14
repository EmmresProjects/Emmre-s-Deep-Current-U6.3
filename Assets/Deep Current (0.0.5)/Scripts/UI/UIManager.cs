using UnityEngine;

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

        RectTransform canvasRoot;
        HudView hud;
        TitlePresenter titlePresenter;
        CustomizePresenter customizePresenter;
        PauseScreen pause;
        GameOverScreen over;

        protected override void Awake()
        {
            base.Awake();
            UIWidgets.WhiteSprite = Resources.Load<Sprite>("Sprites/UI/ui_white");
            if (UIWidgets.WhiteSprite == null)
                Debug.LogWarning("[DeepCurrent] Missing sprite: Sprites/UI/ui_white");

            canvasRoot = CanvasFactory.Create(transform);

            var t = Theme;
            Primary = new ButtonStyle(t.amber, t.abyss, new Color(1.12f, 1.12f, 1.12f, 1f));
            Ghost = new ButtonStyle(t.panel, t.ice, new Color(1.2f, 1.2f, 1.2f, 1f));
            Exit = new ButtonStyle(t.panel, t.coral, new Color(1.2f, 1.2f, 1.2f, 1f));

            hud = new HudView(this, NewLayer("HUD"));
            ITitleView titleView = new TitleScreen(this, NewLayer("TitleScreen"));
            ICustomizeView customizeView = new CustomizeScreen(this, NewLayer("CustomizeScreen"));
            titlePresenter = new TitlePresenter(titleView, GameRoot.Instance, ShowCustomize);
            customizePresenter = new CustomizePresenter(customizeView, GameRoot.Instance, ShowTitle);
            pause = new PauseScreen(this, NewLayer("PauseScreen"));
            over = new GameOverScreen(this, NewLayer("OverScreen"));
            HideAll();
        }

        RectTransform NewLayer(string name)
        {
            return UIWidgets.Stretch(UIWidgets.Rect(name, canvasRoot));
        }

        void HideAll()
        {
            hud.Show(false);
            titlePresenter.Hide();
            customizePresenter.Hide();
            pause.Hide();
            over.Hide();
        }

        public void ShowTitle() { HideAll(); titlePresenter.Show(); }
        public void ShowCustomize() { HideAll(); customizePresenter.Show(); }
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
