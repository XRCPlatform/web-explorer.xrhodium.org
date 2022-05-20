namespace BitcoinRhExplorer.Entities.Blocks
{
    public class Block : RichEntity
    {
        public int Height { get; set; }
        public string BlockJson { get; set; }
        public string Hash { get; set; }

        public int HashIndex { get; set; }

        public int TxCount { get; set; }
        public decimal TotalOut { get; set; }
    }
}
