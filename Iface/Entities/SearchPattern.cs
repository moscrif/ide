using System;
using System.Collections.Generic;

namespace Moscrif.IDE.Iface.Entities
{
	public class SearchPattern
	{
		public SearchPattern()
		{
		}

		public object Expresion;
		public bool CaseSensitive;
		public bool WholeWorlds;
		public object ReplaceExpresion;
		public TypSearch SearchTyp;

		public List<string> CloseFiles;
		public List<string> OpenFiles;

		public enum TypSearch{

			CurentDocument,
			AllOpenDocument,
			CurentProject,
			AllOpenProject
		}

		public SearchPattern Clone(){
			SearchPattern sp = new SearchPattern();
			sp.CaseSensitive = this.CaseSensitive;
			sp.Expresion = this.Expresion;
			sp.ReplaceExpresion = this.ReplaceExpresion;
			sp.SearchTyp = this.SearchTyp;
			sp.WholeWorlds =this.WholeWorlds;
			sp.CloseFiles = new List<string>(this.CloseFiles.ToArray());
			sp.OpenFiles = new List<string>(this.OpenFiles.ToArray());
			return sp;
		}
	}
}

