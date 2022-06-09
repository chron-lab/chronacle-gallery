using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Abstractions.Dtos
{

	/// <summary>
	/// ERC721 style metadata combined with content info
	/// </summary>
	public class GalleryTokenMetadataDto
	{
		[JsonProperty("name")]
		public string Name { get; init; }

		[JsonProperty("description")]
		public string Description { get; init; }

		[JsonProperty("image")]
		public string ContentUri { get; init; }

		[JsonProperty("Stream")]
		public string ContentStreamName { get; init; }

		[JsonProperty("key")]
		public string ContentKey { get; init; }

		[JsonProperty("creator")]
		public string Creator { get; init; }

		public GalleryTokenMetadataDto()
		{
		}

		public GalleryTokenMetadataDto(string name, string description, string contentStreamName, string contentUri, string key, string creatorAddressId)
		{
			Name = name;
			Description = description;
			ContentKey = key;
			ContentStreamName = contentStreamName;
			ContentUri = contentUri;
			Creator = creatorAddressId;
		}
	}
}
