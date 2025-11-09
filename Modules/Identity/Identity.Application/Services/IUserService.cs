using Identity.Application.DTOs;

namespace Identity.Application.Services;

public interface IUserService
{
    public Task<UserDTO> CreateUser(string email);
}