using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Gtk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Components;
using Moscrif.IDE.Tool;
//using System.Management;

namespace Moscrif.IDE.Controls
{
	public class FileExplorer : VBox
	{

		const int COL_PATH = 0;
		const int COL_DISPLAY_NAME = 1;
		const int COL_PIXBUF = 2;
		const int COL_IS_DIRECTORY = 3;

		DirectoryInfo parent = new DirectoryInfo ("/");
		Gdk.Pixbuf dirIcon, fileIcon, upIcon;
		ListStore store;
		ToolButton upButton;
		ToolButton refreshButton;
		NavigationBar navigBar;
		IconView iconView;

		private string selectedItem;
		private bool isDir;
		private Menu popupMenu;

		public FileExplorer()
		{
			navigBar = new NavigationBar(NavigationBar.NavigationType.favorites);

			navigBar.OnChangePath+= NavigateBarChange;

			Toolbar toolbar = new Toolbar ();
			this.PackStart (toolbar, false, false, 0);

			refreshButton = new ToolButton ("refresh.png");//Stock.Home);
			refreshButton.IsImportant = true;
			refreshButton.Label = MainClass.Languages.Translate("menu_refresh");
			refreshButton.BorderWidth = 1;
			refreshButton.Clicked+= delegate {
				FillStore (true);
			};
			toolbar.Insert (refreshButton, -1);

			upButton = new ToolButton  ("go-up.png");//Stock.GoUp
			upButton.Sensitive = false;
			upButton.Label ="Up";
			upButton.BorderWidth = 1;
			toolbar.Insert (upButton, -1);

			Gtk.Menu menu = new Gtk.Menu ();
			MenuItem mi = new MenuItem ("Workspace");
			mi.Activated += OnWorkspaceClicked;
			menu.Insert (mi, -1);
			mi = new MenuItem ("Project");
			mi.Activated += OnProjectClicked;
			menu.Insert (mi, -1);
			menu.ShowAll ();
			mi = new MenuItem ("User home folder");
			mi.Activated += OnHomeClicked;
			menu.Insert (mi, -1);
			menu.ShowAll ();
			mi = new MenuItem ("Output folder");
			mi.Activated += OnOutputClicked;
			menu.Insert (mi, -1);
			menu.ShowAll ();
			mi = new MenuItem ("Disk root");
			mi.Activated += OnOutputClicked;
			menu.Insert (mi, -1);
			menu.ShowAll ();

			if(MainClass.Platform.IsWindows){

				Toolbar toolbarDisk = new Toolbar ();

				SeparatorMenuItem smi = new SeparatorMenuItem();
				menu.Insert (smi, -1);

				string[] drives = Environment.GetLogicalDrives();
				 foreach(string strDrive in drives)
				 {
					mi = new MenuItem (strDrive);
					mi.TooltipText = strDrive;
					mi.Activated += delegate(object sender, EventArgs e)
					{
						string drive = (sender as  MenuItem).TooltipText;
						parent = new DirectoryInfo(drive);
						FillStore (true);
						upButton.Sensitive = false;
					};
					menu.Insert (mi, -1);
					menu.ShowAll ();

				};
			}


			MenusToolButton gotoButton= new MenusToolButton(menu,"workspace.png");
			gotoButton.IsImportant = true;
			gotoButton.Label = "Go To";
			toolbar.Insert (gotoButton, -1);


			Gtk.Menu menuAdd = new Gtk.Menu ();
			mi = new MenuItem (MainClass.Languages.Translate("menu_create_file"));
			mi.Activated += OnCreateFileClicked;
			menuAdd.Insert (mi, -1);
			mi = new MenuItem (MainClass.Languages.Translate("menu_create_dir"));
			mi.Activated += OnCreateDirectoryClicked;
			menuAdd.Insert (mi, -1);
			menuAdd.ShowAll ();

			MenusToolButton mtbCreate= new MenusToolButton(menuAdd,"file-new.png");
			mtbCreate.IsImportant = true;
			mtbCreate.Label = "Create";
			toolbar.Insert (mtbCreate, -1);

			fileIcon = GetIcon ("file.png");//Stock.File);
			dirIcon = GetIcon ("folder.png");//Stock.Open);
			upIcon = GetIcon ("go-up.png");


			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			this.PackStart (sw, true, true, 0);

			// Create the store and fill it with the contents of '/'
			store = CreateStore ();

			iconView = new IconView (store);
			iconView.ButtonReleaseEvent+= OnButtonRelease;

			iconView.SelectionMode = SelectionMode.Single;

			iconView.Columns = 1;
			iconView.Orientation = Orientation.Horizontal;

			upButton.Clicked += new EventHandler (OnUpClicked);

			iconView.TextColumn = COL_DISPLAY_NAME;
			iconView.PixbufColumn = COL_PIXBUF;
			iconView.TooltipColumn = COL_PATH;
			iconView.RowSpacing = -6;
			iconView.Spacing = -1;
			iconView.ColumnSpacing=0;
			iconView.Margin=-5;

			iconView.ItemActivated += new ItemActivatedHandler (OnItemActivated);

			sw.Add (iconView);

			iconView.SelectionChanged+= delegate(object sender, EventArgs e) {
				Gtk.TreePath[] selRow = iconView.SelectedItems;
				if(selRow.Length<1) return;

				Gtk.TreePath tp = selRow[0];
				TreeIter ti = new TreeIter();
				store.GetIter(out ti,tp);

				if(tp.Equals(TreeIter.Zero))return;

				string name = store.GetValue(ti, 1).ToString();
				if(name == ".."){
					selectedItem = null;
				} else {
					selectedItem = store.GetValue(ti, 0).ToString();
					isDir = (bool)store.GetValue(ti, 3);
				}
			};
			this.PackEnd (navigBar, false, false, 0);
		}

