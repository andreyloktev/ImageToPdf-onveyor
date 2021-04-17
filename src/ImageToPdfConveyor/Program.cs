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
    using ImageToPdfConveyor.Logger.Contracts;
    using System.Timers;

    internal sealed class Program
    {
        private static IServiceProvider serviceProvider;
        private static ILogger logger;
        private static GetNextDocumentService nextDocumentService;
        private static int imagesCount = 0;
        private static ConveyorOptions conveyorOptions;
        private const int MaxBatchSize = 50;
        private static Timer Timer = new Timer(1000);
        private static object lockObject = new object();
        private static int amountProcessedImages = 0;

        static void Main(string[] args)
        {
            Configure();
            using var scope = serviceProvider.CreateScope();
            var imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepositoryService>();
            logger = scope.ServiceProvider.GetRequiredService<ILogger>();

            imagesCount = scope.ServiceProvider.GetRequiredService<IImageRepositoryService>().GetFilesAmount();

            logger.Info($"Amount file to process: {imagesCount}");
            SetTimer();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(PdfBuilderProc));
            }

            Task.WaitAll(tasks.ToArray());
            PrintPercents();

            logger.Dispose();
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

            nextDocumentService = new GetNextDocumentService(conveyorOptions.AmountImagesInPdf);

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

            var nextDocument = nextDocumentService.GetNextDocument();
            int take = conveyorOptions.AmountImagesInPdf;

            do
            {
                using var imagesCollection = await imageRepository.GetImages(skip: nextDocument.skip, take: take);

                if (imagesCollection.Images.Count == 0)
                {
                    break;
                }

                try
                {
                    var pdfDocument = new PdfDocument($"{conveyorOptions.BasePdfDocumentName}_{nextDocument.documentNumber}", DocumentPageSize.A4);
                    pdfDocument.AddPages(imagesCollection.Images
                        .Select(image =>
                        {
                            return new PdfPage
                            {
                                DataStream = image.DataStream,
                            };
                        })
                        .ToArray());

                    pdfBuilder.BuildDocument(conveyorOptions.OutputDirectory, pdfDocument);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Can not create pdf document");
                }


                lock (lockObject)
                {
                    amountProcessedImages += imagesCollection.Images.Count;
                }

                nextDocument = nextDocumentService.GetNextDocument();
            }
            while (true);
        }

        private static void SetTimer()
        {
            // Hook up the Elapsed event for the timer. 
            Timer.Elapsed += OnTimedEvent;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e) => PrintPercents();

        private static void PrintPercents()
        {
            var value = (int)((amountProcessedImages / (double)imagesCount) * 100);
            Console.Write($"\rProcess: {value}%");
        }
    }
}
