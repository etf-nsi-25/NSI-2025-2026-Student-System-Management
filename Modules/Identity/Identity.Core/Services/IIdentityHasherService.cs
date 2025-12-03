namespace Identity.Core.Services;

public interface IIdentityHasherService
{
    string HashPassword(string password);
    
}