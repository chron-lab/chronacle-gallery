using C8c.Gallery.LocalApi.Abstractions;
using C8c.Gallery.LocalApi.Abstractions.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace C8c.GalleryLocalApi.WindowsService.Controllers.ChronAcle
{
	[ApiController]
	[Route("config")]
	public class ConfigController : ControllerBase
	{
		private IServiceProvider _container;
		private string _resourceServer;
		private ILogger<ConfigController> _logger;

		public ConfigController(IServiceProvider container)
		{
			_container = container;
			_resourceServer = container.GetRequiredService<IConfiguration>()["ResourceServer"];
			_logger = _container.GetRequiredService<ILogger<ConfigController>>();
		}

		[HttpGet("oidc")]
		[AllowAnonymous]
		public async Task<ActionResult<OidcConfigDto>> OidcConfig()
		{
			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var response = await http.GetStringAsync($"/api/v1/oidc/config-testnet");
				var result = JsonConvert.DeserializeObject<OidcConfigDto>(response);
				return result;
			}
			catch (HttpRequestException hre)
			{
				if (hre.StatusCode == HttpStatusCode.Unauthorized)
					return StatusCode(500, "Api Server unauthorized.");
				if (hre.StatusCode == HttpStatusCode.Forbidden)
					return StatusCode(500, "Api Server forbidden.");
				if (hre.StatusCode == HttpStatusCode.NotFound)
					return StatusCode(500, "Api Server not found.");

				_logger.LogError(hre.Message, hre);
				return BadRequest(hre.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return BadRequest(ex.Message);
			}
		}

	}
}
