using System;
using Gdk;
using Gtk;
using System.IO;
using System.Collections.Generic;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;
using Moscrif.IDE.Actions;

namespace Moscrif.IDE.Workspace
{
	public delegate void FileIsSelectedHandler (string fileName, int fileType, string appFileName);

	public class WorkspaceTree : VBox
	{
		//TreeView

		private const int ICO_ROW = 0;
		private const int NAME_ROW = 1;
		private const int PATH_ROW = 2;
		private const int TYPE_ROW = 3;

		public event FileIsSelectedHandler FileIsSelected;

		/*public Dictionary<string, string> XValues
		{
			get;
			private set;
		}*/

		public string ApplicationFile;
		public string ApplicationDirectory;

		//private ComboBox ComboFilter = new ComboBox();
		//private ListStore filterModel = new ListStore(typeof(string), typeof(string), typeof(LogicalSystem));
		private LogicalSystem activetFilter = null;

		private Gtk.TreeModelFilter filter = null;

		private TreeView treeView = null;
		private ScrolledWindow sw = new ScrolledWindow();

		TreeStore store = new TreeStore(typeof(Gdk.Pixbuf), typeof(string), typeof(string), typeof(TypeFile));

		ToolButton refreshButton;

		private Menu popupMenu;
		private MenusToolButton mtbCreate;
		// Icon,Show Name, Full Path, TypFile
		public WorkspaceTree()
		{
			treeView = new TreeView();
			
			treeView.ButtonReleaseEvent += new ButtonReleaseEventHandler(OnButtonRelease);
			treeView.KeyReleaseEvent += delegate(object o, KeyReleaseEventArgs args) {
				if(args.Event.Key == Gdk.Key.Delete){
					MainClass.MainWindow.DeleteFile();
				}
			};

			//treeView.Model = modelStore = store;
			
			filter = new Gtk.TreeModelFilter(store, null);
			filter.VisibleFunc = new Gtk.TreeModelFilterVisibleFunc(FilterTree);
			treeView.Model = filter;
			
			treeView.HeadersVisible = true;
			//this.ExpandAll();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			treeView.RowActivated += OnRowActivate;
			treeView.Selection.Changed+= OnRowSelection;
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

			Toolbar toolbar = new Toolbar ();

			refreshButton = new ToolButton ("refresh.png");//Stock.Home);
			refreshButton.IsImportant = true;
			refreshButton.Label = MainClass.Languages.Translate("menu_refresh");
			refreshButton.BorderWidth = 1;
			refreshButton.Clicked+= delegate {
				MainClass.MainWindow.RefreshProject();
			};
			toolbar.Insert (refreshButton, -1);

			Gtk.Menu menuAdd = new Gtk.Menu ();

			MenuItem mi = new MenuItem (MainClass.Languages.Translate("menu_create_file"));
			mi.Activated += delegate(object sender, EventArgs e) {
				NewAction nw = new NewAction();
				nw.Activate();
			};

			menuAdd.Insert (mi, -1);
			mi = new MenuItem (MainClass.Languages.Translate("menu_create_dir"));
			mi.Activated += delegate(object sender, EventArgs e) {
				NewDirectoryAction nw = new NewDirectoryAction();
				nw.Activate();
			}; 

			menuAdd.Insert (mi, -1);
			menuAdd.ShowAll ();

			mtbCreate= new MenusToolButton(menuAdd,"file-new.png");
			mtbCreate.IsImportant = true;
			mtbCreate.Label = "Create";
			toolbar.Insert (mtbCreate, -1);

			if(MainClass.Settings.LogicalSort == null)
				MainClass.Settings.LogicalSort = LogicalSystem.GetDefaultLogicalSystem();

			Gtk.Menu menuFilter = new Gtk.Menu (); 

			MenuItem menuItemFilter = new MenuItem (MainClass.Languages.Translate("all_files") );
			menuItemFilter.Activated += delegate(object sender, EventArgs e) {
				activetFilter = null;
				filter.Refilter();
				treeView.QueueDraw();					
			};
			menuFilter.Insert (menuItemFilter, -1);

		
			foreach (LogicalSystem ls in MainClass.Settings.LogicalSort){
				LogicalSystem lsTemp = ls;
				menuItemFilter = new MenuItem (lsTemp.Display);
				menuItemFilter.Activated += delegate(object sender, EventArgs e) {
					if (lsTemp != null) {
						activetFilter = lsTemp;
					} else {
						activetFilter = null;
					}
					filter.Refilter();
					treeView.QueueDraw();					
				};
				menuFilter.Insert (menuItemFilter, -1);
				//filterModel.AppendValues(ls.Display, "", ls);
			}
			menuFilter.ShowAll();

			MenusToolButton mtbFilter= new MenusToolButton(menuFilter,"filter.png");
			mtbFilter.IsImportant = true;
			mtbFilter.Label = "Filter";
			toolbar.Insert (mtbFilter, -1);
			this.PackStart(toolbar, false, false, 0);
			//this.PackStart(ComboFilter, false, false, 0);
			//ComboFilter.Active = 0;
		}


