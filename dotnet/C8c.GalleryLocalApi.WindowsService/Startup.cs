using C8c.Gallery.LocalApi.Abstractions;
using C8c.GalleryLocalApi.WindowsService.Controllers.MultiChain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MultiChainDotNet.Core;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using UtilsDotNet.Extensions;

namespace C8c.GalleryLocalApi.WindowsService
{
	public class Startup
	{
		readonly string AllowedOrigins = "_allowedOrigins";

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			HostEnvironment = env;
		}

		public IConfiguration Configuration { get; }
		public IWebHostEnvironment HostEnvironment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddControllers()
				.AddNewtonsoftJson();

			// --- Websocket ---
			services.AddSignalR();

			// --- CORS ---
			var allowedOrigins = Configuration.GetSection("AllowedOrigins").Get<string>();
			services.AddCors(options =>
			{
				options.AddPolicy(name: AllowedOrigins,
					builder =>
					{
						builder
							.WithOrigins(allowedOrigins.Split(','))
							.AllowCredentials()
							.AllowAnyHeader()
							.AllowAnyMethod();
					});
			});

			// --- AUTHENTICATION
			var authority = Configuration["AuthorizationServer:Authority"];
			services.AddAuthentication("Bearer")
				.AddJwtBearer("Bearer", options =>
				{
					options.Authority = authority;
					options.TokenValidationParameters =
						new TokenValidationParameters
						{
							// Disabled for local proxy
							ValidateAudience = false,
							ValidateIssuer = false,
						};
				});

			// --- AUTHORIZATION
			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.AddPolicy("McGalleryController.GalleryTokenWrite", policy =>
				{
					policy
						.RequireAuthenticatedUser()
						.RequireClaim("scope", "gallery:token.create")
						;
				});
				options.AddPolicy("McGalleryController.GalleryTokenRead", policy =>
				{
					policy
						.RequireAuthenticatedUser()
						.RequireClaim("scope", "gallery:token");
				});
			});

			// --- SWAGGER ---
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gallery Local API", Version = "v1" });
			});

			// --- MC ---
			var mcConfig = Configuration.GetSection("MultiChainConfiguration").Get<MultiChainConfiguration>();
			NLog.LogManager.GetCurrentClassLogger().Log(NLog.LogLevel.Info, mcConfig.ToJson());
			services
				.AddMultiChain(Configuration)
				.AddScoped<JsonRpcCommand>()
					.AddHttpClient<JsonRpcCommand>(c => c.BaseAddress = new Uri($"http://{mcConfig.Node.NetworkAddress}:{mcConfig.Node.NetworkPort}"))
						.SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
						.AddPolicyHandler(ExceptionPolicyHandler.RetryPolicy())
						.AddPolicyHandler(ExceptionPolicyHandler.TimeoutPolicy())
						.ConfigurePrimaryHttpMessageHandler(() =>
						{
							return new HttpClientHandler()
							{
								Credentials = new NetworkCredential(mcConfig.Node.RpcUserName, mcConfig.Node.RpcPassword)
							};
						});

			if (HostEnvironment.EnvironmentName == "sandbox")
				services.AddHttpLogging(options =>
				{
					options.LoggingFields = HttpLoggingFields.All;
				});

		}

		public void Configure(IApplicationBuilder app)
		{
			app.UsePathBase(Configuration["PathBase"]);

			if (HostEnvironment.IsDevelopment() || HostEnvironment.EnvironmentName == "sandbox")
			{
				if (HostEnvironment.EnvironmentName == "sandbox")
					IdentityModelEventSource.ShowPII = true;
				app.UseHttpLogging();

				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gallery Local API Version 1");
					c.RoutePrefix = String.Empty;
				});
			}

			app.UseCors(AllowedOrigins);
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers()
					//.RequireAuthorization()
					;
				endpoints.MapHub<TransactionHub>("/transaction");
			});

		}
	}
}
