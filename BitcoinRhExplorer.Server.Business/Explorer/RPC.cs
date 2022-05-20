using NBitcoin.RPC;
using System;
using System.Configuration;
using System.Net;

namespace BitcoinRhExplorer.Server.Business.Explorer
{
    public class RPCBridge
    {
        public RPCClient GetRpcClient()
        {
            var rpcUser = ConfigurationManager.AppSettings["RPC_BTRNodeUser"];
            var rpcPsw = ConfigurationManager.AppSettings["RPC_BTRNodePassword"];
            var rpcIp = ConfigurationManager.AppSettings["RPC_BTRNodeIP"];
            var rpcPort = ConfigurationManager.AppSettings["RPC_BTRNodePort"];
            var rpcProtocol = ConfigurationManager.AppSettings["RPC_BTRNodeProtocol"];
            var rpcCrypt = ConfigurationManager.AppSettings["RPC_BTRNodeCRYPT"];

            var rpcClient = new RPCClient(new NetworkCredential(rpcUser, rpcPsw), new Uri(string.Format("{0}://{1}:{2}", rpcProtocol, rpcIp, rpcPort)), 
                NBitcoin.Network.BRhodiumTest);
            return rpcClient;
        }


    }
}
