using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business.Stats;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using static NBitcoin.RPC.RPCClient;

namespace BitcoinRhExplorer.Components
{
    public class ExplorerHumanizedBlockModel
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public string TxCount { get; set; }
        public string DateTime { get; set; }
        public string Money { get; set; }
    }

    public class RichHumanizedModel
    {
        public string Address { get; set; }
        public int Height { get; set; }
        public decimal Amount { get; set; }
    }

    public static class RPCCacheHelper
    {
        private const string KEY_CACHE_LATESTBLOCKS = "KEY_LATEST_BLOCKS";
        private const string KEY_CACHE_NODEINFO = "KEY_NODE_INFO";
        private const string KEY_CACHE_MEMPOOLINFO = "KEY_MEMPOOL_INFO";
        private const string KEY_CACHE_RAWMEMPOOL = "KEY_MEMPOOL_RAW";
        private const string KEY_CACHE_BLOCKHASH = "KEY_B_";
        private const string KEY_CACHE_TXHASH = "KEY_T_";
        private const string KEY_CACHE_ADDRESSHASH = "KEY_A_";
        private const string KEY_CACHE_ADDRESSHASH_COUNT = "KEY_A_COUNT";
        private const string KEY_CACHE_INCIRCULATIONS = "KEY_CACHE_INCIRCULATIONS";
        private const string KEY_CACHE_TOTALCIRCULATIONS = "KEY_CACHE_TOTALCIRCULATIONS";
        private const string KEY_CACHE_RICH = "KEY_CACHE_RICH";
        private const string KEY_CACHE_RAWTXINFO = "KEY_TXINFO_";

        public static List<RichHumanizedModel> GetCachedRich(RichStatComponent richStats)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.LongCache.Get(KEY_CACHE_RICH);
            var richAddr = new List<RichHumanizedModel>();

            if (cached == null)
            {
                var list = richStats.GetAll(1000);

                foreach (var item in list)
                {
                    var address = new RichHumanizedModel();
                    address.Address = item.Address;
                    address.Amount = item.Amount;
                    address.Height = item.Height;
                    richAddr.Add(address);
                }

                BitcoinRhExplorerServer.Current.Cache.LongCache.Add(KEY_CACHE_RICH, JsonConvert.SerializeObject(richAddr));
            }
            else
            {
                richAddr = JsonConvert.DeserializeObject<List<RichHumanizedModel>>(cached);
            }

            return richAddr;
        }

        public static decimal GetCachedCirculationSupply(RPCClient rpc)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.LongCache.Get(KEY_CACHE_INCIRCULATIONS);
            decimal totalInCirculation = 0;

            if (cached == null)
            {
                var blocks = GetCachedLatestBlocks(rpc, 20);

                if (blocks != null)
                {
                    var block = blocks.First();
                    var targetHeight = block.Height;
                    decimal inCirculation = 0;

                    var airdropped = Int32.Parse(ConfigurationManager.AppSettings["Explorer_AirDropped"].ToString());
                    var pr = 105000;
                    var dev = 105000;
                    var sh = 257143;

                    for (int i = 1; i < targetHeight; i++) //0 = premine block
                    {
                        int div = i / 210000;
                        decimal reward = (decimal)2.5 / (div + 1);

                        inCirculation = inCirculation + reward;
                    }

                    totalInCirculation = inCirculation + airdropped + pr + dev + sh;

                    var devnotused = new Money(long.Parse(ConfigurationManager.AppSettings["Explorer_DevNotUsedSatoshi"].ToString()), MoneyUnit.Satoshi)
                        .ToUnit(MoneyUnit.XRC);
                    var shnotused = new Money(long.Parse(ConfigurationManager.AppSettings["Explorer_SHNotUsedSatoshi"].ToString()), MoneyUnit.Satoshi)
                        .ToUnit(MoneyUnit.XRC);

                    totalInCirculation = totalInCirculation - devnotused - shnotused;

                    //var block = blocks.First();
                    //var targetHeight = block.Height;
                    //decimal inCirculation = 0;

                    //var airdropped = int.Parse(ConfigurationManager.AppSettings["Explorer_AirDropped"].ToString());
                    //var shrewarded = int.Parse(ConfigurationManager.AppSettings["Explorer_SHRewarded"].ToString());
                    //var pr = 105000;
                    //var dev = 105000;
                    //var devnotused = new Money(long.Parse(ConfigurationManager.AppSettings["Explorer_DevNotUsedSatoshi"].ToString()), MoneyUnit.Satoshi)
                    //    .ToUnit(MoneyUnit.XRC);

                    //for (int i = 1; i < targetHeight; i++) //0 = premine block
                    //{
                    //    int div = i / 210000;
                    //    decimal reward = (decimal)2.5 / (div + 1);

                    //    inCirculation = inCirculation + reward;
                    //}

                    //totalInCirculation = inCirculation + airdropped + pr + dev + shrewarded - devnotused;

                    BitcoinRhExplorerServer.Current.Cache.LongCache.Add(KEY_CACHE_INCIRCULATIONS, JsonConvert.SerializeObject(totalInCirculation));
                }
            }
            else
            {
                totalInCirculation = JsonConvert.DeserializeObject<decimal>(cached);
            }

            return totalInCirculation;
        }

        internal static decimal GetCachedTotalSupply(RPCClient rpc)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.LongCache.Get(KEY_CACHE_TOTALCIRCULATIONS);
            decimal totalInCirculation = 0;

            if (cached == null)
            {
                var blocks = GetCachedLatestBlocks(rpc, 20);

                if (blocks != null)
                {
                    var block = blocks.First();
                    var targetHeight = block.Height;
                    decimal inCirculation = 0;

                    var airdropped = Int32.Parse(ConfigurationManager.AppSettings["Explorer_AirDropped"].ToString());
                    var pr = 105000;
                    var dev = 105000;
                    var sh = 257143;

                    for (int i = 1; i < targetHeight; i++) //0 = premine block
                    {
                        int div = i / 210000;
                        decimal reward = (decimal)2.5 / (div + 1);

                        inCirculation = inCirculation + reward;
                    }

                    totalInCirculation = inCirculation + airdropped + pr + dev + sh;

                    BitcoinRhExplorerServer.Current.Cache.LongCache.Add(KEY_CACHE_TOTALCIRCULATIONS, JsonConvert.SerializeObject(totalInCirculation));
                }
            }
            else
            {
                totalInCirculation = JsonConvert.DeserializeObject<decimal>(cached);
            }

            return totalInCirculation;
        }

        public static NodeInfo GetCachedNodeInfo(RPCClient rpc)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_NODEINFO);
            NodeInfo nodeInfo = null;

            if (cached == null)
            {
                try
                {
                    nodeInfo = rpc.GetNodeInfo();
                    BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_NODEINFO, JsonConvert.SerializeObject(nodeInfo));
                }
                catch (Exception e)
                {
                    if (!HttpContext.Current.Request.IsLocal)
                        MailProcessing.Send(e.Message + e.InnerException.ToString(), ConfigurationManager.AppSettings["RPC_BTRNodeReportEmail_Subject"], ConfigurationManager.AppSettings["RPC_BTRNodeReportEmail"]);
                    //server is down
                }
            }
            else
            {
                nodeInfo = JsonConvert.DeserializeObject<NodeInfo>(cached);
            }

            return nodeInfo;
        }

        public static MempoolInfo GetCachedMemPoolInfo(RPCClient rpc)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_MEMPOOLINFO);
            MempoolInfo mempoolInfo = null;

            if (cached == null)
            {
                try
                {
                    mempoolInfo = rpc.GetMemPoolInfo();
                    BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_MEMPOOLINFO, JsonConvert.SerializeObject(mempoolInfo));
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                mempoolInfo = JsonConvert.DeserializeObject<MempoolInfo>(cached);
            }

            return mempoolInfo;
        }

        public static Dictionary<string, MemPoolEntry> GetCachedRawMemPool(RPCClient rpc)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_RAWMEMPOOL);
            Dictionary<string, MemPoolEntry> mempool = null;

            if (cached == null)
            {
                try
                {
                    mempool = rpc.GetRawMempoolInfo();
                    BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_RAWMEMPOOL, JsonConvert.SerializeObject(mempool));
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                mempool = JsonConvert.DeserializeObject<Dictionary<string, MemPoolEntry>>(cached);
            }

            return mempool;
        }

        public static Transaction GetCachedRawTxInfo(RPCClient rpc, string txId)
        {
            var key = string.Format("{0}{1}", KEY_CACHE_RAWTXINFO, txId);
            var cached = BitcoinRhExplorerServer.Current.Cache.LongCache.Get(key);
            Transaction txinfo = null;

            if (cached == null)
            {
                try
                {
                    txinfo = rpc.GetRawTransaction(new uint256(txId));
                    BitcoinRhExplorerServer.Current.Cache.LongCache.Add(key, JsonConvert.SerializeObject(txinfo));
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                txinfo = JsonConvert.DeserializeObject<Transaction>(cached);
            }

            return txinfo;
        }

        public static List<ExplorerBlockModel> GetCachedLatestBlocks(RPCClient rpc, int limit)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_LATESTBLOCKS);
            var latestBlocks = new List<ExplorerBlockModel>();

            if (cached == null)
            {
                var blocks = rpc.GetExplorerLatestBlocks(limit + 20);
                DBCacheHelper.AddBlocks(blocks);
                blocks = blocks.Take(limit).ToList();

                foreach (var itemBlock in blocks)
                {
                    latestBlocks.Add(HumanizeBlock(itemBlock));
                }

                BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_LATESTBLOCKS, JsonConvert.SerializeObject(latestBlocks));
            }
            else
            {
                latestBlocks = JsonConvert.DeserializeObject<List<ExplorerBlockModel>>(cached);
            }

            return latestBlocks;
        }

        public static ExplorerBlockModel GetCachedBlock(RPCClient rpc, string hash)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_BLOCKHASH + hash);
            var block = new ExplorerBlockModel();

            if (cached == null)
            {
                block = DBCacheHelper.GetExplorerBlock(hash);

                if (block == null)
                {
                    block = rpc.GetExplorerBlock(hash);
                    if ((block == null) || (block.Version == 0))
                    {
                        return null;
                    }
                    DBCacheHelper.AddBlock(block);
                }

                block = HumanizeBlock(block);

                BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_BLOCKHASH + hash, JsonConvert.SerializeObject(block));
            }
            else
            {
                block = JsonConvert.DeserializeObject<ExplorerBlockModel>(cached);
            }

            return block;
        }

        public static ExplorerTransactionModel GetCachedTransaction(RPCClient rpc, string hash)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_TXHASH + hash);
            var block = new RPCClient.ExplorerBlockModel();

            if (cached == null)
            {
                block = DBCacheHelper.GetTransactionBlock(hash);

                if (block == null)
                {
                    block = rpc.GetExplorerTransaction(hash);
                    if ((block == null) || (block.Version == 0))
                    {
                        return null;
                    }
                    DBCacheHelper.AddBlock(block);
                }

                block = HumanizeBlock(block);

                BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_TXHASH + hash, JsonConvert.SerializeObject(block));
            }
            else
            {
                block = JsonConvert.DeserializeObject<ExplorerBlockModel>(cached);
            }

            if (block.Transactions == null)
            {
                return null;
            }

            return block.Transactions.Where(a => a.Hash == hash).FirstOrDefault();
        }

        public static Tuple<List<ExplorerTransactionModel>, int> GetCachedAddress(RPCClient rpc, string address, int offset)
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_ADDRESSHASH + address + offset);
            var cachedCount = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_ADDRESSHASH_COUNT + address + offset);
            var blocks = new List<ExplorerBlockModel>();

            var txCount = 0;
            var txs = new List<ExplorerTransactionModel>();

            if ((cached == null) || (cachedCount == null))
            {
                var dbBlocks = DBCacheHelper.GetAddressBlock(address, offset);
                txCount = DBCacheHelper.GetCountAddressBlock(address);

                //var blockOffset = DBCacheHelper.GetBlockOffset();
                //var blockIgnore = DBCacheHelper.GetBlockIgnore(blockOffset);

                //var rpcBlocks = rpc.GetExplorerAddress(blockOffset, JsonConvert.SerializeObject(blockIgnore), address);
                //DBCacheHelper.AddBlocks(rpcBlocks);

                //blocks = rpcBlocks;

                if (dbBlocks != null)
                {
                    blocks.AddRange(dbBlocks);
                }

                if (blocks == null)
                {
                    return null;
                }

                blocks = blocks.OrderByDescending(a => a.Height).ToList();

                var latestBlocks = new List<ExplorerBlockModel>();
                foreach (var itemBlock in blocks)
                {
                    latestBlocks.Add(HumanizeBlock(itemBlock));
                }
                blocks = latestBlocks;

                BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_ADDRESSHASH + address + offset, JsonConvert.SerializeObject(blocks));
                BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_ADDRESSHASH_COUNT + address + offset, JsonConvert.SerializeObject(txCount));
            }
            else
            {
                blocks = JsonConvert.DeserializeObject<List<ExplorerBlockModel>>(cached);
                txCount = JsonConvert.DeserializeObject<int>(cachedCount);
            }

            foreach (var itemBlock in blocks)
            {
                foreach (var itemTx in itemBlock.Transactions)
                {
                    if (itemTx.AddressFrom.Exists(a => a.Address == address))
                    {
                        txs.Add(itemTx);
                        continue;
                    }
                    if (itemTx.AddressTo.Exists(a => a.Address == address))
                    {
                        txs.Add(itemTx);
                        continue;
                    }
                }
            }

            return new Tuple<List<ExplorerTransactionModel>, int>(txs, txCount);
        }

        internal static bool UpdateCacheExplorerAddressByHeight(RPCClient rpc, List<int> heights)
        {
            var rpcBlocks = rpc.GetExplorerAddressByHeight(JsonConvert.SerializeObject(heights));
            return DBCacheHelper.UpdateBlocks(rpcBlocks);
        }

        internal static ExplorerBlockModel HumanizeBlock(ExplorerBlockModel block)
        {
            block.AgeFormatted = block.Age.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
            block.TotalSatoshiFormatted = Money.Satoshis(block.TotalSatoshi).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);
            block.TransactionFeesFormatted = Money.Satoshis(block.TransactionFees).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);

            if (block.Transactions != null)
            {
                foreach (var itemTx in block.Transactions)
                {
                    itemTx.FeeFormatted = Money.Satoshis(itemTx.Fee).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);
                    itemTx.TimeFormatted = itemTx.Time.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                    itemTx.SatoshiFormatted = Money.Satoshis(itemTx.Satoshi).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);

                    foreach (var itemAddrFrom in itemTx.AddressFrom)
                    {
                        itemAddrFrom.SatoshiFormatted = Money.Satoshis(itemAddrFrom.Satoshi).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);
                    }

                    foreach (var itemAddrFrom in itemTx.AddressTo)
                    {
                        itemAddrFrom.SatoshiFormatted = Money.Satoshis(itemAddrFrom.Satoshi).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            return block;
        }
    }
}