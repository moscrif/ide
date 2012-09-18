using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace  Moscrif.IDE.Iface.Entities
{
	public class Condition : ICloneable
	{

		#region ICloneable implementation
		object ICloneable.Clone()
		{
			Condition cd = (Condition)MemberwiseClone();
			cd.Rules =  new List<Rule>(cd.Rules.ToArray());//MainClass.Tools.Clone(cd.Rules);

			return cd;

		}

		#endregion

		public Condition Clone()
		{
			return (Condition)this.MemberwiseClone();
		}

		public Condition()
		{
			Rules = new List<Rule>();
		}

		[XmlAttribute("id")]
		public int Id;
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("system")]
		public bool System;
		[XmlArray("rules")]
		[XmlArrayItem("rule")]
		public List<Rule> Rules;

	}

	public class Rule : ICloneable
	{

		public Rule()
		{
		}

		public Rule(int id,string name,string specific)
		{
			Id = id;
			Name = name;
			Specific = specific;
		}

		public Rule(int id,string name,string specific,int tag)
		{
			Id = id;
			Name = name;
			Specific = specific;
			Tag = tag;
		}

		public Rule(int id,string name,string specific,int width,int height)
		{
			Id = id;
			Name = name;
			Specific = specific;
			Width = width;
			Height= height;
		}

		#region ICloneable implementation
		object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

		#endregion

		[XmlAttribute("id")]
		public int Id;
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("specific")]
		public string Specific;

		[XmlAttribute("width")]
		public int Width;
		[XmlAttribute("height")]
		public int Height;

		[XmlAttribute("tag")]
		public int Tag;
	}

	public class ConditionRule
	{

		public ConditionRule()
		{
		}

		public ConditionRule(int conditionId,int ruleId)
		{
			ConditionId = conditionId;
			RuleId = ruleId;
		}

		[XmlAttribute("id")]
		public int ConditionId;
		[XmlAttribute("rule")]
		public int RuleId;

	}

	public class CombinePublish: ICloneable
	{
		[XmlAttribute("projectName")]
		public string ProjectName;
		[XmlAttribute("selected")]
		public bool IsSelected;

		object ICloneable.Clone()
		{
			CombinePublish cc = (CombinePublish)MemberwiseClone();
			cc.ProjectName = ProjectName;
			cc.IsSelected = IsSelected;
			cc.combineRule = new List<CombineCondition>(combineRule.ToArray()); //MainClass.Tools.Clone(combineRule);//new List<combineRule>(combineRule.ToArray());
			return cc;
		}

		public CombinePublish Clone()
		{

			CombinePublish cc = (CombinePublish)MemberwiseClone();
			cc.ProjectName = ProjectName;
			cc.IsSelected = IsSelected;
			cc.combineRule = new List<CombineCondition>(combineRule.ToArray());//CloneList(combineRule);
			return cc;
			//return (combineCondition)this.MemberwiseClone();

		}

		public List<CombineCondition> combineRule;

		public override string ToString()
		{
			string oiut = "";

			foreach (CombineCondition cr in combineRule)
				oiut = oiut + ">>" + cr.ToString();

			return oiut;
		}

	}

	public class CombineCondition: ICloneable
	{

		object ICloneable.Clone()
		{
			CombineCondition cr = (CombineCondition)MemberwiseClone();
			return cr;
		}
		[XmlAttribute("conditionName")]
		public string ConditionName;
		[XmlAttribute("conditionId")]
		public int ConditionId;
		[XmlAttribute("ruleId")]
		public int RuleId;
		[XmlAttribute("ruleName")]
		public string RuleName;

		public override string ToString()
		{
			return string.Format("{0}({1}):{2}({3})", ConditionName, ConditionId, RuleName, RuleId);
		}

	}
}

