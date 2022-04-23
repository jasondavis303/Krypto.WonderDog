using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Krypto.WonderDog
{
    public class Key
    {
        private byte[] _salt = new byte[8];
        private byte[] _password;
        private int _iterations = 1000;
        
        public Key() { }

        public Key(string password) => SetPassword(password);

        public Key(byte[] password) => Password = password;

        public Key(string password, string salt)
        {
            SetPassword(password);
            SetSalt(salt);
        }

        public Key(string password, byte[] salt)
        {
            SetPassword(password);
            Salt = salt;
        }

        public Key(byte[] password, string salt)
        {
            Password = password;
            SetSalt(salt);
        }

        public Key(byte[] password, byte[] salt)
        {
            Password = password;
            Salt = salt;
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long", nameof(password));

            Password = Encoding.Unicode.GetBytes(password);
        }

        public void SetSalt(string salt)
        {
            if (salt == null)
                throw new ArgumentNullException(nameof(salt));

            if (salt.Length < 8)
                throw new ArgumentException("Salt must be at least 8 characters long", nameof(salt));

            Salt = Encoding.Unicode.GetBytes(salt);
        }

        public string GetPasswordString() => Encoding.Unicode.GetString(Password);

        public string GetSaltString() => Encoding.Unicode.GetString(Salt);

        /// <summary>
        /// Default is 8 bytes, all zero. It is recommended to set and store the Salt value
        /// </summary>
        public byte[] Salt
        {
            get => _salt;
            set
            {
                if (value == null)
                    throw new Exception("Salt may not be null");
                if (value.Length < 8)
                    throw new Exception("Salt must be at least 8 bytes long");
                _salt = value;
            }
        }

        public byte[] Password
        {
            get => _password;
            set
            {
                if (value == null)
                    throw new Exception("Password may not be null");
                if (value.Length < 8)
                    throw new Exception("Password must be at least 8 bytes long");
                _password = value;
            }
        }

        public int Iterations
        {
            get => _iterations;
            set
            {
                if (value < 1)
                    throw new Exception("Iterations may not be less than 1");
                _iterations = value;
            }
        }

        public HashAlgorithmName Algorithm { get; set; } = HashAlgorithmName.SHA512;

        /// <summary>
        /// Sets the <see cref="Salt"/> to random, with a random length betweeen 256 and 2048 bits
        /// </summary>
        public void GenerateRandomSalt() => Salt = GenerateRandom();


        /// <summary>
        /// Sets the <see cref="Password"/> to random, with a random length betweeen 256 and 2048 bits
        /// </summary>
        public void GenerateRandomPassword() => Password = GenerateRandom();

        private static byte[] GenerateRandom()
        {
            //Generate a random byte array of random length between 32 and 256 bytes
            var ret = new byte[new Random().Next(32, 257)];
            using var rand = new RNGCryptoServiceProvider();
            rand.GetBytes(ret);
            return ret;
        }

        /// <summary>
        /// Sets the key on the specified algorithm
        /// </summary>
        internal void SetKey(SymmetricAlgorithm alg) => alg.Key = DeriveKey(alg.LegalKeySizes.Max(item => item.MaxSize));


        /// <summary>
        /// Derives a Rfc2898 key from the <see cref="Password"/> and <see cref="Salt"/>
        /// </summary>
        /// <param name="size">Size in bits of the derived key. Must be a factor of 8</param>
        public byte[] DeriveKey(int size = 256)
        {
            if (Password == null)
                throw new Exception("Password has not been set");

            if (size % 8 != 0)
                throw new ArgumentException("Size is not a factor of 8", nameof(size));

            using var gen = new Rfc2898DeriveBytes(Password, Salt, Iterations, Algorithm);
            return gen.GetBytes(size / 8);
        }
    }
}
