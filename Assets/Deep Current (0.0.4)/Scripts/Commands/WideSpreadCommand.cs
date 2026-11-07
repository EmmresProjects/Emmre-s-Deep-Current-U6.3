namespace DeepCurrent
{
    // Command Pattern: triple the bolt spread. PlayerShooting reads the timer when it fires.
    // PlayerPowerups is the receiver.
    public class WideSpreadCommand : ICommand
    {
        readonly PlayerPowerups powerups;
        readonly float seconds;

        public WideSpreadCommand(PlayerPowerups powerups, float seconds)
        {
            this.powerups = powerups;
            this.seconds = seconds;
        }

        public void Execute()
        {
            powerups.ActivateSpread(seconds);
            UIManager.Instance.Banner("WIDE SPREAD");
        }
    }
}
