using Faculty.Core.Interfaces;

namespace Faculty.API;
//MADE JUST TO TEST WITHOUT JWT, SINCE LOGIN IS NOT WORKING
public class DevTenantServiceFAKE : ITenantService
{
    public Guid GetCurrentFacultyId()
        => Guid.Parse("641a4bfe-d83b-403d-be28-db9fa4130e5e");
}
