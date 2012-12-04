using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface.Entities
{

	public class ProxySetting
	{
		public ProxySetting(){
			ProxyType = ProxyTyp.SYSTEM;
			Proxy="";
			Password="";
		}
		
		
		public enum ProxyTyp{
			[XmlEnum("0")]
			NOPROXY = 0,
			[XmlEnum("1")]
			SYSTEM = 1,
			[XmlEnum("2")]
			CUSTON = 2
		}
		
		
		[XmlAttribute("proxyType")]
		public ProxyTyp ProxyType;
		
		[XmlAttribute("proxy")]
		public string Proxy;
		
		[XmlAttribute("port")]
		public int Port;
		
		[XmlAttribute("name")]
		public string Name;
		
		[XmlAttribute("password")]
		public string Password;
	}

}

