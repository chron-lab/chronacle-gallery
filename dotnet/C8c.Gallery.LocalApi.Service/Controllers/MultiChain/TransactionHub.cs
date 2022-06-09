using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MultiChainDotNet.Core.MultiChainTransaction;
using System.Threading.Tasks;
using UtilsDotNet.Extensions;

namespace C8c.Gallery.LocalApi.Service.Controllers.MultiChain
{
	public class TransactionHub : Hub
	{
		private ILogger<TransactionHub> _logger;

		public TransactionHub(ILogger<TransactionHub> logger)
		{
			_logger = logger;
		}
		public async Task Publish(DecodeRawTransactionResult raw)
		{
			_logger.LogInformation($"MultiChain tx received: {raw.Txid}");
			await Clients.All.SendAsync("Publish", raw);
		}

		public async Task Block(BlockNotifyResult block)
		{
			_logger.LogInformation($"MultiChain block received: {block.Block}");
			await Clients.All.SendAsync("Block", block);
		}

	}
}
