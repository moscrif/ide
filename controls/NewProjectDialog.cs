using System;
using Gtk;
using System.IO;
using System.Collections.Generic;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Components;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Execution;
using System.Text.RegularExpressions;

namespace Moscrif.IDE.Controls
{
	public partial class NewProjectDialog : Gtk.Dialog
	{

		public NewProjectDialog() 
		{
			this.TransientFor = MainClass.MainWindow;			
			this.Build();
			this.Title="New Project"; 
			skinthemecontrol1.SetLabelWidth(50);
		}

		public string ProjectName {
			get {
				return this.entry3.Text;
			}
		}

		public string Skin {
			get {
				return this.skinthemecontrol1.GetSkin();
			}
		}

		public string Theme {
			get {
				return this.skinthemecontrol1.GetTheme();
			}
		}

		protected void OnEntry3KeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return){
				if(!CheckForm()){
					return;
				}			
				this.Respond(ResponseType.Ok);
			}
		}
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{

			if(!CheckForm()){
				return;
			}

			this.Respond(ResponseType.Ok);

		}


		private bool CheckForm(){

			if(string.IsNullOrEmpty(entry3.Text) ){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_project_name"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				return false;
			}

			if(this.entry3.Text.Contains(" ")){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_proj"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}
			return true;

		}

	}
}

