using System;
using System.IO;
using Mono.TextEditor;
using Moscrif.IDE.Completion;
using Moscrif.IDE.Controls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Extensions
{
	public static class TextEditorExtensions
	{
		#region context

		/// <summary>
		/// Vrati viac autokompletion, do zoznamu neprida slova z dokumentu
		/// </summary>
		/// <returns>
		/// Vrati viac jedine autocompletion zodpovedajuce poslednemu slovu v fullWord, odpovedajuce typu parenta slovu
		/// </returns>
		public static List<CompletionData> GetCompletionMemberData(this TextEditor editor ,string fullWord )
		{
			CompletionDataList listComplete = new CompletionDataList();
			listComplete.CompletionSelectionMode = CompletionSelectionMode.OwnTextField;

			string codestring = editor.Document.Text;
			string type = "";
			string parent = "";
			
			editor.ParseString(fullWord,out parent,out type);
			
			string[] words =  fullWord.Split('.');
			string word =words[words.Length-1];
			
			Completion.CompletionData cd;
			
			if(!String.IsNullOrEmpty(type)){
				return MainClass.CompletedCache.ListDataMembers.FindAll(x=>x.Parent == type && x.CompletionText == word);
			}
			
			return MainClass.CompletedCache.ListDataMembers.FindAll(x=>x.CompletionText == word);
		}

		/// <summary>
		/// Vrati jedno jedine autokompletion, do zoznamu neprida slova z dokumentu
		/// </summary>
		/// <returns>
		/// Vrati jedno jedine autocompletion zodpovedajuce poslednemu slovu v fullWord, odpovedajuce typu parenta slovu
		/// </returns>
		public static Completion.CompletionData GetCompletionData(this TextEditor editor ,string fullWord )
		{

			string codestring = editor.Document.Text;
			string type = "";
			string parent = "";

			editor.ParseString(fullWord,out parent,out type);

			string[] words =  fullWord.Split('.');
			string word =words[words.Length-1];

			Completion.CompletionData cd;

			if(!String.IsNullOrEmpty(type)){
				//List<CompletionData> lst = MainClass.CompletedCache.AllCompletionOnlyOne.FindAll(x=>x.Parent == type);
				if(MainClass.CompletedCache==null)
					Console.WriteLine("MainClass.CompletedCache");

				if(MainClass.CompletedCache.AllCompletionRepeat==null)
					Console.WriteLine("MainClass.CompletedCache.AllCompletionRepeat");

				cd = MainClass.CompletedCache.AllCompletionRepeat.Find(x=>x.Parent == type && x.CompletionText == word);

				if(cd != null ){
					return cd;
				}
			}

			cd = MainClass.CompletedCache.AllCompletionOnlyOne.Find(word);
			return cd;
		}

		/// <summary>
		/// Vrati autokompletion pre include,
		/// </summary>
		/// <returns>
		/// Zoznam autoCompletion slov pre include
		/// </returns>
		public static ICompletionDataList GetCompletionData(this TextEditor editor ,string baseWord,string fullWord)
		{
			/*
				lib - ms a adresare z framevorku
				app - ms adresare z projektu

			 */
			List<string> libsDefine = new List<string>();
			GetAllFiles(ref libsDefine,MainClass.Settings.LibDirectory);
			//GetAllFiles(ref libsDefine,MainClass.Workspace.ActualProject.AbsolutProjectDir);

			CompletionDataList listComplete = new CompletionDataList();
			foreach (string str in libsDefine){
				CompletionData cd = new CompletionData(str,null,"","\""+str+"\"");
				listComplete.Add(cd);
			}

			//listComplete.AddRange(MainClass.CompletedCache.IncludeCompletion);
			return listComplete;
		}

 		private static void GetAllFiles(ref List<string> filesList,string path)
		{
			if (!Directory.Exists(path)){
				Tool.Logger.LogDebugInfo(String.Format("Directory Not Exist-> {0}",path),null);
				return;
			}

			DirectoryInfo di = new DirectoryInfo(path);

			DirectoryInfo diOutput = new DirectoryInfo(MainClass.Workspace.ActualProject.OutputMaskToFullPath) ;

			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForPublish);

				string file = d.FullName.Replace(MainClass.Settings.LibDirectory,"");
				filesList.Add( "lib://"+file.Replace("\\","/") );

				if(di.FullName == diOutput.FullName)
					continue;

				if(indx<0){
					GetAllFiles(ref filesList, d.FullName);
				}
			}

			foreach (FileInfo f in di.GetFiles()) {

				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForPublish);
				if(indx >-1)continue;


				//if (f.Extension == ".msc") continue;
				if (f.Extension != ".ms") continue;

				//string file = FileUtility.AbsoluteToRelativePath(MainClass.Settings.LibDirectory ,f.FullName);
				string file = f.FullName.Replace(MainClass.Settings.LibDirectory,"");
				filesList.Add( "lib://"+file.Replace("\\","/") );
			}

		}

		/// <summary>
		/// Vrati vsetky mozne autokompletion , do zoznamu prida aj slova zo samotneho dokumentu
		/// </summary>
		/// <returns>
		/// Zoznam autoCompletion slov odpovesajuci baseWord a completiontype
		/// </returns>
		public static ICompletionDataList GetCompletionData(this TextEditor editor ,string baseWord,string fullWord ,CompletionTyp completiontype)
		{
			string codestring = editor.Document.Text;
			string type = "";
			string parent = "";

			editor.ParseString(fullWord,out parent,out type);

			Regex regex = new Regex(@"\W", RegexOptions.Compiled);
			codestring = regex.Replace(codestring, " ");

			string[] list = codestring.Split(' ');
			CompletionDataList listComplete = new CompletionDataList();
			listComplete.CompletionSelectionMode = CompletionSelectionMode.OwnTextField;

			if(!String.IsNullOrEmpty(type)){
				//List<CompletionData> lst = MainClass.CompletedCache.AllCompletionOnlyOne.FindAll(x=>x.Parent == type);
				List<CompletionData> lst = MainClass.CompletedCache.AllCompletionRepeat.FindAll(x=>x.Parent == type);
				foreach ( CompletionData cd in lst){
					string expres =cd.Parent+".on";
					if(cd.Signature.StartsWith(expres) ){
						//expres = cd.Signature.Replace(cd.Parent+".", cd.DisplayText +" = function ");
						expres = cd.Signature.Replace(cd.Parent+"."+ cd.DisplayText, cd.DisplayText +" = function ");
						cd.DisplayDescription =expres+"{}";
						cd.CompletionText =expres+"{"+Environment.NewLine+"}";
					}

				}

				if (lst != null)
					listComplete.AddRange(lst.ToArray());

				if(listComplete != null && listComplete.Count>0){
					return listComplete;
				}
			}

			switch (completiontype) {
			case CompletionTyp.allType:
				{
					listComplete.AddRange(MainClass.CompletedCache.AllCompletionOnlyOne);
					break;
				}
			case CompletionTyp.newType:
				{
					listComplete.AddRange(MainClass.CompletedCache.NewCompletion);
					break;
				}
			case CompletionTyp.dotType:
				{
					listComplete.AddRange(MainClass.CompletedCache.DotCompletion);
					break;
				}
			}


			int i = 0;
			foreach (string s in list) {
				if ( !String.IsNullOrEmpty(s.Trim()) ){
					if (s==baseWord)
						i++;

					if ((listComplete.Find(s)== null) && (s.Length>2) && ( (s!= baseWord) || (i ==1) ) ){
						CompletionData cd = new CompletionData(s, null, s, s);

						if (completiontype == CompletionTyp.newType){
							if(char.ToUpper(s[0]) == s[0]  && !char.IsDigit(s[0]) && !char.IsSymbol(s[0]) &&char.IsLetter(s[0]) ){

								CompletionData cdParent =listComplete.Find(cd.DisplayText);
								if (cdParent== null){
									listComplete.Add(cd);
								} else {
									if(!cdParent.Description.Contains(cd.Description))
										cdParent.Description =cdParent.Description+Environment.NewLine+cd.Description;
								}
							}
						}  else{
							CompletionData cdParent =listComplete.Find(cd.DisplayText);
							if (cdParent== null){
								listComplete.Add(cd);
							} else {
								if(!cdParent.Description.Contains(cd.Description))
									cdParent.Description =cdParent.Description+Environment.NewLine+cd.Description;
							}
						}
					}
				}
			}

 			return listComplete;
		}

		#endregion


		private static void ParseString(this TextEditor editor ,string fullWord , out string parent,out string type){

			string typeCode =editor.Document.Text;

			type = "";
			parent = "";

			if((!string.IsNullOrEmpty(fullWord)) && fullWord.Contains(".") ){

				string[] words =  fullWord.Split('.');

				parent  = words[0];

				// find parent this.super.super.app -> app
				int j = 0;
				while ((parent == "this" || parent == "super") && j<words.Length-1 ){
					parent = words[j];
					j++;
				}

				if( !string.IsNullOrEmpty(parent) && !parent.Contains("this") && !parent.Contains("super") ){

					try {
						// find  var meno = new Typ() var app = new Windows();
						string strRegex =String.Format(@"var\s*?{0}\s*?=\s*?new[\s0-9a-zA-Z\s]+\(.*?\)\s*?\;",parent);
						Regex regex2 = new Regex(strRegex, RegexOptions.Compiled);

						type = parent;
						MatchCollection mc = regex2.Matches(typeCode);
		
						foreach(Match m in mc){

							string typeDef = m.Value;
							string obj = m.Value;


							int indx1 =typeDef.IndexOf("var");
							int indx2 =typeDef.IndexOf("=");
	
							indx1 = indx1+3;
	
							if(indx1<indx2)
								obj =typeDef.Substring(indx1,indx2-indx1).Trim();
	
							indx1 =typeDef.IndexOf("new");
							indx2 =typeDef.IndexOf("(");
	
							indx1 = indx1+3;
							if(obj == parent){
								if(indx1<indx2){
									type =typeDef.Substring(indx1,indx2-indx1).Trim();
									return;
								}
							}
						}

						strRegex =String.Format(@"var\s*?{0}\s*?=\s*?[\s0-9a-zA-Z\s]+\.",parent);
						regex2 = new Regex(strRegex, RegexOptions.Compiled);

						
						type = parent;
						mc = regex2.Matches(typeCode);
						
						foreach(Match m in mc){

							string typeDef = m.Value;
							string obj = m.Value;
							
							
							int indx1 =typeDef.IndexOf("var");
							int indx2 =typeDef.IndexOf("=");
							
							indx1 = indx1+3;
							
							if(indx1<indx2)
								obj =typeDef.Substring(indx1,indx2-indx1).Trim();
							
							indx1 =typeDef.IndexOf("=");
							indx2 =typeDef.IndexOf(".");
							
							indx1 = indx1+1;
							if(obj == parent){
								if(indx1<indx2){
									type =typeDef.Substring(indx1,indx2-indx1).Trim();
									break;
								}
							}
						}
					}catch(Exception ex){
						Tool.Logger.Error(ex.Message);
					}
				}

			}


		}

	}
}

