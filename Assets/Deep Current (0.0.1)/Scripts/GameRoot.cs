using System.Collections.Generic;
using UnityEngine;

namespace DeepCurrent
{
    // Basic rules as of the moment. Just to make it work as much as possible.
    [DefaultExecutionOrder(-50)]
    public class GameRoot : MonoBehaviour
    {
        [Header("Prefabs (auto-loaded from Resources if empty)")]
        public GameObject playerPrefab;
        public GameObject bulletPrefab;
        public GameObject enemyBulletPrefab;
        public GameObject jellyPrefab;
        public GameObject anglerPrefab;
        public GameObject spitterPrefab;
        public GameObject elitePrefab;

        public GameMode Mode { get; private set; } = GameMode.Title;
        public int Score { get; private set; }
        public int Kills { get; private set; }
        public int Best { get; private set; }
        public int WaveNum { get; private set; }

        enum WaveState { Idle, Announce, Active, Clear }
        WaveState waveState = WaveState.Idle;
        float announceT, clearT, releaseT, deathT;
        float wallT;
        readonly List<EnemyKind> queue = new List<EnemyKind>();

        public PlayerController Player { get; private set; }
        readonly List<Bullet> bullets = new List<Bullet>();
        readonly List<EnemyBullet> ebullets = new List<EnemyBullet>();
        readonly List<Enemy> enemies = new List<Enemy>();

        GOPool bulletPool, ebulletPool;
        readonly GOPool[] enemyPools = new GOPool[4];

        public int ContactCount { get { return enemies.Count + queue.Count; } }

        void Awake()
        {
            DC.Main = this;
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
            Best = PlayerPrefs.GetInt("deepcurrent_best", 0);
            LoadPrefabs();
        }

        void LoadPrefabs()
        {
            if (playerPrefab == null) playerPrefab = Res.Load<GameObject>("Prefabs/Player");
            if (bulletPrefab == null) bulletPrefab = Res.Load<GameObject>("Prefabs/Bullet");
            if (enemyBulletPrefab == null) enemyBulletPrefab = Res.Load<GameObject>("Prefabs/EnemyBullet");
            if (jellyPrefab == null) jellyPrefab = Res.Load<GameObject>("Prefabs/EnemyJelly");
            if (anglerPrefab == null) anglerPrefab = Res.Load<GameObject>("Prefabs/EnemyAngler");
            if (spitterPrefab == null) spitterPrefab = Res.Load<GameObject>("Prefabs/EnemySpitter");
            if (elitePrefab == null) elitePrefab = Res.Load<GameObject>("Prefabs/EnemyElite");
        }

        void Start()
        {
            var world = new GameObject("World").transform;
            world.SetParent(transform, false);

            // Start empty so new objects can be seen being made during play. After use, the same objects can be reused by the pool.
            bulletPool = new GOPool(bulletPrefab, world, "Bullet", 0);
            ebulletPool = new GOPool(enemyBulletPrefab, world, "EBullet", 0);
            enemyPools[0] = new GOPool(jellyPrefab, world, "Jelly", 0);
            enemyPools[1] = new GOPool(anglerPrefab, world, "Angler", 0);
            enemyPools[2] = new GOPool(spitterPrefab, world, "Spitter", 0);
            enemyPools[3] = new GOPool(elitePrefab, world, "Elite", 0);

            var pgo = Instantiate(playerPrefab, world);
            pgo.name = "Player_DSV07";
            Player = pgo.GetComponent<PlayerController>();
            Player.ResetPlayer();
            pgo.SetActive(false);
            DC.UI.ShowTitle(Best);
        }

        void Update()
        {
            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            HandleHotkeys();
            if (Mode == GameMode.Playing)
            {
                wallT += dt;
                UpdatePlaying(dt, !Player.dead);
            }
            DC.UI.UpdateHUD(dt);
        }

