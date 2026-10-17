using UnityEngine;

namespace DeepCurrent
{
    // The conductor. It owns the state machine, the score, the wave director and the two spawners, and it connects them.
    // It forwards input to the current state.
    // It does not draw anything and does not run the rules of single enemies or bullets.
    //
    //  If you don't want a feature anymore, you want change it, or add to it, check below:"
    //   Balance Numbers = GameSettings asset
    //   Colors = Theme asset
    //   Skins & paints = DiverCatalog asset
    //   A new enemy = Enemies folder (new prefab, class and factory)
    //   A new screen mode = States folder (one class per state)
    [DefaultExecutionOrder(-50)]
    public class GameRoot : Singleton<GameRoot>
    {
        public const string Version = "0.0.3";

        // Longest step the simulation takes in one frame.
        // Longer frames are cut short so a lag spike does not make bullets tunnel through enemies.
        const float MaxFrameTime = 0.05f;

        [Header("Data assets (edit these, not code)")]
        public GameSettings settings;
        public Theme theme;
        public DiverCatalog diverCatalog;

        [Header("Prefabs (auto-loaded from Resources/Prefabs if empty)")]
        public GameObject playerPrefab;
        public GameObject bulletPrefab;
        public GameObject enemyBulletPrefab;
        public GameObject jellyPrefab;
        public GameObject anglerPrefab;
        public GameObject spitterPrefab;
        public GameObject elitePrefab;

        public GameState State { get; private set; }
        public bool IsPlaying => State.IsPlaying;
        public bool IsPaused => State.IsPaused;

        public ScoreKeeper Scores { get; private set; }
        public WaveDirector Director { get; private set; }
        public EnemySpawner Spawner { get; private set; }
        public ProjectileSpawner Projectiles { get; private set; }
        public PlayerController Player { get; private set; }

        public int WaveNum => Director.WaveNum;
        public int ContactCount => Director.QueuedCount + Spawner.Count;
        public float HullPct => Player != null ? Mathf.Clamp01(Player.Hp / settings.playerMaxHp) : 1f;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
            LoadPrefabsIfEmpty();

            Scores = new ScoreKeeper();
            Director = new WaveDirector(settings);

            var world = new GameObject("World").transform;
            world.SetParent(transform, false);
            Spawner = new EnemySpawner(jellyPrefab, anglerPrefab, spitterPrefab, elitePrefab, world);
            Projectiles = new ProjectileSpawner(bulletPrefab, enemyBulletPrefab, world, settings);

            Director.OnSpawn += kind => Spawner.SpawnAtEdge(kind, WaveNum, Player.Pos);
            Director.OnBanner += txt => UIManager.Instance.Banner(txt);
            Director.OnWaveClearBonus += bonus => Scores.AddBonus(bonus);
        }

        void LoadPrefabsIfEmpty()
        {
            if (playerPrefab == null) playerPrefab = Load("Prefabs/Player");
            if (bulletPrefab == null) bulletPrefab = Load("Prefabs/Bullet");
            if (enemyBulletPrefab == null) enemyBulletPrefab = Load("Prefabs/EnemyBullet");
            if (jellyPrefab == null) jellyPrefab = Load("Prefabs/EnemyJelly");
            if (anglerPrefab == null) anglerPrefab = Load("Prefabs/EnemyAngler");
            if (spitterPrefab == null) spitterPrefab = Load("Prefabs/EnemySpitter");
            if (elitePrefab == null) elitePrefab = Load("Prefabs/EnemyElite");
        }

        static GameObject Load(string path)
        {
            var go = Resources.Load<GameObject>(path);
            if (go == null) Debug.LogWarning("[DeepCurrent] Missing prefab: " + path);
            return go;
        }

        void Start()
        {
            DiverLoadout.Load();
            var go = Instantiate(playerPrefab, transform);
            go.name = "Player";
            Player = go.GetComponent<PlayerController>();
            Player.ResetPlayer();
            go.SetActive(false);

            ChangeState(new TitleState());
        }

        void Update()
        {
            var input = InputBridge.Instance;
            if (input != null)
            {
                if (input.PausePressed) State.OnPauseRequested(this);
                if (input.EnterPressed) State.OnStartRequested(this);
            }

            State.Tick(this, Mathf.Min(Time.deltaTime, MaxFrameTime));
        }

        void OnApplicationFocus(bool focused)
        {
            if (!focused) State?.OnFocusLost(this);
        }

        //State control (called by the states and by the UI buttons)

        public void ChangeState(GameState next)
        {
            State?.Exit(this);
            State = next;
            State.Enter(this);
        }

        public void StartRun()
        {
            ChangeState(new PlayingState(true));
        }

        public void TogglePause()
        {
            State.OnPauseRequested(this);
        }

        public void ReturnToTitle()
        {
            ChangeState(new TitleState());
        }

        // Sets up a new run: clears the field, resets the player and starts wave 1.
        public void BeginRun()
        {
            ClearRun();
            Player.gameObject.SetActive(true);
            Player.ResetPlayer();
            DiverBuilder.FromLoadout(diverCatalog).ApplyTo(Player);
            UIManager.Instance.ShowGameplay();
            Director.StartWave(1);
        }

        // Removes every bullet and enemy and hides the player.
        public void ClearRun()
        {
            Time.timeScale = 1f;
            Projectiles.ClearAll();
            Spawner.DespawnAll();
            Director.Reset();
            Scores.ResetRun();
            Player.gameObject.SetActive(false);
        }

        // Called by an enemy when it is hit or touches the player.
        public void KillEnemy(Enemy e, bool byPlayer)
        {
            if (!Spawner.Despawn(e)) return;
            if (byPlayer) Scores.AddKill(e.score);
        }

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
