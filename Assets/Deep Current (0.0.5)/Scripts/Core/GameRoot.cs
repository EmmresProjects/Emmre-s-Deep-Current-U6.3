using UnityEngine;

namespace DeepCurrent
{
    [DefaultExecutionOrder(-50)]
    public class GameRoot : Singleton<GameRoot>
    {
        public const string Version = "0.0.5";

        [Header("Data assets")]
        public GameSettings settings;
        public Theme theme;
        public DiverCatalog diverCatalog;

        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject bulletPrefab;
        public GameObject enemyBulletPrefab;
        public GameObject jellyPrefab;
        public GameObject anglerPrefab;
        public GameObject spitterPrefab;
        public GameObject elitePrefab;
        public GameObject capsulePrefab;

        GameServices services;
        GameStateController stateController;

        public GameState State => stateController != null ? stateController.Current : null;
        public bool IsPlaying => State != null && State.IsPlaying;
        public bool IsPaused => State != null && State.IsPaused;

        public ScoreKeeper Scores => services != null ? services.Scores : null;
        public WaveDirector Director => services != null ? services.Director : null;
        public EnemySpawner Spawner => services != null ? services.Spawner : null;
        public ProjectileSpawner Projectiles => services != null ? services.Projectiles : null;
        public CapsuleSpawner Capsules => services != null ? services.Capsules : null;
        public CommandInvoker Commands => services != null ? services.Commands : null;
        public PlayerController Player => services != null ? services.Player : null;

        public int WaveNum => Director != null ? Director.WaveNum : 0;
        public int ContactCount => (Director != null ? Director.QueuedCount : 0)
                                   + (Spawner != null ? Spawner.Count : 0);
        public float HullPct => Player != null && settings != null && settings.playerMaxHp > 0f
            ? Mathf.Clamp01(Player.Hp / settings.playerMaxHp)
            : 1f;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
            stateController = new GameStateController(this);
            services = new GameServices(this);
        }

        void Start()
        {
            services.CreatePlayer();
            ChangeState(new TitleState());
        }

        void Update() => stateController.Tick(SimTime.Delta);

        void OnApplicationFocus(bool focused)
        {
            if (!focused) stateController.OnFocusLost();
        }

        public void ChangeState(GameState next) => stateController.ChangeState(next);
        public void StartRun() => stateController.StartRun();
        public void TogglePause() => stateController.TogglePause();
        public void ReturnToTitle() => stateController.ReturnToTitle();

        public void BeginRun() => services.Runs.BeginRun();
        public void ClearRun() => services.Runs.ClearRun();
        public void KillEnemy(Enemy enemy, bool byPlayer) => services.Runs.KillEnemy(enemy, byPlayer);

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
