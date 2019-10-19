using Microsoft.Extensions.DependencyInjection;
using AbeServices.Common.Helpers;
using AbeServices.Common.Protocols;

namespace AbeServices.Common.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProtocolBuilders(this IServiceCollection services)
        {
            services.AddTransient<IDataSymmetricEncryptor, DataSymmetricEncryption>();
            services.AddTransient<IDataSerializer, ProtobufDataSerializer>();
            services.AddTransient<IKeyDistributionBuilder, KeyDistributionBuilder>();
            
            services.AddTransient<IAbeDecorator, AbeDecorator>();           
            services.AddTransient<IAbeAuthBuilder, AbeAuthBuilder>();
            
            return services;
        }
    }
}