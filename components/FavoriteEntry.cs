using System;
using System.IO;
using Gtk;
using Gdk;

namespace Moscrif.IDE.Components
{

	public class FavoriteEntry : Gtk.HBox
	{
		string name;
		
		NavigationBar navBar;
		Button browse;
		bool loading;
		bool isFolder;

		public event EventHandler PathChanged;
		
		public FavoriteEntry (NavigationBar.NavigationType navigationType): base (false, 6)//(string name, bool isFolder) : base (false, 6)
		{
			//this.isFolder = isFolder;
			//this.name = name;
			navBar = new NavigationBar (navigationType);
			browse = Button.NewWithMnemonic (MainClass.Languages.Translate("browse"));
			
			//navBar.Changed += new EventHandler (OnTextChanged);
			navBar.OnChangePath += NavigateBarChange;
			browse.Clicked += new EventHandler (OnButtonClicked);
			
			PackStart (navBar, true, true, 0);
			PackEnd (browse, false, false, 0);
		}
		//abstract
		protected string ShowBrowseDialog (string name, string start_in)
		{
			FileChooserDialog fd;
			if (isFolder){
				fd = new Gtk.FileChooserDialog(name,
		                            MainClass.MainWindow,
				            FileChooserAction.SelectFolder,
				            //FileChooserAction.CreateFolder,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);
			} else {
				fd = new Gtk.FileChooserDialog(name,
		                            MainClass.MainWindow,
		                            FileChooserAction.Open,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);
			}

			if (start_in != null){

				fd.SetCurrentFolder(start_in);
			}
			else {
				fd.SetCurrentFolder(MainClass.Workspace.RootDirectory);
			}

			//fd.TransientFor = GetTransientFor ();
			string file = String.Empty;
			if (fd.Run() == (int)ResponseType.Accept){
				file = fd.Filename;
			}

			fd.Destroy();
			return file;
		}

		public bool IsFolder {
			get { return isFolder; }
			set { isFolder = value; }
		}

		public string BrowserTitle {
			get { return name; }
			set { name = value; }
		}
		
		/*string default_path;
		public string DefaultPath {
			get { return default_path; }
			set {
				default_path = value;
				navBar.SetPath(value);
			}
		}*/

		/*public void SetDefaultPathInvisible(string defaultpath){

			default_path =defaultpath;
		}*/
		
		public virtual Gtk.Window TransientFor {
			get;
			set;
		}
		
		protected Gtk.Window GetTransientFor ()
		{
			return TransientFor ?? Toplevel as Gtk.Window;
		}

		//private string path;
		public new string Path {
			get {
				return navBar.ActivePath;//path;
			}
			set {
				loading = true; 
				navBar.SetPath(value);
				//navBar.Text = value;
				loading = false;
			}
		}
		
		void OnButtonClicked (object o, EventArgs args)
		{
			string start_in;

			if (isFolder){
				//if (Directory.Exists (Path))
					start_in = Path;
				//else
				//	start_in = default_path;
			} else {
				//if (File.Exists (Path))
					start_in =  System.IO.Path.GetDirectoryName(Path);
				//else {

				//	if (Directory.Exists (default_path))
				//		start_in = default_path;
				//	else start_in = MainClass.Workspace.RootDirectory;
				//}
			}
			
			string path = ShowBrowseDialog (name, start_in + System.IO.Path.DirectorySeparatorChar);
			//if (path != null) {
			if (!String.IsNullOrEmpty(path)) {
				navBar.SetPath(path);
				//navBar.Text = path;
			}
		}

		void NavigateBarChange (string newPath)
		{
			if (!loading && PathChanged != null)
				PathChanged (this, EventArgs.Empty);
			//SetPath(newPath);
		}
		
		/*void OnTextChanged (object o, EventArgs args)
		{
			if (!loading && PathChanged != null)
				PathChanged (this, EventArgs.Empty);
		}*/
	}
}
