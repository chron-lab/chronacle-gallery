using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiChainDotNet.Api.Abstractions;
using MultiChainDotNet.Core;
using MultiChainDotNet.Core.Base;
using MultiChainDotNet.Core.MultiChainAddress;
using MultiChainDotNet.Core.MultiChainBlockchain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilsDotNet.Extensions;

namespace C8c.GalleryLocalApi.WindowsService.Controllers.MultiChain
{

	/// <summary>
	/// This controller is for proxying incoming JSON-RPC requests to the multichain node
	/// </summary>
	[ApiController]
	[Route("jsonrpc")]
	public class JsonRpcController : ControllerBase
	{
		private JsonRpcCommand _rpc;
		private IServiceProvider _container;
		ILogger _logger;

		public JsonRpcController(ILogger<JsonRpcController> logger, JsonRpcCommand rpc, IServiceProvider container)
		{
			_logger = logger;
			_rpc = rpc;
			_container = container;
		}

		[HttpGet("getAddress")]
		//[AllowAnonymous]
		public async Task<ActionResult<AddressResult>> GetAddress()
		{
			using (var scope = _container.CreateScope())
			{
				var addrCmd = scope.ServiceProvider.GetRequiredService<MultiChainAddressCommand>();
				var result = await addrCmd.ListAddressesAsync();
				if (result.IsError)
				{
					_logger.LogError(result.ExceptionMessage, result.Exception);
					if (result.ExceptionMessage.Contains("Polly timeout"))
						return StatusCode(500, new { error = $"Connecting local node timeout. Check if local node is running." });

					return StatusCode(500, new { error = $"MultiChainApi Error: {result.ExceptionMessage}" });
				}
				if (result.Result is null || result.Result.Count == 0)
					return NotFound();

				return Ok(result.Result[0]);
			}
		}

		[HttpGet("getinfo")]
		[AllowAnonymous]
		public async Task<ActionResult<GetInfoResult>> GetInfo()
		{
			using (var scope = _container.CreateScope())
			{
				var bcCmd = scope.ServiceProvider.GetRequiredService<MultiChainBlockchainCommand>();
				var result = await bcCmd.GetInfoAsync();
				if (result.IsError)
				{
					_logger.LogError(result.ExceptionMessage, result.Exception);
					return StatusCode(500, result.ExceptionMessage);
				}
				if (result.Result is null)
					return NotFound();

				return Ok(result.Result);
			}
		}

		[HttpGet("getpeerinfo")]
		[AllowAnonymous]
		public async Task<ActionResult<IList<GetPeerInfoResult>>> GetPeerInfo()
		{
			using (var scope = _container.CreateScope())
			{
				var bcCmd = scope.ServiceProvider.GetRequiredService<MultiChainBlockchainCommand>();
				var result = await bcCmd.GetPeerInfoAsync();
				if (result.IsError)
				{
					_logger.LogError(result.ExceptionMessage, result.Exception);
					return StatusCode(500, result.ExceptionMessage);
				}
				if (result.Result is null)
					return NotFound();

				return Ok(result.Result);
			}
		}


		[HttpPost()]
		[AllowAnonymous]
		public async Task<ActionResult<JToken>> Execute(JsonRpcRequest request)
		{
			_logger.LogInformation($"JsonRpcRequest - {request.ToJson()}");
			if (request.Method is null)
				return BadRequest("MultiChain API method cannot be missing.");

			MultiChainResult<JToken> result = null;
			if (request.Params is null || ((request.Params.Length == 1) && (request.Params[0] is null)))
				result = await _rpc.JsonRpcRequestAsync(request.Method);
			else
				result = await _rpc.JsonRpcRequestAsync(request.Method, request.Params);

			if (result.IsError)
				return BadRequest(result.ExceptionMessage);
			return Ok(result.Result);
		}
	}
}
