using System;
using Gtk;
using Moscrif.IDE.Iface;

namespace Moscrif.IDE.Controls
{
	public partial class PermissionEditor : Gtk.Dialog
	{
		Gtk.Window parentWindow;

		public PermissionEditor()
		{
			this.Build();
		}

		public PermissionEditor(string permission,Gtk.Window parent)
		{
			parentWindow =parent;
			this.TransientFor =parentWindow;
			this.Build();

			Pango.FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			tvPermission.ModifyFont(customFont);

			AndroidPermission ap = new AndroidPermission();
			foreach(string str in ap.Sections()){
				//Console.WriteLine(str);
				Button btn = new Button();
				btn.Name = str;
				btn.Label = str;
				btn.HeightRequest = 25;
				btn.Clicked+= delegate(object sender, EventArgs e) {
					string[] values = ap.ValuesInSection(btn.Name);
					string tmp = String.Join("\n",values);

					if(!String.IsNullOrEmpty(tvPermission.Buffer.Text))
						tvPermission.Buffer.Text = tvPermission.Buffer.Text+"\n";

					tvPermission.Buffer.Text = tvPermission.Buffer.Text+ tmp;
				};

				vbButton.PackEnd(btn,true,true,0);
			}
			vbButton.ShowAll();

			if(!string.IsNullOrEmpty(permission)){
				tvPermission.Buffer.Text = permission;
			} else {
				// insert default
			}
		}

		public string Permission {
			get{
				return tvPermission.Buffer.Text;
			}
		}

		protected void OnTvPermissionPasteClipboard (object sender, System.EventArgs e)
		{
			//throw new System.NotImplementedException ();
		}
	}
}

