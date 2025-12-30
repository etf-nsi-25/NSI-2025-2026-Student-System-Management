using Faculty.Infrastructure.Http;

namespace Support.API.IntegrationTests;

public sealed class TestTenantService : ITenantService
{
	public static readonly Guid FacultyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

	public Guid GetCurrentFacultyId() => FacultyId;
}
