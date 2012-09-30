using System;
using Gtk;
using Moscrif.IDE.FileTemplates;

namespace Moscrif.IDE.Components
{
	public class AttributeCheckBox : HBox
	{
		public AttributeCheckBox()
		{
		}

		private FileTemplate.Attribute attribute;
		public AttributeCheckBox(FileTemplate.Attribute attribute)
		{
			this.attribute = attribute;


			Label lblApp = new Label(attribute.Name.Replace("_","__")+": ");
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 100;
			CheckButton chb = new CheckButton();
			chb.Label = "";

			bool defValue = false;
			if(attribute.Value!= null){
				Boolean.TryParse(attribute.Value.ToString(), out defValue);
			}
			chb.Active = defValue;

			chb.Toggled += delegate(object sender, EventArgs e) {
				this.attribute.Value = (object)chb.Active;
			};


			this.PackStart(lblApp,false,false,2);
			this.PackEnd(chb,true,true,2);
		}

	}
}

