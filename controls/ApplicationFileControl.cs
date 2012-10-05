using System;
using Moscrif.IDE.Workspace;
using Gtk;
using Moscrif.IDE.Iface.Entities;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace Moscrif.IDE.Controls
{

	public partial class ApplicationFileControl : Gtk.Bin
	{
		private Mode mode;
		private AppFile appFile;
		Button btnSave;
		Gtk.Menu popupLibs = new Gtk.Menu();
		int major=0;
		int minor=0;
		Gtk.Window parentWindow;
		private ListStore orientationModel = new ListStore(typeof(string), typeof(string));

		public ApplicationFileControl(AppFile appFile,Mode mode,Gtk.Window parent )
		{
			parentWindow = parent;
			this.Build();
			this.mode = mode;
			this.appFile = appFile;


			CellRendererText textRenderer = new CellRendererText();
			cbOrientation.PackStart(textRenderer, true);
			//cbOrientation.AddAttribute(textRenderer, "text", 0);


			//cbOrientation.AppendValues("Landscape Left");
			//cbOrientation.AppendValues("Landscape Right");

			cbOrientation.Model = orientationModel;

			FillControl();
		}


		private void FillControl(){

			btnSave = new Button();
			btnSave.Label = MainClass.Languages.Translate("save");
			btnSave.Clicked+= delegate(object sender, EventArgs e) {
				Save();
			};

			if(String.IsNullOrEmpty(appFile.Title)){
				cbOrientation.Active = 0;
			}

			//cbOrientation.AppendValues("Portrait");

			if ((MainClass.Settings.DisplayOrientations == null) || ((MainClass.Settings.DisplayOrientations.Count <1 )) ){
				MainClass.Settings.GenerateOrientations();
			}
			TreeIter ti = new TreeIter();

			foreach(SettingValue ds in MainClass.Settings.DisplayOrientations){
				if(ds.Value == appFile.Orientation){
					ti = orientationModel.AppendValues(ds.Display,ds.Value);
					cbOrientation.SetActiveIter(ti);
				} else  orientationModel.AppendValues(ds.Display,ds.Value);
			}
			if(cbOrientation.Active <0)
				cbOrientation.Active =0;

			entTitle.Text =appFile.Title;
			entHomepage.Text =appFile.Homepage;
			entDescription.Text =appFile.Description;
			entCopyright.Text =appFile.Copyright;
			entAuthor.Text =appFile.Author;
			//lblId2.LabelProp = appFile.Id;
			lblName2.LabelProp = appFile.Name;
			entUses.Text = appFile.Uses;

			if(!String.IsNullOrEmpty(appFile.Version)){
				string[] version = appFile.Version.Split('.');

				if (version.Length >=2){
					Int32.TryParse(version[0].Trim(),out major);
					Int32.TryParse(version[1].Trim(),out minor);
				} else {

					Int32.TryParse(version[0].Trim(),out major);
				}
			}

			entrVersionMajor.Text = major.ToString();
			entrVersionMinor.Text = minor.ToString();

			entrVersionMajor.Changed+= delegate(object sender, EventArgs e) {
				int mn = 0;
				if(!Int32.TryParse(entrVersionMajor.Text,out mn)){
					entrVersionMajor.Text = major.ToString();
				} else major = mn;

			};

			entrVersionMinor.Changed+= delegate(object sender, EventArgs e) {
				int mn = 0;
				if(!Int32.TryParse(entrVersionMinor.Text,out mn)){
					entrVersionMinor.Text = minor.ToString();
				} else minor = mn;

			};

			if (mode == ApplicationFileControl.Mode.Read){

				entTitle.IsEditable = false;
				entHomepage.IsEditable = false;
				entDescription.IsEditable = false;
				entCopyright.IsEditable = false;
				entAuthor.IsEditable = false;
				//lblId2.Visible = false;
				lblName2.Visible = false;
				entUses.IsEditable = false;
			}

			if (mode == Mode.Edit){
				table1.Attach(btnSave,0,1,8,9);
			}

			Gdk.Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Gdk.Pixbuf(file);

				Gtk.Button btnClose = new Gtk.Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = MainClass.Languages.Translate("insert_libs");
				btnClose.Relief = Gtk.ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 22;
				btnClose.Clicked += delegate {
					popupLibs.Popup();
				};
				//table1.Attach(btnClose,2,3,7,8, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);

				popupLibs = new Gtk.Menu();

				if ((MainClass.Settings.LibsDefine == null) || (MainClass.Settings.LibsDefine.Count<1)) MainClass.Settings.GenerateLibs();

				foreach (string lib in MainClass.Settings.LibsDefine) {
						AddMenuItem(lib);
					}

				popupLibs.ShowAll();
			}

			Gdk.Pixbuf default_pixbuf2 = null;
			string file2 = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-add.png");
			if (System.IO.File.Exists(file2)) {
				default_pixbuf2 = new Gdk.Pixbuf(file2);

				Gtk.Button btnAddMajor = new Gtk.Button(new Gtk.Image(default_pixbuf2));
				btnAddMajor.Relief = Gtk.ReliefStyle.None;
				btnAddMajor.CanFocus = false;
				btnAddMajor.WidthRequest = btnAddMajor.HeightRequest = 19;
				btnAddMajor.Clicked += delegate {
					major++;
					entrVersionMajor.Text = major.ToString();
				};
				tblVersion.Attach(btnAddMajor, 1, 2, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);


				Gtk.Button btnAddMinor = new Gtk.Button(new Gtk.Image(default_pixbuf2));
				btnAddMinor.Relief = Gtk.ReliefStyle.None;
				btnAddMinor.CanFocus = false;
				btnAddMinor.WidthRequest = btnAddMinor.HeightRequest = 19;
				btnAddMinor.Clicked += delegate {
					minor++;
					entrVersionMinor.Text = minor.ToString();
				};
				tblVersion.Attach(btnAddMinor, 4, 5, 0, 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			}

			if(mode==Mode.Create){
				entUses.Visible = false;
				Uses.Visible = false;
				btnManage.Visible = false;
				table1.Remove(entUses);
				table1.Remove(Uses);
				table1.Remove(btnManage);
			}
			entTitle.KeyReleaseEvent+= delegate(object o, KeyReleaseEventArgs args) {
				titleChange= true;	
			};

		}

		private void AddMenuItem(string lib)
		{
			Gtk.MenuItem mi = new Gtk.MenuItem(lib);
			mi.Name = lib;
			mi.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						string txt = (sender as Gtk.MenuItem).Name;

						int indx = entUses.Text.IndexOf(txt);

						if (indx <0)
							entUses.Text = String.Format("{0} {1}", entUses.Text, txt);
					}
				};
			popupLibs.Add(mi);

		}

		private bool titleChange= false;
		public bool TitleChange{
			get{ return titleChange;}
		}

		public void SetTitle(string title){
			entTitle.Text = title;
		}

		protected virtual void OnBtnSaveAppFileClicked (object sender, System.EventArgs e)
		{
			Save();
		}

		public AppFile AppFile{
			get{
				FillAppFile();
				return appFile;
			}
		}

		private void FillAppFile(){

			appFile.Title= entTitle.Text;
			appFile.Homepage= entHomepage.Text;
			appFile.Description=entDescription.Text;
			appFile.Copyright=entCopyright.Text;
			appFile.Author=entAuthor.Text;
			appFile.Uses = entUses.Text;
			appFile.Version = String.Format("{0}.{1}",major,minor);

			TreeIter ti = new TreeIter();
			cbOrientation.GetActiveIter(out ti);
			string orient = orientationModel.GetValue(ti,1).ToString();

			appFile.Orientation =orient;
		}

		public bool Save(){

			//appFile.Orientation = cbOrientation.ActiveText;
			try{
				FillAppFile();
				appFile.Save();
			} catch(Exception ex){
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", appFile.ApplicationFile), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;

			}

			return true;
		}
		
		protected virtual void OnBtnManageClicked (object sender, System.EventArgs e)
		{
			LibsManagerDialog lmd = new LibsManagerDialog(entUses.Text,parentWindow);
			int result = lmd.Run();

			if (result == (int)ResponseType.Ok) {
				string libs =lmd.LibsString;
				if(!String.IsNullOrEmpty(libs))
					entUses.Text = libs;

				MainClass.MainWindow.FrameworkTree.LoadLibs(MainClass.Workspace.RootDirectory);

			}
			lmd.Destroy();
		}
		
		
		public enum Mode
		{
			Read,
			Edit,
			EditNoSaveButton,
			Create
		}
	}
}

