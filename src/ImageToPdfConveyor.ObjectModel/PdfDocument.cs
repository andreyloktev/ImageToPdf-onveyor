namespace ImageToPdfConveyor.ObjectModel
{
    using System.Collections.Generic;

    public sealed class PdfDocument
    {
        private readonly List<PdfPage> pages = new List<PdfPage>();

        public PdfDocument(string title, DocumentPageSize pageSize)
        {
            Title = title;
            PageSize = pageSize;
        }

        public string Title { get; private set; }

        public IReadOnlyCollection<PdfPage> Pages
        {
            get
            {
                return pages;
            }
        }

        public DocumentPageSize PageSize { get; private set; }

        public void AddPage(PdfPage page)
        {
            pages.Add(page);
        }

        public void AddPages(IReadOnlyCollection<PdfPage> pages)
        {
            this.pages.AddRange(pages);
        }
    }
}
