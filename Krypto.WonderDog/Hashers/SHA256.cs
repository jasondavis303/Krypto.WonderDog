namespace Krypto.WonderDog.Hashers
{
    class SHA256 : HasherBase, IHasher
    {
        public override string Algorithm => "SHA256";

        public override int HASH_BYTE_LENGTH => 32;
    }
}
