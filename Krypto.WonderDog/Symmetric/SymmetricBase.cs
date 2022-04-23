    using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Symmetric
{
    abstract class SymmetricBase : ISymmetric
    {
        public abstract string Algorithm { get; }

        public abstract int BlockSize { get; }

        public abstract int KeySize { get; }

        private int IVSize => BlockSize / 8;



        public byte[] Encrypt(Key key, byte[] data)
        {
            using var alg = CreateAlg(key);
            using var encryptor = alg.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(alg.IV, 0, alg.IV.Length);
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        public byte[] Decrypt(Key key, byte[] data)
        {
            using var alg = CreateAlg(key);
            using var msEncrypted = new MemoryStream(data);
            alg.IV = ReadBytes(msEncrypted, IVSize);
            using var decryptor = alg.CreateDecryptor();
            using var cs = new CryptoStream(msEncrypted, decryptor, CryptoStreamMode.Read);
            using var msDecrypted = new MemoryStream();
            cs.CopyTo(msDecrypted);
            return msDecrypted.ToArray();
        }



        public string Encrypt(Key key, string data) => Convert.ToBase64String(Encrypt(key, Encoding.Unicode.GetBytes(data)));

        public string Decrypt(Key key, string data) => Encoding.Unicode.GetString(Decrypt(key, Convert.FromBase64String(data)));



        public void EncryptFile(Key key, string sourceFile, string destinationFile)
        {
            using var src = Utilities.OpenFile(sourceFile, false);
            using var dst = Utilities.CreateFile(destinationFile, false);
            EncryptStream(key, src, dst);
        }

        public void DecryptFile(Key key, string sourceFile, string destinationFile)
        {
            using var src = Utilities.OpenFile(sourceFile, false);
            using var dst = Utilities.CreateFile(destinationFile, false);
            DecryptStream(key, src, dst);
        }






        public void EncryptStream(Key key, Stream sourceStream, Stream destinationStream)
        {
            using var alg = CreateAlg(key);
            destinationStream.Write(alg.IV, 0, alg.IV.Length);
            using var encryptor = alg.CreateEncryptor();
            using var cs = new CryptoStream(destinationStream, encryptor, CryptoStreamMode.Write, true);
            sourceStream.CopyTo(cs, Utilities.BUFFER_SIZE);
            cs.FlushFinalBlock();
        }


        public void DecryptStream(Key key, Stream sourceStream, Stream destinationStream)
        {
            using var alg = CreateAlg(key);
            alg.IV = ReadBytes(sourceStream, IVSize);
            using var decryptor = alg.CreateDecryptor();
            using var cs = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read, true);
            cs.CopyTo(destinationStream, Utilities.BUFFER_SIZE);
        }





        public async Task EncryptFileAsync(Key key, string sourceFile, string destinationFile, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            using var sourceStream = Utilities.OpenFile(sourceFile, true);
            using var destinationStream = Utilities.CreateFile(destinationFile, true);
            await EncryptStreamAsync(key, sourceStream, destinationStream, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task DecryptFileAsync(Key key, string sourceFile, string destinationFile, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            using var sourceStream = Utilities.OpenFile(sourceFile, true);
            using var destinationStream = Utilities.CreateFile(destinationFile, true);
            await DecryptStreamAsync(key, sourceStream, destinationStream, progress, cancellationToken).ConfigureAwait(false);
        }





        public Task EncryptStreamAsync(Key key, Stream sourceStream, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            long size = -1;
            try { size = sourceStream.Length; }
            catch { }
            return EncryptStreamAsync(key, sourceStream, size, destinationStream, progress, cancellationToken);
        }

        public Task DecryptStreamAsync(Key key, Stream sourceStream, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            long size = -1;
            try { size = sourceStream.Length; }
            catch { }
            return DecryptStreamAsync(key, sourceStream, size, destinationStream, progress, cancellationToken);
        }








        public async Task EncryptStreamAsync(Key key, Stream sourceStream, long sourceLength, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            DateTime started = DateTime.Now;
            long totalRead = 0;

            using var alg = CreateAlg(key);
#if NET472
            await destinationStream.WriteAsync(alg.IV, 0, alg.IV.Length, cancellationToken).ConfigureAwait(false);
#else
            await destinationStream.WriteAsync(alg.IV.AsMemory(0, alg.IV.Length), cancellationToken).ConfigureAwait(false);
#endif
            if (progress != null)
            {
                sourceLength += alg.IV.Length;
                totalRead += alg.IV.Length;
                progress.Report(new KryptoProgress(started, totalRead, sourceLength, false));
            }

            using var encryptor = alg.CreateEncryptor();
            using var cs = new CryptoStream(destinationStream, encryptor, CryptoStreamMode.Write, true);

            if (progress == null)
            {
                await sourceStream.CopyToAsync(cs, Utilities.BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                byte[] buffer = new byte[Utilities.BUFFER_SIZE];
                int bytesRead;
                do
                {
#if NET472
                    bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
#else
                    bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
#endif
                    if (bytesRead > 0)
                    {
#if NET472
                        await cs.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
#else
                        await cs.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
#endif
                        totalRead += bytesRead;
                        progress?.Report(new KryptoProgress(started, totalRead, sourceLength, false));
                    }
                } while (bytesRead > 0);
            }

            cs.FlushFinalBlock();

            progress?.Report(new KryptoProgress(started, totalRead, totalRead, true));
        }

        public async Task DecryptStreamAsync(Key key, Stream sourceStream, long sourceLength, Stream destinationStream, IProgress<KryptoProgress> progress = null, CancellationToken cancellationToken = default)
        {
            DateTime started = DateTime.Now;
            long totalRead = 0;

            using var alg = CreateAlg(key);
            alg.IV = await ReadBytesAsync(sourceStream, IVSize, cancellationToken).ConfigureAwait(false);

            if (progress != null)
            {
                sourceLength += alg.IV.Length;
                totalRead += alg.IV.Length;
                progress.Report(new KryptoProgress(started, totalRead, sourceLength, false));
            }

            using var decryptor = alg.CreateDecryptor();
            using var cs = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read, true);

            if (progress == null)
            {
                await cs.CopyToAsync(destinationStream, Utilities.BUFFER_SIZE, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                byte[] buffer = new byte[Utilities.BUFFER_SIZE];
                int bytesRead;
                do
                {
#if NET472
                    bytesRead = await cs.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
#else
                    bytesRead = await cs.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
#endif
                    if (bytesRead > 0)
                    {
#if NET472
                        await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
#else
                        await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
#endif
                        totalRead += bytesRead;
                        progress?.Report(new KryptoProgress(started, totalRead, sourceLength, false));
                    }
                } while (bytesRead > 0);
            }

            progress?.Report(new KryptoProgress(started, totalRead, totalRead, true));
        }









        private SymmetricAlgorithm CreateAlg(Key key)
        {
            var ret = SymmetricAlgorithm.Create(Algorithm);
            key.SetKey(ret);
            return ret;
        }


        private static byte[] ReadBytes(Stream stream, int cnt)
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

        private static async Task<byte[]> ReadBytesAsync(Stream stream, int cnt, CancellationToken cancellationToken)
        {
            var ret = new byte[cnt];
            int totalRead = 0;
            int read;
#if NET472
            while ((read = await stream.ReadAsync(ret, totalRead, ret.Length - totalRead, cancellationToken).ConfigureAwait(false)) > 0)
#else
            while ((read = await stream.ReadAsync(ret.AsMemory(totalRead, ret.Length - totalRead), cancellationToken).ConfigureAwait(false)) > 0)
#endif
            {
                totalRead += read;
                if (totalRead == ret.Length)
                    break;
            }

            return ret;
        }
    }
}
