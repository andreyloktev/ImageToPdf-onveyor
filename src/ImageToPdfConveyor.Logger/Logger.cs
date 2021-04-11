namespace ImageToPdfConveyor.Logger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using ImageToPdfConveyor.Logger.Contracts;
    using ImageToPdfConveyor.Logger.Models;
    using Microsoft.Extensions.Options;

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

        public void Error(Exception e, string message)
        {
            var internalMessage = $"[Error]{DateTime.UtcNow} {message}\n{e.StackTrace}";
            WriteLogInternal(internalMessage);
        }

        public void Info(string message)
        {
            var internalMessage = $"[Info]{DateTime.UtcNow} {message}";
            WriteLogInternal(internalMessage);
        }

        public void Warning(string message)
        {
            var internalMessage = $"[Warning]{DateTime.UtcNow} {message}";
            WriteLogInternal(internalMessage);
        }

        private void WriteLogInternal(string message)
        {
            try
            {
                var innerMessage = Encoding.UTF8.GetBytes(message);
                if (file != null)
                {
                    lock (file)
                    {
                        file.Write(innerMessage, 0, innerMessage.Length);
                    }
                }

                if (enableConsoleLogging)
                {
                    Console.WriteLine(innerMessage);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
