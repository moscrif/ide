using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Iface.Entities;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Controls
{
	public partial class SettingPlatformDialog : Gtk.Dialog
	{

		Gtk.Menu popupColor = new Gtk.Menu();
		//private bool isRequired = false;

		const string NOTHING ="Nothing";
		const string WIN ="Visual Studio";
		const string MACOSX ="XCode";
		const string JAVA ="Eclipse";
		const string VisualC ="Visual C++";

		public SettingPlatformDialog(bool isRequired)
		{
			this.Build();
			//this.isRequired = isRequired;
			if(isRequired){
				button34.Visible=false;
			}

			if(MainClass.Settings.BackgroundColor==null){
				MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(218,218,218);
				/*if(MainClass.Platform.IsMac)
					MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(218,218,218);
				else
					MainClass.Settings.BackgroundColor = new Moscrif.IDE.Settings.Settings.BackgroundColors(224,41,47);
					*/
			}

			if(!isRequired){
				cbKeyBinding.AppendText(NOTHING);
				cbKeyBinding.Active = 0;
			}

			cbKeyBinding.AppendText(WIN);
			cbKeyBinding.AppendText(MACOSX);
			cbKeyBinding.AppendText(JAVA);
			cbKeyBinding.AppendText(VisualC);

			if(isRequired){
				if(MainClass.Platform.IsMac){
					cbKeyBinding.Active = 1;
				} else {
					cbKeyBinding.Active = 0;
				}
			}

			Gdk.Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Gdk.Pixbuf(file);

				popupColor = new Gtk.Menu();
				CreateMenu();

				Gtk.Button btnClose = new Gtk.Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = MainClass.Languages.Translate("select_color");
				btnClose.Relief = Gtk.ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 22;

				popupColor.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));
				btnClose.Clicked += delegate {
					popupColor.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
				};
				table1.Attach(btnClose,2,3,0,1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
				table1.ShowAll();
				popupColor.ShowAll();
			}

			cbBackground.Color = new Gdk.Color(MainClass.Settings.BackgroundColor.Red,
						MainClass.Settings.BackgroundColor.Green,MainClass.Settings.BackgroundColor.Blue);

		}

		private void CreateMenu(){

			Gtk.MenuItem miRed = new Gtk.MenuItem("Red");
			miRed.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(224, 41, 47);
					}
				};
			popupColor.Add(miRed);
			Gtk.MenuItem miBlue = new Gtk.MenuItem("Blue");
			miBlue.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(164, 192,222);
					}
				};
			popupColor.Add(miBlue);
			Gtk.MenuItem miUbuntu = new Gtk.MenuItem("Ubuntu");
			miUbuntu.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(240, 119,70);
					}
				};
			popupColor.Add(miUbuntu);

			Gtk.MenuItem miOsx = new Gtk.MenuItem("Mac");
			miOsx.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						cbBackground.Color =  new Gdk.Color(218,218 ,218);
					}
				};
			popupColor.Add(miOsx);
		}


		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}
		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);
			//Console.WriteLine("GetOrigin -->>> x->{0} ; y->{1}",x,y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{

			MainClass.Settings.BackgroundColor.Red = (byte)cbBackground.Color.Red;
			MainClass.Settings.BackgroundColor.Green= (byte)cbBackground.Color.Green;
			MainClass.Settings.BackgroundColor.Blue= (byte)cbBackground.Color.Blue;

			string active = cbKeyBinding.ActiveText;
			string file = System.IO.Path.Combine(MainClass.Paths.SettingDir, "keybinding");

			switch (active) {
			case WIN:{
				KeyBindings.CreateKeyBindingsWin(file);
				break;
			}
			case MACOSX:{
				KeyBindings.CreateKeyBindingsMac(file);
				break;
			}
			case JAVA:{
				KeyBindings.CreateKeyBindingsJava(file);
				break;
			}
			case VisualC:{
				KeyBindings.CreateKeyBindingsVisualC(file);
				break;
			}
			default:
				break;
			}

			MainClass.Settings.SaveSettings();

		}
	}
}

