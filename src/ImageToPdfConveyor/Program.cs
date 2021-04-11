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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.NewtonsoftJson;
    using ImageToPdfConveyor.ObjectModel.Settings;
    using ImageToPdfConveyor.Logger.Extensions;

    internal sealed class Program
    {
        private static IServiceProvider serviceProvider;
        private static GetOffsetService offsetService = new GetOffsetService();
        private static int imagesCount = 0;
        private static ConveyorOptions conveyorOptions;

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
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(new NewtonsoftJsonConfigurationSource()
            {
                Path = "appsettings.json",
            });

            IConfiguration configuration = configurationBuilder.Build();

            conveyorOptions = configuration.GetSection(nameof(ConveyorOptions)).Get<ConveyorOptions>() ?? new ConveyorOptions();

            serviceProvider = new ServiceCollection()
                .AddLogger(configuration)
                .AddImageRepository(conveyorOptions.PathToImages, conveyorOptions.ImageFormat)
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
                int takeImages = conveyorOptions.AmountImagesInPdf;

                do
                {
                    imagesToProcess = images.Skip(skipImages).Take(takeImages).ToArray();

                    var pdfDocument = new PdfDocument($"{conveyorOptions.BasePdfDocumentName}_{documnetNumber}", DocumentPageSize.A4);
                    pdfDocument.AddPages(imagesToProcess
                        .Select(image =>
                        {
                            return new PdfPage
                            {
                                DataStream = image.DataStream,
                            };
                        })
                        .ToArray());

                    pdfBuilder.BuildDocument(conveyorOptions.OutputDirectory, pdfDocument);

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
