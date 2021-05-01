namespace Krypto.WonderDog.HMAC
{
    class SHA512 : HMACBase, IHMAC
    {
        public override string Algorithm => "SHA512";

        public override int HASH_BYTE_LENGTH => 64;
    }
}
