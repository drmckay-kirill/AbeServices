using Microsoft.Extensions.DependencyInjection;
using AbeServices.Common.Helpers;
using AbeServices.Common.Protocols;

namespace AbeServices.Common.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProtocolBuilders(this IServiceCollection services)
        {
            services.AddSingleton<IDataSymmetricEncryptor, DataSymmetricEncryption>();
            services.AddSingleton<IDataSerializer, ProtobufDataSerializer>();
            services.AddSingleton<IKeyDistributionBuilder, KeyDistributionBuilder>();
            
            services.AddSingleton<IAbeDecorator, AbeDecorator>();           
            services.AddSingleton<IAbeAuthBuilder, AbeAuthBuilder>();
            
            return services;
        }
    }
}