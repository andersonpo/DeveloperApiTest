using DeveloperApiTest.Dominio.Mapeamentos;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperTestXUnit;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Mapeamentos).Assembly);
    }
}
