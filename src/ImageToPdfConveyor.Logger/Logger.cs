namespace ImageToPdfConveyor.Logger
{
    using System;
    using System.IO;
    using System.Text;
    using ImageToPdfConveyor.Logger.Contracts;
    using ImageToPdfConveyor.Logger.Models;

    internal sealed class Logger : ILogger
    {
        private readonly FileStream file;
        private readonly bool enableConsoleLogging;

        public Logger(LoggerOptions options)
        {
            enableConsoleLogging = options.EnableConsoleLogging;

            var filePath = options.FilePath;
            if (filePath != null)
            {
                file = File.OpenWrite(filePath);
            }
        }

        public void Dispose()
        {
            file.Close();
            file.Dispose();
        }

        public void Error(Exception e, string message, bool onlyConsole)
        {
            WriteLogInternal($"[Error]{DateTime.UtcNow} ", $"{message}\n{e.StackTrace}", onlyConsole);
        }

        public void Info(string message, bool onlyConsole)
        {
            WriteLogInternal($"[Info]{DateTime.UtcNow} ", message, onlyConsole);
        }

        public void Warning(string message, bool onlyConsole)
        {
            WriteLogInternal($"[Warning]{DateTime.UtcNow} ", message, onlyConsole);
        }

        private void WriteLogInternal(string prefix, string message, bool onlyConsole)
        {
            try
            {
                if (!onlyConsole && (file != null))
                {
                    lock (file)
                    {
                        var msg = Encoding.UTF8.GetBytes(prefix + message);
                        file.Write(msg, 0, msg.Length);
                    }
                }

                if (enableConsoleLogging)
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
