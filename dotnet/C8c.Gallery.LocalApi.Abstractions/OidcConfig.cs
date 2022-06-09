using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Abstractions
{
	public class OidcConfig
	{
		[JsonProperty("authority")]
		public string Authority { get; set; }

		[JsonProperty("client_id")]
		public string ClientId { get; set; }

		[JsonProperty("redirect_uri")]
		public string RedirectUri { get; set; }

		[JsonProperty("response_type")]
		public string ResponseType { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("post_logout_redirect_uri")]
		public string PostLogoutRedirectUri { get; set; }

		[JsonProperty("automaticSilentRenew")]
		public bool AutomaticSilentRenew { get; set; }

		[JsonProperty("silent_redirect_uri")]
		public string SilentRedirectUri { get; set; }
	}
}
