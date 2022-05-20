using System.ComponentModel.DataAnnotations.Schema;

namespace BitcoinRhExplorer.Entities.Blocks
{
    public class AddressFromTo : RichEntity
    {
        public long BlockId { get; set; }
        public Block Block { get; set; }

        public string Address { get; set; }
        public int AddressIndex { get; set; }
    }
}
