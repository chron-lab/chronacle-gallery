using C8c.Gallery.LocalApi.Abstractions.Dtos;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace C8c.GalleryLocalApi.WindowsService.Controllers.ChronAcle
{
	[ApiController]
	[Route("gallery")]
	public class GalleryController : ControllerBase
	{
		private IServiceProvider _container;
		private string _resourceServer;
		private ILogger<GalleryController> _logger;

		public GalleryController(IServiceProvider container)
		{
			_container = container;
			_resourceServer = _container.GetRequiredService<IConfiguration>()["ResourceServer"];
			_logger = _container.GetRequiredService<ILogger<GalleryController>>();
		}

		[HttpGet("test/upload")]
		[Authorize(Policy = "McGalleryController.GalleryTokenWrite")]
		//[AllowAnonymous]
		public async Task<ActionResult> TestContentUploadAccess()
		{
			return Ok(new { status = "success" });
		}

		[HttpPost("content/upload")]
		[Authorize(Policy = "McGalleryController.GalleryTokenWrite")]
		public async Task<ActionResult<GalleryTokenDto>> ContentUpload([FromBody] ContentUploadRequestDto request)
		{
			if (request is null)
				return BadRequest($"{nameof(request)} cannot be null");
			if (string.IsNullOrEmpty(request.Name))
				return BadRequest($"{nameof(request.Name)} cannot be null or empty");
			if (string.IsNullOrEmpty(request.CreatorAddress))
				return BadRequest($"{nameof(request.CreatorAddress)} cannot be null or empty");
			if (string.IsNullOrEmpty(request.Cid))
				return BadRequest($"{nameof(request.Cid)} cannot be null or empty");
			if (string.IsNullOrEmpty(request.Base64))
				return BadRequest($"{nameof(request.Base64)} cannot be null or empty");

			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				http.SetBearerToken(accessToken);

				var response = await http.PostAsJsonAsync($"/api/v1/mc/gallery/content/upload", request);
				var result = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
					throw new Exception(String.IsNullOrEmpty(result) ? response.ReasonPhrase : result);

				var dto = JsonConvert.DeserializeObject<GalleryTokenDto>(result);
				return dto;
			}
			catch (HttpRequestException hre)
			{
				_logger.LogError(hre.Message, hre);
				return StatusCode((int)hre.StatusCode, new
				{
					error =
				$"Unable to access resource."
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500, new { error = ex.Message });
			}

		}

		[HttpGet("content/download/{nftId}")]
		[Authorize(Policy = "McGalleryController.GalleryTokenRead")]
		public async Task<ActionResult<ContentDownloadResponseDto>> ContentDownloadByNftId(string nftId)
		{
			if (string.IsNullOrEmpty(nftId))
				return BadRequest($"{nameof(nftId)} cannot be null or empty");

			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				http.SetBearerToken(accessToken);

				var response = await http.GetStringAsync($"/api/v1/mc/gallery/content/download/{nftId}");
				var dto = JsonConvert.DeserializeObject<ContentDownloadResponseDto>(response);
				return dto;
			}
			catch (HttpRequestException hre)
			{
				_logger.LogError(hre.Message, hre);
				return StatusCode((int)hre.StatusCode, $"Unable to access resource.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500, new { error = ex.Message });
			}
		}

		[HttpGet("token/nftid/{nftid}")]
		[Authorize(Policy = "McGalleryController.GalleryTokenRead")]
		public async Task<ActionResult<GalleryTokenDto>> GetTokenByNftId(string nftId)
		{
			if (string.IsNullOrEmpty(nftId))
				return BadRequest($"{nameof(nftId)} cannot be null or empty");

			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				http.SetBearerToken(accessToken);

				var response = await http.GetStringAsync($"/api/v1/mc/gallery/token/nftid/{nftId}");
				var dto = JsonConvert.DeserializeObject<GalleryTokenDto>(response);
				return dto;
			}
			catch (HttpRequestException hre)
			{
				_logger.LogError(hre.Message, hre);
				return StatusCode((int)hre.StatusCode, $"Unable to access resource.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500, new { error = ex.Message });
			}
		}

		[HttpGet("tokens")]
		[Authorize(Policy = "McGalleryController.GalleryTokenRead")]
		public async Task<ActionResult<IList<GalleryTokenDto>>> ListTokens()
		{
			try
			{
				var http = new HttpClient() { BaseAddress = new Uri($"{_resourceServer}") };
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				http.SetBearerToken(accessToken);

				var response = await http.GetStringAsync($"/api/v1/mc/gallery/tokens");
				var dto = JsonConvert.DeserializeObject<List<GalleryTokenDto>>(response);
				return dto;
			}
			catch (HttpRequestException hre)
			{
				_logger.LogError(hre.Message, hre);
				return StatusCode((int)hre.StatusCode, new { error = $"Unable to access resource. {hre.Message}" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500, new { error = ex.Message });
			}

		}

	}
}
