namespace Krypto.WonderDog.HMAC
{
    class MD5 : HMACBase, IHMAC
    {
        public override string Algorithm => "MD5";

        public override int HASH_BYTE_LENGTH => 16;
    }
}
