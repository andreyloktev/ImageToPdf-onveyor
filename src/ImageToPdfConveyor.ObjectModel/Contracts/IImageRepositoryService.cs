namespace ImageToPdfConveyor.ObjectModel.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IImageRepositoryService
    {
        int GetFilesAmount();

        Task<IReadOnlyCollection<IImage>> GetImages(
            Rectangle dstRectangle = null,
            int? skip = null,
            int take = 100,
            CancellationToken cancellationToken = default);
    }
}
