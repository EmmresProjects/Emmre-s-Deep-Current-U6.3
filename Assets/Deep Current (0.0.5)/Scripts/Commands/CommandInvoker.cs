namespace DeepCurrent
{
    // Command Pattern (Invoker): runs whatever command it is given.
    // It does not care which command it holds or what the command does.
    public class CommandInvoker
    {
        public void Execute(ICommand command)
        {
            if (command == null) return;

            command.Execute();
        }
    }
}