		[GLib.ConnectBefore]
		void OnButtonRelease(object obj, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3) {

				this.popupMenu = new Menu();


				Gtk.TreePath[] selRow = iconView.SelectedItems;
				if(selRow.Length<1) {
					this.popupMenu = GenerateMenu(false);
				} else {
					Gtk.TreePath tp = selRow[0];
					TreeIter ti = new TreeIter();
					store.GetIter(out ti,tp);

					if(tp.Equals(TreeIter.Zero))return;


					selectedItem = store.GetValue(ti, 0).ToString();
					string  name = store.GetValue(ti, 1).ToString();

					isDir = (bool)store.GetValue(ti, 3);

					if(name == ".."){ // Up select
						selectedItem = null;
						this.popupMenu =GenerateMenu(false);
					} else {
						this.popupMenu = GenerateMenu(true);
					}
				}

				this.popupMenu.ShowAll();
				this.popupMenu.Popup();
			}
		}

		private Menu GenerateMenu(bool isItem){
		        Menu popupMenu = new Menu();

			ImageMenuItem refreshItem = new ImageMenuItem (MainClass.Languages.Translate("menu_refresh") );
			refreshItem.Activated += delegate(object sender, EventArgs e) {
				FillStore (true);
			};
			refreshItem.Image =new Gtk.Image("refresh.png",IconSize.Menu);
			popupMenu.Append(refreshItem);
			SeparatorMenuItem smi2 = new SeparatorMenuItem();
			popupMenu.Append(smi2);

			if(isItem){
				ImageMenuItem openItem = new ImageMenuItem(MainClass.Languages.Translate("menu_open") );
				openItem.Activated += delegate(object sender, EventArgs e) {
					if(!String.IsNullOrEmpty(selectedItem))
						OpenFile(selectedItem);
				};
				popupMenu.Append(openItem);
			}
			Menu addMenu = new Menu();

			MenuItem mi = new MenuItem("Add");

			ImageMenuItem newFile = new ImageMenuItem (MainClass.Languages.Translate("menu_create_file"));
			newFile.Activated += OnCreateFileClicked;
			addMenu.Append(newFile);

			ImageMenuItem newFolder = new ImageMenuItem (MainClass.Languages.Translate("menu_create_dir"));
			newFolder.Activated += OnCreateDirectoryClicked;
			addMenu.Append(newFolder);

			ImageMenuItem addFile = new ImageMenuItem (MainClass.Languages.Translate("menu_add_file"));
			addFile.Activated += OnAddFileClick;
			addMenu.Append(addFile);

			ImageMenuItem addFolder = new ImageMenuItem (MainClass.Languages.Translate("menu_add_dir"));
			addFolder.Activated += OnAddFolderClick;
			addMenu.Append(addFolder);

			mi.Submenu = addMenu;
			popupMenu.Append(mi);

			if(isItem){
				SeparatorMenuItem smi = new SeparatorMenuItem();
				popupMenu.Append(smi);

				ImageMenuItem deleteItem = new ImageMenuItem (MainClass.Languages.Translate("menu_delete"));
				deleteItem.Image =new Gtk.Image("delete.png",IconSize.Menu);
				deleteItem.Activated += OnDeleteItemClick;
				popupMenu.Append(deleteItem);

				ImageMenuItem renameItem = new ImageMenuItem (MainClass.Languages.Translate("menu_rename"));
				renameItem.Image =new Gtk.Image("rename.png",IconSize.Menu);
				renameItem.Activated += OnRenameItemClick;
				popupMenu.Append(renameItem);
			}


			return popupMenu;

		}

