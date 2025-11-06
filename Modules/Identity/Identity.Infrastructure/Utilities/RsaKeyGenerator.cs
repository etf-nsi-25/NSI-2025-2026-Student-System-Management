using System.Security.Cryptography;

namespace Identity.Infrastructure.Utilities;

/// <summary>
/// Utility class to generate RSA key pairs for JWT signing
/// </summary>
public static class RsaKeyGenerator
{

    public static (string privateKey, string publicKey) GenerateKeyPair()
    {
        using var rsa = RSA.Create(2048);

        // Export private key
        var privateKeyBytes = rsa.ExportRSAPrivateKey();
        var privateKey = Convert.ToBase64String(privateKeyBytes);

        // Export public key
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var publicKey = Convert.ToBase64String(publicKeyBytes);

        return (privateKey, publicKey);
    }

    /// <summary>
    /// Generates keys and prints them to console for configuration
    /// Run this once and add the private key to appsettings.json
    /// </summary>
    public static void GenerateAndPrintKeys()
    {
        var (privateKey, publicKey) = GenerateKeyPair();

        Console.WriteLine("=== RSA KEY PAIR GENERATED ===");
        Console.WriteLine();
        Console.WriteLine("PRIVATE KEY (Add to appsettings.json - JwtSettings:PrivateKey):");
        Console.WriteLine(privateKey);
        Console.WriteLine();
        Console.WriteLine("PUBLIC KEY (For reference - will be exposed via /api/identity/public-key endpoint):");
        Console.WriteLine(publicKey);
        Console.WriteLine();
        Console.WriteLine("IMPORTANT: Keep the private key secure and never commit it to source control!");
        Console.WriteLine("Use User Secrets or Environment Variables in production.");
    }
}