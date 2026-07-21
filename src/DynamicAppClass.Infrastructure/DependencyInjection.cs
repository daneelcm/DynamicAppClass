using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Infrastructure.Persistence;
using DynamicAppClass.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicAppClass.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=dynamic-app-class.db";
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IClassTypeRepository, ClassTypeRepository>();
        services.AddScoped<IClassInstanceRepository, ClassInstanceRepository>();
        return services;
    }
}
