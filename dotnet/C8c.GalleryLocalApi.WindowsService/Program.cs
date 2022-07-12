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
								.AddEnvironmentVariables("GALLERY_")
								.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json")
								.AddUserSecrets("db8f1127-a057-4229-acb8-c886766bee9e");

                            try
                            {
								//get current user temp folder path
								string path = Path.GetTempPath();

								//remove local and temp from the path result
								string newPath = path.Replace("\\Local\\Temp\\", "");

								// Read the file as one string.
								string path2File = @$"{newPath}\Roaming\Microsoft\UserSecrets\db8f1127-a057-4229-acb8-c886766bee9e\secrets.json";
								//string text = File.ReadAllText(@"C:\Users\Layi\AppData\Roaming\Microsoft\UserSecrets\db8f1127-a057-4229-acb8-c886766bee9e\secrets.json");

								string secretFile = File.ReadAllText(path2File);
                                builder.AddJsonFile(secretFile, optional: true, reloadOnChange: true);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.WriteLine(ex);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            //this resolves an error from hosts trying to go

                            // below the root.


                            if (hostingContext.HostingEnvironment.EnvironmentName == "sandbox")
                            {
								builder.AddUserSecrets("db8f1127-a057-4229-acb8-c886766bee9e");
							}
						})
						;
				}).UseWindowsService();
	}
}
