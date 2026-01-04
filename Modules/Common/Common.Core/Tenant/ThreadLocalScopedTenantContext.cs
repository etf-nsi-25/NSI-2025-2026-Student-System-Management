using System.Collections.Immutable;

namespace Common.Core.Tenant;

public class ThreadLocalScopedTenantContext : IScopedTenantContext
{
    private static readonly AsyncLocal<ImmutableStack<Guid>> ScopeStack = new();

    public Guid? CurrentTenantId() =>
        ScopeStack.Value?.IsEmpty == false ? ScopeStack.Value.Peek() : null;

    public IDisposable Use(Guid tenantId)
    {
        var previous = ScopeStack.Value ??= ImmutableStack<Guid>.Empty;
        ScopeStack.Value = ScopeStack.Value.Push(tenantId);
        
        return new TenantScope(previous);
    }

    private sealed class TenantScope(ImmutableStack<Guid> previousStack) : IDisposable
    {
        public void Dispose() => ScopeStack.Value = previousStack;
    }
}
