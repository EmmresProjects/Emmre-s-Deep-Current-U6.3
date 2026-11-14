using UnityEngine;

namespace DeepCurrent
{
    // Keeps run setup, cleanup, and enemy rewards out of the game root.
    public sealed class RunCoordinator
    {
        readonly GameRoot game;

        public RunCoordinator(GameRoot gameRoot)
        {
            game = gameRoot;
        }

        public void BeginRun()
        {
            ClearRun();
            game.Player.gameObject.SetActive(true);
            game.Player.ResetPlayer();
            DiverBuilder.FromLoadout(game.diverCatalog).ApplyTo(game.Player);
            UIManager.Instance.ShowGameplay();
            game.Director.StartWave(1);
        }

        public void ClearRun()
        {
            Time.timeScale = 1f;
            game.Projectiles.ClearAll();
            game.Capsules.ClearAll();
            game.Spawner.DespawnAll();
            game.Director.Reset();
            game.Scores.ResetRun();
            game.Player.gameObject.SetActive(false);
        }

        public void KillEnemy(Enemy enemy, bool byPlayer)
        {
            if (!game.Spawner.Despawn(enemy) || !byPlayer) return;

            game.Scores.AddKill(enemy.score);
            game.Capsules.TryDrop(enemy.Pos, enemy.Kind == EnemyKind.Elite);
        }
    }
}
