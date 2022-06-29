using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting.WindowsServices;
using MultiChainDotNet.Core;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using UtilsDotNet.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace C8c.GalleryLocalApi.WindowsService
{
	public class Program
	{

		public static void Main(string[] args)
		{
			var Logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

			try
			{
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception exception)
			{
				Logger.Error(exception, "Stopped program because of exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) =>
			{
				services.Configure<KestrelServerOptions>(
					context.Configuration.GetSection("Kestrel"));
			})
			.UseWindowsService()
			.ConfigureWebHostDefaults(builder =>
			{
				builder
					.UseStartup<Startup>()
					.ConfigureLogging((hostingContext, logging) =>
					{
						logging.ClearProviders();
						NLog.LogManager.Configuration = new NLogLoggingConfiguration(hostingContext.Configuration.GetSection("NLog"));
					})
					.UseNLog()
					.ConfigureAppConfiguration((hostingContext, builder) =>
					{
						builder
							.AddEnvironmentVariables("GALLERY_");

						builder.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);

						if (hostingContext.HostingEnvironment.EnvironmentName == "sandbox")
							builder.AddUserSecrets("db8f1127-a057-4229-acb8-c886766bee9e");
					});
			});
	}
}
