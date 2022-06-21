using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MultiChainDotNet.Core;
using MultiChainDotNet.Core.MultiChainTransaction;
using Newtonsoft.Json;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UtilsDotNet.Extensions;

namespace C8c.GalleryLocalApi.WindowsService.Controllers.MultiChain
{

	/// <summary>
	/// This endpoint is used for receiving new transaction notifications from MultiChain node and broadcast to websocket
	/// </summary>
	[ApiController]
	[Route("transaction")]
	public class TransactionController : ControllerBase
	{
		private IHubContext<TransactionHub> _transactionHub;
		private ILogger<TransactionController> _logger;
		private MultiChainConfiguration _mcConfig;

		public class WalletNotifyResult
		{
			[JsonProperty("txn")]
			public DecodeRawTransactionResult Transaction { get; set; }

			[JsonProperty("height")]
			public int Height { get; set; }
		}

		public TransactionController(ILogger<TransactionController> logger, IHubContext<TransactionHub> transactionHub, MultiChainConfiguration mcConfig)
		{
			_transactionHub = transactionHub;
			_logger = logger;
			_mcConfig = mcConfig;
		}

		/// <summary>
		/// This method receives transaction notifications from the multichain node and send the "Publish" signal to signalR hub
		/// 
		/// The allowed IP address is hardcoded to local node.
		/// 
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		[HttpPost()]
		[AllowAnonymous]
		public async Task<ActionResult> Post(WalletNotifyResult transaction)
		{
			// Explicitly forbid notifications not from local node
			if (HttpContext.Connection.RemoteIpAddress.ToString() != "::ffff:172.35.0.1"        // Testnet from host
				&& HttpContext.Connection.RemoteIpAddress.ToString() != "127.0.0.1"             // Sandbox from host
				&& HttpContext.Connection.RemoteIpAddress.ToString() != "::ffff:172.35.0.99"    // Testnet from docker
				&& HttpContext.Connection.RemoteIpAddress.ToString() != "::ffff:172.111.0.99"   // Sandbox from docker
				)
			{
				_logger.LogError($"RemoteIpAddress {HttpContext.Connection.RemoteIpAddress.ToString()} is FORBIDDEN.");
				return StatusCode(403, $"RemoteIpAddress {HttpContext.Connection.RemoteIpAddress.ToString()} is FORBIDDEN.");
			}

			var json = JsonConvert.SerializeObject(transaction);
			_logger.LogInformation($"McWebSocket: Broadcast new transaction {transaction.ToJson()}");
			await _transactionHub.Clients.All.SendAsync("Publish", json);
			return Ok();
		}

	}
}
