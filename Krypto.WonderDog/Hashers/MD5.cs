namespace Krypto.WonderDog.Hashers
{
    class MD5 : HasherBase, IHasher
    {
        public override string Algorithm => "MD5";

        public override int HASH_BYTE_LENGTH => 16;
    }
}
