using System;
using Moscrif.IDE.Controls;

namespace Moscrif.IDE.Components
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class LoginLogoutControl : Gtk.Bin
	{
		public LoginLogoutControl()
		{
			this.Build();
		}

		protected virtual void OnBtnActionClicked (object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("http://moscrif.com/buy");
		}

		protected virtual void OnBtnStateClicked (object sender, System.EventArgs e)
		{
			LoginLogout();
		}


		public void LoginLogout(){

			if (MainClass.User == null){
				LoginDialog ld = new LoginDialog(null);
				ld.Run();
				ld.Destroy();
			} else{
				MainClass.User = null;
			}
			SetLogin();
		}

		public bool Relogin(){
			bool respons = false;

			LoginDialog ld = new LoginDialog(null);
			int res = ld.Run();
			if (res == (int)Gtk.ResponseType.Ok)
				respons = true;
			ld.Destroy();

			return respons;
		}

		public void SetLogin(){

			if (MainClass.User!= null){
				lblName.LabelProp = MainClass.User.Login;
				btnState.Label = MainClass.Languages.Translate("log_out");
			}else{				;
				lblName.LabelProp = MainClass.Languages.Translate("open_source");
				btnState.Label = MainClass.Languages.Translate("login");
			}

		}
		
		
	}
}

