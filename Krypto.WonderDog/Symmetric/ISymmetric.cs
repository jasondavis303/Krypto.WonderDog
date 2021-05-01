using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Symmetric
{
    public interface ISymmetric
    {
        public string Algorithm { get; }

        public int BlockSize { get; }

        public int KeySize { get; }

        /// <summary>
        /// Encrypt small chunks of data
        /// </summary>
        public byte[] Encrypt(Key key, byte[] data);

        /// <summary>
        /// Encrypt small strings
        /// </summary>
        public string Encrypt(Key key, string data);

        /// <summary>
        /// Decrypt small  chunks of data
        /// </summary>
        public byte[] Decrypt(Key key, byte[] data);

        /// <summary>
        /// Decrypt small strings
        /// </summary>
        public string Decrypt(Key key, string data);

        public void EncryptFile(Key key, string sourceFile, string destinationFile);

        public void DecryptFile(Key key, string sourceFile, string destinationFile);


        public void EncryptStream(Key key, Stream sourceStream, Stream destinationStream);

        public void DecryptStream(Key key, Stream sourceStream, Stream destinationStream);


        public Task EncryptFileAsync(Key key, string sourceFile, string destinationFile, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task DecryptFileAsync(Key key, string sourceFile, string destinationFile, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);


        public Task EncryptStreamAsync(Key key, Stream sourceStream, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task DecryptStreamAsync(Key key, Stream sourceStream, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);


        public Task EncryptStreamAsync(Key key, Stream sourceStream, long sourceLength, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

        public Task DecryptStreamAsync(Key key, Stream sourceStream, long sourceLength, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default);

    }
}