		private bool FilterTree(Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			object fileObj = model.GetValue(iter, PATH_ROW);
			
			if (fileObj != null) {
				string filePath = fileObj.ToString();
				string fileExtension = System.IO.Path.GetExtension(filePath);
				
				if (activetFilter == null)
					return true;
				else {
					
					FileAttributes fa = File.GetAttributes(filePath);
					if ((fa & FileAttributes.Directory) == FileAttributes.Directory)
						return true;
					
					if (fileExtension.ToLower() == ".app") {
						return true;
					}
					
					int indx = activetFilter.Mask.FindIndex(x => x.ToLower() == fileExtension.ToLower());
					if (indx > -1)
						return true;
					else
						return false;
				}
			}
			return true;
		}

		void OnComboFilterChanged(object o, EventArgs args)
		{
			ComboBox combo = o as ComboBox;
			if (o == null)
				return;
			
			TreeIter iter;
			if (combo.GetActiveIter(out iter)) {
				LogicalSystem ls = (LogicalSystem)combo.Model.GetValue(iter, 2);
				
				if (ls != null) {
					activetFilter = ls;
				} else {
					activetFilter = null;
				}
				filter.Refilter();
				treeView.QueueDraw();
			}
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
				fd.Weight = Pango.Weight.Bold; 
			else if (type == (int)TypeFile.ExcludetFile){
				//Gdk.Color(228, 228, 228);
				(cell as Gtk.CellRendererText).Foreground = "LightGray";
			}
			else {
				fd.Weight = Pango.Weight.Normal;
				(cell as Gtk.CellRendererText).Foreground = "Black";
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;
			
		}



		[GLib.ConnectBefore]
		void OnButtonRelease(object obj, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3) {

				TreeIter ti = GetSelectedIter();

				//if (ti == TreeIter.Zero) return;
				if(ti.Equals(TreeIter.Zero))return;
				//if (ti. != null) {
				string ti_PATH = store.GetValue(ti, PATH_ROW).ToString();
				int ti_TYP = (Int32)store.GetValue(ti, TYPE_ROW);
				this.popupMenu = new Menu();

				if (ti_TYP == (int)TypeFile.AppFile) {
					this.popupMenu = (Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/workspacePagePopupProject");
					MainClass.MainWindow.ActionUiManager.SetActionLabel("refreshproject", "Refresh Project");

				} else if (ti_TYP == (int)TypeFile.Directory) {
					this.popupMenu = (Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/workspacePagePopupDirectory");
					MainClass.MainWindow.ActionUiManager.SetActionLabel("refreshproject", "Refresh Directory");

				} else {
					this.popupMenu = (Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/workspacePagePopupFile");

					if ((ti_TYP != (int)TypeFile.SourceFile) && (ti_TYP != (int)TypeFile.ExcludetFile)) {
						MainClass.MainWindow.ActionUiManager.SetSensitive("setasstart", false);
						MainClass.MainWindow.ActionUiManager.SetSensitive("toggleexclude", false);
					} else {
						MainClass.MainWindow.ActionUiManager.SetSensitive("setasstart", true);
						MainClass.MainWindow.ActionUiManager.SetSensitive("toggleexclude", true);
					}
				}
				this.popupMenu.ShowAll();
				this.popupMenu.Popup();
			}
		}


		private void OnRowSelection(object sender, EventArgs e) {

			TreeIter iter1 = GetSelectedIter();
			if(iter1.Equals(TreeIter.Zero)){
				FileIsSelected("",-1,"");
				return;
			}

			string pFile1 = store.GetValue(iter1, PATH_ROW).ToString();
			object tFile1 = store.GetValue(iter1, TYPE_ROW);

			if (FileIsSelected != null){

				string appFile = GetSelectedProjectApp();

				FileIsSelected(pFile1,(int)tFile1,appFile);
			}



		}

		private void OnRowActivate(object o, RowActivatedArgs args)
		{
			TreeIter ti = new TreeIter();
			//filter.GetIter(out ti, args.Path);
			TreePath childTP = filter.ConvertPathToChildPath(args.Path);
			store.GetIter(out ti, childTP);

			string pFile = store.GetValue(ti, PATH_ROW).ToString();
			// Full Path
			object tFile = store.GetValue(ti, TYPE_ROW);

			// Typ
			if ((int)tFile != (int)TypeFile.Directory) {
				MainClass.MainWindow.EditorNotebook.Open(pFile);
			}
		}

		/*private TreeIter GetSelectedTopParentTreeIter()
		{
			TreeSelection ts = treeView.Selection;
			Gtk.TreePath[] selRow = ts.GetSelectedRows();
			Gtk.TreePath tp = selRow[0];
			
			string path = filter.ConvertPathToChildPath(tp).ToString();
			
			int indx = path.IndexOf(':');
			if (indx > -1) {
				path = path.Remove(indx);
			}
			
			TreeIter tiParent = new TreeIter();
			TreePath tpPath = new TreePath(path);
			store.GetIter(out tiParent, tpPath);
			
			return tiParent;
		}*/

		private TreeIter GetSelectedParentTreeIter(bool topParent)
		{
			TreeSelection ts = treeView.Selection;
			Gtk.TreePath[] selRow = ts.GetSelectedRows();
			if(selRow.Length==0) return TreeIter.Zero;

			Gtk.TreePath tp = selRow[0];
			
			string path = filter.ConvertPathToChildPath(tp).ToString();

			int indx = path.LastIndexOf(':');

			if(topParent)
				indx = path.IndexOf(':');

			if (indx > -1) {
				path = path.Remove(indx);
			}
			
			TreeIter tiParent = new TreeIter();
			TreePath tpPath = new TreePath(path);
			store.GetIter(out tiParent, tpPath);
			
			return tiParent;
		}

		public void SetCreateSensitive(bool sensitive){
			mtbCreate.Sensitive = sensitive;
		}


		public string GetSelectedProjectApp()
		{
			TreeIter tiParent = GetSelectedParentTreeIter(true);
			
			string filename = store.GetValue(tiParent, PATH_ROW).ToString();
			return filename;
			
		}

		public void GetSelectedFileDirectory(out string filename, out int fileTyp, out Gtk.TreeIter ti)
		{
			GetSelectedFile(out filename, out fileTyp, out ti);
			if(fileTyp == (int)TypeFile.Directory)
				return;

			ti = GetSelectedParentTreeIter(false);

			//if (ti. != null) {
			if(!ti.Equals(TreeIter.Zero)){
				filename = store.GetValue(ti, PATH_ROW).ToString();
				fileTyp = (Int32)store.GetValue(ti, TYPE_ROW);
			} else {
				filename = "";
				fileTyp = -1;
			}
		}

		public void GetSelectedFile(out string filename, out int fileTyp, out Gtk.TreeIter ti)
		{
			ti = GetSelectedIter();
			//if(ti.Equals(TreeIter.Zero)) return;
			//if (ti == TreeIter.Zero)

			//if (ti. != null) {
			if(!ti.Equals(TreeIter.Zero)){
				filename = store.GetValue(ti, PATH_ROW).ToString();
				fileTyp = (Int32)store.GetValue(ti, TYPE_ROW);
			} else {
				filename = "";
				fileTyp = -1;
			}
		}

		public void SetSelectedFile(string filename)
		{
			TreeIter iter = new TreeIter();
			store.Foreach((model, path, iterr) =>
			{
				string myPath = store.GetValue(iterr, PATH_ROW).ToString();
				int myTyp = (int)store.GetValue(iterr, TYPE_ROW);
				
				if ((myPath == filename) ) {
					//iter = iterr;
					iter =filter.ConvertChildIterToIter(iterr);
					if(!iter.Equals(TreeIter.Zero)){
						TreePath tp = filter.GetPath(iter);
						treeView.ExpandToPath(tp);
						treeView.ScrollToCell(tp,null,true,0.9f,0);
						//treeView.Selection.UnselectAll();
						treeView.Selection.SelectIter(iter);
					}
					return true;
				}
				return false;
			});
			//return GetSelectedProjectApp();

		}

		public TreeIter GetSelectedIter()
		{
			TreeSelection ts = treeView.Selection;
			Gtk.TreePath[] selRow = ts.GetSelectedRows();

			if(selRow.Length<1) return TreeIter.Zero;

			Gtk.TreePath tp = selRow[0];
			TreeIter ti = new TreeIter();
			
			TreePath tpChild = filter.ConvertPathToChildPath(tp);
			
			store.GetIter(out ti, tpChild);
			
			return ti;
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

		public void LoadProject(Project project)
		{
			TreeIter iter = store.AppendValues(MainClass.Tools.GetIconFromStock("project-folder.png", IconSize.SmallToolbar), project.ProjectName, project.AbsolutAppFilePath, TypeFile.AppFile);
			getChild(iter, project.AbsolutProjectDir, store, project);
			//this.ExpandAll();
		}

		public void RefreshProject(Project project, TreeIter tiParent)
		{
			store.SetValue(tiParent,1,project.ProjectName);
			store.SetValue(tiParent,2,project.AbsolutAppFilePath);

			RemoveAllChildren(store, tiParent);
			getChild(tiParent, project.AbsolutProjectDir, store, project);
		}

		public void RefreshDir(string dir, Project project, TreeIter tiParent)
		{
			RemoveAllChildren(store, tiParent);
			getChild(tiParent, dir, store, project);
		}

		public void AddFileToTree(string fileName, Gtk.TreeIter ti)
		{
			FileInfo f = new FileInfo(fileName);
			string stockIcon = MainClass.Tools.GetIconForExtension(f.Extension);
			
			TreeIter iter = store.AppendValues(ti, MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.SmallToolbar), f.Name, f.FullName, TypeFile.SourceFile);
			
		}


		public void AddThemeDirectoryToTree(string fileName, Gtk.TreeIter ti)
		{
			FileInfo f = new FileInfo(fileName);
			
			DirectoryInfo di = new DirectoryInfo(fileName);

			TreeIter themeIter;
			bool search = false;
			int indx = 0;

			while (store.IterNthChild(out themeIter,ti,indx))
			{
				string myName = store.GetValue(themeIter, NAME_ROW).ToString();

				if (myName == MainClass.Settings.ThemeDir){
					search = true;
					break;
				}
				indx++;
			}

			if (!search){
				string themeDir = System.IO.Path.GetDirectoryName(fileName);
				themeIter = store.AppendValues(ti, MainClass.Tools.GetIconFromStock("themes.png", Gtk.IconSize.SmallToolbar), ".theme", themeDir, TypeFile.Directory);
			}

			TreeIter iter = store.AppendValues(themeIter, MainClass.Tools.GetIconFromStock("folder.png", Gtk.IconSize.SmallToolbar), di.Name, di.FullName, TypeFile.Directory);
			getChild(iter, fileName, store, null);
		}

		public void AddDirectoryToTree(string fileName, Gtk.TreeIter ti)
		{
			FileInfo f = new FileInfo(fileName);
			
			DirectoryInfo di = new DirectoryInfo(fileName);
			
			TreeIter iter = store.AppendValues(ti, MainClass.Tools.GetIconFromStock("folder.png", Gtk.IconSize.SmallToolbar), di.Name, di.FullName, TypeFile.Directory);
			
			getChild(iter, fileName, store, null);
		}

		public void RemoveTree(Gtk.TreeIter ti)
		{
			store.Remove(ref ti);
		}


		public TreeIter GetStartUpIter(string filePath)
		{
			TreeIter iter = new TreeIter();
			store.Foreach((model, path, iterr) =>
			{
				string myPath = store.GetValue(iterr, PATH_ROW).ToString();
				int myTyp = (int)store.GetValue(iterr, TYPE_ROW);

				if ((myPath == filePath) && (myTyp == (int)TypeFile.StartFile)) {
					iter = iterr;
					return true;
				}
				return false;
			});
			
			return iter;
		}

		public void UpdateIter(TreeIter ti, string newName, string newPath, int newTyp)
		{
			if (!String.IsNullOrEmpty(newPath))
				store.SetValue(ti, PATH_ROW, newPath);

			if (!String.IsNullOrEmpty(newName))
				store.SetValue(ti, NAME_ROW, newName);
			
			if (newTyp > -1)
				store.SetValue(ti, TYPE_ROW, (TypeFile)newTyp);
		}

		public void Clear()
		{
			store.Clear();
		}


		private static void getChild(TreeIter parent, string path, TreeStore rootStore, Project project)
		{
			if (!Directory.Exists(path))
				return;
			
			DirectoryInfo di = new DirectoryInfo(path);
			
			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				//if (!d.Name.StartsWith(".")) {
				if(indx<0){
					string ico = "folder.png";
					if (d.Name == MainClass.Settings.ThemeDir)//".themes")
						ico= "themes.png";

					TreeIter iter = rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock(ico, Gtk.IconSize.SmallToolbar), d.Name, d.FullName, TypeFile.Directory);
					getChild(iter, d.FullName, rootStore, project);
				}
			}
			
			foreach (FileInfo f in di.GetFiles()){

				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForIde);
				if(indx >-1)continue;

				if (!MainClass.Tools.IsIgnoredExtension(f.Extension)) {
					
					string stockIcon = MainClass.Tools.GetIconForExtension(f.Extension);
					if (project != null) {
						if (f.FullName == MainClass.Workspace.GetFullPath(project.StartFile)) {
							rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.SmallToolbar), f.Name, f.FullName, TypeFile.StartFile);
							continue;
						}
						
						FileItem fi = project.FilesProperty.Find(x => MainClass.Workspace.GetFullPath(x.FilePath) == f.FullName);
						if ((fi != null) && (fi.IsExcluded)){
							rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock("file-exclude.png", Gtk.IconSize.SmallToolbar), f.Name, f.FullName, TypeFile.ExcludetFile);
							continue;
						}
					}
					
					rootStore.AppendValues(parent, MainClass.Tools.GetIconFromStock(stockIcon, Gtk.IconSize.SmallToolbar), f.Name, f.FullName, TypeFile.SourceFile);
				}
			}
		}

		/*private Menu GetPopUp(){
			return  (Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/workspacePagePopupDirectory");
		}*/
		/*
		private static string GetIconString(string extension)
		{
			string stockIcon = Stock.Dnd;
			
			switch (extension) {
			case ".xml":
				stockIcon = "file-html.png";
				break;
			case ".ms":
				stockIcon = "file-ms.png";
				break;
			case ".mso":
				stockIcon = "file-ms.png";
				break;
			case ".txt":
				stockIcon = "file-text.png";
				break;
			case ".db":
				stockIcon = "file-database.png";
				break;
			case ".png":
			case ".jpg":
				stockIcon = "file-image.png";
				break;
			default:
				stockIcon = "empty.png";
				break;
			}
			
			return stockIcon;
		}*/

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

