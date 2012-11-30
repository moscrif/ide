using System;
using System.IO;
using Gtk;
using Gdk;
using System.Collections.Generic;

namespace Moscrif.IDE.Components
{
	public class FileMaskEntry : Gtk.HBox
	{
		string name;
		
		Entry text;
		Button browse;
		//bool loading;
		bool isFolder;
		object parent;
		Gtk.Window windowParent;

		Gtk.Menu popupCondition = new Gtk.Menu();

		public event EventHandler PathChanged;

		public FileMaskEntry (List<string> mask, object parent): this (mask, parent,MainClass.MainWindow){}

		public FileMaskEntry (List<string> mask, object parent,Gtk.Window parentWindow): base (false, 6)//(string name, bool isFolder) : base (false, 6)
		{
			windowParent =parentWindow;

			text = new Entry ();
			this.parent= parent;
			browse = Button.NewWithMnemonic (MainClass.Languages.Translate("browse"));
			
			text.Changed += new EventHandler (OnTextChanged);
			browse.Clicked += new EventHandler (OnButtonClicked);

			PackStart (text, true, true, 0);

			PackEnd (browse, false, false, 0);

			Gdk.Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");

			popupCondition = new Gtk.Menu();

			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Gdk.Pixbuf(file);

				Gtk.Button btnClose = new Gtk.Button(new Gtk.Image(default_pixbuf));
				btnClose.TooltipText = MainClass.Languages.Translate("insert_path_mask");
				btnClose.Relief = Gtk.ReliefStyle.None;
				btnClose.CanFocus = false;
				btnClose.WidthRequest = btnClose.HeightRequest = 22;

				popupCondition.AttachToWidget(btnClose,new Gtk.MenuDetachFunc(DetachWidget));

				btnClose.Clicked += delegate {
					popupCondition.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
				};
				PackEnd (btnClose, false, false, 0);
			}

			if (mask != null)
				foreach (string cd in mask) {

					AddMenuItem(cd);
				}
			popupCondition.ShowAll();

			this.ShowAll();
		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}

		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);
			//Console.WriteLine("GetOrigin -->>> x->{0} ; y->{1}",x,y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		private void AddMenuItem(string condition)
		{
			Gtk.MenuItem mi = new Gtk.MenuItem(condition.Replace("_","__"));
			mi.Name = condition;
			mi.Activated += delegate(object sender, EventArgs e) {
				if (sender.GetType() == typeof(Gtk.MenuItem)){
					int start = 0;
					int end = 0;
					if(text.GetSelectionBounds(out start, out end)){
						text.DeleteText(start,end);
						text.InsertText(condition, ref start);
						text.Position = text.Position + condition.Length;
					} else {
						int curPos = text.CursorPosition;
						text.InsertText(condition, ref curPos);
						text.Position = text.Position + condition.Length;
					}				
					//entrName.Text = entrName.Text = String.Format("{0}$({1})", entrName.Text, (sender as Gtk.MenuItem).Name);
				}
			};
			popupCondition.Add(mi);
		}

		//abstract
		protected string ShowBrowseDialog (string name, string start_in)
		{
			FileChooserDialog fd;
			if (isFolder){
				fd = new Gtk.FileChooserDialog(name,
		                            windowParent,
		                            FileChooserAction.SelectFolder,
				            //FileChooserAction.CreateFolder,
		                            "Cancel",ResponseType.Cancel,
		                            "Open",ResponseType.Accept);
				//fd.
			} else {
				fd = new Gtk.FileChooserDialog(name,
		                            windowParent,
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

		string visiblePath;

		public string VisiblePath {
			get { return visiblePath; }
			set {
				visiblePath = value;
				text.Text = value;
				if(parent != null){
					if( parent.GetType() == typeof(Workspace.Workspace) ){
						default_path = (parent as Workspace.Workspace).ConvertOutputMaskToFullPath(visiblePath);
					} else if ( parent.GetType() == typeof(Workspace.Project)){
						default_path = (parent as Workspace.Project).ConvertProjectMaskPathToFull(visiblePath);	}
				}
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
				return text.Text;
			}
			/*set {
				loading = true; 
				text.Text = value;
				loading = false;
			}*/
		}
		
		void OnButtonClicked (object o, EventArgs args)
		{
			string start_in;

			string visibleP = "";
			if(parent != null){
				if( parent.GetType() == typeof(Workspace.Workspace) ){
					visibleP = (parent as Workspace.Workspace).ConvertOutputMaskToFullPath(text.Text);
				} else if ( parent.GetType() == typeof(Workspace.Project)){
					visibleP = (parent as Workspace.Project).ConvertProjectMaskPathToFull(text.Text);	}
			}


			if (isFolder){
				if (Directory.Exists (visibleP))
					start_in = visibleP;
				else
					start_in = default_path;
			} else {
				if (File.Exists (visibleP))
					start_in =  System.IO.Path.GetDirectoryName(visibleP);
				else if(Directory.Exists (visibleP)){
					start_in = visibleP;
				}
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
			if (/*!loading &&*/ PathChanged != null)
				PathChanged (this, EventArgs.Empty);
		}
	}
}
