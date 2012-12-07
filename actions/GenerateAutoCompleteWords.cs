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
using System.Text;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using System.Linq;

using Mono.Data.Sqlite;

using Moscrif.IDE.Tool;
using Moscrif.IDE.Completion;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;


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

			sql = "CREATE TABLE completed (id INTEGER PRIMARY KEY, name TEXT, signature TEXT, type NUMERIC, parent TEXT,summary TEXT ,returnType TEXT ) ;";
			sqlLiteDal.RunSqlScalar(sql);

			SyntaxMode mode = new SyntaxMode();
			mode = SyntaxModeService.GetSyntaxMode("text/moscrif");

			progressDialog = new ProgressDialog("Generated...",ProgressDialog.CancelButtonType.None, mode.Keywords.Count() ,MainClass.MainWindow);

			foreach (Keywords kw in mode.Keywords){

				progressDialog.Update(kw.ToString());
				foreach (string wrd in kw.Words){
						insertNewRow(wrd,wrd,(int)CompletionDataTyp.keywords,"","","");
					}
			}


			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog("data.json", null, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			fc.TransientFor = MainClass.MainWindow;
			fc.SetCurrentFolder(@"d:\Work\docs-api\output\");

			if (fc.Run() != (int)ResponseType.Accept) {
				return;
			}
			string json ;
			string fileName = fc.Filename;
			progressDialog.Destroy();
			fc.Destroy();
			progressDialog = new ProgressDialog("Generated...",ProgressDialog.CancelButtonType.None,100 ,MainClass.MainWindow);

			using (StreamReader file = new StreamReader(fileName)) {
				json = file.ReadToEnd();
				file.Close();
				file.Dispose();
			}

			//XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
			//doc.Save(fileName+"xml");

			/*JObject jDoc= JObject.Parse(json);
			//classes
			Console.WriteLine("o.Count->"+jDoc.Count);

			foreach (JProperty jp in jDoc.Properties()){
				Console.WriteLine(jp.Name);
			}
			Console.WriteLine("------------");
			JObject classes = (JObject)jDoc["classes"];
			foreach (JProperty jp in classes.Properties()){
				Console.WriteLine(jp.Name);
				JObject classDefin = (JObject)classes[jp.Name];
				string name = (string)classDefin["name"];
				string shortname = (string)classDefin["shortname"];
				string description = (string)classDefin["description"];
				//string type = (string)classDefin["type"];
				insertNewRow(name,name,(int)CompletionDataTyp.types,"",description,name);
			}
			Console.WriteLine("------------");

			JArray classitems = (JArray)jDoc["classitems"];
			foreach (JObject classitem in classitems){

				string name = (string)classitem["name"];
				Console.WriteLine(name);

				string description = (string)classitem["description"];
				string itemtype = (string)classitem["itemtype"];
				string classParent = (string)classitem["class"];
				string signature = (string)classitem["name"];
				CompletionDataTyp type = CompletionDataTyp.noting;
				string returnType= classParent;

				switch (itemtype){
					case "method":{
						JArray paramsArray = (JArray)classitem["params"];
						signature = signature+ GetParams(paramsArray);	
						type = CompletionDataTyp.members;
						JObject returnJO =(JObject)classitem["return"] ;
						if(returnJO!=null){
							returnType = (string)returnJO["type"];
						}

						break;
					}
					case "property":{
						
						string tmpType = (string)classitem["type"];
						if(!String.IsNullOrEmpty(tmpType)){
							returnType=tmpType.Replace("{","").Replace("}","");
						}
						type = CompletionDataTyp.properties;
						break;
					}
					case "event":{
						JArray paramsArray = (JArray)classitem["params"];
						signature = signature+ GetParams(paramsArray);
						type = CompletionDataTyp.events;
						break;
					}
					case "attribute":{
						continue;
						break;
					} 
					default:{
						type = CompletionDataTyp.noting;
						break;
					}

				}

				insertNewRow(name,signature,(int)type,classParent,description,returnType);
			}*/
			//classitems

//			string name = (string)o["project"]["name"];
//			Console.WriteLine(name);

			progressDialog.Destroy();
			
			md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Done", "", Gtk.MessageType.Info);
			md.ShowDialog();

			return;
			/*

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
			md.ShowDialog();*/
		}

		private void getObject(string file ){
			XmlDocument rssDoc;
			XmlNode nodeRss = new XmlDocument();

			//XmlNode nodeChannel = new XmlDocument();
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
								insertNewRow(signature,signature,(int)CompletionDataTyp.types,"",summary,"");

							}

							if(name.Contains("this"))
								continue;

							if (name.StartsWith("P:")){
								int indx = signature.Trim().IndexOf(".");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									insertNewRow(tmp,signature,(int)CompletionDataTyp.properties,parent,summary,"");
									//listMemberSignature.Add(tmp);
									continue;
								}
								if(signature.Length<2)
										continue;

								insertNewRow(signature,signature,(int)CompletionDataTyp.properties,parent,summary,"");

							} else if(name.StartsWith("M:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									if(tmp.Length<2)
										continue;

									insertNewRow(tmp,signature,(int)CompletionDataTyp.members,parent,summary,"");
									continue;
								}
								if(signature.Length<2)
										continue;
								insertNewRow(signature,signature,(int)CompletionDataTyp.members,parent,summary,"");

							} else if(name.StartsWith("E:")){
								int indx = signature.IndexOf("(");
								if(indx>0){
									string tmp = signature.Substring(0,indx);

									insertNewRow(tmp,signature,(int)CompletionDataTyp.events,parent,summary,"");

									continue;
								}
								insertNewRow(signature,signature,(int)CompletionDataTyp.events,parent,summary,"");
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
		/*
		private string GetParams(JArray paramsArray){
			if(paramsArray == null)
				return "()";
			string paramsString = "(";
			foreach (JObject jo in paramsArray){
				StringBuilder param = new StringBuilder();
				string name = (string)jo["name"];
				string description = (string)jo["description"];
				string type = (string)jo["type"];

				JValue optionalStr = (JValue)jo["optional"];
				bool optional = false;
				if(optionalStr!=null){
					if (!bool.TryParse(optionalStr.ToString(),out optional)){
						optional = false;
					}
				}

				string optdefault = (string)jo["optdefault"];

				JValue multipleStr = (JValue)jo["multiple"];
				bool multiple = false;
				if(multipleStr!=null){
					if (!bool.TryParse(multipleStr.ToString(),out multiple)){
						multiple = false;
					}
				}

				param.Append(name);

				if(!String.IsNullOrEmpty(optdefault))
					param.Append("="+optdefault);
				if((optional) && (String.IsNullOrEmpty(optdefault)))
				   	param.Append("=undefined");

				if(multiple)
					param.Append(",..");

				paramsString = paramsString + param.ToString()+",";
			}
			paramsString = paramsString.TrimEnd(',');
			paramsString = paramsString+ ")";

			return paramsString;

		}
		*/
		private void insertNewRow(string name,string signature,int type,string  parent,string summary,string returnType){

			string	sql;

			signature = signature.Replace("'", "" );
			if(!String.IsNullOrEmpty(summary))
				summary = summary.Replace("'", "" );

			if(String.IsNullOrEmpty(parent))
				sql = String.Format("INSERT INTO completed (name,signature,type,summary,returnType) values ( '{0}' , '{1}' , '{2}', '{3}','{4}' ) ;",name,signature,type,summary,returnType);
			else
				sql = String.Format("INSERT INTO completed (name,signature,type, parent,summary,returnType) values ( '{0}' , '{3}.{1}' , '{2}', '{3}', '{4}','{5}' ) ;",name,signature,type,parent,summary,returnType);

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

