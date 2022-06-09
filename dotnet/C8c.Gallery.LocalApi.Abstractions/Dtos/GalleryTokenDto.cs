using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Abstractions.Dtos
{
	/// <summary>
	/// 
	///			ChronnetToken combines the characteristics of MultiChain stream and token for content integration.
	///			This can be used as the NFT metadata.
	///			
	///			NOTE:
	///			-	The key is unique to the content and is based on IPFS CID content addressing scheme.
	///			-	The creator is the AddressId of the creator on Chronnet.
	/// 
	/// </summary>
	public class GalleryTokenDto
	{
		public string IssueTxid { get; set; }
		public string TokenRef { get; set; }
		public string NftId { get; set; }
		public string MetadataUri => $"ipfs://{MetadataCid}";
		public string MetadataCid { get; set; }
		public GalleryTokenMetadataDto Metadata { get; set; }

	}
}
