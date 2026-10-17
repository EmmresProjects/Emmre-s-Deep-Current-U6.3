namespace DeepCurrent
{
    // Jelly: It drifts at the player and pops on contact.
    // All of its numbers live on the EnemyJelly prefab, so there is no code to write here.
    public class EnemyJelly : Enemy
    {
        public override EnemyKind Kind => EnemyKind.Jelly;
    }
}
