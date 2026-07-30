using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Application.Services;
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

        // Register repositories
        services.AddScoped<IClassTypeRepository, ClassTypeRepository>();
        services.AddScoped<IClassFieldRepository, ClassFieldRepository>();
        services.AddScoped<IClassActionRepository, ClassActionRepository>();
        services.AddScoped<IClassInstanceRepository, ClassInstanceRepository>();
        services.AddScoped<IClassInstanceFieldValueRepository, ClassInstanceFieldValueRepository>();
        services.AddScoped<IFeatureRepository, FeatureRepository>();
        services.AddScoped<IClassTypeFeatureRepository, ClassTypeFeatureRepository>();
        services.AddScoped<IClassInstanceContactRepository, ClassInstanceContactRepository>();
        services.AddScoped<IContactConfigRepository, ContactConfigRepository>();

        // Register application services
        services.AddScoped<ContactService>();
        services.AddScoped<ClassWorkflowService>();

        return services;
    }
}
