namespace Krypto.WonderDog.Symmetric
{
    class DES : SymmetricBase, ISymmetric
    {
        public override string Algorithm => SymmetricFactory.ALGORITHM_DES;

        public override int BlockSize => 64;

        public override int KeySize => 64;
    }
}
