using System;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using System.Linq;

using Mono.Data.Sqlite;

using Moscrif.IDE.Tool;
using Moscrif.IDE.Completion;


namespace Moscrif.IDE.Actions
{

	public class GenerateAutoCompleteWords : Gtk.Action
	{
		public GenerateAutoCompleteWords():base("generateAutoComplete",MainClass.Languages.Translate("menu_generate_autoComplete"),MainClass.Languages.Translate("menu_title_generate_autoComplete"),null)
		{
		}

		private SqlLiteDal sqlLiteDal;


		protected override void OnActivated ()
		{

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.OkCancel, "Are you sure?", "", Gtk.MessageType.Question);
			int result = md.ShowDialog();
			if(result != (int)Gtk.ResponseType.Ok){
				return;
			}

			ProgressDialog progressDialog;

			string filename = System.IO.Path.Combine(MainClass.Paths.ConfingDir, "completedcache");
			sqlLiteDal = new SqlLiteDal(filename);

			string sql = "";

			if(sqlLiteDal.CheckExistTable("completed") ){
				sql = "DROP TABLE completed ;";
				sqlLiteDal.RunSqlScalar(sql);
			}

			sql = "CREATE TABLE completed (id INTEGER PRIMARY KEY, name TEXT, signature TEXT, type NUMERIC, parent TEXT,summary TEXT ) ;";
			sqlLiteDal.RunSqlScalar(sql);

			SyntaxMode mode = new SyntaxMode();
			mode = SyntaxModeService.GetSyntaxMode("text/moscrif");

			progressDialog = new ProgressDialog("Generated...",ProgressDialog.CancelButtonType.None, mode.Keywords.Count() ,MainClass.MainWindow);

			foreach (Keywords kw in mode.Keywords){

				progressDialog.Update(kw.ToString());
				foreach (string wrd in kw.Words){
						insertNewRow(wrd,wrd,(int)CompletionDataTyp.keywords,"","");

						//sql = String.Format("INSERT INTO completed (name,signature,type) values ( {0} , {0} ,K ) ;",wrd);
						//sqlLiteDal.RunSqlScalar(sql);
					}
			}



			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Select DOC Directory (with xml)", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			FileInfo[] xmls = new FileInfo[]{};

			if (fc.Run() == (int)ResponseType.Accept) {

				DirectoryInfo di = new DirectoryInfo(fc.Filename);
				xmls = di.GetFiles("*.xml");
				//List<string> output = new List<string>();

				//MainClass.CompletedCache.ListTypes= listSignature.Distinct().ToList();
				//MainClass.CompletedCache.ListMembers= listMemberSignature.Distinct().ToList();
			}
			progressDialog.Destroy();
			fc.Destroy();
			progressDialog = new ProgressDialog("Generated...",ProgressDialog.CancelButtonType.None,xmls.Length ,MainClass.MainWindow);
		            foreach (FileInfo xml in xmls)
		            {
		                try
		                {
					progressDialog.Update(xml.Name);
					if(!xml.Name.StartsWith("_"))
						getObject(xml.FullName);
		                }
		                catch(Exception ex) {
		                    Console.WriteLine(ex.Message);
		                    Console.WriteLine(ex.StackTrace);
		                    return;
		                }
		            }
			progressDialog.Destroy();

			md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Done", "", Gtk.MessageType.Info);
			md.ShowDialog();
		}

