using System.Security.Cryptography;
using System.Text;
using Identity.Core.Services;
using Identity.Application.Interfaces;

namespace Identity.Infrastructure.Services
{
    public class SecretEncryptionService : ISecretEncryptionService
    {
        // TEMP KEY – kasnije se izvuče iz configa (Azure KeyVault, env var…)
        private static readonly byte[] Key =
            Encoding.UTF8.GetBytes("THIS_IS_TEMP_KEY_32BYTES!!");

        public string Encrypt(string plaintext)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plaintext);
            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // IV + cipher → base64
            var combined = aes.IV.Concat(encrypted).ToArray();
            return Convert.ToBase64String(combined);
        }

        public string Decrypt(string ciphertext)
        {
            var bytes = Convert.FromBase64String(ciphertext);

            var iv = bytes.Take(16).ToArray();
            var cipher = bytes.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
