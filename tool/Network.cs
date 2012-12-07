using System;
using System.Net;
using System.Collections.Generic;

namespace Moscrif.IDE.Tool
{
	public static class Network
	{

		/// <summary>
		/// Get avalible Ip Adress. Only local ip Adress (192.168.; 10.0.; 172.16.)
		/// </summary>
		/// <returns>Return list available ip adress</returns>
		public static List<string> GetIpAdress(){
		
			List<string> ipList = new List<string>();
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());//Resolve(Dns.GetHostName());
			foreach (IPAddress ip in ipHostInfo.AddressList){
				if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
					string ipAdress = ip.ToString();
					if(ipAdress.StartsWith("192.168.") ||
					   ipAdress.StartsWith("10.0.") ||
					   ipAdress.StartsWith("172.16.") ){
						ipList.Add(ip.ToString());
					}
				}
			}
			return ipList;
		}

	}
}

