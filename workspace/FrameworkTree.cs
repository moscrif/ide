using System;
using Gdk;
using Gtk;
using System.IO;
using System.Collections.Generic;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Tool;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;


namespace  Moscrif.IDE.Workspace
{
	public class FrameworkTree  : VBox
	{
		private const int ICO_ROW = 0;
		private const int NAME_ROW = 1;
		private const int PATH_ROW = 2;
		private const int TYPE_ROW = 3;

		public Dictionary<string, string> XValues
		{
			get;
			private set;
		}
		public string ApplicationFile;
		public string ApplicationDirectory;

		ToolButton fileCreateButton;
		ToolButton dirCreateButton;
		ToolButton refreshButton;

		private TreeView treeView = null;
		private ScrolledWindow sw = new ScrolledWindow();

		TreeStore store = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(string), typeof(TypeFile));

		//private Menu popupMenu;

		public FrameworkTree()
		{
			Toolbar toolbar = new Toolbar ();
			this.PackStart (toolbar, false, false, 0);

			refreshButton = new ToolButton ("refresh.png");//Stock.Home);
			refreshButton.IsImportant = true;
			refreshButton.Label = "Refresh";
			refreshButton.BorderWidth = 1;
			refreshButton.Sensitive = false;
			refreshButton.Clicked+= delegate {
				if(MainClass.Workspace != null && !String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
					LoadLibs(MainClass.Workspace.RootDirectory);
				}
			};
			toolbar.Insert (refreshButton, -1);


			fileCreateButton = new ToolButton ("file-new.png");//Stock.Home);
			fileCreateButton.IsImportant = true;
			fileCreateButton.Label = "New File";
			fileCreateButton.BorderWidth = 1;
			fileCreateButton.Sensitive = false;
			fileCreateButton.Clicked += new EventHandler (OnAddFileClicked);
			toolbar.Insert (fileCreateButton, -1);

			dirCreateButton = new ToolButton ("folder-new.png");//Stock.Home);
			dirCreateButton.IsImportant = true;
			dirCreateButton.Label = "New Folder";
			dirCreateButton.BorderWidth = 1;
			dirCreateButton.Sensitive = false;
			dirCreateButton.Clicked += new EventHandler (OnAddDirectoryClicked);
			toolbar.Insert (dirCreateButton, -1);


			treeView = new TreeView();
			
			treeView.ButtonReleaseEvent += new ButtonReleaseEventHandler(OnButtonRelease);
			
			treeView.Model = store;

			treeView.HeadersVisible = true;
			//this.ExpandAll();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			treeView.RowActivated += OnRowActivate;
			treeView.ShowExpanders = true;

		//	TreeViewColumn display_column.PackStart (text_render, true);

			CellRendererPixbuf crp = new CellRendererPixbuf();
			crp.Ypad = 0;
			crp.Yalign = 0;

			Gtk.CellRendererText fileNameRenderer = new Gtk.CellRendererText();
			fileNameRenderer.Ypad =0;

			TreeViewColumn icon_column = new TreeViewColumn();//("Icon", crp, "pixbuf", ICO_ROW);
			icon_column.PackStart (crp, false);
			icon_column.AddAttribute(crp,"pixbuf",ICO_ROW);

			icon_column.PackStart (fileNameRenderer, true);
			icon_column.AddAttribute (fileNameRenderer, "markup", NAME_ROW);


			treeView.AppendColumn(icon_column);

			//treeView.AppendColumn("Icon", crp, "pixbuf", ICO_ROW);

			treeView.AppendColumn("Name", fileNameRenderer, "text", NAME_ROW);
			treeView.Columns[NAME_ROW].Visible = false;

			CellRendererText textRenderer = new CellRendererText();
			textRenderer.Ypad =0;

			treeView.AppendColumn("FullPath", textRenderer, "text", PATH_ROW);
			treeView.Columns[PATH_ROW].Visible = false;
			treeView.HeadersVisible = false;
			treeView.EnableTreeLines = true;
			treeView.HoverExpand = false;
			treeView.HoverSelection = false;
			
			treeView.Columns[NAME_ROW].SetCellDataFunc(fileNameRenderer, new Gtk.TreeCellDataFunc(RenderFileNme));
			
			sw.ShadowType = ShadowType.Out;
			sw.Add(treeView);
			
			this.PackEnd(sw, true, true, 0);
		}


