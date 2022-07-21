using BSSProformaInvioceRenewalApp.Models;
using Microsoft.Extensions.Configuration;

namespace BSSProformaInvioceRenewalApp
{
    public class EnvironmentService
    {
        private static AppConfig _appConfig = new();

        private static readonly EnvironmentService _environmentServiceInstance = new();

        private EnvironmentService()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            _appConfig = config.GetRequiredSection("AppConfig").Get<AppConfig>();
        }

        public static EnvironmentService GetInstance() => _environmentServiceInstance;

        public AppConfig GetAppConfig() => _appConfig;
    }
}
