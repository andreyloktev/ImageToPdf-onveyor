namespace ImageToPdfConveyor.ObjectModel.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public interface IImage
    {
        string Name { get; }

        int Height { get; }

        int Width { get; }

        Stream DataStream { get; }
    }
}
