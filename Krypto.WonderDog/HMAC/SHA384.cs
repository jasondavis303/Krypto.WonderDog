namespace Krypto.WonderDog.HMAC
{
    class SHA384 : HMACBase, IHMAC
    {
        public override string Algorithm => "SHA384";

        public override int HASH_BYTE_LENGTH => 48;
    }
}
