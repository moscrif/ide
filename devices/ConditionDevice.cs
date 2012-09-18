using System;
using System.Xml.Serialization;
using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace Moscrif.IDE.Devices
{
	//[JsonObject(MemberSerialization.OptIn)]
	public class ConditionDevice //: ICloneable
	{
		[XmlIgnore]
		//[JsonProperty(PropertyName = "name")]
		public string Name;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "value")]
		public string Value;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "width")]
		public int Width;

		[XmlIgnore]
		//[JsonProperty(PropertyName = "height")]
		public int Height;

		public ConditionDevice (){
		}

		public ConditionDevice (string Name, string Value){
			this.Name = Name;
			this.Value = Value;
			this.Width = -1;
			this.Height = -1;
		}


		/*	object ICloneable.Clone ()
			{
				combineRule cr = (combineRule)MemberwiseClone ();
				return cr;
			}

			public string ConditionName;
			public int ConditionId;
			public int RuleId;
			public string RuleName;

			public override string ToString ()
			{
				return string.Format ("{0}({1}):{2}({3})",ConditionName,ConditionId,RuleName,RuleId);
			}*/

	}
}

