using Domain.Base.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApi.Common.Events;
using WebApi.Common.Middleware;

namespace WebApi.Common.Persistence.Interceptors;

public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDomainEventDispatcher _dispatcher;

    public DispatchDomainEventsInterceptor(IHttpContextAccessor httpContextAccessor, IDomainEventDispatcher dispatcher)
    {
        _httpContextAccessor = httpContextAccessor;
        _dispatcher = dispatcher;
    }

    // NOTA: Existem duas opções para disparar eventos de domínio:
    // Opção 1. Antes de as alterações serem salvas no banco de dados (SavingChanges)
    // Opção 2. Após as alterações serem salvas no banco de dados (SavedChanges)
    //
    // Estamos utilizando a Opção 2 por vários motivos:
    // - Os manipuladores de eventos (event handlers) podem consultar o banco de dados e esperar que os dados estejam atualizados.
    // - Manipuladores de eventos que disparam eventos de integração, os quais afetam outros sistemas, só o farão se as alterações forem salvas com sucesso no banco de dados.
    //
    // A desvantagem disso é que podemos ter múltiplas chamadas para salvar alterações em uma única requisição ao banco de dados.
    // Isso significa que não temos mais uma única operação de escrita no banco de dados; portanto, pode ser necessário envolver toda a operação em uma transação para garantir a consistência.

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEvents(eventData.Context).ConfigureAwait(false).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public async override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null)
            return;

        var domainEvents = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        if (domainEvents.Count is 0)
            return;

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(domainEvents);
        }
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private async Task PublishDomainEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            await _dispatcher.DispatchAsync(domainEvent);
    }

    private void AddDomainEventsToOfflineProcessingQueue(IEnumerable<IDomainEvent> domainEvents)
    {
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value) &&
                                value is Queue<IDomainEvent> existingDomainEvents
            ? existingDomainEvents
            : new Queue<IDomainEvent>();

        // Queue is processed by EventualConsistencyMiddleware
        foreach (var domainEvent in domainEvents)
            domainEventsQueue.Enqueue(domainEvent);

        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }
}
