using System;

namespace Krypto.WonderDog.Symmetric
{
    public static class SymmetricFactory
    {
        public const string ALGORITHM_AES = "AES";
        public const string ALGORITHM_DES = "DES";
        public const string ALGORITHM_RC2 = "RC2";
        public const string ALGORITHM_Rijndael = "Rijndael";
        public const string ALGORITHM_TripleDES = "TripleDES";


        public static ISymmetric CreateAES() => new AES();

        public static ISymmetric CreateDES() => new DES();

        public static ISymmetric CreateRC2() => new RC2();

        public static ISymmetric CreateRijndael() => new Rijndael();

        public static ISymmetric CreateTripleDES() => new TripleDES();

        public static ISymmetric Create(string name)
        {
            return name switch
            {
                ALGORITHM_AES => new AES(),
                ALGORITHM_DES => new DES(),
                ALGORITHM_RC2 => new RC2(),
                ALGORITHM_Rijndael => new Rijndael(),
                ALGORITHM_TripleDES => new TripleDES(),
                _ => throw new Exception("Invalid algorithm"),
            };
        }
    }
}
