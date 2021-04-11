namespace ImageToPdfConveyor.Logger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class LoggerOptions
    {
        public bool EnableConsoleLogging { get; set; }

        public string FilePath { get; set; }
    }
}
