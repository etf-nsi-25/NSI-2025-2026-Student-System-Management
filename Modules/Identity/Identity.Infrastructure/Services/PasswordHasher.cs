using Identity.Core.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Services;

internal class PasswordHasher : IPasswordHasher
{
    
    
    public byte[] HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] subkey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32 
        );

        var output = new byte[salt.Length + subkey.Length];
        Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
        Buffer.BlockCopy(subkey, 0, output, salt.Length, subkey.Length);
        
        return output;
    }

    public bool VerifyPassword(string password, byte[] passwordHash)
    {
        byte[] salt = new byte[16];
        Buffer.BlockCopy(passwordHash, 0, salt, 0, salt.Length);

        byte[] subkey = new byte[passwordHash.Length - salt.Length];
        Buffer.BlockCopy(passwordHash, salt.Length, subkey, 0, subkey.Length);

        byte[] hashedInput = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32
        );

        return CryptographicOperations.FixedTimeEquals(hashedInput, subkey);
    }
}