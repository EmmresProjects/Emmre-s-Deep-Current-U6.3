using UnityEngine;

namespace DeepCurrent
{
    // Command Pattern: weld hull plating back on. PlayerController is the receiver.
    public class RepairHullCommand : ICommand
    {
        readonly PlayerController player;
        readonly float amount;

        public RepairHullCommand(PlayerController player, float amount)
        {
            this.player = player;
            this.amount = amount;
        }

        public void Execute()
        {
            player.Heal(amount);
            UIManager.Instance.Banner("+" + Mathf.RoundToInt(amount) + " HULL");
        }
    }
}
