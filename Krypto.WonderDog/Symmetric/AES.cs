namespace Krypto.WonderDog.Symmetric
{
    class AES : SymmetricBase, ISymmetric
    {
        public override string Algorithm => SymmetricFactory.ALGORITHM_AES;

        public override int BlockSize => 128;

        public override int KeySize => 256;
    }
}
