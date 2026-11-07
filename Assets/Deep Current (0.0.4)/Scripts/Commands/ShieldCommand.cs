namespace DeepCurrent
{
    // Command Pattern: sonar shield, so bolts and bodies bounce off for a while.
    // PlayerPowerups is the receiver.
    public class ShieldCommand : ICommand
    {
        readonly PlayerPowerups powerups;
        readonly float seconds;

        public ShieldCommand(PlayerPowerups powerups, float seconds)
        {
            this.powerups = powerups;
            this.seconds = seconds;
        }

        public void Execute()
        {
            powerups.ActivateShield(seconds);
            UIManager.Instance.Banner("SHIELD ONLINE");
        }
    }
}
