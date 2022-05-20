using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BitcoinRhExplorer.Entities.Blocks;
using BitcoinRhExplorer.Entities.Stats;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business.Block;
using BitcoinRhExplorer.Server.Business.Stats;
using NBitcoin.RPC;
using Newtonsoft.Json;

namespace BitcoinRhExplorer.Components
{
    public static class DBCacheHelper //2 level cache
    {
        static readonly object _objectThread = new object();

        private const string KEY_CACHE_DIFFSTATS = "KEY_CACHE_DIFFSTATS";
        private const string KEY_CACHE_TOTALOUTSTATS = "KEY_CACHE_TOTALOUTSTATS";

        internal static void AddBlock(RPCClient.ExplorerBlockModel block)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            var DiffStats = new DiffStatComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            var dbAddresses = new List<AddressFromTo>();
            var dbTransactions = new List<Transaction>();
            var dbBlock = new Block();

            var blockJson = JsonConvert.SerializeObject(block);
            dbBlock.BlockJson = ZipHelper.ZipBase64(blockJson);
            dbBlock.Hash = block.Hash;
            dbBlock.HashIndex = TextHelper.GetNumberHash(dbBlock.Hash);
            dbBlock.Height = block.Height;

            if (block.Transactions != null)
            {
                foreach (var itemTx in block.Transactions)
                {
                    var dbTx = new Transaction();
                    dbTx.Hash = itemTx.Hash;
                    dbTx.HashIndex = TextHelper.GetNumberHash(dbTx.Hash);
                    dbTransactions.Add(dbTx);

                    if (itemTx.AddressFrom != null)
                    {
                        foreach (var itemAddress in itemTx.AddressFrom)
                        {
                            var address = new AddressFromTo();
                            address.Address = itemAddress.Address;
                            address.AddressIndex = TextHelper.GetNumberHash(address.Address);
                            dbAddresses.Add(address);
                        }
                    }

                    if (itemTx.AddressTo != null)
                    {
                        foreach (var itemAddress in itemTx.AddressTo)
                        {
                            var address = new AddressFromTo();
                            address.Address = itemAddress.Address;
                            address.AddressIndex = TextHelper.GetNumberHash(address.Address);
                            dbAddresses.Add(address);
                        }
                    }
                }

                dbBlock.TxCount = block.Transactions.Count();
                dbBlock.TotalOut = block.TotalSatoshi;
            }

            Blocks.Add(dbBlock, dbTransactions, dbAddresses);

            //add new difficult
            if ((block.Height > 0) && (block.Height % 50 == 0))
            {
                var newDiffStaf = DiffStats.Create();
                newDiffStaf.Height = block.Height;
                newDiffStaf.Diff = block.Difficult;
                DiffStats.Add(newDiffStaf);
            }
        }