		private string GetDirectory(ref TreeIter tiDirectory){

			TreeSelection ts = treeView.Selection;
			Gtk.TreePath[] selRow = ts.GetSelectedRows();

			if(selRow.Length<1) return null;

			Gtk.TreePath tp = selRow[0];

			store.GetIter(out tiDirectory, tp);

			string pFile = store.GetValue(tiDirectory, PATH_ROW).ToString();
			// Full Path
			object tFile = store.GetValue(tiDirectory, TYPE_ROW);
			string directory = "";

			// Typ
			if ((int)tFile != (int)TypeFile.Directory) {
				string path = tp.ToString();
				int indx = path.LastIndexOf(':');

				if (indx > -1) {
					path = path.Remove(indx);
				}
		
				TreePath tpPath = new TreePath(path);

				store.GetIter(out tiDirectory, tpPath);
				directory = store.GetValue(tiDirectory, PATH_ROW).ToString();


			} else{
				directory =pFile;
			}

			if(!System.IO.Directory.Exists(directory) ){
				return null;
			}

			return directory;
		}

		void OnAddFileClicked (object sender, EventArgs a){
			TreeIter tiDirectory = new TreeIter();
			string pathDir = GetDirectory(ref tiDirectory);

			if(String.IsNullOrEmpty(pathDir)){
				return;
			}

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

			string newFile = System.IO.Path.Combine(pathDir, fileName);

			try {
				FileUtility.CreateFile(newFile,content);
			} catch {
				MessageDialogs md = 
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_file"), fileName, Gtk.MessageType.Error);
				md.ShowDialog();
			}
			RefreshDir(pathDir, tiDirectory);
			/*if(MainClass.Workspace != null && !String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
				LoadLibs(MainClass.Workspace.RootDirectory);
			};*/
		}

		void OnAddDirectoryClicked (object sender, EventArgs a)
		{
			TreeIter tiDirectory = new TreeIter();
			string pathDir = GetDirectory(ref tiDirectory);

			if(String.IsNullOrEmpty(pathDir)){
				return;
			}

			string directory="";
			EntryDialog ed = new EntryDialog("","New Directory");
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				directory = MainClass.Tools.RemoveDiacritics(ed.TextEntry);
			}
			ed.Destroy();
			if (String.IsNullOrEmpty(directory) ) return;

			string newDir = System.IO.Path.Combine(pathDir, directory);

			try {
				FileUtility.CreateDirectory(newDir);
			} catch {
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_dir"), directory, Gtk.MessageType.Error);
				md.ShowDialog();
			}

			RefreshDir(pathDir, tiDirectory);

