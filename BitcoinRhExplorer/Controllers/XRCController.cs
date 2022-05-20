using BitcoinRhExplorer.App_Start;
using BitcoinRhExplorer.Components;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Server.Business.Block;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BitcoinRhExplorer.Controllers
{
    public class XRCController : BaseController
    {
        private BlockComponent Blocks;
        private TransactionComponent Transactions;

        [CustomRequireHttpsFilter]
        public ActionResult Block(string hash)
        {
            var viewModel = ViewModel<BlockViewModel>();

            try
            {
                hash = hash.Trim();
                viewModel.Hash = hash;

                var rpcBridge = new Server.Business.Explorer.RPCBridge();
                var rpc = rpcBridge.GetRpcClient();
                var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
                if (nodeInfo == null)
                {
                    return RedirectToAction("Index", "Maintenance");
                }

                if (hash.Length == 64)
                {
                    var block = Blocks.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);
                    if (block == null)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                BitcoinRhExplorerServer.Current.Errors.Add(
                    new Exception("Your hash is incorrect. Please try it again or wait for new block confirmation."));

                return RedirectToAction("Index", "Home");
            }

            return View("Block", viewModel);
        }

        [CustomRequireHttpsFilter]
        public ActionResult BlockByHeight(int height)
        {
            var viewModel = ViewModel<BlockViewModel>();

            try
            {
                var rpcBridge = new Server.Business.Explorer.RPCBridge();
                var rpc = rpcBridge.GetRpcClient();
                var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
                if (nodeInfo == null)
                {
                    return RedirectToAction("Index", "Maintenance");
                }

                var block = DBCacheHelper.GetExplorerBlockByHeight(height);
                if (block == null)
                {
                    throw new Exception();
                }
                else
                {
                    return RedirectToAction("block", "xrc", new { @hash = block.Hash });
                }
            }
            catch (Exception)
            {
                BitcoinRhExplorerServer.Current.Errors.Add(
                    new Exception("Your height #id is incorrect. Please try it again or wait for new block confirmation."));

                return RedirectToAction("Index", "Home");
            }
        }

        [CustomRequireHttpsFilter]
        public ActionResult Tx(string hash)
        {
            var viewModel = ViewModel<TxViewModel>();

            try
            {
                var rpcBridge = new Server.Business.Explorer.RPCBridge();
                var rpc = rpcBridge.GetRpcClient();
                var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
                if (nodeInfo == null)
                {
                    return RedirectToAction("Index", "Maintenance");
                }

                hash = hash.Trim();
                viewModel.Hash = hash;

                if (hash.Length == 64)
                {
                    var tx = Transactions.GetByHashIndex(TextHelper.GetNumberHash(hash), hash);
                    if (tx == null)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                BitcoinRhExplorerServer.Current.Errors.Add(
                    new Exception("Your hash is incorrect. Please try it again or wait for new block confirmation."));

                return RedirectToAction("Index", "Home");
            }

            return View("Tx", viewModel);
        }

        [CustomRequireHttpsFilter]
        public ActionResult Address(string hash)
        {
            var viewModel = ViewModel<AddressViewModel>();

            hash = hash.Trim();
            viewModel.Address = hash;

            if (!string.IsNullOrEmpty(hash) && ((hash.Length > 32) && (hash.Length < 40)))
            {
                var rpcBridge = new Server.Business.Explorer.RPCBridge();
                var rpc = rpcBridge.GetRpcClient();
                var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
                if (nodeInfo == null)
                {
                    return RedirectToAction("Index", "Maintenance");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Address", viewModel);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            Transactions = new TransactionComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
        }
    }

    public class BlockViewModel
    {
        public string Hash { get; set; }
        public RPCClient.ExplorerBlockModel Block { get; set; }
    }

    public class TxViewModel
    {
        public string Hash { get; set; }
    }

    public class AddressViewModel
    {
        public string Address { get; set; }
    }
}