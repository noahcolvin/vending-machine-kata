namespace VendingMachine.Console
{
    public class ConsoleWriter : IConsoleWriter
    {
        public void Write(string message)
        {
            Colorful.Console.WriteLine(message);
        }
    }
}