        internal static Block GetExplorerBlockByHeight(int height)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            var dbBlock = Blocks.GetByHeight(height);
            if (dbBlock != null)
            {
                return dbBlock;
            }
            else
            {
                return null;
            }
        }

        internal static void AddBlocks(List<RPCClient.ExplorerBlockModel> blocks)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            var cloneToJson = JsonConvert.SerializeObject(blocks);
            var clonedBlock = JsonConvert.DeserializeObject<List<RPCClient.ExplorerBlockModel>>(cloneToJson);

            new Thread(() =>
            {
                lock (_objectThread)
                {
                    Thread.CurrentThread.IsBackground = true;

                    foreach (var block in clonedBlock)
                    {
                        var dbBlock = Blocks.GetByHeight(block.Height);

                        if (dbBlock == null)
                        {
                            AddBlock(block);
                        }
                        else
                        {
                            UpdateBlock(dbBlock, block);
                        }
                    }
                }
            }).Start();
        }

        internal static RPCClient.ExplorerBlockModel GetExplorerBlock(string hash)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            var dbBlock = Blocks.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);

            if (dbBlock != null)
            {
                var blockJson = ZipHelper.UnZipBase64(dbBlock.BlockJson);
                return JsonConvert.DeserializeObject<RPCClient.ExplorerBlockModel>(blockJson);
            }
            else
            {
                return null;
            }
        }

        internal static RPCClient.ExplorerBlockModel GetTransactionBlock(string hash)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            var Transactions = new TransactionComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            var dbTransaction = Transactions.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);
            if (dbTransaction != null)
            {
                var dbBlock = Blocks.GetById(dbTransaction.BlockId);

                if (dbBlock != null)
                {
                    var blockJson = ZipHelper.UnZipBase64(dbBlock.BlockJson);
                    return JsonConvert.DeserializeObject<RPCClient.ExplorerBlockModel>(blockJson);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal static List<RPCClient.ExplorerBlockModel> GetAddressBlock(string address, int offset)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            var AddressFromTo = new AddressComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            var result = new List<RPCClient.ExplorerBlockModel>();

            var blocksIds = AddressFromTo.GetByAddressIndex(TextHelper.GetNumberHash(address), address, offset);
            if ((blocksIds != null) && (blocksIds.Count > 0))
            {
                var dbBlocks = Blocks.GetByIds(blocksIds);

                foreach (var itemBlock in dbBlocks)
                {
                    var blockJson = ZipHelper.UnZipBase64(itemBlock.BlockJson);
                    result.Add(JsonConvert.DeserializeObject<RPCClient.ExplorerBlockModel>(blockJson));
                }
            }
            else
            {
                return null;
            }

            return result;
        }

        internal static int GetCountAddressBlock(string address)
        {
            var AddressFromTo = new AddressComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            return AddressFromTo.GetByAddressCount(TextHelper.GetNumberHash(address), address);
        }

        internal static int GetBlockOffset()
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            return Blocks.GetOffset();
        }

        internal static List<int> GetBlockIgnore(int offset)
        {

            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            return Blocks.GetBlockForIgnore(offset);
        }

        internal static bool UpdateBlocks(List<RPCClient.ExplorerBlockModel> blocksToUpdate)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            try
            {
                foreach (var block in blocksToUpdate)
                {
                    var dbBlock = Blocks.GetByHeight(block.Height);

                    if (dbBlock != null)
                    {
                        UpdateBlock(dbBlock, block);
                    }
                }

                return true;
            }
            catch (Exception)
            {
               //calm down
            }
            
            return false;
        }

        internal static void UpdateBlock(Block oldDbBlock, RPCClient.ExplorerBlockModel rpcBlock)
        {
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            var newJsonBlock = JsonConvert.SerializeObject(rpcBlock);
            var newJsonBlockZip = ZipHelper.ZipBase64(newJsonBlock);

            if (oldDbBlock.BlockJson != newJsonBlockZip)
            {
                var dbAddresses = new List<AddressFromTo>();
                var dbTransactions = new List<Transaction>();
                var dbBlock = oldDbBlock;

                dbBlock.BlockJson = newJsonBlockZip;
                dbBlock.Hash = rpcBlock.Hash;
                dbBlock.HashIndex = TextHelper.GetNumberHash(dbBlock.Hash);

                if (rpcBlock.Transactions != null)
                {
                    foreach (var itemTx in rpcBlock.Transactions)
                    {
                        var dbTx = new Transaction();
                        dbTx.Hash = itemTx.Hash;
                        dbTx.HashIndex = TextHelper.GetNumberHash(dbTx.Hash);
                        dbTransactions.Add(dbTx);

                        if (itemTx.AddressFrom != null)
                        {
                            foreach (var itemAddress in itemTx.AddressFrom)
                            {
                                var address = new AddressFromTo();
                                address.Address = itemAddress.Address;
                                address.AddressIndex = TextHelper.GetNumberHash(address.Address);
                                dbAddresses.Add(address);
                            }
                        }

                        if (itemTx.AddressTo != null)
                        {
                            foreach (var itemAddress in itemTx.AddressTo)
                            {
                                var address = new AddressFromTo();
                                address.Address = itemAddress.Address;
                                address.AddressIndex = TextHelper.GetNumberHash(address.Address);
                                dbAddresses.Add(address);
                            }
                        }
                    }

                    dbBlock.TxCount = rpcBlock.Transactions.Count();
                    dbBlock.TotalOut = rpcBlock.TotalSatoshi;
                }

                Blocks.Update(dbBlock, dbTransactions, dbAddresses);
            }
            else
            {
                Blocks.Update(oldDbBlock);
            }
        }

        public static List<DiffStat> GetDiffStats()
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_DIFFSTATS);
            var stats = new List<DiffStat>();
            var DiffStats = new DiffStatComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);


            if (cached == null)
            {
                try
                {
                    stats = DiffStats.GetAll();

                    BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_DIFFSTATS, JsonConvert.SerializeObject(stats));
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                stats = JsonConvert.DeserializeObject<List<DiffStat>>(cached);
            }

            return stats;
        }

        public static List<Block> GetLatestBlockTxOut()
        {
            var cached = BitcoinRhExplorerServer.Current.Cache.CommonCache.Get(KEY_CACHE_TOTALOUTSTATS);
            var txOuts = new List<Block>();
            var Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);

            if (cached == null)
            {
                try
                {
                    txOuts = Blocks.GetLastBlocks(50);

                    BitcoinRhExplorerServer.Current.Cache.CommonCache.Add(KEY_CACHE_TOTALOUTSTATS, JsonConvert.SerializeObject(txOuts));
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                txOuts = JsonConvert.DeserializeObject<List<Block>>(cached);
            }

            return txOuts;
        }
    }
}