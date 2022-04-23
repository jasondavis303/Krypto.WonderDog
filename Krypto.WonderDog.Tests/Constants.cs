using System.Text;

namespace Krypto.WonderDog.Tests
{
    static class Constants
    {
        public const string TEST_STRING = "Krypto the Wonder Dog";
        public const string PASSWORD = "Cooler than Superman!";

        public static byte[] TEST_DATA => Encoding.Unicode.GetBytes(TEST_STRING);
    }
}
