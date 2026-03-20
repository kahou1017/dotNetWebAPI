using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Dimensions.Admin.Api.Tests.TestHost;

internal sealed class DimensionsAdminApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Database:Provider"] = "Sqlite",
                ["Database:ConnectionStringName"] = "DefaultConnection",
                ["System:SystemCode"] = "Dimensions-Test"
            });
        });
    }
}
