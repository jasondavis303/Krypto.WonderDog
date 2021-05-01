namespace Krypto.WonderDog.Hashers
{
    public static class HasherFactory
    {
        public static IHasher CreateMD5() => new MD5();

        public static IHasher CreateSHA1() => new SHA1();

        public static IHasher CreateSHA256() => new SHA256();

        public static IHasher CreateSHA384() => new SHA384();

        public static IHasher CreateSHA512() => new SHA512();
    }
}
