using System;
using Gdk;
using GLib;
using Gtk;
using  Moscrif.IDE.Components;
using System.Collections.Generic;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Editors
{
	public class StartPage : IEditor
	{

		private StartEventControl eventBox = null;
		private Gtk.Widget control = null;
		private Gtk.ActionGroup editorAction = null;
		private string fileName = String.Empty;


		//Pixbuf logoPixbuf;
		//Pixbuf bgPixbuf;
		//DrawingArea drawingArea;
//		private Gdk.Pixbuf imagePb;
//		private Gtk.Image image;

		public StartPage(string path)
		{
			//string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "moscrif.png");
			//logoPixbuf = new Gdk.Pixbuf (file);

			//bgPixbuf = new Gdk.Pixbuf (file);
			editorAction = new Gtk.ActionGroup("startpage");
			this.fileName = path;
			control = new Gtk.ScrolledWindow();
			(control as Gtk.ScrolledWindow).ShadowType = Gtk.ShadowType.Out;
			eventBox = new StartEventControl();//new Gtk.EventBox();

			//(control as Gtk.ScrolledWindow).Add(eventBox);
			(control as Gtk.ScrolledWindow).AddWithViewport (eventBox);
			(control as Gtk.ScrolledWindow).FocusChain = new Widget[] { eventBox };

			control.ShowAll();

			//eventBox.ExposeEvent+= Expose;
			eventBox.ShowAll();
		}

		#region IEditor implementation

		public void Rename(string newName){

			fileName = newName;
		}

		public bool RefreshSettings(){
			return true;
		}		

		public object GetSelected(){
			return null;
		}		
		
		public bool Save ()
		{
			return true;
			//throw new NotImplementedException ();
		}

		public bool SaveAs (string newPath)
		{
			return true;
			//throw new NotImplementedException ();
		}

		public void Close()
		{

		}

		public bool SearchExpression (SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchNext(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool SearchPreviu(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool Replace(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public bool ReplaceAll(SearchPattern expresion){
			return false;
			//throw new NotImplementedException ();
		}

		public List<FindResult> FindReplaceAll(SearchPattern expresion)
		{
			return null;
		}		

		public bool Undo(){
			return false;
			//throw new NotImplementedException ();
		}

		public bool Redo(){
			return false;
			//throw new NotImplementedException ();
		}

		public string Caption {
			get { return MainClass.Languages.Translate("start_page"); }
		}

		public string FileName {
			get { return fileName; }
		}

		public bool Modified {
			get { return false; }
		}

		public void GoToPosition(object position){
		}

		public Gtk.Widget Control {
			get { return control; }
		}

		public Gtk.ActionGroup EditorAction
		{
			get {return editorAction;}
		}

		public void ActivateEditor(bool updateStatus){

		}

		public event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;
		public event EventHandler<WriteStatusEventArgs> WriteStatusChange;						
		#endregion

		void OnModifiedChanged(bool newModified)
		{
			ModifiedChangedEventArgs mchEventArg = new ModifiedChangedEventArgs(false);//modified);

			if (ModifiedChanged != null)
				ModifiedChanged(this, mchEventArg);
		}
		
		void OnWriteToStatusbar(string message)
		{
			WriteStatusEventArgs mchEventArg = new WriteStatusEventArgs(message);
			if (WriteStatusChange != null)
				WriteStatusChange(this, mchEventArg);
		}

	}
}

