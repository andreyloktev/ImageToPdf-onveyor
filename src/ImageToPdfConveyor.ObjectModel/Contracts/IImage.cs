namespace ImageToPdfConveyor.ObjectModel.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public interface IImage : IDisposable
    {
        string Name { get; }

        int Height { get; }

        int Width { get; }

        Stream DataStream { get; }
    }
}
