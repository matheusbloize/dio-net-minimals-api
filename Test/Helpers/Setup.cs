using dio_net_minimals_api.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Test.Mocks;

public class Setup {
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext) {
        Setup.testContext = testContext;
        http = new WebApplicationFactory<Startup>();

        http = http.WithWebHostBuilder(builder => {
            builder.UseSetting("https_port", PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services => {
                services.AddScoped<IAdministratorService, AdministratorServiceMock>();
                services.AddScoped<IVehicleService, VehicleServiceMock>();
            });

        });

        client = http.CreateClient();
    }

    public static void ClassCleanup() {
        http.Dispose();
    }
}