			/*if(MainClass.Workspace != null && !String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
				LoadLibs(MainClass.Workspace.RootDirectory);
			};*/
		}


		private void RenderFileNme(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			//if ( iter == Gtk.TreeIter.Zero) return;
			//return;
			int type = (int)model.GetValue(iter, TYPE_ROW);
			//(int)modelStore.GetValue(iter, TYPE_ROW);
			Pango.FontDescription fd = new Pango.FontDescription();
			(cell as Gtk.CellRendererText).Foreground = "Black";
			
			if (type == (int)TypeFile.AppFile) {
				fd.Weight = Pango.Weight.Bold;
			} else if (type == (int)TypeFile.StartFile)
				fd.Weight = Pango.Weight.Bold; else if (type == (int)TypeFile.ExcludetFile)
				//Gdk.Color(228, 228, 228);
				(cell as Gtk.CellRendererText).Foreground = "LightGray";
			else {
				fd.Weight = Pango.Weight.Normal;
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;
			
		}

		[GLib.ConnectBefore]
		void OnButtonRelease(object obj, ButtonReleaseEventArgs args)
		{}

		private void OnRowActivate(object o, RowActivatedArgs args)
		{
			TreeIter ti = new TreeIter();
			//filter.GetIter(out ti, args.Path);
			//TreePath childTP = filter.ConvertPathToChildPath(args.Path);
			store.GetIter(out ti, args.Path);

			string pFile = store.GetValue(ti, PATH_ROW).ToString();
			// Full Path
			object tFile = store.GetValue(ti, TYPE_ROW);

			// Typ
			if ((int)tFile != (int)TypeFile.Directory) {
				MainClass.MainWindow.EditorNotebook.Open(pFile);
			}
		}

		private static void RemoveAllChildren(Gtk.TreeStore model, TreeIter ti)
		{
			TreeIter child;
			
			if (model.IterChildren(out child, ti)) {
				int depth = model.IterDepth(ti);
				
				while (model.Remove(ref child) && model.IterDepth(child) > depth)
					;
			}
		}

		private void SetVisibilityButton(bool visible){
			dirCreateButton.Sensitive = visible;
			fileCreateButton.Sensitive = visible;
		}

		public void Clear()
		{
			store.Clear();
		}

		public void RefreshDir(string dir, TreeIter tiParent)
		{
			RemoveAllChildren(store, tiParent);
			getChild(tiParent, dir, store);
		}

		public void LoadLibs(string directory)
		{
			refreshButton.Sensitive = true;
			if (String.IsNullOrEmpty(directory)){
				SetVisibilityButton(false);
				return;
			}

			store.Clear();
			DirectoryInfo di = new DirectoryInfo(directory);
			List<FileInfo> listAPP = new List<FileInfo>(di.GetFiles("*.app"));
			int count = 0;

			foreach (DirectoryInfo d in di.GetDirectories()){

				if(!MainClass.Workspace.ShowAllLibs && ((d.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)){// linked libs -no
					continue;
				}

				try {
					d.GetDirectories();
				}catch{
					Tool.Logger.Error("Framework directory {0} in Workspace {1} is invalid!",di.FullName,MainClass.Workspace.Name);
					continue;
				}

				int indx =  listAPP.FindIndex (x=> System.IO.Path.GetFileNameWithoutExtension(x.FullName) ==d.Name);

				if(indx<0){
					//int indxLibs = -1;
					//indxIgnore = MainClass.Settings.IgnoreFolder.FindIndex(x => x == d.Name);

					//indxLibs = MainClass.Settings.LibsDefine.FindIndex(x => x == d.Name);

					//if(indxLibs>0){
						TreeIter iter = store.AppendValues(MainClass.Tools.GetIconFromStock("folder.png", IconSize.SmallToolbar), d.Name, d.FullName, TypeFile.Directory);
						count++;
						getChild(iter, d.FullName, store);
					//}
				}
			}
			//this.ExpandAll();
			if(count ==0){
				SetVisibilityButton(false);
			} else {
				SetVisibilityButton(true);
			}
		}

		private static void getChild(TreeIter parent, string path, TreeStore rootStore)
		{
			if (!Directory.Exists(path))
				return;
			
			DirectoryInfo di = new DirectoryInfo(path);

			try {
				di.GetDirectories();
			}catch{
				Tool.Logger.Error("Framework directory {0} in Workspace {1} is invalid!",path,MainClass.Workspace.Name);
				return;
			}


			foreach (DirectoryInfo d in di.GetDirectories()){

				if(!MainClass.Workspace.ShowAllLibs && ((d.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)){// linked libs -no
					continue;
				}

				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				//if (!d.Name.StartsWith(".")) {
				if(indx<0){
					string ico = "folder.png";
					if (d.Name == MainClass.Settings.ThemeDir)//".themes")
						ico= "themes.png";

					TreeIter iter = rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock(ico, Gtk.IconSize.SmallToolbar), d.Name, d.FullName, TypeFile.Directory);
					getChild(iter, d.FullName, rootStore);
				}
			}
			
			foreach (FileInfo f in di.GetFiles()){

				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForIde);
				if(indx >-1)continue;

				if (!MainClass.Tools.IsIgnoredExtension(f.Extension)) {
					string stockIcon = MainClass.Tools.GetIconForExtension(f.Extension);
					rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.SmallToolbar), f.Name, f.FullName, TypeFile.SourceFile);
				}
			}
		}

		private static Gdk.Pixbuf GetIcon(string name)
		{
			return Gtk.IconTheme.Default.LoadIcon(name, 12, (IconLookupFlags)0);
		}



		protected override void OnDestroyed()
		{
			
			base.OnDestroyed();
		}

	}
}

