using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface
{
	public class WebClientTimeout: WebClient
	{
		//time in milliseconds
		private int timeout;
		public int Timeout
		{
			get {
				return timeout;
			}
			set {
				timeout = value;
			}
		}
		
		public WebClientTimeout()
		{
			this.timeout = 5000;
		}
		
		public WebClientTimeout(int timeout)
		{
			this.timeout = timeout;
		}
		
		protected override WebRequest GetWebRequest(Uri address)
		{
			var result = base.GetWebRequest(address);
			result.Timeout = this.timeout;
			return result;
		}
	}

}
