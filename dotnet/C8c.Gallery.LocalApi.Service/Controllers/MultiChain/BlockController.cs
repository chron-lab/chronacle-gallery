using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Service.Controllers.MultiChain
{
	public class BlockNotifyResult
	{
		[JsonProperty("block")]
		public string Block { get; set; }
	}

	/// <summary>
	/// This endpoint is used for receiving new transaction notifications from MultiChain node and broadcast to websocket
	/// </summary>
	[ApiController]
	[Route("block")]
	public class BlockController : ControllerBase
	{
		private IHubContext<TransactionHub> _transactionHub;
		private ILogger<BlockController> _logger;

		public BlockController(ILogger<BlockController> logger, IHubContext<TransactionHub> transactionHub)
		{
			_transactionHub = transactionHub;
			_logger = logger;
		}

		[HttpPost()]
		[BasicAuthorization]
		public async Task Post(BlockNotifyResult block)
		{
			_logger.LogDebug($"McWebSocket: Received blockhash {block.Block}");
			await _transactionHub.Clients.All.SendAsync("Block", JsonConvert.SerializeObject(block));
		}

	}
}
