namespace Krypto.WonderDog.Symmetric
{
    class TripleDES : SymmetricBase, ISymmetric
    {
        public override string Algorithm => SymmetricFactory.ALGORITHM_TripleDES;

        public override int BlockSize => 64;

        public override int KeySize => 192;
    }
}
