using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Abstractions.Dtos
{
	public class ContentUploadResponseDto
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("image")]
		public string ContentUri { get; set; }

		[JsonProperty("stream")]
		public string ContentStreamName { get; set; }

		[JsonProperty("key")]
		public string ContentKey { get; set; }

		[JsonProperty("creator")]
		public string Creator { get; set; }

		[JsonProperty("issuetxid")]
		public string IssueTxid { get; set; }

		[JsonProperty("metadatacid")]
		public string MetadataCid { get; set; }

	}

}
