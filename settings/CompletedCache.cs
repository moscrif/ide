using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Completion;
using Moscrif.IDE.Controls.SqlLite;
using Mono.Data.Sqlite;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Option
{
	public class CompletedCache
	{
		[XmlIgnore]
		public string FilePath
		{
			get {
				return System.IO.Path.Combine(MainClass.Paths.ConfingDir, "completedcache");
			}
		}

		public CompletedCache()
		{
			//if (ListKeywords == null) ListKeywords = new List<string>();

			ListDataKeywords = new List<CompletionData>();
			ListDataTypes = new List<CompletionData>();
			ListDataProperties = new List<CompletionData>();
			ListDataMembers  = new List<CompletionData>();
			ListDataEvents  = new List<CompletionData>();
		}

		/*public CompletedCache(string filePath)
		{
			FilePath = filePath;
			if (ListKeywords == null) ListKeywords = new List<string>();
		}*/


		public void SaveCompletedCache()
		{
			/*using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(CompletedCache));
				serializer.Serialize(fs, this);
			}*/
		}


		static public CompletedCache OpenCompletedCache(string filePath)
		{
			/*if (System.IO.File.Exists(filePath)) {

				try {
					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(CompletedCache));
						CompletedCache s = (CompletedCache)serializer.Deserialize(fs);

						return s;
					}
				} catch {//(Exception ex) {
					return new CompletedCache();
				}
			} else {
				return new CompletedCache();
			}
			*/
			return new CompletedCache();
		}


		public void GetCompletedData()
		{

			ListDataKeywords=new List<CompletionData>();
			ListDataTypes =new List<CompletionData>();
			ListDataMembers =new List<CompletionData>();
			ListDataProperties =new List<CompletionData>();
			ListDataEvents =new List<CompletionData>();

			AllCompletionRepeat=new CompletionDataList();
			AllCompletionOnlyOne =new CompletionDataList();
			NewCompletion =new CompletionDataList();
			DotCompletion =new CompletionDataList();

			if(!System.IO.File.Exists(FilePath)){
				Tool.Logger.Error("CodeCompletion file not exist!",null);
				return;
			}

			SqlLiteDal sqlLiteDal = new SqlLiteDal(FilePath);

			SqliteConnection dbcon =  (SqliteConnection)sqlLiteDal.GetConnect();//GetConnect();
			if (dbcon == null){
				return;
			}

			SqliteCommand dbcmd = dbcon.CreateCommand();

			string sql = "SELECT *  FROM completed;";
			dbcmd.CommandText = sql;
			SqliteDataReader reader = null;
			try {
				reader = dbcmd.ExecuteReader();

				int numberCollumns = reader.FieldCount;

				if (numberCollumns <5)return;

				while (reader.Read()) {

					CompletionData cd;
					string name = reader.GetValue(1).ToString();
					string signature= reader.GetValue(2).ToString();
					int type=  reader.GetInt32(3);
					string parent= reader.GetValue(4).ToString();
					string summary = "";
					string returnType = "";
					if(numberCollumns >=6)
						summary= reader.GetValue(5).ToString();

					if(numberCollumns >=7)
						returnType= reader.GetValue(6).ToString();

					cd = new CompletionData(name,null,signature,name,1,parent,returnType);

					cd.Signature =signature;


					if(!string.IsNullOrEmpty(summary)){
						cd.Description = cd.Description + Environment.NewLine+ summary;//+Environment.NewLine;
					}

					if(type == (int)CompletionDataTyp.keywords){
						ListDataKeywords.Add(cd);
					} else if(type == (int)CompletionDataTyp.members){
						ListDataMembers.Add(cd);
					} else if(type == (int)CompletionDataTyp.types){
						ListDataTypes.Add(cd);
					} else if(type == (int)CompletionDataTyp.properties){
						ListDataProperties.Add(cd);
					} else if(type == (int)CompletionDataTyp.events){
						ListDataEvents.Add(cd);
					}
				}

			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				Tool.Logger.Error("ERROR LOADING COMPLETED CACHE");
				Tool.Logger.Error(ex.Message);

				//MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", ex.Message, MessageType.Error);
				//ms.ShowDialog();

			} finally {

				if (reader != null) reader.Close();
				reader = null;
				dbcmd.Dispose();
				dbcmd = null;
				dbcon.Close();
				dbcon = null;
			}
			AllCompletionRepeat=GetCompletionData(CompletionTyp.allType,false );
			AllCompletionOnlyOne =GetCompletionData(CompletionTyp.allType,true );
			NewCompletion =GetCompletionData(CompletionTyp.newType,true );
			DotCompletion =GetCompletionData(CompletionTyp.dotType,true );
			IncludeCompletion =GetCompletionData(CompletionTyp.includeType,true );

		}

	/*	[XmlArrayAttribute("keywords")]
		[XmlArrayItem("keyword")]
		public List<string> ListKeywords;

		[XmlArrayAttribute("types")]
		[XmlArrayItem("type")]
		public List<string> ListTypes;

		[XmlArrayAttribute("members")]
		[XmlArrayItem("member")]
		public List<string> ListMembers;
		 */

		[XmlIgnore]
		public List<CompletionData> ListDataKeywords;

		[XmlIgnore]
		public List<CompletionData> ListDataTypes;

		[XmlIgnore]
		public List<CompletionData> ListDataMembers;

		[XmlIgnore]
		public List<CompletionData> ListDataProperties;

		[XmlIgnore]
		public List<CompletionData> ListDataEvents;

		[XmlIgnore]
		public CompletionDataList AllCompletionOnlyOne;

		[XmlIgnore]
		public CompletionDataList AllCompletionRepeat;

		[XmlIgnore]
		public CompletionDataList NewCompletion;

		[XmlIgnore]
		public CompletionDataList DotCompletion;

		[XmlIgnore]
		public CompletionDataList IncludeCompletion;


		private CompletionDataList GetCompletionData(CompletionTyp completiontype, bool onlyOne ){

			CompletionDataList listComplete = new CompletionDataList();

			if(completiontype == CompletionTyp.includeType){
				listComplete.Add(new CompletionData("lib",null,"lib","lib://"));
				listComplete.Add(new CompletionData("app",null,"app","app://"));
				return listComplete;
			}

			if(MainClass.CompletedCache.ListDataKeywords != null){
				foreach (CompletionData cd in MainClass.CompletedCache.ListDataKeywords) {
					if ( cd != null ){

						CompletionData cdParent =listComplete.Find(cd.DisplayText);

						if ((cdParent== null) || (!onlyOne))  {

							if (completiontype != CompletionTyp.newType){
								cdParent =cd.Clone();
								cdParent.OverloadedData.Add(cd.Clone());
								listComplete.Add(cdParent);
							}
						} else {
							if(!cdParent.Description.Contains(cd.Description)){
								cdParent.IsOverloaded = true;
								cdParent.OverloadedData.Add(cd.Clone());
								cdParent.Description =cdParent.Description+Environment.NewLine+Environment.NewLine+ cd.Description;

							}
						}
					}
				}
			}

			//Types (from doc T:)
			//i = 0;
			if(MainClass.CompletedCache.ListDataTypes != null){
				foreach (CompletionData cd in MainClass.CompletedCache.ListDataTypes) {
					if ( cd != null ){
						CompletionData cdParent =listComplete.Find(cd.DisplayText);
						if ((cdParent== null) || (!onlyOne)){

							if (completiontype != CompletionTyp.dotType){
								cdParent =cd.Clone();
								cdParent.OverloadedData.Add(cd.Clone());
								listComplete.Add(cdParent);
							}
						} else {
							if(!cdParent.Description.Contains(cd.Description)){
								cdParent.IsOverloaded = true;
								cdParent.OverloadedData.Add(cd.Clone());
								cdParent.Description =cdParent.Description+Environment.NewLine+Environment.NewLine+ cd.Description;
							}
						}
					}
				}
			}

			// M P E

			//Member (from doc M: )
			//i = 0;
			if(MainClass.CompletedCache.ListDataMembers != null){
				foreach (CompletionData cd in MainClass.CompletedCache.ListDataMembers) {
					if ( cd != null ){
						//if (cd.DisplayText==baseWord)
						//	i++;

						CompletionData cdParent =listComplete.Find(cd.DisplayText);
						if ((cdParent== null) || (!onlyOne)){
							if (completiontype != CompletionTyp.newType){
								cdParent =cd.Clone();
								cdParent.OverloadedData.Add(cd.Clone());
								listComplete.Add(cdParent);
							}
						}  else {
							if(!cdParent.Description.Contains(cd.Description)){
								cdParent.IsOverloaded = true;
								cdParent.OverloadedData.Add(cd.Clone());
								cdParent.Description =cdParent.Description+Environment.NewLine+Environment.NewLine+ cd.Description;
							}
						}
					}
				}
			}

			//Member (from doc P:)
			//i = 0;
			if(MainClass.CompletedCache.ListDataProperties != null){
				foreach (CompletionData cd in MainClass.CompletedCache.ListDataProperties) {
					if ( cd != null ){
						//if (cd.DisplayText==baseWord)
						//	i++;
						//if(!onlyOne)
						//	if(cd.DisplayText=="width")
						//		Console.WriteLine("1");

						CompletionData cdParent =listComplete.Find(cd.DisplayText);
						if ((cdParent== null) || (!onlyOne)){
							if (completiontype != CompletionTyp.newType){
								cdParent =cd.Clone();
								cdParent.OverloadedData.Add(cd.Clone());
								listComplete.Add(cdParent);
							}
						}else {
							if(!cdParent.Description.Contains(cd.Description)){
								cdParent.IsOverloaded = true;
								cdParent.OverloadedData.Add(cd.Clone());
								cdParent.Description =cdParent.Description+Environment.NewLine+Environment.NewLine+cd.Description;
							}
						 }
					}
				}
			}

			//Member (from doc  E:)
			//i = 0;
			if(MainClass.CompletedCache.ListDataEvents != null){
				foreach (CompletionData cd in MainClass.CompletedCache.ListDataEvents) {
					if ( cd != null ){
						//if (cd.DisplayText==baseWord)
						//	i++;

						CompletionData cdParent =listComplete.Find(cd.DisplayText);
						if ((cdParent== null) || (!onlyOne)){
							if (completiontype != CompletionTyp.newType){
								cdParent =cd.Clone();
								cdParent.OverloadedData.Add(cd.Clone());
								listComplete.Add(cdParent);
							}
						}else {
							if(!cdParent.Description.Contains(cd.Description)){
								cdParent.IsOverloaded = true;
								cdParent.OverloadedData.Add(cd.Clone());
								cdParent.Description =cdParent.Description+Environment.NewLine+ Environment.NewLine +cd.Description;
							}
						}
					}
				}
			}

			return listComplete;

		}

	}
}

