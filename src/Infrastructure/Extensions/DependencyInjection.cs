using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var type = configuration["Database:DataStoreType"];

        switch (type)
        {
            case "main":
                services.AddDbContext<DbContext, VanguardContext>(opt =>
                {
                    opt.UseInMemoryDatabase(configuration["Database:Name"]);
                });
                break;
            case "backup":
                services.AddDbContext<DbContext, VanguardBackupContext>(opt =>
                {
                    opt.UseInMemoryDatabase(configuration["Database:Name"]);
                });
                break;
            default:
                throw new NotSupportedException($"The type {type} is not a supported data store type.");
        }

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));


        return services;
    }
}
