using System;
using System.Linq;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using Gtk;
using System.Collections.Generic;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Iface.Entities;
using System.Threading;

namespace Moscrif.IDE.Editors
{

	public delegate void PageIsChangedHandler (string fileName);

	public class EditorNotebook : DragNotebook
	{
		private List<IEditor> listEditor = null;
		private SearchPattern searchPatern = null;
		public bool OnLoadFinish = false;

		public event PageIsChangedHandler PageIsChanged;

		public EditorNotebook()
		{
			listEditor = new List<IEditor>();
			this.Scrollable = true;
			
			ButtonPressEvent += new ButtonPressEventHandler(OnButtonPress);
			ButtonReleaseEvent += new ButtonReleaseEventHandler(OnButtonRelease);
			this.CanFocus = false;

			this.SwitchPage += new SwitchPageHandler(OnSwitchPage);

		/*	filllStartPageThread = new Thread(new ThreadStart(ExecEditorThread));
			//filllStartPageThread.Priority = ThreadPriority.Normal;
			filllStartPageThread.Name = "ExecEditorThread";
			filllStartPageThread.IsBackground = true;
			//filllStartPageThread.Start();*/

		}

	/*	private void ExecEditorThread(){

			bool play = true;
			bool isBussy = false;
			try {
			//Gtk.Application.Invoke(delegate
			//{
				while (play){
					if ((listEditor != null)&&  !isBussy) {
						isBussy = true;
						//lock (secondTaskList) {
							Widget wdt = this.CurrentPageWidget;
							IEditor se = listEditor.Find(x => x.Control == wdt);
							if(se!=null)
								Console.WriteLine(se.FileName);
						//}
						isBussy = false;
					}
					Thread.Sleep (2000);
				}

			//});
			}catch(ThreadAbortException tae){
				Thread.ResetAbort ();
				Tool.Logger.Error("ERROR - Cannot run editor thread.");
				Tool.Logger.Error(tae.Message);
			}finally{

			}
		}*/

		//[GLib.ConnectBefore]
		void OnSwitchPage(object o, SwitchPageArgs args) {
			IEditor se = CurentEditor();
			if (se != null){
				se.ActivateEditor(OnLoadFinish);
				if(PageIsChanged != null)
					PageIsChanged(se.FileName);
			}
		}

		[GLib.ConnectBefore]
		void OnButtonPress(object obj, ButtonPressEventArgs args)
		{
			int tab = this.FindTabAtPosition(args.Event.XRoot, args.Event.YRoot);
			if (tab < 0)
				return;
			this.CurrentPage = tab;
			//if (e.Event.Type == Gdk.EventType.TwoButtonPress){}
		}

		[GLib.ConnectBefore]
		void OnButtonRelease(object obj, ButtonReleaseEventArgs args)
		{
			int tab = FindTabAtPosition(args.Event.XRoot, args.Event.YRoot);
			if (tab < 0)
				return;
			
			if (args.Event.Button == 3) {
				Menu popupMenu = (Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/netbookPagePopup");
				if (popupMenu != null) {
					popupMenu.ShowAll();
					popupMenu.Popup();
				}
			}
			if (args.Event.Button == 2) {
				IEditor ie = FindEditor(tab);
				if (ie != null)
					this.ClosePage(ie);
			}
		}

		public IEditor Open(string path)
		{
			string extension = System.IO.Path.GetExtension(path);
			if (!System.IO.File.Exists(path) && (path != "StartPage") ){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("file_not_exist"), path, MessageType.Error,null);
				md.ShowDialog();
				return null;
			}

			string ext = System.IO.Path.GetExtension(path);

			/*if(ext.ToLower() ==".ttf"){
				if (!String.IsNullOrEmpty(path)){
					System.Diagnostics.Process.Start(path);
				}
				return null;
			}*/
			IEditor se = FindEditor(path);
			if (se != null) {
				this.CurrentPage = this.PageNum(se.Control);
				return null;
			}

			Option.Settings.ExtensionSetting exSet =MainClass.Tools.FindFileTyp(ext);
			if(exSet == null){
				if (path == "StartPage"){
					se = new StartPage(path);
					//return null;
				} else {//	if (!String.IsNullOrEmpty(path)){
					System.Diagnostics.Process.Start(path);
					return null;
				}

			}
			try {
				if(se == null){
					switch (exSet.OpenType) {
					case Option.Settings.ExtensionSetting.OpenTyp.IMAGE:
						se = new ImageEditor(path);
						break;
					case Option.Settings.ExtensionSetting.OpenTyp.DATABASE:
						se = new DatabaseEditor(path);
						break;
					case Option.Settings.ExtensionSetting.OpenTyp.TEXT:
						se = new SourceEditor(path);
						break;
					case Option.Settings.ExtensionSetting.OpenTyp.SYSTEM:{
						System.Diagnostics.Process.Start(path);
						return null;
					}
					case Option.Settings.ExtensionSetting.OpenTyp.EXTERNAL:{

						MainClass.MainWindow.RunProcess(exSet.ExternalProgram, exSet.Parameter+" "+path, "", false,null);

						return null;
					}
					default:
						se = new SourceEditor(path);
						break;
					}
					/*switch (extension) {
					case ".png":
					case ".jpg":
					case ".jpeg":
					case ".bmp":
					case ".gif":
					case ".tif":
					case ".svg":
						se = new ImageEditor(path);
						break;
					case ".db":
						se = new DatabaseEditor(path);
						break;
					//case ".html":
					//	se = new WebViewer(path);
					//	break;
					default:
						se = new SourceEditor(path);
						break;
					}*/
				}
			} catch(Exception ex) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, ex.Message, path, MessageType.Error,null);
				md.ShowDialog();
				Logger.Error(ex.Message);
				Logger.Error(ex.Source);
				Logger.Error(ex.StackTrace);
				if (ex.InnerException != null){
					Logger.Error(ex.InnerException.Message);
					Logger.Error(ex.InnerException.Source);
					Logger.Error(ex.InnerException.StackTrace);
				}
				return null;
			}
			
