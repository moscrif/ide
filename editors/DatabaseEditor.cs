using System;
using Gtk;
//using System.Data;
using Mono.Data.Sqlite;
using Moscrif.IDE.Editors.DatabaseView;
using Gdk;
using System.Runtime;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using System.Collections.Generic;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Editors
{
	public class DatabaseEditor : Frame,IEditor
	{

		private Notebook control = null;

		private Gtk.ActionGroup editorAction = null;
		private string fileName = String.Empty;

		public DatabaseEditor(string filePath)
		{

			fileName = filePath;
			control = new Notebook();

			control.TabPos = PositionType.Left;
			control.AppendPage(new StructureDatabaseView(filePath),new Label(MainClass.Languages.Translate("sql_structure")));
			control.AppendPage(new DataDatabaseView(filePath),new Label(MainClass.Languages.Translate("sql_data")));
			control.AppendPage(new SqlDatabaseView(filePath),new Label(MainClass.Languages.Translate("sql_sql")));

			control.SwitchPage += new SwitchPageHandler(OnSwitchPage);

			control.ShowAll();
		}

		void OnSwitchPage(object o, SwitchPageArgs args) {
			//Console.WriteLine(1);
			Widget wdt = control.CurrentPageWidget;
			if (wdt != null)
				(wdt as IDataBaseView).RefreshData();

		}


		#region IEditor implementation
		public void Rename(string newName){

			fileName = newName;
		}

		public bool RefreshSettings(){
			return true;
		}

		bool IEditor.Save()
		{
			return true;
			//throw new NotImplementedException();
		}

		public object GetSelected(){
			return null;
		}		
		
		public bool SaveAs (string newPath)
		{
			if (System.IO.File.Exists(newPath)) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, "", MainClass.Languages.Translate("overwrite_file", newPath), Gtk.MessageType.Question);
				int result = md.ShowDialog();

				if (result != (int)Gtk.ResponseType.Yes)
					return false;
			}

			try {

				System.IO.File.Copy(fileName,newPath);

				fileName = newPath;

			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", newPath), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
			// do save
			return true;
			//return true;
			//throw new NotImplementedException ();
		}

		public void Close()
		{

		}		

		bool IEditor.Undo()
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.Redo()
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.SearchExpression(SearchPattern expresion)
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.SearchNext(SearchPattern expresion)
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.SearchPreviu(SearchPattern expresion)
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.Replace(SearchPattern expresion)
		{
			return false;
			//throw new NotImplementedException();
		}

		bool IEditor.ReplaceAll(SearchPattern expresion)
		{
			return false;
			//throw new NotImplementedException();
		}

		public List<FindResult> FindReplaceAll(SearchPattern expresion)
		{
			return null;
		}		

		public string Caption
		{
			get { return System.IO.Path.GetFileName(fileName); }
		}

		public string FileName
		{
			get { return fileName; }
		}

		public bool Modified
		{
			get { return false; }
		}

		public void GoToPosition(object position){
		}

		public Gtk.Widget Control
		{
			get { return control; }
		}

		public Gtk.ActionGroup EditorAction
		{
			get { return editorAction; }
		}

		public void ActivateEditor(bool updateStatus){
			
		}

		public event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;
		public event EventHandler<WriteStatusEventArgs> WriteStatusChange;												
		#endregion

		void OnModifiedChanged(bool newModified)
		{
			/*if (newModified != modified)
				modified = newModified;
			else
				return;*/
			
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

