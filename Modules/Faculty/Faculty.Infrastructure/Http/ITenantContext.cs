namespace Faculty.Infrastructure.Http;

public interface ITenantContext
{
    Guid? CurrentFacultyId { get; set; }
    IDisposable Use(Guid tenantId);
}
