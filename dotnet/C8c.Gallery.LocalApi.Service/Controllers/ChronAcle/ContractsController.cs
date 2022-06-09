using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Service.Controllers.ChronAcle
{
	[ApiController]
	[Route("contracts")]
	public class ContractsController : ControllerBase
	{
		private IServiceProvider _container;
		private string _resourceServer;
		private ILogger<ContractsController> _logger;
		private IList<string> _supportedChains;

		public ContractsController(IServiceProvider container)
		{
			_container = container;
			_resourceServer = _container.GetRequiredService<IConfiguration>()["ResourceServer"];
			_logger = _container.GetRequiredService<ILogger<ContractsController>>();
			var config = _container.GetRequiredService<IConfiguration>();
			_supportedChains = config.GetSection("SupportedChains").Get<IList<string>>();

		}


		[HttpGet("{networkName}")]
		[AllowAnonymous]
		public async Task<ActionResult<Dictionary<string, string>>> ListContracts(string networkName)
		{
			if (string.IsNullOrEmpty(networkName))
				return BadRequest($"{nameof(networkName)} cannot be null or empty.");
			if (!_supportedChains.Contains(networkName))
				return BadRequest($"{networkName} is not supported. Please check if the correct network is selected in Metamask.");

			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				//var accessToken = await HttpContext.GetTokenAsync("access_token");
				//http.SetBearerToken(accessToken);

				var response = await http.GetStringAsync($"{HttpContext.Request.PathBase}{HttpContext.Request.Path}");
				var contracts = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
				return contracts;
			}
			catch (HttpRequestException hre)
			{
				if (hre.StatusCode == HttpStatusCode.Unauthorized)
					return Unauthorized();
				if (hre.StatusCode == HttpStatusCode.Forbidden)
					return Forbid();
				_logger.LogError(hre.Message, hre);
				return BadRequest(hre.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{networkName}/abi/{contractName}")]
		[AllowAnonymous]
		public async Task<ActionResult<string>> GetContractAbi(string networkName, string contractName)
		{
			if (string.IsNullOrEmpty(networkName))
				return BadRequest($"{nameof(networkName)} cannot be null or empty");
			if (string.IsNullOrEmpty(contractName))
				return BadRequest($"{nameof(contractName)} cannot be null or empty");
			if (!_supportedChains.Contains(networkName))
				return BadRequest($"{networkName} is not supported. Please check if the correct network is selected in Metamask.");

			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				http.SetBearerToken(accessToken);

				var response = await http.GetStringAsync($"{HttpContext.Request.PathBase}{HttpContext.Request.Path}");
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return BadRequest(ex.Message);
			}

		}

	}
}