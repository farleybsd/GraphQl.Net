using EntityFramework.Exceptions.SqlServer;
using WebApi.Common.Persistence.Interceptors;

namespace WebApi.Common.Persistence;

public static class DependencyInjection
{
    public const string ConnectionName = "AppDb";

    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // TODO: reativar os interceptors depois de criar CurrentUserService e DomainEventDispatcher.
        // services.AddScoped<EntitySaveChangesInterceptor>();
        // services.AddScoped<DispatchDomainEventsInterceptor>();
        // services.AddSingleton(TimeProvider.System);

        var configuration = builder.Configuration;

        // Uma factory, em vez de um simples AddDbContext, pois o GraphQL resolve campos irmãos em
        // paralelo, enquanto um DbContext permite apenas uma operação por vez. O HotChocolate fornece
        // a cada resolver seu próprio contexto, obtido a partir desta factory (veja RegisterDbContextFactory em GraphQlExt),
        // e o descarta ao final da requisição.
        //
        // Com tempo de vida "scoped" (por escopo), em vez do padrão "singleton": os interceptors abaixo
        // dependem do ICurrentUserService, que possui escopo definido (scoped) por realizar a leitura
        // da requisição HTTP atual.
        services.AddDbContextFactory<ApplicationDbContext>(
            (serviceProvider, options) =>
            {
                // A leitura ocorre no momento em que as opções são construídas, e não durante a inicialização. 
                // O Aspire fornece a string de conexão apenas para os recursos de API e migrações; 
                // no entanto, o `schema export` e o `dotnet ef migrations add` iniciam este host sem ela. 
                // A ausência da string resulta em uma falha explícita: o SqlClient gera um erro na primeira tentativa de conexão
                options.UseSqlServer(configuration.GetConnectionString(ConnectionName));

                // Executados automaticamente no SaveChanges:
                // options.AddInterceptors(
                //     // Antes de salvar: preenche a auditoria (CreatedAt/By, UpdatedAt/By)
                //     serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>(),
                //     // Depois de salvar: despacha os eventos de domínio
                //     serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());

                // Retorna exceções úteis e fortemente tipadas
                options.UseExceptionProcessor();
            },
            ServiceLifetime.Scoped);

        // Tudo o que está fora de um resolver — os interceptors, o middleware de consistência eventual,
        // o seeder, os testes de integração — continua solicitando um DbContext diretamente.
        services.AddScoped<ApplicationDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        // Adiciona a política de repetição, a verificação de integridade e a telemetria que o AddSqlServerDbContext teria
        // adicionado. Essa chamada registra seu próprio DbContext com escopo (scoped), motivo pelo qual não é utilizada aqui.
        builder.EnrichSqlServerDbContext<ApplicationDbContext>();
    }
}
