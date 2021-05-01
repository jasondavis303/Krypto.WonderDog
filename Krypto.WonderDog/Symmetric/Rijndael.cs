namespace Krypto.WonderDog.Symmetric
{
    class Rijndael : SymmetricBase, ISymmetric
    {
        public override string Algorithm => SymmetricFactory.ALGORITHM_Rijndael;

        public override int BlockSize => 128;

        public override int KeySize => 256;
    }
}
