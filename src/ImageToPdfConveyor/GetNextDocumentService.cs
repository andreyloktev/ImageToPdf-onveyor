namespace ImageToPdfConveyor
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class GetNextDocumentService
    {
        private int offset = 0;

        private int documentNumber = 0;

        private readonly object lockObj = new object();

        private readonly int blockSize;

        public GetNextDocumentService(int blockSize)
        {
            this.blockSize = blockSize;
        }

        public (int skip, int documentNumber) GetNextDocument()
        {
            lock (lockObj)
            {
                documentNumber++;
                var result = offset;
                offset += blockSize;
                return (result, documentNumber);
            }
        }
    }

}
