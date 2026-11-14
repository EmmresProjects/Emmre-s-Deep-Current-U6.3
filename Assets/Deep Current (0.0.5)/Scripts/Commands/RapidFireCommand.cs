namespace DeepCurrent
{
    // Command Pattern: overcharge the bolt launcher. PlayerShooting reads the timer when it fires.
    // PlayerPowerups is the receiver.
    public class RapidFireCommand : ICommand
    {
        readonly PlayerPowerups powerups;
        readonly float seconds;

        public RapidFireCommand(PlayerPowerups powerups, float seconds)
        {
            this.powerups = powerups;
            this.seconds = seconds;
        }

        public void Execute()
        {
            powerups.ActivateRapid(seconds);
            UIManager.Instance.Banner("RAPID FIRE");
        }
    }
}
