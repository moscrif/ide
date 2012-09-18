using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;


namespace Moscrif.IDE.Iface.Entities
{
	public class PlatformResolution
	{
		public PlatformResolution()
		{
		}

		public PlatformResolution(int idPlatform)//, string namePlatform)
		{
			this.IdPlatform= idPlatform;
			//this.NamePlatform=namePlatform;
		}

		public bool IsValidResolution(int resolution){
			if (AllowResolution == null) return false;

			int indx = AllowResolution.FindIndex(x=>x ==resolution);
			if(indx >-1) return true;

			return false;

		}

		[XmlAttribute("id")]
		public int IdPlatform{
			get;
			set;
		}

		[XmlArrayAttribute("allowResolutions")]
		[XmlArrayItem("resolution")]
		public List<int> AllowResolution{
			get;
			set;
		}

		/*[XmlAttribute("name")]
		public string NamePlatform{
			get;
			set;
		}*/

		/*[XmlArrayAttribute("allowResolutions")]
		[XmlArrayItem("resolution")]
		public List<DisplayOrientation> AllowResolution{
			get;
			set;
		}*/
	}
}

