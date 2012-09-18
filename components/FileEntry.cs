using System;
using System.IO;
using Gtk;
using Gdk;

namespace Moscrif.IDE.Components
{
	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class FileEntry : Gtk.HBox
	{
		string name;
		
		Entry text;
		Button browse;
		bool loading;
		bool isFolder;

		public event EventHandler PathChanged;
		
		public FileEntry (): base (false, 6)//(string name, bool isFolder) : base (false, 6)
		{
			//this.isFolder = isFolder;
			//this.name = name;
			text = new Entry ();
			browse = Button.NewWithMnemonic (MainClass.Languages.Translate("browse"));
			
			text.Changed += new EventHandler (OnTextChanged);
			browse.Clicked += new EventHandler (OnButtonClicked);
			
			PackStart (text, true, true, 0);
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
		
		string default_path;
		public string DefaultPath {
			get { return default_path; }
			set {
				default_path = value;
				text.Text = value;
			}
		}

		public void SetDefaultPathInvisible(string defaultpath){

			default_path =defaultpath;
		}
		
		public virtual Gtk.Window TransientFor {
			get;
			set;
		}
		
		protected Gtk.Window GetTransientFor ()
		{
			return TransientFor ?? Toplevel as Gtk.Window;
		}

		public new string Path {
			get {
				return default_path != null && text.Text.Length > 0 ? System.IO.Path.Combine (default_path, text.Text) : text.Text;
			}
			set {
				loading = true; 
				text.Text = value;
				loading = false;
			}
		}
		
		void OnButtonClicked (object o, EventArgs args)
		{
			string start_in;

			if (isFolder){
				if (Directory.Exists (Path))
					start_in = Path;
				else
					start_in = default_path;
			} else {
				if (File.Exists (Path))
					start_in =  System.IO.Path.GetDirectoryName(Path);
				else {

					if (Directory.Exists (default_path))
						start_in = default_path;
					else start_in = MainClass.Workspace.RootDirectory;
				}
			}
			
			string path = ShowBrowseDialog (name, start_in + System.IO.Path.DirectorySeparatorChar);
			//if (path != null) {
			if (!String.IsNullOrEmpty(path)) {
				text.Text = path;
			}
		}
		
		void OnTextChanged (object o, EventArgs args)
		{
			if (!loading && PathChanged != null)
				PathChanged (this, EventArgs.Empty);
		}
	}
}
