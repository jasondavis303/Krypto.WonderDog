using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Krypto.WonderDog
{
    public class Key
    {
        public Key()
        {
            Password = new byte[new Random().Next(16, 129)];
            using var rand = new RNGCryptoServiceProvider();
            rand.GetBytes(Password);

            GenerateSalt();
        }

        public Key(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            Password = Encoding.UTF8.GetBytes(password);
            GenerateSalt();
        }

        public Key(byte[] password)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentNullException(nameof(password));

            Password = password;
            GenerateSalt();
        }

        public Key(string password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentNullException(nameof(salt));

            Password = Encoding.UTF8.GetBytes(password);
            Salt = Encoding.UTF8.GetBytes(salt);
            if (Salt.Length < 8)
                throw new ArgumentException("Salt is not at least 8 bytes", nameof(salt));
        }

        public Key(string password, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            if (salt == null || salt.Length == 0)
                throw new ArgumentNullException(nameof(salt));

            Password = Encoding.UTF8.GetBytes(password);
            Salt = salt;
            if (Salt.Length < 8)
                throw new ArgumentException("Salt is not at least 8 bytes", nameof(salt));
        }

        public Key(byte[] password, string salt)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentNullException(nameof(salt));

            Password = password;
            Salt = Encoding.UTF8.GetBytes(salt);
            if (Salt.Length < 8)
                throw new ArgumentException("Salt is not at least 8 bytes", nameof(salt));
        }

        public Key(byte[] password, byte[] salt)
        {
            if (password == null || password.Length == 0)
                throw new ArgumentNullException(nameof(password));

            if (salt == null || salt.Length == 0)
                throw new ArgumentNullException(nameof(salt));

            Password = password;
            Salt = salt;
            if (Salt.Length < 8)
                throw new ArgumentException("Salt is not at least 8 bytes", nameof(salt));
        }


        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public int Iterations { get; set; } = 1000;
        public HashAlgorithmName Algorithm { get; set; } = HashAlgorithmName.SHA512;

        private void GenerateSalt()
        {
            Salt = new byte[new Random().Next(16, 129)];
            using var rand = new RNGCryptoServiceProvider();
            rand.GetBytes(Salt);
        }

        internal void SetKey(SymmetricAlgorithm alg)
        {
            alg.Key = DeriveKey(alg.LegalKeySizes.Max(item => item.MaxSize));
        }

        public byte[] DeriveKey(int bits)
        {
            using var gen = new Rfc2898DeriveBytes(Password, Salt, Iterations, Algorithm);
            return gen.GetBytes(bits / 8);
        }

        /// <summary>
        /// Derives a 256 bit key
        /// </summary>
        /// <returns></returns>
        public byte[] DeriveKey() => DeriveKey(256);
    }
}
