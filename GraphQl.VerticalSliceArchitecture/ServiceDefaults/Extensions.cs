using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ServiceDefaults;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddDefaultHealtchChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Ativar a resiliência por padrão
            http.AddStandardResilienceHandler();

            // Ativar descoberta de serviço por padrão
            http.AddServiceDiscovery();
        });
        return builder;
    }


    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adicionar endpoints de verificação de integridade(health checks) a aplicações em ambientes que não sejam de desenvolvimento traz implicações de segurança.
// Consulte https://aka.ms/dotnet/aspire/healthchecks para obter mais detalhes antes de habilitar esses endpoints em ambientes que não sejam de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            // Todas as verificações de integridade precisam ser bem-sucedidas para que a aplicação seja considerada pronta para receber tráfego após a inicialização
            app.MapHealthChecks(HealthEndpointPath);

            // Apenas as verificações de integridade marcadas com a tag "live" precisam ser bem-sucedidas para que a aplicação seja considerada ativa
            app.MapHealthChecks(AlivenessEndpointPath,
                new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
        }

        return app;
    }
    private static TBuilder AddDefaultHealtchChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
             // Adiciona uma verificação de atividade (liveness check) padrão para garantir que o aplicativo esteja responsivo
             .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }


}
