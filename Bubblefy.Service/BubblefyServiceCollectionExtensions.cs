using Bubblefy.Business;
using Bubblefy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bubblefy.Service;

public static class BubblefyServiceCollectionExtensions
{
    public static IServiceCollection AddBubblefyServices(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<MotorBuscaMusical>();
        services.AddScoped<FaixaService>();
        services.AddScoped<PlaylistService>();
        return services;
    }

    public static void InitializeBubblefyDatabase(this IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        if (!context.Faixas.Any())
        {
            context.Faixas.AddRange(DadosSemente.ObterFaixas());
            context.SaveChanges();
        }
    }
}