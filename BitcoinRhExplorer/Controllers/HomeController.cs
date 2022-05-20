using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BitcoinRhExplorer.App_Start;
using NBitcoin.RPC;
using BitcoinRhExplorer.Components;
using NBitcoin;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business.Block;
using System.Web.Routing;
using BitcoinRhExplorer.Entities.Stats;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace BitcoinRhExplorer.Controllers
{
    [CustomRequireHttpsFilter]
    public class HomeController : BaseController
    {
        private BlockComponent Blocks;
        private TransactionComponent Transactions;

        public ActionResult Index(HomeViewModel post)
        {
            var viewModel = ViewModel<HomeViewModel>();

            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
            if (nodeInfo != null)
            {
                viewModel.MemPool = RPCCacheHelper.GetCachedRawMemPool(rpc);
                viewModel.MemPoolInfo = RPCCacheHelper.GetCachedMemPoolInfo(rpc);
                viewModel.MemPoolTxs = new Dictionary<string, Transaction>();

                if ((viewModel.MemPool != null) && (viewModel.MemPool.Any()))
                {
                    foreach (var item in viewModel.MemPool)
                    {
                        var memTxInfo = RPCCacheHelper.GetCachedRawTxInfo(rpc, item.Key);
                        if (memTxInfo != null)
                        {
                            viewModel.MemPoolTxs.Add(item.Key, memTxInfo);
                        }
                    }
                }

                viewModel.NodeInfo = nodeInfo;

                var circulationSupply = RPCCacheHelper.GetCachedCirculationSupply(rpc);
                viewModel.ActualSupply = new Money(circulationSupply, MoneyUnit.XRC).ToUnit(MoneyUnit.XRC);
                viewModel.MaxSupply = 2100000;
                viewModel.ActualSupplyFormated = Math.Round(Money.FromUnit(viewModel.ActualSupply, MoneyUnit.XRC).ToUnit(MoneyUnit.XRC), 2).ToString();
                viewModel.MaxSupplyFormated = Money.FromUnit(viewModel.MaxSupply, MoneyUnit.XRC).ToUnit(MoneyUnit.XRC).ToString();
                viewModel.PercentSupply = (int)Math.Round((decimal)(100 * viewModel.ActualSupply) / viewModel.MaxSupply);
                viewModel.PercentBlock = (int)Math.Round((decimal)(100 * viewModel.MemPoolInfo.Usage) / PowBlock.MaxBlockSize);

                var diffStats = DBCacheHelper.GetDiffStats();
                viewModel.DiffStats = diffStats;

                var txOuts = DBCacheHelper.GetLatestBlockTxOut();
                viewModel.TxOuts = txOuts;
            }
            else
            {
                return RedirectToAction("Index", "Maintenance");
            }

            viewModel.TimeStamp = DateTime.UtcNow;

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(string token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new Exception();
                    }

                    token = token.Trim();

                    if (token.Length == 64)
                    {
                        var hash = token;

                        var tx = Transactions.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);
                        if (tx != null)
                        {
                            return RedirectToAction("tx", "xrc", new { hash = token });
                        }
                        else
                        {
                            var block = Blocks.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);
                            if (block != null)
                            {
                                return RedirectToAction("block", "xrc", new { hash = token });
                            }
                        }

                        throw new Exception();
                    }
                    else if (token.Substring(0, 1) == "#")
                    {
                        int height;
                        if (int.TryParse(token.Substring(1, token.Length - 1), out height)) {
                            return RedirectToAction("blockbyheight", "xrc", new { height });
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else if ((token.Length > 32) && (token.Length < 40))
                    {
                        return RedirectToAction("address", "xrc", new { hash = token } );
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    BitcoinRhExplorerServer.Current.Errors.Add(
                        new Exception("Your search token is incorrect. Please try it again."));
                }
            }

            return RedirectToAction("Index", "Home");
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            Transactions = new TransactionComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
        }

    }

    public class HomeViewModel
    {
        public List<RPCClient.ExplorerBlockModel> LatestBlocks { get; set; }
        public NodeInfo NodeInfo { get; set; }
        public DateTime TimeStamp { get; set; }
        public string SearchToken { get; set; }

        public decimal MaxSupply { get; set; }
        public string MaxSupplyFormated { get; set; }
        public decimal ActualSupply { get; set; }
        public string ActualSupplyFormated { get; set; }
        public int PercentSupply { get; set; }

        public List<DiffStat> DiffStats { get; set; }
        public List<Entities.Blocks.Block> TxOuts { get; set; }

        public Dictionary<string, Transaction> MemPoolTxs { get; set; }
        public MempoolInfo MemPoolInfo { get; set; }
        public Dictionary<string, MemPoolEntry> MemPool { get; set; }

        public int PercentBlock { get; set; }
    }
}