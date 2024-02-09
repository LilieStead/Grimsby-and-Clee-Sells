using System.CodeDom.Compiler;
using System.Security.Cryptography;
namespace Grimsby_and_Clee_Sells.UserSession
{
    public class SecretKeyGen
    {
        private static readonly Lazy<string> LazySecretKey = new Lazy<string>(GenSecretKey);
        public string SecretKey => LazySecretKey.Value;
        // gentrates a random key each time api is started 
        private static string GenSecretKey()
        {
            const int keylength = 32;
            byte[] keyBytes = GenerateBytes(keylength);
            return Convert.ToBase64String(keyBytes);
        }

        private static byte[] GenerateBytes(int length)
        {
            byte[] keyBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return keyBytes;
        }
    }
}
