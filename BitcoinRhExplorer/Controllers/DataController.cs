using BitcoinRhExplorer.Components;
using BitcoinRhExplorer.Server;
using BitcoinRhExplorer.Library;
using BitcoinRhExplorer.Server.Business;
using BitcoinRhExplorer.Server.Business.Block;
using log4net;
using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using BitcoinRhExplorer.Server.Business.Stats;
using BitcoinRhExplorer.Entities.Stats;
using System.Web.Http.Results;
using System.Globalization;

namespace BitcoinRhExplorer.Controllers
{
    public class DataController : ApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DataController));

        private BlockComponent Blocks;
        private RichStatComponent RichStats;

        public HttpResponseMessage GetCirculationSupply(MoneyUnit moneyUnit)
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();
            decimal humanizedCirculationSupply = 0;

            var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
            if (nodeInfo != null)
            {
                var circulationSupply = RPCCacheHelper.GetCachedCirculationSupply(rpc);
                var money = new Money(circulationSupply, MoneyUnit.XRC);

                switch (moneyUnit)
                {
                    case MoneyUnit.XRC:
                        humanizedCirculationSupply = money.ToUnit(MoneyUnit.XRC);
                        break;
                    case MoneyUnit.Satoshi:
                        humanizedCirculationSupply = money.Satoshi;
                        break;
                    default:
                        humanizedCirculationSupply = money.ToUnit(MoneyUnit.XRC);
                        break;
                }
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(humanizedCirculationSupply, Formatting.None, serializerSettings), System.Text.Encoding.UTF8, "application/json")
            };
        }

        public HttpResponseMessage GetTotalSupply(MoneyUnit moneyUnit)
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();
            decimal humanizedTotalSupply = 0;

            var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
            if (nodeInfo != null)
            {
                var totalSupply = RPCCacheHelper.GetCachedTotalSupply(rpc);
                var money = new Money(totalSupply, MoneyUnit.XRC);

                switch (moneyUnit)
                {
                    case MoneyUnit.XRC:
                        humanizedTotalSupply = money.ToUnit(MoneyUnit.XRC);
                        break;
                    case MoneyUnit.Satoshi:
                        humanizedTotalSupply = money.Satoshi;
                        break;
                    default:
                        humanizedTotalSupply = money.ToUnit(MoneyUnit.XRC);
                        break;
                }
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(humanizedTotalSupply, Formatting.None, serializerSettings), System.Text.Encoding.UTF8, "application/json")
            };
        }

        public HttpResponseMessage GetMaxSupply(MoneyUnit moneyUnit)
        {
            decimal humanizedCirculationSupply = 0;

            var money = new Money(2100000, MoneyUnit.XRC);

            switch (moneyUnit)
            {
                case MoneyUnit.XRC:
                    humanizedCirculationSupply = money.ToUnit(MoneyUnit.XRC);
                    break;
                case MoneyUnit.Satoshi:
                    humanizedCirculationSupply = money.Satoshi;
                    break;
                default:
                    humanizedCirculationSupply = money.ToUnit(MoneyUnit.XRC);
                    break;
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(humanizedCirculationSupply, Formatting.None, serializerSettings), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpPost]
        public HttpResponseMessage GetBlocks()
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            var humanizedBlocks = new List<ExplorerHumanizedBlockModel>();

            var nodeInfo = RPCCacheHelper.GetCachedNodeInfo(rpc);
            if (nodeInfo != null)
            {
                var blocks = RPCCacheHelper.GetCachedLatestBlocks(rpc, 20);

                foreach (var itemBlock in blocks)
                {
                    Blocks.PostProcess(itemBlock);

                    var humanizeBlock = new ExplorerHumanizedBlockModel();

                    humanizeBlock.TxCount = (itemBlock.Transactions != null ? itemBlock.Transactions.Count.ToString() : "N/A");
                    humanizeBlock.Size = itemBlock.Size;
                    humanizeBlock.Height = itemBlock.Height;
                    humanizeBlock.DateTime = itemBlock.Age.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                    humanizeBlock.Money = Money.Satoshis((decimal)itemBlock.TotalSatoshi).ToUnit(MoneyUnit.XRC).ToString(CultureInfo.InvariantCulture);
                    humanizeBlock.Url = "/xrc/block/" + itemBlock.Hash;

                    humanizedBlocks.Add(humanizeBlock);
                }
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(humanizedBlocks, Formatting.None, serializerSettings))
            };
        }

        [HttpPost]
        public HttpResponseMessage GetBlock([FromBody]string hash)
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            RPCClient.ExplorerBlockModel block = null;

            if (!string.IsNullOrEmpty(hash))
            {
                Logger.Debug("Block: " + hash);
                block = RPCCacheHelper.GetCachedBlock(rpc, hash);

                var maxHeight = Blocks.GetMaxHeight();

                Blocks.PostProcess(block, maxHeight);
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(block, Formatting.None, serializerSettings))
            };
        }

        [HttpPost]
        public HttpResponseMessage GetTransaction([FromBody]string hash)
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            RPCClient.ExplorerTransactionModel transaction = null;

            if (!string.IsNullOrEmpty(hash))
            {
                Logger.Debug("Transaction: " + hash);
                transaction = RPCCacheHelper.GetCachedTransaction(rpc, hash);

                Blocks.PostProcess(transaction);
            }
            
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(transaction, Formatting.None, serializerSettings))
            };
        }

        [HttpPost]
        public HttpResponseMessage GetAddress([FromBody]AddressModel data)
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            var result = new AddressResultModel();

            if (!string.IsNullOrEmpty(data.Address))
            {
                Logger.Debug("Address: " + data.Address);
                var txData = RPCCacheHelper.GetCachedAddress(rpc, data.Address, data.Offset);

                if (txData.Item1 != null)
                {
                    foreach (var itemTransaction in txData.Item1)
                    {
                        Blocks.PostProcess(itemTransaction);
                    }

                    result.Txs = txData.Item1;
                    result.TxCount = txData.Item2;
                }
            }
            
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(result, Formatting.None, serializerSettings))
            };
        }

        [HttpGet]
        public HttpResponseMessage UpdateOldBlocks()
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            var result = false;
            var heightsToUpdate = Blocks.GetHeightsForUpdate();

            if (heightsToUpdate.Count > 0)
            {
                Logger.Debug("Heights: " + heightsToUpdate.ToString());
                result = RPCCacheHelper.UpdateCacheExplorerAddressByHeight(rpc, heightsToUpdate);
            }

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(result, Formatting.None, serializerSettings))
            };
        }

        [HttpGet]
        public HttpResponseMessage IBD()
        {
            var rpcBridge = new Server.Business.Explorer.RPCBridge();
            var rpc = rpcBridge.GetRpcClient();

            var result = false;
            var heights = new List<int>();
            var start = Blocks.GetOffset();

            for (int i = start; i < (start + 2000); i++)
            {
                heights.Add(i);
            }

            var rpcBlocks = rpc.GetExplorerAddressByHeight(JsonConvert.SerializeObject(heights));
            DBCacheHelper.AddBlocks(rpcBlocks);

            result = true;

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(result, Formatting.None, serializerSettings))
            };
        }

        [HttpGet]
        public HttpResponseMessage CalculateRich()
        {
            var min = RichStats.GetMinHeight() + 1;
            var max = Blocks.GetMaxHeight();
            var height = 0;
            var richAddr = RichStats.GetAll();

            for (int i = min; i <= max; i++)
            {
                var dbBlock = Blocks.GetByHeight(i);

                if ((dbBlock != null) && (dbBlock.TxCount > 0))
                {
                    var blockBase64Data = ZipHelper.UnZipBase64(dbBlock.BlockJson);
                    var block = JsonConvert.DeserializeObject<RPCClient.ExplorerBlockModel>(blockBase64Data);
                    height = dbBlock.Height;

                    foreach (var itemTx in block.Transactions)
                    {
                        if (itemTx.AddressFrom != null)
                        {
                            foreach (var itemAddress in itemTx.AddressFrom)
                            {
                                var address = richAddr.FirstOrDefault(a => a.Address == itemAddress.Address);
                                if (address != null)
                                {
                                    address.Amount = address.Amount - itemAddress.Satoshi;
                                    address.Height = height;
                                }
                                else
                                {
                                    address = RichStats.Create();
                                    address.Address = itemAddress.Address;
                                    address.Height = i;
                                    address.Amount = -itemAddress.Satoshi;
                                    richAddr.Add(address);
                                }
                            }
                        }

                        if (itemTx.AddressTo != null)
                        {
                            foreach (var itemAddress in itemTx.AddressTo)
                            {
                                var address = richAddr.FirstOrDefault(a => a.Address == itemAddress.Address);
                                if (address != null)
                                {
                                    address.Amount = address.Amount + itemAddress.Satoshi;
                                    address.Height = height;
                                }
                                else
                                {
                                    address = RichStats.Create();
                                    address.Address = itemAddress.Address;
                                    address.Height = i;
                                    address.Amount = itemAddress.Satoshi;
                                    richAddr.Add(address);
                                }
                            }
                        }
                    }
                }
            }

            //do some processing only if there are new blocks
            if (height > 0)
            {
                richAddr = richAddr.OrderByDescending(a => a.Amount).ToList();
                var richForAdd = richAddr.Where(a => a.Id == 0).OrderByDescending(a => a.Amount).ToList();
                var richForUpdate = richAddr.Where(a => a.Id != 0 && a.Height >= min).OrderByDescending(a => a.Amount).ToList();

                RichStats.UpdateWithTransaction(richForUpdate);
                RichStats.AddWithTransaction(richForAdd);
                RichStats.UpdateHeight(height);
            }

            var serializerSettings = new JsonSerializerSettings { };
            return new HttpResponseMessage()
            {
                Content = new JsonHttpContent(JsonConvert.SerializeObject(true, Formatting.None, serializerSettings))
            };
        }

        [HttpGet]
        public HttpResponseMessage GetRich()
        {
            var richList = RPCCacheHelper.GetCachedRich(RichStats);

            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(richList, Formatting.None, serializerSettings), System.Text.Encoding.UTF8, "application/json")
            };
        }

        public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            Blocks = new BlockComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            RichStats = new RichStatComponent(BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextScopeFactory, BitcoinRhExplorerServer.Current.BitcoinRhExplorerDbContextLocator);
            return await base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }

    public class AddressModel
    {
        public string Address { get; set; }
        public int Offset { get; set; }
    }
    public class AddressResultModel
    {
        public List<RPCClient.ExplorerTransactionModel> Txs { get; set; }
        public int TxCount { get; set; }
    }

    public class JsonHttpContent : HttpContent
    {
        private readonly JToken _value;

        public JsonHttpContent(JToken value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream,
        TransportContext context)
        {
            var jw = new JsonTextWriter(new StreamWriter(stream))
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            _value.WriteTo(jw);
            jw.Flush();
            return Task.FromResult<object>(null);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
