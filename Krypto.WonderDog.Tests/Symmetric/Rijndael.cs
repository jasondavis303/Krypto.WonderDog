using Krypto.WonderDog.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Symmetric
{
    [TestClass]
    public class Rijndael
    {
        [TestMethod]
        public void EncryptData()
        {
            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            var test = sa.Decrypt(key, sa.Encrypt(key, Constants.TEST_DATA));
            Assert.IsTrue(Utilities.ValidateData(test));
        }

        [TestMethod]
        public void EncryptString()
        {
            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            string test = sa.Decrypt(key, sa.Encrypt(key, Constants.TEST_STRING));
            Assert.AreEqual(test, Constants.TEST_STRING);
        }

        [TestMethod]
        public void EncryptFile()
        {
            string srcFile = Path.GetTempFileName();
            string encFile = Path.GetTempFileName();
            string decFile = Path.GetTempFileName();

            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            try
            {
                File.WriteAllText(srcFile, Constants.TEST_STRING);
                sa.EncryptFile(key, srcFile, encFile);
                sa.DecryptFile(key, encFile, decFile);
                string test = File.ReadAllText(decFile);
                Assert.AreEqual(test, Constants.TEST_STRING);
            }
            finally
            {
                Utilities.Cleanup(srcFile, encFile, decFile);
            }
        }

        [TestMethod]
        public void EncryptStream()
        {
            using var src = new MemoryStream(Constants.TEST_DATA);
            using var enc = new MemoryStream();
            using var dec = new MemoryStream();

            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            sa.EncryptStream(key, src, enc);
            enc.Seek(0, SeekOrigin.Begin);
            sa.DecryptStream(key, enc, dec);
            var test = dec.ToArray();
            Assert.IsTrue(Utilities.ValidateData(test));
        }



        [TestMethod]
        public async Task EncryptFileAsync()
        {
            string srcFile = Path.GetTempFileName();
            string encFile = Path.GetTempFileName();
            string decFile = Path.GetTempFileName();

            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            try
            {
                File.WriteAllText(srcFile, Constants.TEST_STRING);
                await sa.EncryptFileAsync(key, srcFile, encFile).ConfigureAwait(false);
                await sa.DecryptFileAsync(key, encFile, decFile).ConfigureAwait(false);
                string test = File.ReadAllText(decFile);
                Assert.AreEqual(test, Constants.TEST_STRING);
            }
            finally
            {
                Utilities.Cleanup(srcFile, encFile, decFile);
            }
        }

        [TestMethod]
        public async Task EncryptStreamAsync()
        {
            using var src = new MemoryStream(Constants.TEST_DATA);
            using var enc = new MemoryStream();
            using var dec = new MemoryStream();

            var sa = SymmetricFactory.CreateRijndael();
            var key = new Key(Constants.PASSWORD);
            await sa.EncryptStreamAsync(key, src, enc).ConfigureAwait(false);
            enc.Seek(0, SeekOrigin.Begin);
            await sa.DecryptStreamAsync(key, enc, dec).ConfigureAwait(false);
            var test = dec.ToArray();
            Assert.IsTrue(Utilities.ValidateData(test));
        }

    }
}
