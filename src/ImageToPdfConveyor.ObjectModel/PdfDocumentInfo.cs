namespace ImageToPdfConveyor.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class PdfDocumentInfo
    {
        public string FilePath { get; set; }

        public DateTime Created { get; set; }
    }
}
