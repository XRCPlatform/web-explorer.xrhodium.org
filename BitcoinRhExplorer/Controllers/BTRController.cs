using BitcoinRhExplorer.App_Start;
using BitcoinRhExplorer.Components;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business.Block;
using NBitcoin.RPC;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace BitcoinRhExplorer.Controllers
{
    public class BTRController : BaseController
    {
        private BlockComponent Blocks;
        private TransactionComponent Transactions;

        [CustomRequireHttpsFilter]
        public ActionResult Block(string hash)
        {
            return RedirectToAction("Block", "Xrc", new { hash });
        }

        [CustomRequireHttpsFilter]
        public ActionResult BlockByHeight(int height)
        {
            return RedirectToAction("BlockByHeight", "Xrc", new { height });
        }

        [CustomRequireHttpsFilter]
        public ActionResult Tx(string hash)
        {
            return RedirectToAction("Tx", "Xrc", new { hash });
        }

        [CustomRequireHttpsFilter]
        public ActionResult Address(string hash)
        {
            return RedirectToAction("Address", "Xrc", new { hash });
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            Transactions = new TransactionComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
        }
    }
}