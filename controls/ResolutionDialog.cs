using System;
using Gtk;
using System.IO;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;
using System.Collections.Generic;

namespace Moscrif.IDE.Controls
{
	public partial class ResolutionDialog : Gtk.Dialog
	{
		public ResolutionDialog(Gtk.Window parentWindow)
		{
			this.TransientFor = parentWindow;
			resolution = new Rule();
			resolution.Id=0;
			InitializeDialog();
		}

		public ResolutionDialog(Rule resolution,Gtk.Window parentWindow)
		{
			this.TransientFor = parentWindow;
			this.resolution =resolution;
			InitializeDialog();
			entrName.Text = resolution.Name;
			entrSpecific.Text = resolution.Specific;

			sbWidth.Value= resolution.Width;
			sbHeight.Value= resolution.Height;
			chbCreateFile.Active = false;
			chbCreateFile.Visible= false;
		}

		private void InitializeDialog(){
			this.Title=MainClass.Languages.Translate("resolution_title");
			this.Build();
		}

		private Rule resolution;
		public Rule Resolution{
			get{
				return resolution;
			}
			set{
				resolution = value;
			}
		}

		public bool CreateFile{
			get{
				return chbCreateFile.Active;
			}
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string message = "";
			if(String.IsNullOrEmpty(entrName.Text)){
				message = MainClass.Languages.Translate("new_resolution_error_f1");
			}

			if(String.IsNullOrEmpty(entrSpecific.Text)){
				message = MainClass.Languages.Translate("new_resolution_error_f2");
			}

			if((sbWidth.Value<1)|| (sbHeight.Value<1) ){
				message = MainClass.Languages.Translate("new_resolution_error_f3");
			}

			if(!String.IsNullOrEmpty(message)){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, message, "", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}
			resolution.Name=entrName.Text;
			resolution.Specific=entrSpecific.Text;
			resolution.Width=(int)sbWidth.Value;
			resolution.Height=(int)sbHeight.Value;

			this.Respond(ResponseType.Ok);
		}
	}
}

