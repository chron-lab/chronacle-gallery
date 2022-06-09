using IdentityModel.Client;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// GET OIDC Config
var configClient = new HttpClient();
var oidc = await configClient.GetStringAsync("http://localhost:12038/api/v1/config/oidc");
Console.WriteLine("OIDC config:");
Console.WriteLine(oidc);


// GET DISCOVERY DOCUMENT

var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:12040");
if (disco.IsError) throw new Exception(disco.Error);
Console.WriteLine(await disco.HttpResponse.Content.ReadAsStringAsync());
Console.WriteLine();

// TEST UPLOAD ACCESS
Console.Write("Enter Access Token:");
var accessToken = Console.ReadLine();

var apiClient = new HttpClient();
apiClient.SetBearerToken(accessToken);

// CALL LIST API
string input = null;
IdentityModelEventSource.ShowPII = true;

while (string.IsNullOrEmpty(input))
{

	try
	{
		var content = await apiClient.GetStringAsync("http://localhost:12038/gallery/test/upload");
		Console.WriteLine(content);
	}
	catch (Exception e)
	{
		Console.WriteLine(e);
	}
	finally
	{
		Console.WriteLine("Press ENTER to retry or any key to quit");
		input = Console.ReadLine();
	}
}