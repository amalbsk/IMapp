namespace InventoryManagementApp.Services
{
    using Microsoft.Extensions.Configuration;

    public interface IConfigurationService
    {
        string GetConnectionString(string name);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }
    }
}