        void HandleHotkeys()
        {
            if (DC.Input == null) return;
            if (DC.Input.PausePressed)
            {
                if (Mode == GameMode.Playing) Pause();
                else if (Mode == GameMode.Paused) Resume();
            }
            if (DC.Input.EnterPressed && (Mode == GameMode.Title || Mode == GameMode.Over))
                StartGame();
        }

        void BuildQueue(int w)
        {
            queue.Clear();
            int count = Mathf.Min(6 + w * 2, 24);
            for (int i = 0; i < count; i++)
            {
                float r = Random.value;
                EnemyKind k = EnemyKind.Angler;
                if (w >= 3 && r < 0.22f) k = EnemyKind.Spitter;
                else if (r < 0.62f) k = EnemyKind.Jelly;
                queue.Add(k);
            }
            // Show two different factory results in every wave, even wave 1.
            queue[0] = EnemyKind.Jelly;
            queue[1] = EnemyKind.Angler;
            int elites = w >= 4 ? w / 4 : 0;
            for (int i = 0; i < elites; i++) queue.Add(EnemyKind.Elite);
            // Keep the first two slots, shuffle only the remaining enemies.
            for (int i = queue.Count - 1; i > 2; i--)
            {
                int j = Random.Range(2, i + 1);
                var t = queue[i]; queue[i] = queue[j]; queue[j] = t;
            }
        }

        void StartWave(int n)
        {
            WaveNum = n;
            waveState = WaveState.Announce;
            announceT = DC.WaveAnnounceTime;
            BuildQueue(n);
            DC.UI.Banner("WAVE " + n + " - ENEMIES INCOMING");
        }

        Vector2 EdgePos()
        {
            float marginX = Mathf.Max(45f, Mathf.Min(85f, Arena.W * 0.08f));
            float marginY = Mathf.Max(45f, Mathf.Min(85f, Arena.H * 0.08f));
            float x = 0f, y = 0f;
            Vector2 pp = Player.Pos;
            for (int i = 0; i < 20; i++)
            {
                int s = Random.Range(0, 4);
                if (s == 0) { x = Random.Range(-Arena.HalfW + marginX, Arena.HalfW - marginX); y = Arena.HalfH - marginY; }
                else if (s == 1) { x = Arena.HalfW - marginX; y = Random.Range(-Arena.HalfH + marginY, Arena.HalfH - marginY); }
                else if (s == 2) { x = Random.Range(-Arena.HalfW + marginX, Arena.HalfW - marginX); y = -Arena.HalfH + marginY; }
                else { x = -Arena.HalfW + marginX; y = Random.Range(-Arena.HalfH + marginY, Arena.HalfH - marginY); }
                if ((new Vector2(x, y) - pp).sqrMagnitude > 160f * 160f) break;
            }
            return new Vector2(x, y);
        }

        void UpdateWaveLogic(float dt)
        {
            if (waveState == WaveState.Announce)
            {
                announceT -= dt;
                if (announceT <= 0f) { waveState = WaveState.Active; releaseT = 0.3f; }
            }
            else if (waveState == WaveState.Active)
            {
                if (queue.Count > 0)
                {
                    releaseT -= dt;
                    if (releaseT <= 0f)
                    {
                        releaseT = Mathf.Max(0.35f, 1.15f - WaveNum * 0.05f);
                        var kind = queue[0];
                        queue.RemoveAt(0);
                        SpawnEnemy(kind, EdgePos());
                    }
                }
                else if (enemies.Count == 0)
                {
                    waveState = WaveState.Clear;
                    int bonus = 100 + WaveNum * 50;
                    Score += bonus;
                    DC.UI.Banner("WAVE CLEAR - BONUS " + bonus);
                    clearT = DC.WaveClearTime;
                }
            }
            else if (waveState == WaveState.Clear)
            {
                clearT -= dt;
                if (clearT <= 0f) StartWave(WaveNum + 1);
            }
        }

        public void SpawnPlayerBullet(Vector2 pos, Vector2 vel, float ang)
        {
            var b = bulletPool.Get().GetComponent<Bullet>();
            b.Setup(pos, vel, ang);
            bullets.Add(b);
        }

