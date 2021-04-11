namespace ImageToPdfConveyor.Logger.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using EnsureThat;
    using ImageToPdfConveyor.Logger.Contracts;
    using ImageToPdfConveyor.Logger.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogger(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            EnsureArg.IsNotNull(configuration, nameof(configuration));

            var options = configuration
                .GetSection(nameof(LoggerOptions))
                .Get<LoggerOptions>() ?? new LoggerOptions();

            return serviceCollection.AddSingleton<ILogger>(provider => new Logger(options));
        }
    }
}
