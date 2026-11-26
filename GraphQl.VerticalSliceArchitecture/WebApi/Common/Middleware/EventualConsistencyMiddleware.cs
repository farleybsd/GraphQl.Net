using Domain.Base.EventualConsistency;
using Domain.Base.Interfaces;
using WebApi.Common.Events;

namespace WebApi.Common.Middleware;

public class EventualConsistencyMiddleware
{
    public const string DomainEventsKey = "DomainEventsKey";

    private readonly RequestDelegate _next;

    public EventualConsistencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, IDomainEventDispatcher dispatcher)
    {
        context.Response.OnCompleted(async () =>
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(async () =>
            {
                await PublishEvents(context, dispatcher);
            }, null!);
        });

        await _next(context);
    }

    private static async Task PublishEvents(HttpContext context, IDomainEventDispatcher dispatcher)
    {
        try
        {
            if (context.Items.TryGetValue(DomainEventsKey, out var value) &&
                value is Queue<IDomainEvent> domainEvents)
            {
                // Não se trata de RequestAborted: isso é executado após o envio da resposta;
                // caso um cliente se desconecte assim que obtém a resposta, os manipuladores
                // seriam cancelados no meio do processo, deixando a alteração salva sem a sua
                // etapa subsequente (o total da equipe nunca seria recalculado).
                while (domainEvents.TryDequeue(out var nextEvent))
                    await dispatcher.DispatchAsync(nextEvent, CancellationToken.None);
            }
        }
        // ReSharper disable once RedundantCatchClause
        catch (EventualConsistencyException)
        {
            // TODO: handle eventual consistency exception
            throw;
        }
    }
}

