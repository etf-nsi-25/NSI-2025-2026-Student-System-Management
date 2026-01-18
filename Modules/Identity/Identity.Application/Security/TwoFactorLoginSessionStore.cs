using System;
using System.Collections.Concurrent;
using Identity.Application.Interfaces;

namespace Identity.Application.Security;

public class TwoFactorLoginSessionStore : ITwoFactorLoginSessionStore
{
    private sealed record Entry(string UserId, DateTimeOffset ExpiresAt);

    private static readonly ConcurrentDictionary<string, Entry> Tokens = new();

    public string Create(string userId, TimeSpan ttl)
    {
        var token = Guid.NewGuid().ToString("N");

        Tokens[token] = new Entry(
            UserId: userId,
            ExpiresAt: DateTimeOffset.UtcNow.Add(ttl));

        return token;
    }

    public bool TryGetUserId(string token, out string userId)
    {
        userId = string.Empty;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (!Tokens.TryGetValue(token, out var entry))
        {
            return false;
        }

        if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            Tokens.TryRemove(token, out _);
            return false;
        }

        userId = entry.UserId;
        return true;
    }

    public void Invalidate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        Tokens.TryRemove(token, out _);
    }
}
