namespace Krypto.WonderDog.HMAC
{
    public static class HMACFactory
    {
        public static IHMAC CreateMD5() => new MD5();

        public static IHMAC CreateSHA1() => new SHA1();

        public static IHMAC CreateSHA256() => new SHA256();

        public static IHMAC CreateSHA384() => new SHA384();

        public static IHMAC CreateSHA512() => new SHA512();
    }
}
