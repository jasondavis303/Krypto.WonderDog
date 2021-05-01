namespace Krypto.WonderDog.HMAC
{
    class SHA256 : HMACBase, IHMAC
    {
        public override string Algorithm => "SHA256";

        public override int HASH_BYTE_LENGTH => 32;
    }
}
