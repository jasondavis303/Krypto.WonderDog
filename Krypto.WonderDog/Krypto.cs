using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
    public class Krypto
    {
        const int BUFFER_SIZE = 81920;
        


        private Aes CreateAes(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var ret = Aes.Create();

            //AES Key = 256 bits, SHA256 = 256 bits, so...
            using var sha = SHA256.Create();
            ret.Key = sha.ComputeHash(passwordBytes);

            //AES IV = 128 bits, MD5 = 128 bits, so...
            Array.Reverse(passwordBytes);
            using var md5 = MD5.Create();
            ret.IV = md5.ComputeHash(passwordBytes);

            return ret;
        }





       
        /// <summary>
        /// Encrypt small strings
        /// </summary>
        public string Encrypt(string data, string password)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), password), Base64FormattingOptions.None);
        }

        /// <summary>
        /// Encrypt small chunks of data
        /// </summary>
        public byte[] Encrypt(byte[] data, string password)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        public async Task EncryptFileAsync(string filename, string password)
        {
            string tmpFile = filename + ".tmp";
            await EncryptFileAsync(filename, tmpFile, password).ConfigureAwait(false);
            File.Delete(filename);
            File.Move(tmpFile, filename);
        }

        public async Task EncryptFileAsync(string srcFile, string dstFile, string password)
        {
            using var src = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true);
            using var aes = CreateAes(password);
            using var encryptor = aes.CreateEncryptor();
            using var dst = new FileStream(dstFile, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
            using var cs = new CryptoStream(dst, encryptor, CryptoStreamMode.Write);
            await src.CopyToAsync(cs, BUFFER_SIZE).ConfigureAwait(false);
            cs.FlushFinalBlock();
        }







        /// <summary>
        /// Decrypt small strings
        /// </summary>
        public string Decrypt(string data, string password)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(data));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }


        /// <summary>
        /// Decrypt small chunks of data
        /// </summary>
        public byte[] Decrypt(byte[] data, string password)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(data);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var ret = new byte[data.Length];
            int totalRead = 0;
            int read;
            while ((read = cs.Read(ret, totalRead, ret.Length - totalRead)) > 0)
            {
                totalRead += read;
                if (totalRead == ret.Length)
                    break;
            }

            return ret;
        }

        public async Task DecryptFileAsync(string filename, string password)
        {
            string tmpFile = filename + ".tmp";
            await DecryptFileAsync(filename, tmpFile, password).ConfigureAwait(false);
            File.Delete(filename);
            File.Move(tmpFile, filename);
        }

        public async Task DecryptFileAsync(string srcFile, string dstFile, string password)
        {
            using var src = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true);
            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read);
            using var dst = new FileStream(dstFile, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true);
            await cs.CopyToAsync(dst, BUFFER_SIZE).ConfigureAwait(false);
        }


    }
}
