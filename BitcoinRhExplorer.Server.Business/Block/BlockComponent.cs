using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Blocks;
using BitcoinRhExplorer.Library;
using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Transaction = BitcoinRhExplorer.Entities.Blocks.Transaction;

namespace BitcoinRhExplorer.Server.Business.Block
{
    public class BlockComponent : BaseDbComponent<Entities.Blocks.Block>
    {
        public BlockComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public Entities.Blocks.Block GetByHeight(int height)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<Entities.Blocks.Block>()
                    .FirstOrDefault(e => (e.Height == height) && ((!e.IsDeleted) || (e.IsDeleted)));
            }
        }

        public Entities.Blocks.Block GetByHash(string hash)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<Entities.Blocks.Block>()
                    .FirstOrDefault(e => (e.Hash == hash) && ((!e.IsDeleted) || (e.IsDeleted)));
            }
        }

        public void PostProcess(RPCClient.ExplorerBlockModel item, int? maxHeight = null)
        {
            if ((item != null) && (item.TransactionCount > 0) && (item.Height != 16))
            {
                foreach (var itemTx in item.Transactions)
                {
                    //clean change address
                    //if ((itemTx.Satoshi > 5000000000000) && (itemTx.AddressTo.Count > 1))
                    //{
                    //    var changeAddress = itemTx.AddressTo.OrderByDescending(x => x.Satoshi).FirstOrDefault();
                    //    if (changeAddress != null)
                    //    {
                    //        var diffSatoshi = item.TotalSatoshi - changeAddress.Satoshi;

                    //        if (changeAddress.Satoshi > diffSatoshi)
                    //        {
                    //            item.TotalSatoshi = diffSatoshi;
                    //            item.TotalSatoshiFormatted = Money.Satoshis((decimal)item.TotalSatoshi).ToUnit(MoneyUnit.XRC).ToString();
                    //            itemTx.Satoshi = itemTx.Satoshi - changeAddress.Satoshi;
                    //            itemTx.SatoshiFormatted = Money.Satoshis((decimal)itemTx.Satoshi).ToUnit(MoneyUnit.XRC).ToString();
                    //            itemTx.AddressTo.Remove(changeAddress);
                    //        }
                    //    }
                    //}

                    //clean duplicates in input
                    if ((itemTx.AddressFrom != null) && (itemTx.AddressFrom.Count > 0))
                    {
                        itemTx.AddressFrom = itemTx.AddressFrom.GroupBy(a => a.Address).Select(a => a.First()).ToList();
                    }
                }
            }

            if (maxHeight.HasValue)
            {
                item.Confirmations = maxHeight.Value - item.Height;
            }
        }

        public void PostProcess(RPCClient.ExplorerTransactionModel itemTx)
        {
            //clean change address
            //if ((itemTx != null) && (itemTx.Satoshi > 5000000000000) && (itemTx.AddressTo.Count > 1) && (itemTx.BlockHeight != 16))
            //{
            //    var changeAddress = itemTx.AddressTo.OrderByDescending(x => x.Satoshi).FirstOrDefault();
            //    if (changeAddress != null)
            //    {
            //        var diffSatoshi = itemTx.Satoshi - changeAddress.Satoshi;

            //        if (changeAddress.Satoshi > diffSatoshi)
            //        {
            //            itemTx.Satoshi = diffSatoshi;
            //            itemTx.SatoshiFormatted = Money.Satoshis((decimal)itemTx.Satoshi).ToUnit(MoneyUnit.XRC).ToString();
            //            itemTx.AddressTo.Remove(changeAddress);
            //        }
            //    }
            //}

            //clean duplicates in input
            if ((itemTx.AddressFrom != null) && (itemTx.AddressFrom.Count > 0))
            {
                itemTx.AddressFrom = itemTx.AddressFrom.GroupBy(a => a.Address).Select(a => a.First()).ToList();
            }
        }

        public Entities.Blocks.Block GetByHashIndex(int hashIndex, string hash)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var list = _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Where(e => (e.HashIndex == hashIndex) && ((!e.IsDeleted) || (e.IsDeleted)))
                    .ToList();

                return list.Where(e => e.Hash == hash).FirstOrDefault();
            }
        }

        public void Add(Entities.Blocks.Block block, List<Transaction> transactions, List<AddressFromTo> addresses)
        {
            var _repositoryTransaction = new BaseRepository<Transaction>(this._ambientDbContextLocator);
            var _repositoryAddressFromTo = new BaseRepository<AddressFromTo>(this._ambientDbContextLocator);

            using (var scope = _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    _repository.Add(block);

                    for (int i = 0; i < transactions.Count; i++)
                    {
                        transactions[i].BlockId = block.Id;
                        _repositoryTransaction.Add(transactions[i]);
                    }

                    for (int i = 0; i < addresses.Count; i++)
                    {
                        addresses[i].BlockId = block.Id;
                        _repositoryAddressFromTo.Add(addresses[i]);
                    }

                    scope.CommitInternal();
                }
                catch (Exception)
                {
                    scope.RollbackInternal();
                }
            }
        }

        public int GetOffset()
        {

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var heights = _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Where(e => (!e.IsDeleted) || (e.IsDeleted))
                    .Select(e => e.Height)
                    .OrderBy(e => e)
                    .ToList();

                if (heights.Count() == 0) return 0;

                var maxHeight = heights.Max();

                for (int i = 0; i <= maxHeight; i++)
                {
                    if (!heights.Contains(i))
                    {
                        return i;
                    }
                }

                return maxHeight + 1;
            }
        }

        public List<int> GetBlockForIgnore(int offset)
        {
            var result = new List<int>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var heights = _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Where(e => (!e.IsDeleted) || (e.IsDeleted))
                    .Select(e => e.Height)
                    .OrderBy(e => e)
                    .ToList();

                var maxHeight = heights.Max();

                for (int i = offset; i <= maxHeight; i++)
                {
                    if (heights.Contains(i))
                    {
                        result.Add(i);
                    }
                }
            }

            return result;
        }

        public List<int> GetHeightsForUpdate()
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Where(e => (!e.IsDeleted) || (e.IsDeleted))
                    .OrderBy(e => e.UpdatedUtc)
                    .ThenBy(e => e.Height)
                    .Take(50)
                    .Select(e => e.Height)
                    .OrderBy(e => e)
                    .ToList();
                
            }
        }

        public int GetMaxHeight()
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Max(a => a.Height);
            }
        }

        public void Update(Entities.Blocks.Block block, List<Transaction> transactions, List<AddressFromTo> addresses)
        {
            var _repositoryTransaction = new BaseTransactionRepository(this._ambientDbContextLocator);
            var _repositoryAddressFromTo = new BaseAddressFromToRepository(this._ambientDbContextLocator);

            using (var scope = _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    _repository.Update(block);

                    _repositoryTransaction.DeleteByBlockId(block.Id);
                    _repositoryAddressFromTo.DeleteByBlockId(block.Id);

                    for (int i = 0; i < transactions.Count; i++)
                    {
                        transactions[i].BlockId = block.Id;
                        _repositoryTransaction.Add(transactions[i]);
                    }

                    for (int i = 0; i < addresses.Count; i++)
                    {
                        addresses[i].BlockId = block.Id;
                        _repositoryAddressFromTo.Add(addresses[i]);
                    }

                    scope.CommitInternal();
                }
                catch (Exception)
                {
                    scope.RollbackInternal();
                }
            }
        }

        public void UpdateIndex()
        {
            IEnumerable<string> entitySets = null;

            using (_dbContextScopeFactory.Create())
            {
                var all = _repository.GetAll(true);

                foreach (var item in all)
                {

                    item.HashIndex = TextHelper.GetNumberHash(item.Hash);

                    _repository.DbContext.Entry(item).State = EntityState.Modified;
                }

                _repository.DbContext.SaveChanges();

                entitySets = ((IObjectContextAdapter)_repository.DbContext)
                    .ObjectContext
                    .MetadataWorkspace
                    .GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace)
                    .EntitySets
                    .Select(e => e.Name);
            }

            base.ClearLevel2Cache(entitySets);
        }

        public List<Entities.Blocks.Block> GetLastBlocks(int limit)
        {
            var result = new List<Entities.Blocks.Block>();

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                result = _repository.DbContext.Set<Entities.Blocks.Block>()
                    .Where(e => (!e.IsDeleted) || (e.IsDeleted))
                    .OrderByDescending(e => e.Height)
                    .Take(limit)
                    .ToList();
            }

            result.Reverse();
            return result;
        }
    }
}
