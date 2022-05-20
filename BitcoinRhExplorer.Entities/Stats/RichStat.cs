using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinRhExplorer.Entities.Stats
{
    public class RichStat : RichEntity
    {
        public int Height { get; set; }

        public string Address { get; set; }
        public int AddressIndex { get; set; }

        public decimal Amount { get; set; }
    }
}
