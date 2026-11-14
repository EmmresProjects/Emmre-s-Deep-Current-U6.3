using UnityEngine;

namespace DeepCurrent
{
    // Creates and connects the services used during a game run.
    public sealed class GameServices
    {
        readonly GameRoot game;

        public ScoreKeeper Scores { get; }
        public WaveDirector Director { get; }
        public EnemySpawner Spawner { get; }
        public ProjectileSpawner Projectiles { get; }
        public CapsuleSpawner Capsules { get; }
        public CommandInvoker Commands { get; }
        public RunCoordinator Runs { get; }
        public PlayerController Player { get; private set; }

        public GameServices(GameRoot gameRoot)
        {
            game = gameRoot;
            GamePrefabLoader.LoadMissing(game);

            Scores = new ScoreKeeper();
            Director = new WaveDirector(game.settings);
            Commands = new CommandInvoker();

            Transform world = new GameObject("World").transform;
            world.SetParent(game.transform, false);
            Spawner = new EnemySpawner(game.jellyPrefab, game.anglerPrefab,
                game.spitterPrefab, game.elitePrefab, world);
            Projectiles = new ProjectileSpawner(game.bulletPrefab, game.enemyBulletPrefab,
                world, game.settings);
            Capsules = new CapsuleSpawner(game.capsulePrefab, world, game.settings);
            Runs = new RunCoordinator(game);

            Director.OnSpawn += SpawnEnemy;
            Director.OnBanner += ShowBanner;
            Director.OnWaveClearBonus += Scores.AddBonus;
        }

        public void CreatePlayer()
        {
            DiverLoadout.Load();
            GameObject playerObject = Object.Instantiate(game.playerPrefab, game.transform);
            playerObject.name = "Player";
            Player = playerObject.GetComponent<PlayerController>();
            Player.ResetPlayer();
            playerObject.SetActive(false);
        }

        void SpawnEnemy(EnemyKind kind)
        {
            Spawner.SpawnAtEdge(kind, Director.WaveNum, Player.Pos);
        }

        static void ShowBanner(string text)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.Banner(text);
        }
    }
}
