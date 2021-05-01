namespace Krypto.WonderDog.Hashers
{
    class SHA1 : HasherBase, IHasher
    {
        public override string Algorithm => "SHA1";

        public override int HASH_BYTE_LENGTH => 20;
    }
}
