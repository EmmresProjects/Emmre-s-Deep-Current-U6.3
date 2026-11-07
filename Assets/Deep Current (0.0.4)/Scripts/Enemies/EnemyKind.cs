namespace DeepCurrent
{
    // Every enemy type in the game.
    // If you want to add one, add a subclass of Enemy, a prefab, and a factory line in EnemySpawner.
    public enum EnemyKind
    {
        Jelly = 0,
        Angler = 1,
        Spitter = 2,
        Elite = 3
    }
}
