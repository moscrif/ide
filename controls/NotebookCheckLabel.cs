using System;
using Gdk;
using Gtk;
using Moscrif.IDE.Components;
using Moscrif.IDE.Editors;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Controls
{
	public class NotebookCheckLabel : HBox
	{
		public NotebookCheckLabel(string label,DeviceType dt)
		{
			Label lbl = new Label(label.Replace("_","__"));
			
			//Pixbuf default_pixbuf = null;
		//	string file = System.IO.Path.Combine(MainClass.Tools.ResDir, "stock-close.png");
			//if (System.IO.File.Exists(file)) {
				//default_pixbuf = new Pixbuf(file);

				CheckButton chbButton = new CheckButton();

				chbButton.TooltipText = MainClass.Languages.Translate("check_for_select");
				chbButton.CanFocus = false;

				chbButton.Toggled += delegate(object sender, EventArgs e) {
					//Console.WriteLine("chbButton.Toggled");
				};

			this.PackStart(chbButton, false, false, 0);
			this.PackEnd(lbl, false, false, 0);
			//}
			
			this.ShowAll();
		}


		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
		}
	}
}

