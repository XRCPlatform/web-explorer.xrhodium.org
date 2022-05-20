namespace BitcoinRhExplorer.Entities.Blocks
{
    public class Transaction : RichEntity
    {
        public long BlockId { get; set; }
        public Block Block { get; set; }

        public string Hash { get; set; }
        public int HashIndex { get; set; }
    }
}
