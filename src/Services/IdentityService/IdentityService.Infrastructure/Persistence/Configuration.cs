
using Microsoft.Extensions.Configuration;

namespace IdentityService.Infrastructure.Persistence
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                Microsoft.Extensions.Configuration.ConfigurationManager configurationManager = new();
                try
                {
                    configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../IdentityService/IdentityService.Api"));
                    configurationManager.AddJsonFile("appsettings.json");
                }
                catch
                {
                    configurationManager.AddJsonFile("appsettings.Production.json");
                }

                return configurationManager.GetConnectionString("MSSQL");
            }
        }

    }
}
