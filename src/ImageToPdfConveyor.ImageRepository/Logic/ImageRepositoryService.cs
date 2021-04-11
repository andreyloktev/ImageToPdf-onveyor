namespace ImageToPdfConveyor.ImageRepository.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ImageToPdfConveyor.ObjectModel.Contracts;

    using ConveyorRectangle = ImageToPdfConveyor.ObjectModel.Rectangle;

    internal sealed class ImageRepositoryService : IImageRepositoryService
    {
        private readonly string[] files;

        public ImageRepositoryService(string path, string imageExtension)
        {
            files = Directory
                .GetFiles(path, $"*.{imageExtension}");

            Array.Sort(files);
        }

        public int GetFilesAmount()
        {
            return files.Length;
        }

        public async Task<IReadOnlyCollection<IImage>> GetImages(
            ConveyorRectangle dstRectangle,
            int? skip,
            int take,
            CancellationToken cancellationToken)
        {
            var task = Task.Run(() =>
            {
                return files
                    .Skip(skip ?? 0)
                    .Take(take)
                    .Select(file =>
                    {
                        return new Image(file);
                    })
                    .ToArray();
            });

            return await task;
        }
    }
}
