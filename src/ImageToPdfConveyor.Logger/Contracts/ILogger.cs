namespace ImageToPdfConveyor.Logger.Contracts
{
    using System;

    public interface ILogger
    {
        void Error(Exception e, string message);

        void Info(string message);

        void Warning(string message);
    }
}
