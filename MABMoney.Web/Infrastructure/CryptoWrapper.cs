using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MABMoney.Web.Infrastructure
{
    public class CryptoWrapper : ICryptoWrapper
    {
        public string GenerateSalt(int byteLength = 16)
        {
            return Crypto.GenerateSalt(byteLength);
        }

        public string Hash(byte[] input, string algorithm = "sha256")
        {
            return Crypto.Hash(input, algorithm);
        }

        public string Hash(string input, string algorithm = "sha256")
        {
            return Crypto.Hash(input, algorithm);
        }

        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public string SHA1(string input)
        {
            return Crypto.SHA1(input);
        }

        public string SHA256(string input)
        {
            return Crypto.SHA256(input);
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            return Crypto.VerifyHashedPassword(hashedPassword, password);
        }
    }
}