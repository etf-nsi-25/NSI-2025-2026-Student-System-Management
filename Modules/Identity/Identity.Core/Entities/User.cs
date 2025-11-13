namespace Identity.Core.Entities;

public record User(Guid Id,string FullName, string? Email, string PasswordHash, string Role, Guid TenantId);