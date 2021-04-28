using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
    [TestClass]
    public class KryptoTests
    {
        const string TEST_DATA = "Krypto the Wonder Dog";
        const string MAGIC_STRING = "Superman's Dog";
        const string PASSWORD = "Cooler than Superman!";

        [TestMethod]
        public void EncryptString()
        {
            var k = new Krypto();
            string s = k.Decrypt(k.Encrypt(TEST_DATA, PASSWORD), PASSWORD);
           
            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public void EncryptStringWithMagic()
        {
            var k = new Krypto();
            string s = k.Decrypt(k.Encrypt(TEST_DATA, PASSWORD, MAGIC_STRING), PASSWORD, MAGIC_STRING);
            
            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public void EncryptBytes()
        {
            var k = new Krypto();
            string s = Encoding.UTF8.GetString(k.Decrypt(k.Encrypt(Encoding.UTF8.GetBytes(TEST_DATA), PASSWORD), PASSWORD));
            
            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public void EncryptBytesWithMagic()
        {
            var k = new Krypto();
            string s = Encoding.UTF8.GetString(k.Decrypt(k.Encrypt(Encoding.UTF8.GetBytes(TEST_DATA), PASSWORD, MAGIC_STRING), PASSWORD, MAGIC_STRING));
           
            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptSameFileAsync()
        {
            string filename = "temp1";

            File.WriteAllText(filename, TEST_DATA);
            var k = new Krypto();
            await k.EncryptFileAsync(filename, PASSWORD);
            await k.DecryptFileAsync(filename, PASSWORD);
            string s = File.ReadAllText(filename);
            File.Delete(filename);

            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptFileAsync()
        {
            string srcFile = "temp2";
            string dstFile = "temp3";
            string decFile = "temp4";

            File.WriteAllText(srcFile, TEST_DATA);
            var k = new Krypto();
            await k.EncryptFileAsync(srcFile, dstFile, PASSWORD);
            await k.DecryptFileAsync(dstFile, decFile, PASSWORD);
            string s = File.ReadAllText(decFile);
            File.Delete(srcFile);
            File.Delete(dstFile);
            File.Delete(decFile);

            Assert.AreEqual(s, TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptFileWithMagicStringAsync()
        {
            string srcFile = "temp5";
            string dstFile = "temp6";
            string decFile = "temp7";

            File.WriteAllText(srcFile, TEST_DATA);
            var k = new Krypto();
            await k.EncryptFileAsync(srcFile, dstFile, PASSWORD, MAGIC_STRING);
            await k.DecryptFileAsync(dstFile, decFile, PASSWORD, MAGIC_STRING);
            string s = File.ReadAllText(decFile);
            File.Delete(srcFile);
            File.Delete(dstFile);
            File.Delete(decFile);

            Assert.AreEqual(s, TEST_DATA);
        }

    }
}
