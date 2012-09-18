using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Moscrif.IDE.Iface.Entities
{
	public class KeyBindingSection : ICloneable
	{
		#region ICloneable implementation
		object ICloneable.Clone()
		{
			KeyBindingSection ls = (KeyBindingSection)MemberwiseClone();
			ls.KeyBinding =  new List<KeyBinding>(ls.KeyBinding.ToArray());

			return ls;

		}

		#endregion

		public KeyBindingSection Clone()
		{
			return (KeyBindingSection)this.MemberwiseClone();
		}


		public KeyBindingSection()
		{
		}

		public KeyBindingSection(string name)
		{
			Name =name;
			KeyBinding = new List<KeyBinding>();
		}


		[XmlAttribute("name")]
		public string Name{
			get;
			set;
		}

		[XmlArrayAttribute("keybindings")]
		[XmlArrayItem("keybinding")]
		public List<KeyBinding> KeyBinding;

	}
}

