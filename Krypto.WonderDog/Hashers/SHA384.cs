namespace Krypto.WonderDog.Hashers
{
    class SHA384 : HasherBase, IHasher
    {
        public override string Algorithm => "SHA384";

        public override int HASH_BYTE_LENGTH => 48;
    }
}
