namespace ImageToPdfConveyor.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using ImageToPdfConveyor.ObjectModel.Contracts;

    public sealed class ImagesCollection : IDisposable
    {
        public ImagesCollection(IReadOnlyCollection<IImage> images)
        {
            Images = images;
        }

        public void Dispose()
        {
            foreach (var image in Images)
            {
                image.Dispose();
            }
        }

        public IReadOnlyCollection<IImage> Images { get; private set; }
    }
}
