using System;
using Gtk;
using System.Collections.Generic;

namespace Moscrif.IDE.Components
{
	public class EntryMenu : Gtk.HBox
	{

		Gtk.Menu popupCondition = new Gtk.Menu();

		Entry entryTxt = new Entry();
		GLib.SList rbGroup ;//= new GLib.SList(typeof(RadioMenuItem));
		RadioMenuItem g = new RadioMenuItem("g");

		public EntryMenu(List<string> menu1,string labelMenu1, List<string> menu2,string labelMenu2, string defValue): base (false, 6)
		{
			if (!String.IsNullOrEmpty(defValue)){
				LabelText = defValue;
				entryTxt.Text = defValue;
			}



			this.PackStart(entryTxt,true,true,0);

			Gdk.Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Gdk.Pixbuf(file);

				Gtk.Button btnClose = new Gtk.Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = MainClass.Languages.Translate("insert_condition_name");
				btnClose.Relief = Gtk.ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 19;

				popupCondition.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));

				btnClose.Clicked += delegate {
					popupCondition.Popup();
					//popupCondition.Popup(btnClose,null,new Gtk.MenuPositionFunc (GetPosition),3,Gtk.Global.CurrentEventTime);
				};

				this.PackEnd(btnClose,false,false,0);
			}

			rbGroup = new GLib.SList (typeof(RadioMenuItem));

			Gtk.MenuItem mi = new Gtk.MenuItem(labelMenu1);
			mi.Name = labelMenu1;
			mi.Sensitive = false;
			popupCondition.Add(mi);

			foreach(string m in menu1){
				AddMenuItem(popupCondition,m);
			}

			Gtk.MenuItem mi2 = new Gtk.MenuItem(labelMenu2);
			mi2.Name = labelMenu2;
			mi2.Sensitive = false;
			popupCondition.Add(mi2);

			//Gtk.Menu subPopup2 = new Gtk.Menu();
			foreach(string m in menu2){
				AddMenuItem(popupCondition,m);
			}

			//popupCondition.State = StateType.Insensitive;
			//popupCondition.
			popupCondition.ShowAll();
			this.ShowAll();
		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}

		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);
			Console.WriteLine("GetOrigin -->>> x->{0} ; y->{1}",x,y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			//Gtk.Requisition menu_req = menu.SizeRequest();

			push_in = true;
		}

		private void AddMenuItem(Menu miParent, string condition)
		{
			Gtk.RadioMenuItem mi =new Gtk.RadioMenuItem(g.Group,condition);
			//mi.Group = rbGroup;

			if(condition == labelText){
				mi.Active = true;
			}else mi.Active = false;

			mi.Name = condition;

			mi.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.RadioMenuItem)){
						entryTxt.Text =  (sender as Gtk.MenuItem).Name;
					}
				};
			miParent.Add(mi);

		}

		private string labelText;
		public string LabelText {
			get {
				return labelText  = entryTxt.Text;
			}
			set {

				labelText = value;
				entryTxt.Text = value;
			}
		}
	}
}

