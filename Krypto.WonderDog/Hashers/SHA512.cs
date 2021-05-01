namespace Krypto.WonderDog.Hashers
{
    class SHA512 : HasherBase, IHasher
    {
        public override string Algorithm => "SHA512";

        public override int HASH_BYTE_LENGTH => 64;
    }
}
