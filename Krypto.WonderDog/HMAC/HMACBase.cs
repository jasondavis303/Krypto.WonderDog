using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.HMAC
{
    abstract class HMACBase : IHMAC
    {
        public abstract string Algorithm { get; }

        public abstract int HASH_BYTE_LENGTH { get; }

        public int HASH_BIT_LENGTH => HASH_BYTE_LENGTH * 8;

        public byte[] Hash(Key key, byte[] data)
        {
            using var hmac = CreateHMAC(key.DeriveKey());
            return hmac.ComputeHash(data);
        }

        public string Hash(Key key, string data) => BitConverter.ToString(Hash(key, Encoding.UTF8.GetBytes(data)));
        
        public byte[] HashFile(Key key, string filename)
        {
            using var fs = Utilities.OpenFile(filename, false);
            return HashStream(key, fs);
        }

        public Task<byte[]> HashFileAsync(Key key, string filename, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            using var fs = Utilities.OpenFile(filename, true);
            return HashStreamAsync(key, fs, -1, progress, cancellationToken);
        }

        public byte[] HashStream(Key key, Stream stream)
        {
            using var hmac = CreateHMAC(key.DeriveKey());
            return hmac.ComputeHash(stream);
        }

        public Task<byte[]> HashStreamAsync(Key key, Stream stream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default) => HashStreamAsync(key, stream, -1, progress, cancellationToken);

        public async Task<byte[]> HashStreamAsync(Key key, Stream stream, long streamSize, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            DateTime started = DateTime.Now;
            long totalRead = 0;
            if (streamSize == -1)
                try { streamSize = stream.Length; }
                catch { }

            progress?.Report(new KryptoProgress(started, totalRead, streamSize, false));

            using var alg = HashAlgorithm.Create(Algorithm);
            byte[] buffer = new byte[Utilities.BUFFER_SIZE];
            int bytesRead;
            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (bytesRead > 0)
                    alg.TransformBlock(buffer, 0, bytesRead, null, 0);
                totalRead += bytesRead;
                progress?.Report(new KryptoProgress(started, totalRead, streamSize, false));
            } while (bytesRead > 0);
            alg.TransformFinalBlock(buffer, 0, 0);

            progress?.Report(new KryptoProgress(started, totalRead, streamSize, true));

            return alg.Hash;
        }


        private System.Security.Cryptography.HMAC CreateHMAC(byte[] key) =>
            Algorithm switch
            {
                "MD5" => new HMACMD5(key),
                "SHA1" => new HMACSHA1(key),
                "SHA256" => new HMACSHA256(key),
                "SHA384" => new HMACSHA384(key),
                "SHA512" => new HMACSHA512(key),
                _ => throw new Exception("Invalid algorithm"),
            };
    }
}