		void OnAddFolderClick (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog(MainClass.Languages.Translate("chose_dir_to_copy"), 
			                                                     MainClass.MainWindow,
			                                                     FileChooserAction.SelectFolder, 
			                                                     "Cancel", ResponseType.Cancel, 
			                                                     "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) 
			{
				string dirName = System.IO.Path.GetFileName(fc.Filename);
				string newPath = "";

				newPath = System.IO.Path.Combine(parent.FullName, dirName);

				if (System.IO.File.Exists(newPath))
					newPath = System.IO.Path.Combine(parent.FullName, "Copy_" + dirName);

				try {

					MainClass.Tools.CopyDirectory(fc.Filename, newPath, true, true);
				} catch(Exception ex)  {
					MessageDialogs md = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_copy",fc.Filename),ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
				}
			}
			fc.Destroy();
			FillStore (true);
		}

		void OnAddFileClick (object sender, EventArgs e)
		{
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog( MainClass.Languages.Translate("chose_file_to_copy"), null, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fc.Run() == (int)ResponseType.Accept) {
				string filename = System.IO.Path.GetFileName(fc.Filename);
				string newFile = "";

				newFile = System.IO.Path.Combine(parent.FullName, filename);
				if (System.IO.File.Exists(newFile))
					newFile = System.IO.Path.Combine(parent.FullName, "Copy_" + filename);

				try {
					System.IO.File.Copy(fc.Filename, newFile);
				} catch (Exception ex) {
					MessageDialogs md = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_copy",fc.Filename),ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
				}
			}
			fc.Destroy();
			FillStore (true);

		}

		void OnRenameItemClick (object sender, EventArgs e)
		{
			MessageDialogs md;
			string message ="" ;

			if (isDir){

				if ((String.IsNullOrEmpty(selectedItem)) || !Directory.Exists(selectedItem)){
					md = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("file_not_exist_f1",selectedItem),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
				}

			} else {
				if ((String.IsNullOrEmpty(selectedItem)) || !File.Exists(selectedItem)){
					md = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("file_not_exist_f1",selectedItem),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return;
				}
			} 


			string oldName = System.IO.Path.GetFileName(selectedItem);

			EntryDialog ed = new EntryDialog(oldName,MainClass.Languages.Translate("new_name"));

			int result = ed.Run();

			if (result == (int)ResponseType.Ok) { 
				string newName = ed.TextEntry;
				string newPath ="";
				string msg = FileUtility.RenameItem(selectedItem,isDir, newName,out newPath );

				if(!String.IsNullOrEmpty(msg)){
					message = MainClass.Languages.Translate("cannot_rename_file",selectedItem);
					if(isDir)
						message = MainClass.Languages.Translate("cannot_rename_directory",selectedItem);

					MessageDialogs mdd = 
						new MessageDialogs(MessageDialogs.DialogButtonType.Ok, message,msg, Gtk.MessageType.Error);
					mdd.ShowDialog();

					return;
				}
			}
			ed.Destroy();
			FillStore (true);
		}

		void OnDeleteItemClick (object sender, EventArgs e)
		{
			string message ="" ;
			if (isDir) {
				message = MainClass.Languages.Translate("question_delete_folder",selectedItem);
			} else {
				message = MainClass.Languages.Translate("question_delete_file",selectedItem);
			}

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, message, "", Gtk.MessageType.Question);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
					return;

			string msg =  FileUtility.DeleteItem(selectedItem,isDir);

			if(!String.IsNullOrEmpty(msg)){
				message = MainClass.Languages.Translate("cannot_delete_file",selectedItem);
				MessageDialogs mdd = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, message,msg, Gtk.MessageType.Error);
				mdd.ShowDialog();
				return;
			}

