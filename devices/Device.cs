using System;
using System.Text;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Iface.Entities;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Moscrif.IDE.Tool;
using System.Web;

namespace Moscrif.IDE.Devices
{
	//[JsonObject(MemberSerialization.OptIn)]
	public class Device
	{
		public Device(){
			this.Includes = new Includes();
		}


		//public string TargetPlatform;

		[XmlAttribute("targetPlatform")]
		public int TargetPlatformId;

		private Rule plarform;
		public Rule Platform{
			get{
				if(plarform != null)
					return plarform;

				plarform = MainClass.Settings.Platform.Rules.Find(x=>x.Id ==TargetPlatformId );

				return plarform;
			}
		}

		// platforma pre Pc /Win/Linx/Mac
		[XmlAttribute("runningPlatform")]
	//	[JsonProperty(PropertyName = "platform" ) ]
		public string RunningPlatform;

		//[XmlAttribute("publish")]
		[XmlIgnore]
		//[JsonProperty(PropertyName = "publishDir")]
		public string Publish;

		//[XmlAttribute("root")]
		[XmlIgnore]
		//[JsonProperty(PropertyName = "root")]
		public string Root;

		//[XmlAttribute("application")]
		[XmlIgnore]
		//[JsonProperty(PropertyName = "application")]
		public string Application;

		//[XmlAttribute("outputDir")]
		[XmlIgnore]
		//[JsonProperty(PropertyName = "outputDir")]
		public string Output_Dir;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "outputDir")]
		public bool LogDebug;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "outputDir")]
		public string ApplicationType;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "outputDir")]
		public string FacebookAppID;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "tempDir")]
		public string Temp;

		//[XmlAttribute("PublishProperty")]
		//[XmlIgnore]
		[XmlArray("publishProperties")]
		[XmlArrayItem("property")]
		//[JsonProperty(PropertyName = "publish")]
		public List<PublishProperty> PublishPropertisMask;

		//[XmlAttribute("PublishProperty")]
		//[XmlIgnore]
		[XmlIgnore]
		//[JsonProperty(PropertyName = "publish")]
		public List<PublishProperty> PublishPropertisFull;

//		[JsonProperty( PropertyName = "output_name" )]
//		public string Output_NameFormat;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "outputName")]
		public string Output_Name;


		//[JsonProperty(PropertyName = "includes")]
		[XmlElement("includes")]
		public Includes Includes;

		//[XmlAttribute("DeviceType")]
		[XmlIgnore]
		public DeviceType Devicetype {
			get{
				return (DeviceType)TargetPlatformId;
			}
			/*set{
				if(TargetPlatformId == 0){
					TargetPlatformId = (int) value;
				}
			}*/
		}

		[XmlIgnore]
		//[JsonProperty(PropertyName = "conditions")]
		public ConditionDevice[] Conditions;

		public string GenerateJson(){

			string json ="";

			try{

				System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
	        		 new System.Web.Script.Serialization.JavaScriptSerializer();

				oSerializer.RegisterConverters
					(new System.Web.Script.Serialization.JavaScriptConverter[]
					 {
						new DeviceJavaScriptConverter(),
					 	new PublishPropertyJavaScriptConverter(),
						new IncludesJavaScriptConverter(),
						new SkinJavaScriptConverter(),
						new ConditionJavaScriptConverter()
					 });
				string sJSON = oSerializer.Serialize(this);
				json =  sJSON;

			}catch(Exception ex){
				Logger.Error(ex.Message);
				return "";
			}

			DeviceJSonFormatter djf= new DeviceJSonFormatter();
			json = djf.Format(json);

			return json;
		}

		static public bool CheckDevice(string platformSpecific){
			string dirPublish = MainClass.Tools.GetPublishDirectory(platformSpecific);

			if (!System.IO.Directory.Exists(dirPublish)){
					return false;
			}

			string dirApp = System.IO.Path.Combine(MainClass.Settings.PublishDirectory,platformSpecific+".app");

			if (!System.IO.File.Exists(dirApp)){
					return false;
			}

			string dirPublishOS = "";
			if(MainClass.Platform.IsMac){
				dirPublishOS =System.IO.Path.Combine(dirPublish,"mac");
			}
			if(MainClass.Platform.IsWindows){
				dirPublishOS =System.IO.Path.Combine(dirPublish,"windows");
			}
			if(MainClass.Platform.IsX11){
				dirPublishOS =System.IO.Path.Combine(dirPublish,"linux");
			}
			
			if (!System.IO.Directory.Exists(dirPublishOS)){
					return false;
			}
			return true;
		}

	}
}