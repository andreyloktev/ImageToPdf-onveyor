namespace ImageToPdfConveyor.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class PdfPage
    {
        public string Header { get; set; }

        public Stream DataStream { get; set; }
    }
}
