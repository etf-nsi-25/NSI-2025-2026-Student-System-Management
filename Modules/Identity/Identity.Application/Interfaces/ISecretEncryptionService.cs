namespace Identity.Application.Interfaces
{
    public interface ISecretEncryptionService
    {
        string Encrypt(string plaintext);
        string Decrypt(string ciphertext);
    }
}
