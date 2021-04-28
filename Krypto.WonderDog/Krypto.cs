using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
    public class Krypto
    {
        const int BUFFER_SIZE = 81920;
        const string MAGIC_STRING_EXCEPTION = "Unable to decrypt the magic string";


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

        private byte[] ReadBytes(Stream stream, int cnt)
        {
            var ret = new byte[cnt];
            int totalRead = 0;
            int read;
            while ((read = stream.Read(ret, totalRead, ret.Length - totalRead)) > 0)
            {
                totalRead += read;
                if (totalRead == ret.Length)
                    break;
            }

            return ret;
        }

        private async Task<byte[]> ReadBytesAsync(Stream stream, int cnt, CancellationToken cancellationToken)
        {
            var ret = new byte[cnt];
            int totalRead = 0;
            int read;
            while ((read = await stream.ReadAsync(ret, totalRead, ret.Length - totalRead, cancellationToken).ConfigureAwait(false)) > 0)
            {
                totalRead += read;
                if (totalRead == ret.Length)
                    break;
            }

            return ret;
        }

        private async Task<FileStream> OpenAsyncReadAsync(string filename, CancellationToken cancellationToken)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete, BUFFER_SIZE, true);
            try
            {
                byte[] buffer = new byte[1];
                int result = await fileStream.ReadAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(false);
                fileStream.Seek(0, SeekOrigin.Begin);
                return fileStream;
            }
            catch
            {
                fileStream.Dispose();
                return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete, BUFFER_SIZE, false);
            }
        }

        private FileStream CreateFile(string filename, bool async) => new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, async);



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
        /// Encrypt small strings
        /// </summary>
        public string Encrypt(string data, string password, string magicString)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), password, magicString), Base64FormattingOptions.None);
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

        /// <summary>
        /// Encrypt small chunks of data
        /// </summary>
        public byte[] Encrypt(byte[] data, string password, string magicString)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

            var magicBytes = Encoding.UTF8.GetBytes(magicString);
            cs.Write(magicBytes, 0, magicBytes.Length);

            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        public async Task EncryptFileAsync(string filename, string password, CancellationToken cancellationToken = default)
        {
            string tmpFile = filename + ".tmp";
            await EncryptFileAsync(filename, tmpFile, password, cancellationToken).ConfigureAwait(false);
            File.Delete(filename);
            File.Move(tmpFile, filename);
        }

        public async Task EncryptFileAsync(string srcFile, string dstFile, string password, CancellationToken cancellationToken = default)
        {
            using var src = await OpenAsyncReadAsync(srcFile, cancellationToken).ConfigureAwait(false);
            using var aes = CreateAes(password);
            using var encryptor = aes.CreateEncryptor();
            using var dst = CreateFile(dstFile, src.IsAsync);
            using var cs = new CryptoStream(dst, encryptor, CryptoStreamMode.Write);
            await src.CopyToAsync(cs, BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
            cs.FlushFinalBlock();
        }

        public async Task EncryptFileAsync(string srcFile, string dstFile, string password, string magicString, CancellationToken cancellationToken = default)
        {
            using var src = await OpenAsyncReadAsync(srcFile, cancellationToken).ConfigureAwait(false);
            using var aes = CreateAes(password);
            using var encryptor = aes.CreateEncryptor();
            using var dst = CreateFile(dstFile, src.IsAsync);
            using var cs = new CryptoStream(dst, encryptor, CryptoStreamMode.Write);

            var magicBytes = Encoding.UTF8.GetBytes(magicString);
            await cs.WriteAsync(magicBytes, 0, magicBytes.Length, cancellationToken).ConfigureAwait(false);

            await src.CopyToAsync(cs, BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
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
        /// Decrypt small strings
        /// </summary>
        public string Decrypt(string data, string password, string magicString)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(data));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            var magicBytes = ReadBytes(cs, magicString.Length);
            if (Encoding.UTF8.GetString(magicBytes) != magicString)
                throw new Exception(MAGIC_STRING_EXCEPTION);

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
            using var msEncrypted = new MemoryStream(data);
            using var cs = new CryptoStream(msEncrypted, decryptor, CryptoStreamMode.Read);
            using var msDecrypted = new MemoryStream();
            cs.CopyTo(msDecrypted);
            return msDecrypted.ToArray();
        }

        /// <summary>
        /// Decrypt small chunks of data
        /// </summary>
        public byte[] Decrypt(byte[] data, string password, string magicString)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var msEncrypted = new MemoryStream(data);
            using var cs = new CryptoStream(msEncrypted, decryptor, CryptoStreamMode.Read);

            var magicBytes = ReadBytes(cs, magicString.Length);
            if (Encoding.UTF8.GetString(magicBytes) != magicString)
                throw new Exception(MAGIC_STRING_EXCEPTION);

            using var msDecrypted = new MemoryStream();
            cs.CopyTo(msDecrypted);
            return msDecrypted.ToArray();
        }


        public async Task DecryptFileAsync(string filename, string password, CancellationToken cancellationToken = default)
        {
            string tmpFile = filename + ".tmp";
            await DecryptFileAsync(filename, tmpFile, password, cancellationToken).ConfigureAwait(false);
            File.Delete(filename);
            File.Move(tmpFile, filename);
        }

        public async Task DecryptFileAsync(string srcFile, string dstFile, string password, CancellationToken cancellationToken = default)
        {
            using var src = await OpenAsyncReadAsync(srcFile, cancellationToken).ConfigureAwait(false);
            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read);
            using var dst = CreateFile(dstFile, src.IsAsync);
            await cs.CopyToAsync(dst, BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
        }

        public async Task DecryptFileAsync(string srcFile, string dstFile, string password, string magicString, CancellationToken cancellationToken = default)
        {
            using var src = await OpenAsyncReadAsync(srcFile, cancellationToken).ConfigureAwait(false);
            using var aes = CreateAes(password);
            using var decryptor = aes.CreateDecryptor();
            using var cs = new CryptoStream(src, decryptor, CryptoStreamMode.Read);

            var magicBytes = await ReadBytesAsync(cs, magicString.Length, cancellationToken).ConfigureAwait(false);
            if (Encoding.UTF8.GetString(magicBytes) != magicString)
                throw new Exception(MAGIC_STRING_EXCEPTION);

            using var dst = CreateFile(dstFile, src.IsAsync);
            await cs.CopyToAsync(dst, BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
        }
    }
}
