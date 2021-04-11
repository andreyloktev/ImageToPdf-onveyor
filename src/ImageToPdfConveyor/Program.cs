namespace ImageToPdfConveyor
{
    using System;
    using System.IO;
    using ImageToPdfConveyor.ImageRepository.Logic;
    using Microsoft.Extensions.DependencyInjection;
    using ImageToPdfConveyor.ImageRepository.Extensions;
    using ImageToPdfConveyor.ObjectModel.Contracts;
    using System.Threading.Tasks;
    using ImageToPdfConveyor.PdfClient;
    using System.Collections.Generic;
    using System.Linq;
    using ImageToPdfConveyor.ObjectModel;

    //using ImageToPdfConveyor.PdfClient.Logic;
    //using ImageToPdfConveyor.PdfClient.ObjectModel.Model;

    internal sealed class Program
    {
        private static IServiceProvider serviceProvider;

        private static GetOffsetService offsetService = new GetOffsetService();

        private static int imagesCount = 0;

        static void Main(string[] args)
        {
            Configure();
            using var scope = serviceProvider.CreateScope();

            imagesCount = scope.ServiceProvider.GetRequiredService<IImageRepositoryService>().GetFilesAmount();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 1; i++)
            {
                tasks.Add(Task.Run(PdfBuilderProc));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void Configure()
        {
            serviceProvider = new ServiceCollection()
                .AddImageRepository(@"F:\Работа\Pdfconveyor\фото", "jpg")
                //.AddImageRepository(@"F:\PdfConveyor", "jpg")
                .AddPdfBuilder()
                .BuildServiceProvider();

        }

        private static async Task PdfBuilderProc()
        {
            using var scope = serviceProvider.CreateScope();

            var imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepositoryService>();
            var pdfBuilder = scope.ServiceProvider.GetRequiredService<IPdfBuilderService>();

            int documnetNumber = 1;
            int skip = offsetService.GetNextOffset();

            do
            {
                IReadOnlyCollection<IImage> imagesToProcess = null;
                var images = await imageRepository.GetImages(skip: skip, take: 99);

                int skipImages = 0;
                int takeImages = 3;

                do
                {
                    imagesToProcess = images.Skip(skipImages).Take(takeImages).ToArray();

                    foreach (var image in imagesToProcess)
                    {
                        var file = File.OpenWrite($"F:/PdfConveyor/{image.Name}");
                        image.DataStream.CopyTo(file);
                        file.Close();
                    }

                    //var pdfDocument = new PdfDocument($"result_{documnetNumber}", DocumentPageSize.A4);
                    //pdfDocument.AddPages(imagesToProcess
                    //    .Select(image =>
                    //    {
                    //        return new PdfPage
                    //        {
                    //            DataStream = image.DataStream,
                    //        };
                    //    })
                    //    .ToArray());

                    //pdfBuilder.BuildDocument(@"F:\PdfConveyor", pdfDocument);

                    documnetNumber++;

                    skipImages += imagesToProcess.Count;
                }
                while (imagesToProcess.Count != 0);

                skip = offsetService.GetNextOffset();
            }
            while (skip < imagesCount);
        }
    }
}
