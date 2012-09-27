using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Settings
{
	internal class GlobalOptionsPanel : OptionsPanel
	{
		GlobalOptionsWidget2 widget;

		public override Widget CreatePanelWidget ()
		{
			return widget = new  GlobalOptionsWidget2 (ParentDialog);
		}

		public override void ApplyChanges ()
		{
			widget.Store ();
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
		}

		public override void ShowPanel()
		{
		}

		public override bool ValidateChanges ()
		{
			return widget.Validate ();
		}

		public override string Label {
			get { return MainClass.Languages.Translate("general_prferencies_ide"); }
		}

		public override string Icon {
			get { return "preferences.png"; }
		}
	}


	internal partial class  GlobalOptionsWidget2 : Gtk.Bin
	{
		ListStore storeIF = new ListStore(typeof(string),typeof(bool),typeof(bool), typeof(IgnoreFolder));
		private List<IgnoreFolder> ignoreFiles;
		Gtk.Menu popupColor = new Gtk.Menu();
		Gtk.Window parentWindow;

		FavoriteEntry feLib ;
		FavoriteEntry fePublishTool ;
		FavoriteEntry feEmulator ;

		public GlobalOptionsWidget2(Gtk.Window parent)
		{
			parentWindow = parent;

			this.Build();
			feLib = new FavoriteEntry(NavigationBar.NavigationType.libs);
			feLib.IsFolder = true;
			fePublishTool = new FavoriteEntry(NavigationBar.NavigationType.publish);
			fePublishTool.IsFolder = true;
			feEmulator = new FavoriteEntry(NavigationBar.NavigationType.emulator);
			feEmulator.IsFolder = true;

			if (!String.IsNullOrEmpty(MainClass.Settings.EmulatorDirectory) ){
				feEmulator.Path= MainClass.Settings.EmulatorDirectory;
			}

			if (!String.IsNullOrEmpty(MainClass.Settings.LibDirectory) ){
				feLib.Path= MainClass.Settings.LibDirectory;
			}

			if (!String.IsNullOrEmpty(MainClass.Settings.PublishDirectory) ){
				fePublishTool.Path = MainClass.Settings.PublishDirectory;
			}

			table1.Attach(fePublishTool,1,2,0,1,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);
			table1.Attach(feLib,1,2,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);
			table1.Attach(feEmulator,1,2,2,3,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);

			fontbutton1.FontName = MainClass.Settings.ConsoleTaskFont;

			chbAutoselectProject.Active = MainClass.Settings.AutoSelectProject;
			chbOpenLastOpenedW.Active= MainClass.Settings.OpenLastOpenedWorkspace;
			chbShowUnsupportDevic.Active =MainClass.Settings.ShowUnsupportedDevices;
			chbShowDebugDevic.Active =MainClass.Settings.ShowDebugDevices;

			
			cbBackground.UseAlpha = false;

			//ushort red =MainClass.Settings.BackgroundColor.Red;
			//ushort green =MainClass.Settings.BackgroundColor.Green;
			//ushort blue =MainClass.Settings.BackgroundColor.Blue;

			cbBackground.Color = new Gdk.Color(MainClass.Settings.BackgroundColor.Red,
				MainClass.Settings.BackgroundColor.Green,MainClass.Settings.BackgroundColor.Blue);

			/*if(MainClass.Settings.IgnoresFolders == null || MainClass.Settings.IgnoresFolders.Count<1){
				MainClass.Settings.GenerateIgnoreFolder();
			}*/

			ignoreFiles = new List<IgnoreFolder>( MainClass.Settings.IgnoresFolders.ToArray());

			tvIgnoreFile.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);

			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable = true;
			crt.Toggled += delegate(object o, ToggledArgs args) {
					TreeIter iter;
					if (storeIF.GetIter (out iter, new TreePath(args.Path))) {
						bool old = (bool) storeIF.GetValue(iter,1);
					 	IgnoreFolder iFol = (IgnoreFolder) storeIF.GetValue(iter,3);
						iFol.IsForIde =!old;

						storeIF.SetValue(iter,1,!old);
				}
			};

			tvIgnoreFile.AppendColumn(MainClass.Languages.Translate("ignore_for_ide"), crt , "active", 1);

			CellRendererToggle crt2 = new CellRendererToggle();
			crt2.Activatable = true;
			crt2.Toggled += delegate(object o, ToggledArgs args) {
					TreeIter iter;
					if (storeIF.GetIter (out iter, new TreePath(args.Path))) {
						bool old = (bool) storeIF.GetValue(iter,2);
					 	IgnoreFolder iFol = (IgnoreFolder) storeIF.GetValue(iter,3);
						//CombinePublish cp =(CombinePublish) fontListStore.GetValue(iter,2);
						//cp.IsSelected = !old;
						iFol.IsForPublish =!old;

						storeIF.SetValue(iter,2,!old);
				}
			};

			tvIgnoreFile.AppendColumn(MainClass.Languages.Translate("ignore_for_Pub"), crt2 , "active", 2);
			tvIgnoreFile.Model = storeIF;

			foreach (IgnoreFolder ignoref in MainClass.Settings.IgnoresFolders){

				Gtk.TreeIter tir = storeIF.AppendValues(ignoref.Folder,ignoref.IsForIde,ignoref.IsForPublish,ignoref);
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
				btnClose.WidthRequest = btnClose.HeightRequest = 19;

				popupColor.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));
				btnClose.Clicked += delegate {
					popupColor.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
				};
				table1.Attach(btnClose,2,3,3,4, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);

				popupColor.ShowAll();
			}
		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}
		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);
			//Console.WriteLine("GetOrigin -->>> x->{0} ; y->{1}",x,y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		public bool Validate(){

			//return true;

			if ((String.IsNullOrEmpty(feLib.Path) ) ||
			(String.IsNullOrEmpty(fePublishTool.Path) ) ||
			(String.IsNullOrEmpty(feEmulator.Path) )
			 ){
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", MainClass.Languages.Translate("please_set_all_folder"), MessageType.Error);
				ms.ShowDialog();
				return false;
			}


			return true;

		}

		public void Store ()
		{
			MainClass.Settings.EmulatorDirectory =feEmulator.Path;
			MainClass.Settings.LibDirectory =feLib.Path;
			MainClass.Settings.PublishDirectory = fePublishTool.Path;
			MainClass.Settings.AutoSelectProject =chbAutoselectProject.Active;
			MainClass.Settings.OpenLastOpenedWorkspace = chbOpenLastOpenedW.Active;
			MainClass.Settings.ShowUnsupportedDevices = chbShowUnsupportDevic.Active;
			MainClass.Settings.ShowDebugDevices = chbShowDebugDevic.Active;

			MainClass.Settings.ConsoleTaskFont = fontbutton1.FontName;

			//byte red =  (byte)cbBackground.Color.Red;
			//byte green =(byte)cbBackground.Color.Green;
			//byte blue =(byte)cbBackground.Color.Blue;

			MainClass.Settings.BackgroundColor.Red = (byte)cbBackground.Color.Red;
			MainClass.Settings.BackgroundColor.Green= (byte)cbBackground.Color.Green;
			MainClass.Settings.BackgroundColor.Blue= (byte)cbBackground.Color.Blue;

			MainClass.Settings.IgnoresFolders.Clear();
			MainClass.Settings.IgnoresFolders = new List<IgnoreFolder>(ignoreFiles.ToArray());
		}

		protected virtual void OnBtnAddIFClicked (object sender, System.EventArgs e)
		{
			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("name"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){

					IgnoreFolder ifol = new IgnoreFolder(ed.TextEntry,true,true);
					storeIF.AppendValues(ed.TextEntry,true,true,ifol);
					ignoreFiles.Add(ifol);
				}
			}
			ed.Destroy();
		}
		
		protected virtual void OnBtnEditIFClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvIgnoreFile.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			IgnoreFolder iFol = (IgnoreFolder)tvIgnoreFile.Model.GetValue(ti, 3);
			if (String.IsNullOrEmpty(iFol.Folder) ) return;

			EntryDialog ed = new EntryDialog(iFol.Folder,MainClass.Languages.Translate("new_name"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				if (!String.IsNullOrEmpty(ed.TextEntry) ){

					iFol.Folder =ed.TextEntry;
					storeIF.SetValues(ti,ed.TextEntry,iFol.IsForIde,iFol.IsForPublish,iFol);
				}
			}
			ed.Destroy();
		}
		
		protected virtual void OnBtnDeleteIFClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvIgnoreFile.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			IgnoreFolder iFol = (IgnoreFolder)tvIgnoreFile.Model.GetValue(ti, 3);
			if (iFol == null ) return;

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo,  MainClass.Languages.Translate("delete_resolution", iFol.Folder), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			ignoreFiles.Remove(iFol);
			storeIF.Remove(ref ti);

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

		protected void OnButton8Clicked (object sender, System.EventArgs e)
		{

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("question_default_ignorelist"), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
	
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			MainClass.Settings.GenerateIgnoreFolder();
		}
	}
}

