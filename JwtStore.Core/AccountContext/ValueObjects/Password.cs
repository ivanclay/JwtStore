using JwtStore.Core.SharedContext.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace JwtStore.Core.AccountContext.ValueObjects
{
    public class Password : ValueObject
    {
        private const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string Special = "!@#$%ˆ&*(){}[];";
        private string password;

        public string Hash { get; } = string.Empty;
        public string ResetCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        protected Password() { }
        public Password(string? text = null)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                text = Generate();

            Hash = Hashing(text);
        }

        private static string Generate(short lenght = 16, bool includeSpecialChars = true, bool upperCase = false)
        {
            var chars = includeSpecialChars ? (Valid + Special) : Valid;
            var startRandom = upperCase ? 26 : 0;
            var index = 0;
            var res = new char[lenght];
            var rnd = new Random();

            while (index < lenght)
            {
                res[index++] = chars[rnd.Next(startRandom, chars.Length)];
            }
            
            return new string(res);
        }
    
        private static string Hashing(string password, short saltsize = 16, short keySize = 32, int iterations = 10000, char splitChar = '.') 
        {
            if (string.IsNullOrEmpty(password)) 
                throw new Exception("Password should not be null or empty");

            password += Configuration.Secrets.PasswordSaltKey;

            using var algorithm = new Rfc2898DeriveBytes(password, saltsize, iterations, HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{iterations}{splitChar}{salt}{splitChar}{key}";
        }
    
        private static bool Verify(string hash, string password, short keySize = 32, int iterations = 10000, char splitChar = '.')
        {
            password += Configuration.Secrets.PasswordSaltKey;

            var parts = hash.Split(splitChar, 3);
            
            if (parts.Length < 3 )
                return false;

            var hashIteractions = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            if (hashIteractions != iterations )
                return false;

            using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);

            var keyToCheck = algorithm.GetBytes(keySize);
            return keyToCheck.SequenceEqual(key);

        }
    }
}
