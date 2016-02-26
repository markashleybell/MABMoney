namespace MABMoney.Web.Infrastructure
{
    public interface ICryptoProvider
    {
        string GenerateSalt(int byteLength);
        string Hash(byte[] input, string algorithm);
        string Hash(string input, string algorithm);
        string HashPassword(string password);
        string SHA1(string input);
        string SHA256(string input);
        bool VerifyHashedPassword(string hashedPassword, string password);
    }
}