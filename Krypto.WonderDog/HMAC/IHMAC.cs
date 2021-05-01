using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.HMAC
{
    public interface IHMAC
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
        public byte[] Hash(Key key, byte[] data);

        /// <summary>
        /// Hash small strings
        /// </summary>
        public string Hash(Key key, string data);

        public byte[] HashFile(Key key, string filename);

        public Task<byte[]> HashFileAsync(Key key, string filename, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public byte[] HashStream(Key key, Stream stream);

        public Task<byte[]> HashStreamAsync(Key key, Stream stream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task<byte[]> HashStreamAsync(Key key, Stream stream, long streamLength, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);
    }
}
