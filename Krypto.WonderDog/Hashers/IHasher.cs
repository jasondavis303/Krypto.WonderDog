using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Hashers
{
    public interface IHasher
    {
        public string Algorithm { get; }

        /// <summary>
        /// Size of hash in bytes
        /// </summary>
        public int HASH_BYTE_LENGTH { get; }

        /// <summary>
        /// Size of hash in bits
        /// </summary>
        public int HASH_BIT_LENGTH { get; }

        /// <summary>
        /// Hash small chunks of data
        /// </summary>
        public byte[] Hash(byte[] data);

        /// <summary>
        /// Hash small strings
        /// </summary>
        public string Hash(string data);

        public byte[] HashFile(string filename);

        public byte[] HashStream(Stream stream);

        public Task<byte[]> HashFileAsync(string filename, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task<byte[]> HashStreamAsync(Stream stream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task<byte[]> HashStreamAsync(Stream stream, long streamSize, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);
    }
}
