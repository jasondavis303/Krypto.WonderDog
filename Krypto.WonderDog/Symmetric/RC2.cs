namespace Krypto.WonderDog.Symmetric
{
    class RC2 : SymmetricBase, ISymmetric
    {
        public override string Algorithm => SymmetricFactory.ALGORITHM_RC2;

        public override int BlockSize => 64;

        public override int KeySize => 128;
    }
}
