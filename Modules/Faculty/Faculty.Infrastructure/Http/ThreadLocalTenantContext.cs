namespace Faculty.Infrastructure.Http;

/// <summary>
/// Allows overriding tenancy information in case of non-http environment such as event handlers.
/// </summary>
public class ThreadLocalTenantContext : ITenantContext
{
    private static readonly AsyncLocal<Guid?> CurrentTenant = new();

    public Guid? CurrentFacultyId
    {
        get => CurrentTenant.Value;
        set => CurrentTenant.Value = value;
    }

    public IDisposable Use(Guid tenantId)
    {
        var previous = CurrentTenant.Value;
        CurrentTenant.Value = tenantId;
        return new SimpleDisposableAction(() => CurrentTenant.Value = previous);
    }

    private sealed class SimpleDisposableAction(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
