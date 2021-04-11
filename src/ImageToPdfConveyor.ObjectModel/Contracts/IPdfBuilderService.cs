namespace ImageToPdfConveyor.ObjectModel.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IPdfBuilderService
    {
        PdfDocumentInfo BuildDocument(string directoryPath, PdfDocument document);
    }
}
