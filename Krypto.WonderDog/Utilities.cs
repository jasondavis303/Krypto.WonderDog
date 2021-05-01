using System.IO;

namespace Krypto.WonderDog
{
    static partial class Utilities
    {
        public const int BUFFER_SIZE = 1024 * 1024;

        public static FileStream OpenFile(string filename, bool async) => new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, async);

        public static FileStream CreateFile(string filename, bool async) => new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, async);

    }
}
