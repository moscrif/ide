using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace  Moscrif.IDE.Tool
{
	public static class Cryptographer
	{

	        public static string MD5Hash(string text)
	        {
	            return MD5Hash(text, String.Empty);
	        }

		/*public string GetMd5Sum(string str) {
			
			byte[] input = ASCIIEncoding.ASCII.GetBytes(str);
			byte[] output = MD5.Create().ComputeHash(input);
			StringBuilder sb = new StringBuilder();
			
			for(int i=0;i<output.Length;i++) {
				sb.Append(output[i].ToString("X2"));
			}
			return sb.ToString();
		}*/
	
	        public static string MD5Hash(string text, string salt)
	        {
	            ASCIIEncoding enc = new ASCIIEncoding();
	            byte[] buffer = enc.GetBytes(text + salt);
	            return MD5Hash(buffer);
	        }
	
	        public static string MD5Hash(byte[] buffer)
	        {
	            MD5 md = MD5CryptoServiceProvider.Create();
	            byte[] hash = md.ComputeHash(buffer);
	            StringBuilder sb = new StringBuilder();
	            for (int i = 0; i < hash.Length; i++)
	                sb.Append(hash[i].ToString("X2"));
	            return sb.ToString();
	        }

	        public static string SHA1Hash(byte[] buffer)
	        {
	            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
	            byte[] hash = sha1.ComputeHash(buffer);
	            StringBuilder sb = new StringBuilder();
	            for (int i = 0; i < hash.Length; i++)
	                sb.Append(hash[i].ToString("X2"));
	            return sb.ToString();
	        }

	        public static string SHA1HashBase64(byte[] buffer)
	        {
	            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
	            byte[] hash = sha1.ComputeHash(buffer);
		    return Convert.ToBase64String(hash);
	        }

	}
}

