using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Iface
{
	public class Languages
	{
		private Dictionary<string,string> languages;

		public Languages(string filePath)
		{

			if (!System.IO.File.Exists(filePath)){
				languages = null;
				Logger.Error("LANGUAGE FILE MISSING.");
			}

			try{
				languages = ReadDictionaryFile(filePath);
			} catch {
				languages = null;
				Logger.Error("LANGUAGE FILE IS CORRUPT.");
			}
		}

		public string Translate(string id){

			if(languages != null){
				if(languages.ContainsKey(id))
					return languages[id];
			}
			return id;
		}

		public string Translate(string id, params object[] arg){
			if(languages != null){
				if(languages.ContainsKey(id)){
					string text =languages[id];
					if((arg != null) && (arg.Length>0)){
						try{
							text = String.Format(text,arg);
						}catch{
							return id;
						}
					}
					return text;
				}
			}
			return id;
		}

		public static Dictionary<string, string> ReadDictionaryFile(string path)
		{
		    string fileData = "";

		    using (StreamReader sr = new StreamReader(path))
		    {
		        fileData = sr.ReadToEnd().Replace("\r", "");
		    }

		    Dictionary<string, string> properties = new Dictionary<string, string>();

		    string[] records = fileData.Split("\n".ToCharArray());

	            foreach (string record in records)
		    {
			string str = record.Trim();

			 if ((!string.IsNullOrEmpty(str)) &&
			            (!str.StartsWith(";")) &&
			            (!str.StartsWith("#")) &&
		        	    (!str.StartsWith("'")) &&
				    (!str.StartsWith("[")) &&
		            		(str.Contains("="))){

				int indx = str.IndexOf("=");

				if(indx>0){
					string key =str.Substring(0,indx);
					string val = str.Substring(indx+1,str.Length-indx-1);
	
					key = key.Trim();
					if(!properties.ContainsKey(key))
						properties.Add(key, val.TrimStart());
				}
				/*kvp = str.Split("=".ToCharArray());
				if(kvp.Length==2){
					string key = kvp[0].Trim();
					if(!properties.ContainsKey(key))
						properties.Add(key, kvp[1].TrimStart());
				}*/
			}
		    }
		    return properties;
		}

	/*	private static Dictionary<string, string> ReadDictionaryFile(string fileName)
		{
		    Dictionary<string, string> dictionary = new Dictionary<string, string>();
		    foreach (string line in File.ReadAllLines(fileName))
		    {
		        if ((!string.IsNullOrEmpty(line)) &&
		            (!line.StartsWith(";")) &&
		            (!line.StartsWith("#")) &&
		            (!line.StartsWith("'")) &&
		            (line.Contains('=')))
		        {
		            int index = line.IndexOf('=');
		            string key = line.Substring(0, index).Trim();
		            string value = line.Substring(index + 1).Trim();
		
		            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
		                (value.StartsWith("'") && value.EndsWith("'")))
		            {
		                value = value.Substring(1, value.Length - 2);
		            }
		            dictionary.Add(key, value);
		        }
		    }
		
		    return dictionary;
		}*/

	}
}

