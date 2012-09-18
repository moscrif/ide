using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Tool;


namespace  Moscrif.IDE.Iface
{
	public class AndroidPermission
	{
		private IniParser parser;

		public AndroidPermission()
		{
			string path = System.IO.Path.Combine(MainClass.Paths.ConfingDir,"AndroidPermission.ini" );
			parser = new IniParser(path);
		}

		public string[] Sections(){

			if(parser != null){
					return parser.GetAllSection();
			}
			return null;
		}

		public string[] ValuesInSection(string section){

			if(parser != null){
					return parser.EnumSection(section);
			}
			return null;
		}
	}
}

