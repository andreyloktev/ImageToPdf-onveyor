namespace ImageToPdfConveyor.ImageRepository.Extensions
{
    using ImageToPdfConveyor.ImageRepository.Logic;
    using ImageToPdfConveyor.ObjectModel.Contracts;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImageRepository(this IServiceCollection serviceCollection, string rootDirectory, string fileExtension)
        {
            return serviceCollection.AddSingleton<IImageRepositoryService, ImageRepositoryService>(
                serviceProvider => new ImageRepositoryService(rootDirectory, fileExtension));
        }
    }
}
