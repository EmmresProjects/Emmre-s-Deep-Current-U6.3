namespace DeepCurrent
{
    // Command Pattern: one stand-alone object that carries everything one game action needs.
    // The code that picks the action up never needs to know what the action does.
    public interface ICommand
    {
        void Execute();
    }
}
