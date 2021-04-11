namespace ImageToPdfConveyor
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class GetOffsetService
    {
        private int offset = 0;

        private readonly object lockObj = new object();

        private readonly int blockSize = 99;

        public GetOffsetService()
        {
        }

        public int GetNextOffset()
        {
            lock (lockObj)
            {
                var result = offset;
                offset += blockSize;
                return result;
            }
        }
    }

}