			//se.Caption = System.IO.Path.GetFileName(path);
			listEditor.Add(se);
			
			NotebookEditorLabel nl = new NotebookEditorLabel(this, se);

			int i = this.AppendPage(se.Control, nl);

			se.ModifiedChanged += delegate(object sender, ModifiedChangedEventArgs e) {

				//IEditor se = IEditor(sender);
				//NotebookLabel nl =  (NotebookLabel)this.GetTabLabel(se);
				if (nl != null) nl.SetSaveState(e.State);
			};

			se.WriteStatusChange +=  delegate(object sender, WriteStatusEventArgs e) {
				//Console.WriteLine(e.Message);
				MainClass.MainWindow.PushText(e.Message);
			};

			this.CurrentPage = i;
			return se;
		}



		public void GoToFile(string filename, object position){
			IEditor se = FindEditor(filename);

			if (se != null) {
				this.CurrentPage = this.PageNum(se.Control);
				se.GoToPosition(position);
				return;
			} else {

				IEditor se2 =Open(filename);
				se2.ActivateEditor(true);
				GoToCurrentFile(position);
			}
		}

		public void GoToCurrentFile(object position){
			IEditor se = CurentEditor();
			if (se != null) {
				se.GoToPosition(position);
				return;
			}
		}

		public void ClosePage(string fileName, bool forceClose){
			IEditor se =  FindEditor(fileName);//CurentEditor();
			if (se != null) {
				if (forceClose){
					listEditor.Remove(se);
					this.Remove(se.Control);
				} else {
					ClosePage(se);
				}
			}
		}


		public bool ClosePage(IEditor se)
		{
			if (!se.Modified) {
				listEditor.Remove(se);
				se.Close();
				this.Remove(se.Control);
			} else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNoCancel, MainClass.Languages.Translate("save_changes_before_close", se.Caption),  MainClass.Languages.Translate("changes_will_be_lost"), MessageType.Question);

				//MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("save_changes_before_close", se.Caption),  MainClass.Languages.Translate("changes_will_be_lost"), MessageType.Question);
				int response = md.ShowDialog();
				
				if (response == (int)Gtk.ResponseType.Cancel) {
					return false;
					
				} else if (response == (int)Gtk.ResponseType.No) {
					listEditor.Remove(se);
					se.Close();
					this.Remove(se.Control);
				} else if (response == (int)Gtk.ResponseType.Yes) {
					if (Save(se)){
						listEditor.Remove(se);
						se.Close();
						this.Remove(se.Control);
					}
				}
			}
			return true;
		}

		public bool CloseAllPage()
		{
			int count = listEditor.Count;
			for (int i = count - 1; i > -1; i--){
			//	if (listEditor[i].FileName != "StartPage")
				if(!ClosePage(listEditor[i])){
					return false;
				}
			}
			return true;
		}

		public void RefreshSetting(){
			int count = listEditor.Count;
			for (int i = count - 1; i > -1; i--){
			//	if (listEditor[i].FileName != "StartPage")
					listEditor[i].RefreshSettings() ;
			}
		}

		public void CloseCurentPage()
		{
			IEditor se = CurentEditor();
			if (se != null)
				ClosePage(se);
		}

		public void CloseAllButThisPage()
		{
			IEditor se = CurentEditor();
			if (se != null) {
				int count = listEditor.Count;
				for (int i = count - 1; i > -1; i--)
					if (listEditor[i] != se)
						ClosePage(listEditor[i]);
			}
		}

		public object GetSelectedObject(){
			IEditor se = CurentEditor();
			if (se != null){
				return se.GetSelected();
			}
			else return null;

		}

		public void SetSearchExpresion(SearchPattern expresion)
		{
			searchPatern = expresion;
		}

		public void Search(SearchPattern expresion)
		{
			searchPatern = expresion;
			IEditor se = CurentEditor();
			if (se != null)
				se.SearchExpression(expresion);
			
		}

		public void SearchNext()
		{
			IEditor se = CurentEditor();
			if (se != null) {
				//Console.WriteLine(searchPatern.Expresion);
				se.SearchNext(searchPatern);
			}
			
		}

		public void SearchPreviu()
		{
			IEditor se = CurentEditor();
			if (se != null)
				se.SearchPreviu(searchPatern);
			
		}

		public void Replace(SearchPattern expresion)
		{
			IEditor se = CurentEditor();
			if (se != null)
				se.Replace(expresion);
			
		}

		public void ReplaceAll(SearchPattern expresion)
		{
			IEditor se = CurentEditor();
			if (se != null)
				se.ReplaceAll(expresion);
			
		}

		public void Undo()
		{
			IEditor se = CurentEditor();
			if (se != null)
				se.Undo();
		}

		public void Redo()
		{
			IEditor se = CurentEditor();
			if (se != null)
				se.Redo();
		}

		public void SaveCurentPage()
		{
			IEditor se = CurentEditor();
			if (se != null)
				Save(se);
		}

		public void SaveAllPage()
		{
			foreach (IEditor se in listEditor)
				Save(se);
		}

		public void RenameFile(string oldFilePath,string newFilePath){
			IEditor se = FindEditor(oldFilePath);
			if (se != null) {
				se.Rename(newFilePath);

				Widget nl = this.GetTabLabel(se.Control);
				if(  nl!=null && nl.GetType() == typeof(NotebookEditorLabel) )
				{
					(nl as NotebookEditorLabel).SetNewName(newFilePath);
				}
				Save(se);
			}

		}

		public void SaveAs()
		{
			IEditor se = CurentEditor();
			SaveAs(se);
		}

		public Gtk.ActionGroup EditorAction
		{
			get { return CurentEditor().EditorAction; }
		}


		public List<string> OpenFiles
		{
			get { return listEditor.Select(i => i.FileName).ToList(); }
		}


		public string CurrentFile {
			get {
				IEditor se = CurentEditor();
				if (se!= null)
					return se.FileName;
				return null;
			}
		}

		public IEditor FindEditor(string filePath)
		{
			if (MainClass.Platform.IsWindows){
				return listEditor.Find(x => x.FileName.ToLower() == filePath.ToLower());
			} else{
				return listEditor.Find(x => x.FileName == filePath);
			}
		}

		#region private
		private IEditor FindEditor(int i)
		{
			Widget wdt = this.GetNthPage(i);
			IEditor se = listEditor.Find(x => x.Control == wdt);
			return se;
		}

		private IEditor CurentEditor()
		{
			Widget wdt = this.CurrentPageWidget;
			IEditor se = listEditor.Find(x => x.Control == wdt);
			return se;
		}

		private bool Save(IEditor se){

			bool saveStatus =se.Save();

			if (!saveStatus){
				saveStatus = SaveAs(se);
			}
			return saveStatus;
		}

		private bool SaveAs(IEditor se){

			bool saveStatus = false;
			string filePath = se.FileName;

			var dlg = new Gtk.FileChooserDialog("Save as...",MainClass.MainWindow, FileChooserAction.Save,"Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);

			if (filePath != null){
				string fileName = System.IO.Path.GetFileName(filePath);
				string fileDir = System.IO.Path.GetDirectoryName(filePath);

				dlg.CurrentName = fileName;
				dlg.SetCurrentFolder(fileDir);
			}

			if (dlg.Run() == (int)ResponseType.Accept){
					saveStatus= se.SaveAs(dlg.Filename);
					NotebookEditorLabel nl = (NotebookEditorLabel)this.GetTabLabel(se.Control);
					if (nl != null){
						nl.SetNewName(se.FileName);
					}
				}
			dlg.Destroy();
			return saveStatus;
		}
		#endregion
	/*	[GLib.ConnectBefore]
		protected override void OnChangeCurrentPage(int offset)
		{
			base.OnChangeCurrentPage(offset);
			Console.WriteLine(offset);
		}*/

	}
}

