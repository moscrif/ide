using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Moscrif.IDE.Iface
{
	public class SystemWebClient: WebClient
	{
		public SystemWebClient():base()
		{
		//Fix ERROR The remote server returned an error: (417) Expectation failed.
		System.Net.ServicePointManager.Expect100Continue = false;

		if(MainClass.Settings!=null){
			if(MainClass.Settings.Proxy!=null){
					if(MainClass.Settings.Proxy.ProxyType== Option.Settings.ProxySetting.ProxyTyp.NOPROXY ){
					this.Proxy = null;
					} else if(MainClass.Settings.Proxy.ProxyType== Option.Settings.ProxySetting.ProxyTyp.SYSTEM){
					//WebRequest.DefaultWebProxy ;
					//WebRequest.GetSystemWebProxy();
					} else if(MainClass.Settings.Proxy.ProxyType==Option.Settings.ProxySetting.ProxyTyp.CUSTON){
					WebProxy wp = new WebProxy(MainClass.Settings.Proxy.Proxy,MainClass.Settings.Proxy.Port);
					if(!String.IsNullOrEmpty(MainClass.Settings.Proxy.Name)){
						wp.Credentials = new NetworkCredential(MainClass.Settings.Proxy.Name, MainClass.Settings.Proxy.Password);
					}
					this.Proxy = wp;
				}
			}
		}

		}
	}
}

