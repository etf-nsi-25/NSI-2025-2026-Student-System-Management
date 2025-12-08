using System.Security.Cryptography;

namespace Identity.Infrastructure.Utilities;

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


}