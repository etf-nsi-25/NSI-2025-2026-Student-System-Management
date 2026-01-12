namespace Common.Core.Tenant;

/// <summary>
/// Allows overriding tenancy information in case of non-http environment such as event handlers,
/// withing a certain scope. The implementations of the interface must make sure that "nested" scopes work as expected,
/// that is, that the lowest available level of scope is always used.
/// </summary>
public interface IScopedTenantContext
{
    /// <summary>
    /// Returns the tenant id for current scope, if existing.
    /// </summary>
    Guid? CurrentTenantId();
    
    /// <summary>
    /// Starts a new tenant scope.
    /// </summary>
    /// <param name="tenantId">tenant id</param>
    IDisposable Use(Guid tenantId);
}
