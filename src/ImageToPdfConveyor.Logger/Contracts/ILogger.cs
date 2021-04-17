namespace ImageToPdfConveyor.Logger.Contracts
{
    using System;

    public interface ILogger : IDisposable
    {
        void Error(Exception e, string message, bool onlyConsole = false);

        void Info(string message, bool onlyConsole = false);

        void Warning(string message, bool onlyConsole = false);
    }
}