        public void SpawnEnemyBullet(Vector2 pos, Vector2 vel, float r, float life)
        {
            var b = ebulletPool.Get().GetComponent<EnemyBullet>();
            b.Setup(pos, vel, r, life);
            ebullets.Add(b);
        }

        // Pick the prefab factory by kind, then return a ready enemy.
        // This is the Simple Factory form, not a new enemy class for each type.
        Enemy SpawnEnemy(EnemyKind kind, Vector2 pos)
        {
            var go = enemyPools[(int)kind].Get();
            var e = go.GetComponent<Enemy>();
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            e.Setup(WaveNum);
            enemies.Add(e);
            return e;
        }

        public void KillEnemy(Enemy e, bool byPlayer)
        {
            if (!enemies.Remove(e)) return;
            if (byPlayer)
            {
                Score += DC.EnemyScore(e.Kind);
                Kills++;
            }
            enemyPools[(int)e.Kind].Release(e.gameObject);
        }

        public void OnPlayerDied()
        {
            deathT = DC.DeathDelay;
        }

        void UpdatePlaying(float dt, bool live)
        {
            if (live) UpdateWaveLogic(dt);
            if (Player.dead)
            {
                deathT -= dt;
                if (deathT <= 0f) { EndGame(); return; }
            }
            Player.UpdatePlayer(dt, wallT, live);
            for (int i = enemies.Count - 1; i >= 0; i--)
                enemies[i].UpdateEnemy(dt, wallT, live, Player, WaveNum);
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].UpdateVisuals(dt, wallT);
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].UpdateBullet(dt, live, enemies))
                {
                    bulletPool.Release(bullets[i].gameObject);
                    bullets.RemoveAt(i);
                }
            }
            for (int i = ebullets.Count - 1; i >= 0; i--)
            {
                if (ebullets[i].UpdateEnemyBullet(dt, live, Player))
                {
                    ebulletPool.Release(ebullets[i].gameObject);
                    ebullets.RemoveAt(i);
                }
            }
        }

        void ResetGame()
        {
            foreach (var b in bullets) bulletPool.Release(b.gameObject);
            bullets.Clear();
            foreach (var b in ebullets) ebulletPool.Release(b.gameObject);
            ebullets.Clear();
            foreach (var e in enemies) enemyPools[(int)e.Kind].Release(e.gameObject);
            enemies.Clear();
            queue.Clear();
            Score = 0;
            Kills = 0;
            WaveNum = 0;
            waveState = WaveState.Idle;
            Player.gameObject.SetActive(true);
            Player.ResetPlayer();
        }

        public void StartGame()
        {
            ResetGame();
            Mode = GameMode.Playing;
            DC.UI.ShowGameplay();
            StartWave(1);
        }

        public void Pause()
        {
            if (Mode != GameMode.Playing) return;
            Mode = GameMode.Paused;
            DC.UI.ShowPause(true);
        }

        public void Resume()
        {
            if (Mode != GameMode.Paused) return;
            Mode = GameMode.Playing;
            DC.UI.ShowPause(false);
        }

        public void GoToTitle()
        {
            Mode = GameMode.Title;
            ResetGame();
            Player.gameObject.SetActive(false);
            DC.UI.ShowTitle(Best);
        }

        void EndGame()
        {
            Mode = GameMode.Over;
            bool isRecord = Score > Best;
            if (isRecord)
            {
                Best = Score;
                PlayerPrefs.SetInt("deepcurrent_best", Best);
                PlayerPrefs.Save();
            }
            DC.UI.ShowGameOver(Score, WaveNum, Kills, Best, isRecord);
        }

        void OnApplicationFocus(bool focused)
        {
            if (!focused && Mode == GameMode.Playing) Pause();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public float WallTime { get { return wallT; } }
        public float HullPct { get { return Player != null ? Mathf.Clamp01(Player.hp / DC.PlayerMaxHp) : 1f; } }
    }
}