		private void getObject(string file ){
			XmlDocument rssDoc;
			XmlNode nodeRss = new XmlDocument();

			XmlNode nodeChannel = new XmlDocument();
			XmlNode nodeItem;

			try{
				XmlTextReader reader = new XmlTextReader(file);
 				rssDoc = new XmlDocument();
				rssDoc.Load(reader);
 				for (int i = 0; i < rssDoc.ChildNodes.Count; i++) {
                    			if (rssDoc.ChildNodes[i].Name == "doc"){
						nodeRss = rssDoc.ChildNodes[i];
					}
				}
				/*for (int i = 0; i < nodeRss.ChildNodes.Count; i++){
					if (nodeRss.ChildNodes[i].Name == "members"){
						nodeChannel = nodeRss.ChildNodes[i];
					}
				}*/
				string parent = "";

				for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
      				{
					if (nodeRss.ChildNodes[i].Name == "member")
  					{

						nodeItem = nodeRss.ChildNodes[i];
						string name="";

						string signature="";
						string summary = GetSummary(nodeItem);
						bool visibility = false;

						foreach (XmlAttribute atrb  in nodeItem.Attributes){

							if (atrb.Name == "name"){
								name =atrb.InnerText;
							}

							if (atrb.Name == "signature"){
								signature =atrb.InnerText;
							}

							if (atrb.Name == "visibility"){
								if (atrb.InnerText.Trim() == "private"){
									visibility = true;
								}
							}
						}
						if(visibility) continue;


						if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(signature)){

							if(signature.Trim().StartsWith("(") ||
								signature.Trim().StartsWith("{") ||
								signature.Trim().StartsWith("[") ||
								signature.Trim().StartsWith("!")){

								continue;
							}


							if(name.StartsWith("T:")){
								parent =signature;
								insertNewRow(signature,signature,(int)CompletionDataTyp.types,"",summary);

							}

							if(name.Contains("this"))
								continue;

							if (name.StartsWith("P:")){
								int indx = signature.Trim().IndexOf(".");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									insertNewRow(tmp,signature,(int)CompletionDataTyp.properties,parent,summary);
									//listMemberSignature.Add(tmp);
									continue;
								}
								if(signature.Length<2)
										continue;

								insertNewRow(signature,signature,(int)CompletionDataTyp.properties,parent,summary);

							} else if(name.StartsWith("M:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									insertNewRow(tmp,signature,(int)CompletionDataTyp.members,parent,summary);
									continue;
								}
								if(signature.Length<2)
										continue;
								insertNewRow(signature,signature,(int)CompletionDataTyp.members,parent,summary);

							} else if(name.StartsWith("E:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									insertNewRow(tmp,signature,(int)CompletionDataTyp.events,parent,summary);

									continue;
								}
								insertNewRow(signature,signature,(int)CompletionDataTyp.events,parent,summary);
							}


						}

					}
				}
			} catch(Exception ex){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "ERROR", ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
				Console.WriteLine(file);
				Console.WriteLine(ex.Message);
			}
		}


		private string GetSummary (XmlNode nodeItem){

			for (int j = 0; j < nodeItem.ChildNodes.Count; j++)
			{
				if (nodeItem.ChildNodes[j].Name == "summary")
				{
					string summary =nodeItem.ChildNodes[j].InnerText;
					summary = summary.Trim();
					summary = summary.Replace("\t"," ");
					return summary;
				}
			}
			return "";
		}

		private void insertNewRow(string name,string signature,int type,string  parent,string summary){

			string	sql;

			signature = signature.Replace("'", "" );
			summary = summary.Replace("'", "" );

			if(String.IsNullOrEmpty(parent))
				sql = String.Format("INSERT INTO completed (name,signature,type,summary) values ( '{0}' , '{1}' , '{2}', '{3}' ) ;",name,signature,type,summary);
			else
				sql = String.Format("INSERT INTO completed (name,signature,type, parent,summary) values ( '{0}' , '{3}.{1}' , '{2}', '{3}', '{4}' ) ;",name,signature,type,parent,summary);

			sqlLiteDal.RunSqlScalar(sql);

		}


		/*
		protected override void OnActivated ()
		{
			MainClass.CompletedCache.ListKeywords = new System.Collections.Generic.List<string>();
			MainClass.CompletedCache.ListTypes = new System.Collections.Generic.List<string>();

			SyntaxMode mode = new SyntaxMode();
			mode = SyntaxModeService.GetSyntaxMode("text/moscrif");
			foreach (Keywords kw in mode.Keywords){

				foreach (string wrd in kw.Words){
						MainClass.CompletedCache.ListKeywords.Add(wrd);
					}
			}

			MainClass.CompletedCache.SaveCompletedCache();

			List<string> listSignature = new List<string>();

			List<string> listMemberSignature = new List<string>();

			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("Select DOC Directory (with xml)", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) {

				DirectoryInfo di = new DirectoryInfo(fc.Filename);
					FileInfo[] xmls = di.GetFiles("*.xml");
					List<string> output = new List<string>();
			            foreach (FileInfo xml in xmls)
			            {
			                try
			                {
			                    getObject(xml.FullName, ref listSignature, ref listMemberSignature);
			                }
			                catch(Exception ex) {
			                    Console.WriteLine(ex.Message);
			                    Console.WriteLine(ex.StackTrace);
			                    return;
			                }
			            }


				MainClass.CompletedCache.ListTypes= listSignature.Distinct().ToList();
				MainClass.CompletedCache.ListMembers= listMemberSignature.Distinct().ToList();
			}
			fc.Destroy();

			MainClass.CompletedCache.SaveCompletedCache();

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Done", "", Gtk.MessageType.Info);
			md.ShowDialog();

		}

		private void getObject(string file , ref List<string> listSignature,ref List<string> listMemberSignature){
			XmlDocument rssDoc;
			XmlNode nodeRss = new XmlDocument();
			XmlNode nodeChannel = new XmlDocument();
			XmlNode nodeItem;

			try{
				XmlTextReader reader = new XmlTextReader(file);
 				rssDoc = new XmlDocument();
				rssDoc.Load(reader);
 				for (int i = 0; i < rssDoc.ChildNodes.Count; i++) {
                    			if (rssDoc.ChildNodes[i].Name == "doc"){
						nodeRss = rssDoc.ChildNodes[i];
					}
				}
				for (int i = 0; i < nodeRss.ChildNodes.Count; i++){
					if (nodeRss.ChildNodes[i].Name == "members")                {
						nodeChannel = nodeRss.ChildNodes[i];
					}
				}
				for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
      				{
					if (nodeChannel.ChildNodes[i].Name == "member")
  					{

						nodeItem = nodeChannel.ChildNodes[i];
						string name="";
						string signature="";

						foreach (XmlAttribute atrb  in nodeItem.Attributes){

							if (atrb.Name == "name"){
								name =atrb.InnerText;
							}

							if (atrb.Name == "signature"){
								signature =atrb.InnerText;
							}
						}


						if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(signature)){

							if(signature.Trim().StartsWith("(") ||
								signature.Trim().StartsWith("{") ||
								signature.Trim().StartsWith("[") ||
								signature.Trim().StartsWith("!")){

								continue;
							}



							if(name.StartsWith("T:")){
								listSignature.Add(signature);

							} else if (name.StartsWith("P:")){
								int indx = signature.Trim().IndexOf(".");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									listMemberSignature.Add(tmp);
									continue;
								}
								if(signature.Length<2)
										continue;

								listMemberSignature.Add(signature);

							} else if(name.StartsWith("M:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									listMemberSignature.Add(tmp);
									continue;
								}
								if(signature.Length<2)
										continue;

								listMemberSignature.Add(signature);

							} else if(name.StartsWith("E:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);
									listMemberSignature.Add(tmp);
									continue;
								}
								listMemberSignature.Add(signature);
							}
						}

					}
				}
			} catch(Exception ex){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "ERROR", ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
				Console.WriteLine(file);
				Console.WriteLine(ex.Message);
			}
		}*/

	}
}

