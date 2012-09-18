using System;

namespace Moscrif.IDE.Controls.SqlLite
{
	public class FieldTable
	{
		public FieldTable(string name, string type)
		{
			this.Name= name;
			this.Type= type;
		}

		public string Name {get;set;}
		public string Type {get;set;}
		public bool NotNULL {get;set;}
		public string DefaultValue {get;set;}
	}
}

