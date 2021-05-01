namespace Krypto.WonderDog.HMAC
{
    class SHA1 : HMACBase, IHMAC
    {
        public override string Algorithm => "SHA1";

        public override int HASH_BYTE_LENGTH => 20;
    }
}
