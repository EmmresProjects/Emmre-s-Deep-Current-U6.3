namespace DeepCurrent
{
    // Simple Factory: hands out the command that matches a capsule kind.
    // Callers never write `new` for a command themselves.
    public static class CapsuleCommands
    {
        public static ICommand Create(CapsuleKind kind, PlayerController player, PlayerPowerups powerups, GameSettings settings)
        {
            switch (kind)
            {
                case CapsuleKind.Shield: return new ShieldCommand(powerups, settings.shieldDuration);
                case CapsuleKind.Rapid:  return new RapidFireCommand(powerups, settings.rapidDuration);
                case CapsuleKind.Spread: return new WideSpreadCommand(powerups, settings.spreadDuration);
                default:                 return new RepairHullCommand(player, settings.repairAmount);
            }
        }
    }
}
