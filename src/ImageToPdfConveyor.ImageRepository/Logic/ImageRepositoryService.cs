namespace ImageToPdfConveyor.ImageRepository.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ImageToPdfConveyor.Logger.Contracts;
    using ImageToPdfConveyor.ObjectModel.Contracts;

    using ConveyorRectangle = ImageToPdfConveyor.ObjectModel.Rectangle;

    internal sealed class ImageRepositoryService : IImageRepositoryService
    {
        private readonly string[] files;
        private readonly ILogger logger;

        public ImageRepositoryService(string path, string imageExtension, ILogger logger)
        {
            try
            {
                files = Directory
                    .GetFiles(path, $"*.{imageExtension}");
            }
            catch (Exception e)
            {
                logger.Error(e, $"Can not enumerate files in the directory {path}");
                return;
            }

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
                        try
                        {
                            return new Image(file);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, $"Can not create image object for {file}");
                            return null;
                        }
                    })
                    .ToArray();
            });

            return await task;
        }
    }
}
