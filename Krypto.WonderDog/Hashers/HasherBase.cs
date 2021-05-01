using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Hashers
{
    abstract class HasherBase : IHasher
    {
        public abstract string Algorithm { get; }

        public abstract int HASH_BYTE_LENGTH { get; }

        public int HASH_BIT_LENGTH => HASH_BYTE_LENGTH * 8;

        public byte[] Hash(byte[] data)
        {
            using var alg = HashAlgorithm.Create(Algorithm);
            return alg.ComputeHash(data);
        }

        public string Hash(string data) => BitConverter.ToString(Hash(Encoding.UTF8.GetBytes(data)));

        public byte[] HashFile(string filename)
        {
            using var fs = Utilities.OpenFile(filename, false);
            return HashStream(fs);
        }

        public Task<byte[]> HashFileAsync(string filename, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            using var fs = Utilities.OpenFile(filename, true);
            return HashStreamAsync(fs, -1, progress, cancellationToken);
        }

        public byte[] HashStream(Stream stream)
        {
            using var alg = HashAlgorithm.Create(Algorithm);
            return alg.ComputeHash(stream);
        }

        public Task<byte[]> HashStreamAsync(Stream stream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default) => HashStreamAsync(stream, -1, progress, cancellationToken);

        public async Task<byte[]> HashStreamAsync(Stream stream, long streamSize, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            DateTime started = DateTime.Now;
            long totalRead = 0;
            if (streamSize == -1)
                try { streamSize = stream.Length; }
                catch { }

            progress?.Report(new KryptoProgress(started, totalRead, streamSize));

            using var alg = HashAlgorithm.Create(Algorithm);
            byte[] buffer = new byte[Utilities.BUFFER_SIZE];
            int bytesRead;
            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (bytesRead > 0)
                    alg.TransformBlock(buffer, 0, bytesRead, null, 0);
                totalRead += bytesRead;
                progress?.Report(new KryptoProgress(started, totalRead, streamSize));
            } while (bytesRead > 0);
            alg.TransformFinalBlock(buffer, 0, 0);

            progress?.Report(new KryptoProgress(started, totalRead, streamSize));

            return alg.Hash;
        }
    }
}
