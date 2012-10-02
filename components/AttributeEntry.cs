using System;
using Gtk;
using Moscrif.IDE.FileTemplates;
using System.Text.RegularExpressions;

namespace Moscrif.IDE.Components
{
	public class AttributeEntry : HBox
	{
		public AttributeEntry()
		{
		}

		private FileTemplate.Attribute attribute;
		public AttributeEntry(FileTemplate.Attribute attribute)
		{
			this.attribute = attribute;

			Label lblApp = new Label(attribute.Name.Replace("_","__")+": ");
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			Entry entr = new Entry();
			entr.Name = "entr";
			if (attribute.Value != null)
				entr.Text = attribute.Value.ToString();

			entr.Changed+= delegate(object sender, EventArgs e) {

				if (!String.IsNullOrEmpty(attribute.ValidateExpr)){
					Regex regex = new Regex(attribute.ValidateExpr, RegexOptions.Compiled);
					if (regex.IsMatch(entr.Text)){
						this.attribute.Value = (object)entr.Text;
					} else {
						if (attribute.Value != null)
						entr.Text = attribute.Value.ToString();
					}

				} else
					this.attribute.Value = (object)entr.Text;
			};

			this.PackStart(lblApp,false,false,2);
			this.PackEnd(entr,true,true,2);
		}


	}
}