			FillStore (true);
		}


		void NavigateBarChange (string newPath)
		{
			SetPath(newPath);
		}

		Gdk.Pixbuf GetIcon (string name)
		{
			return MainClass.Tools.GetIconFromStock(name,IconSize.SmallToolbar);
		}

		ListStore CreateStore ()
		{
			// path, name, pixbuf, is_dir
			ListStore store = new ListStore (typeof (string), typeof (string), typeof (Gdk.Pixbuf), typeof (bool));
			store.DefaultSortFunc = new TreeIterCompareFunc (SortFunc);
			return store;
		}

		private void OpenFile(string path){


			if(MainClass.Tools.IsOpenedEditorFiles(path)){
				MainClass.MainWindow.OpenFile(path,true);
			} else {

				System.Diagnostics.Process.Start ("file://" + path);
			}

		}


		void FillStore (bool savePath)
		{
			int lngText = 1;
			string maxLng =" ";
			store.Clear ();
			DirectoryInfo[] directoryInfo;

			if (!parent.Exists)
				return;

			try {
				directoryInfo = parent.GetDirectories ();
			}catch {
				return;
			}

			navigBar.SetPath(parent.FullName);

			if((MainClass.Workspace!=null) && savePath)
				MainClass.Workspace.FileExplorerPath = parent.FullName;


			if( (parent.Parent!=null) && (parent.FullName != "/") ) {
				store.AppendValues (parent.Parent.FullName, "..", upIcon, true);
			}

			foreach (DirectoryInfo di in directoryInfo)
			{
				store.AppendValues (di.FullName, di.Name, dirIcon, true);
				if(di.Name.Length>lngText){
					lngText =di.Name.Length;
					maxLng =di.Name;
				}

			}

			foreach (FileInfo file in parent.GetFiles ())
			{
				string icon = MainClass.Tools.GetIconForExtension(file.Extension);
				fileIcon = GetIcon (icon);
				store.AppendValues (file.FullName, file.Name, fileIcon, false);

				if(file.Name.Length>lngText){
					lngText =file.Name.Length;
					maxLng =file.Name;
				}
			}
			iconView.ItemWidth =250;

		}

		private static Cairo.FontWeight PangoToCairoWeight (Pango.Weight weight)
		{
			return (weight == Pango.Weight.Bold) ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal;
		}

		int SortFunc (TreeModel model, TreeIter a, TreeIter b)
		{
			bool a_is_dir = (bool) model.GetValue (a, COL_IS_DIRECTORY);
			bool b_is_dir = (bool) model.GetValue (b, COL_IS_DIRECTORY);
			string a_name = (string) model.GetValue (a, COL_DISPLAY_NAME);
			string b_name = (string) model.GetValue (b, COL_DISPLAY_NAME);

			if (!a_is_dir && b_is_dir)
				return 1;
			else if (a_is_dir && !b_is_dir)
				return -1;
			else
				return String.Compare (a_name, b_name);
		}

		void OnCreateFileClicked (object sender, EventArgs a){

			string fileName = "";
			string content ="";

			NewFileDialog nfd = new NewFileDialog();

			int result = nfd.Run();
			if (result == (int)ResponseType.Ok) {
				fileName = nfd.FileName;
				content = nfd.Content;
			}
			nfd.Destroy();
			if (String.IsNullOrEmpty(fileName) ) return;

			string newFile = System.IO.Path.Combine(parent.FullName, fileName);

			try {
				FileUtility.CreateFile(newFile,content);

			} catch {
				MessageDialogs md = 
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_file"), fileName, Gtk.MessageType.Error);
				md.ShowDialog();
			}
			FillStore (false);
		}

		void OnCreateDirectoryClicked (object sender, EventArgs a)
		{
			string directory="";
			EntryDialog ed = new EntryDialog("","New Directory");
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				directory = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
			}
			ed.Destroy();
			if (String.IsNullOrEmpty(directory) ) return;

			string newFile = System.IO.Path.Combine(parent.FullName, directory);

			try {
				FileUtility.CreateDirectory(newFile);
			} catch {
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_dir"), directory, Gtk.MessageType.Error);
				md.ShowDialog();
			}
			FillStore (false);
		}

		void OnWorkspaceClicked (object sender, EventArgs a)
		{
			if((MainClass.Workspace != null) && !String.IsNullOrEmpty(MainClass.Workspace.RootDirectory)){

				parent = new DirectoryInfo (MainClass.Workspace.RootDirectory);
			} else {
				parent = new DirectoryInfo (Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			}

			FillStore (true);
			upButton.Sensitive = true;
		}

		void OnProjectClicked (object sender, EventArgs a)
		{
			if((MainClass.Workspace != null) && !String.IsNullOrEmpty(MainClass.Workspace.RootDirectory)
				&&(MainClass.Workspace.ActualProject != null) ){

				parent = new DirectoryInfo (MainClass.Workspace.ActualProject.AbsolutProjectDir);
			} else {
				parent = new DirectoryInfo (Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			}

			FillStore (true);
			upButton.Sensitive = true;
		}

		void OnOutputClicked (object sender, EventArgs a)
		{
			if((MainClass.Workspace != null) && !String.IsNullOrEmpty(MainClass.Workspace.RootDirectory)
				&&(MainClass.Workspace.ActualProject != null) ){

				parent = new DirectoryInfo (MainClass.Workspace.ActualProject.OutputMaskToFullPath);
			} else {
				parent = new DirectoryInfo (Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			}

			FillStore (true);
			upButton.Sensitive = true;
		}

		void OnHomeClicked (object sender, EventArgs a)
		{
			parent = new DirectoryInfo (Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			FillStore (true);
			upButton.Sensitive = true;
		}

		void OnItemActivated (object sender, ItemActivatedArgs a)
		{
			TreeIter iter;
			store.GetIter (out iter, a.Path);
			string path = (string) store.GetValue (iter, COL_PATH);
			bool isDir = (bool) store.GetValue (iter, COL_IS_DIRECTORY);

			if (!isDir){
				OpenFile(path);
				return;
			}

			DirectoryInfo di = new DirectoryInfo (path);
			try {
				di.GetDirectories ();
			}catch {
				return;
			}

			parent = di;
			FillStore (true);

			if(parent.Parent==null){
				upButton.Sensitive = false;
			} else {
				upButton.Sensitive = true;
			}

		}

		void OnUpClicked (object sender, EventArgs a)
		{
			parent = parent.Parent;
			FillStore (true);
			upButton.Sensitive = (parent.FullName == "/" ? false : true);

			if(parent.Parent==null){
				upButton.Sensitive = false;
			}
		}

		public void SetPath(string path){

			if(string.IsNullOrEmpty(path)){
				if((MainClass.Workspace!= null) && !string.IsNullOrEmpty(MainClass.Workspace.FilePath)){
					path = MainClass.Workspace.RootDirectory;
				} else{
					path =MainClass.Settings.LibDirectory;
				}
			}

			parent = new DirectoryInfo (path);
			FillStore (false);

			if(parent.Parent==null){
				upButton.Sensitive = false;
			} else {
				upButton.Sensitive = true;
			}

			/*if(!string.IsNullOrEmpty(MainClass.Workspace.FileExplorerPath)){
				parent = new DirectoryInfo (MainClass.Workspace.FileExplorerPath);
				FillStore (false);

				if(parent.Parent==null){
					upButton.Sensitive = false;
				} else {
					upButton.Sensitive = true;
				}
			}*/
		}
	}
}

