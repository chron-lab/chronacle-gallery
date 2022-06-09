using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C8c.Gallery.LocalApi.Abstractions.Dtos
{

	public class ContentUploadRequestDto
	{
		public string CreatorAddress { get; set; }
		public string Cid { get; set; }
		public string Base64 { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}


}
