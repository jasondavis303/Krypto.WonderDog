using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krypto.WonderDog.Tests.Hashers
{
    [TestClass]
    public class MD5
    {
        private const string RAW = "Hello World!";
        private const string HASH = "ED-07-62-87-53-2E-86-36-5E-84-1E-92-BF-C5-0D-8C";

        [TestMethod]
        public void HashFile()
        {
            string tmpFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmpFile, RAW);
                var hasher = WonderDog.Hashers.HasherFactory.CreateMD5();
                string hash = BitConverter.ToString(hasher.HashFile(tmpFile));
                Assert.IsTrue(hash == HASH);
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }

        [TestMethod]
        public async Task HashFileASync()
        {
            string tmpFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmpFile, RAW);
                var hasher = WonderDog.Hashers.HasherFactory.CreateMD5();
                var bhash = await hasher.HashFileAsync(tmpFile).ConfigureAwait(false);
                string hash = BitConverter.ToString(bhash);
                Assert.IsTrue(hash == HASH);
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }
        
    }
}
