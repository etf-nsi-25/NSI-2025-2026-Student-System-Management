using System;

namespace Identity.Application.Interfaces;

public interface ITwoFactorLoginSessionStore
{
    string Create(string userId, TimeSpan ttl);
    bool TryGetUserId(string token, out string userId);
    void Invalidate(string token);
}
