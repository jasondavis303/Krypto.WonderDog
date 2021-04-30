using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Tests
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public void EncryptString()
        {
            var k = new AES();
            string s = k.Decrypt(k.Encrypt(Constants.TEST_DATA, Constants.PASSWORD), Constants.PASSWORD);
           
            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public void EncryptStringWithMagic()
        {
            var k = new AES();
            string s = k.Decrypt(k.Encrypt(Constants.TEST_DATA, Constants.PASSWORD, Constants.MAGIC_STRING), Constants.PASSWORD, Constants.MAGIC_STRING);
            
            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public void EncryptBytes()
        {
            var k = new AES();
            string s = Encoding.UTF8.GetString(k.Decrypt(k.Encrypt(Encoding.UTF8.GetBytes(Constants.TEST_DATA), Constants.PASSWORD), Constants.PASSWORD));
            
            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public void EncryptBytesWithMagic()
        {
            var k = new AES();
            string s = Encoding.UTF8.GetString(k.Decrypt(k.Encrypt(Encoding.UTF8.GetBytes(Constants.TEST_DATA), Constants.PASSWORD, Constants.MAGIC_STRING), Constants.PASSWORD, Constants.MAGIC_STRING));
           
            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptSameFileAsync()
        {
            string filename = "aes-temp1";

            File.WriteAllText(filename, Constants.TEST_DATA);
            var k = new AES();
            await k.EncryptFileAsync(filename, Constants.PASSWORD);
            await k.DecryptFileAsync(filename, Constants.PASSWORD);
            string s = File.ReadAllText(filename);
            File.Delete(filename);

            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptFileAsync()
        {
            string srcFile = "aes-temp2";
            string dstFile = "aes-temp3";
            string decFile = "aes-temp4";

            File.WriteAllText(srcFile, Constants.TEST_DATA);
            var k = new AES();
            await k.EncryptFileAsync(srcFile, dstFile, Constants.PASSWORD);
            await k.DecryptFileAsync(dstFile, decFile, Constants.PASSWORD);
            string s = File.ReadAllText(decFile);
            File.Delete(srcFile);
            File.Delete(dstFile);
            File.Delete(decFile);

            Assert.AreEqual(s, Constants.TEST_DATA);
        }

        [TestMethod]
        public async Task EncryptFileWithMagicStringAsync()
        {
            string srcFile = "aes-temp5";
            string dstFile = "aes-temp6";
            string decFile = "aes-temp7";

            File.WriteAllText(srcFile, Constants.TEST_DATA);
            var k = new AES();
            await k.EncryptFileAsync(srcFile, dstFile, Constants.PASSWORD, Constants.MAGIC_STRING);
            await k.DecryptFileAsync(dstFile, decFile, Constants.PASSWORD, Constants.MAGIC_STRING);
            string s = File.ReadAllText(decFile);
            File.Delete(srcFile);
            File.Delete(dstFile);
            File.Delete(decFile);

            Assert.AreEqual(s, Constants.TEST_DATA);
        }
    }
}
