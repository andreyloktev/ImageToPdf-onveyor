namespace ImageToPdfConveyor.PdfClient
{
    using System;
    using ImageToPdfConveyor.ObjectModel.Contracts;
    using ImageToPdfConveyor.PdfClient.Logic;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPdfBuilder(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IPdfBuilderService, PdfBuilderService>();
        }
    }
}
