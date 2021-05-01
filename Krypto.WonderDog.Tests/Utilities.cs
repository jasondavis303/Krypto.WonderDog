using System.IO;

namespace Krypto.WonderDog.Tests
{
    static class Utilities
    {
        public static bool ValidateData(byte[] data)
        {
            var testData = Constants.TEST_DATA;

            if (data.Length != testData.Length)
                return false;

            for (int i = 0; i < data.Length; i++)
                if (data[i] != testData[i])
                    return false;

            return true;
        }

        public static bool ValidateData(byte[] test, byte[] compare)
        {
            if (test.Length != compare.Length)
                return false;

            for (int i = 0; i < test.Length; i++)
                if (test[i] != compare[i])
                    return false;

            return true;
        }

        public static void Cleanup(string file1, string file2, string file3)
        {
            Cleanup(file1);
            Cleanup(file2);
            Cleanup(file3);
        }

        public static void Cleanup(string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            catch { }
        }
    }
}
