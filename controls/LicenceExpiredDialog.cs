using System;
namespace Moscrif.IDE.Controls
{
	public partial class LicenceExpiredDialog : Gtk.Dialog
	{
		protected virtual void OnBtnBuyNowClicked (object sender, System.EventArgs e)
		{
			string url = "http://moscrif.com/download?t={0}";
			if (MainClass.User!=null && (!String.IsNullOrEmpty(MainClass.User.Token))) {
				url = string.Format(url,MainClass.User.Token);

			}

			System.Diagnostics.Process.Start(url);

			this.Respond( Gtk.ResponseType.Ok );
		}

		
		public LicenceExpiredDialog(string message)
		{
			this.Build();

			Gtk.Image image1 = new Gtk.Image(MainClass.Tools.GetIconFromStock("logo74.png", Gtk.IconSize.Dialog));


			image1.SetPadding(2,2);
			hbox1.PackStart(image1, false, false, 0);


			Gtk.Label lblMessage = new Gtk.Label("<b>"+message+"</b>");
			lblMessage.UseMarkup = true;
			hbox1.PackEnd(lblMessage, true ,true, 0);
			ShowAll ();
	
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();

		}

		public int ShowDialog(){
			int result =   this.Run();
			this.Destroy();
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
			return result;
		}

	}
}

