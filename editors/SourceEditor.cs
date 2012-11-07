using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Timers;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using Mono.TextEditor.Theatrics;
using Mono.TextEditor.Vi;
using Mono.TextEditor.PopupWindow;
using Moscrif.IDE.Components;
using Moscrif.IDE.Actions;
using Moscrif.IDE.Task;
using Moscrif.IDE.Editors.SourceEditorActions;
using IdeBookmarkActions = Moscrif.IDE.Editors.SourceEditorActions.BookmarkActions;
using IdeBreakpointActions = Moscrif.IDE.Editors.SourceEditorActions.BreakpointActions;
using IdeAutocompleteAction = Moscrif.IDE.Editors.SourceEditorActions.AutoCompleteActions;
using IdeEditAction = Moscrif.IDE.Editors.SourceEditorActions.EditAction;

using Moscrif.IDE.Completion;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Iface.Entities;

using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;



namespace Moscrif.IDE.Editors
{
	public class SourceEditor : Gtk.Frame, IEditor
	{
		//,ICompletionWidget
		// Mozno zbytocne, pozri Poznamka1
		private string fileName = String.Empty;
		private bool modified = false;
		//private TypeEditor typeEditor = TypeEditor.TextEditor;
		private TextEdit editor = null;
		private Gtk.Widget control = null;
		private Gtk.ActionGroup editorAction = null;
		private bool isCompileExtension = false;

		//private string statusFormat ="Ln: {0}; Col: {1}";
		private string statusFormat ="Ln: {0}; Col: {1}; In: {2}";

		private List<ErrorMarker> errors;

		//private CodeCompletionContext currentCompletionContext;

		private DateTime lastPrecompile;
		private Timer timer = new Timer();

		private IdeBookmarkActions bookmarkActions;
		private IdeBreakpointActions breakpointActions;
		private IdeAutocompleteAction autoCompleteActions;
		private IdeEditAction editAction;

		private bool onlyRead = false;

		private SearchPattern searchPattern;

		private FileSetting fileSeting;

		public SourceEditor(string filePath)
		{
			if(MainClass.Settings.SourceEditorSettings == null){
				MainClass.Settings.SourceEditorSettings = new  Moscrif.IDE.Settings.Settings.SourceEditorSetting();
			}

			errors = new List<ErrorMarker>();
			lastPrecompile = DateTime.Now;
			control = new Gtk.ScrolledWindow();
			(control as Gtk.ScrolledWindow).ShadowType = Gtk.ShadowType.Out;
			
			editorAction = new Gtk.ActionGroup("sourceeditor");
			searchPattern = null;
			
			editor = new TextEdit();

			LoadSetting();

			SyntaxMode mode = new SyntaxMode();

			string extension = System.IO.Path.GetExtension(filePath);
			
			switch (extension) {
			case ".ms":
			case ".mso":
				{
					try{
						mode = SyntaxModeService.GetSyntaxMode("text/moscrif");
					}catch(Exception ex){
						MessageDialogs msd = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_load_syntax"), MainClass.Languages.Translate("error_load_syntax_f1"), Gtk.MessageType.Error,null);
						msd.ShowDialog();
						Tool.Logger.Error(ex.Message);
					}
					isCompileExtension = true;
					break;
				}
			case ".xml":
				{
					mode = SyntaxModeService.GetSyntaxMode("application/xml");
					break;
				}
			case ".txt":
			case ".app":
				break;
			default:
				break;
			}

			//editor.Document.
			editor.Document.SyntaxMode = mode;
			//modified = true;
			editor.Document.LineChanged += delegate(object sender, LineEventArgs e) {
				OnBookmarkUpdate();
				OnModifiedChanged(true);
			};

			editor.Caret.PositionChanged+= delegate(object sender, DocumentLocationEventArgs e) {
				OnWriteToStatusbar(String.Format(statusFormat,editor.Caret.Location.Line+1,editor.Caret.Location.Column,editor.Caret.Offset));
			};
			
			FileAttributes fa = File.GetAttributes(filePath);
			if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
				onlyRead = true;
				//editor.Document.ReadOnly= true;
			}
			
			try {
				using (StreamReader file = new StreamReader(filePath)) {
					editor.Document.Text = file.ReadToEnd();
					file.Close();
					file.Dispose();
				}
			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("file_cannot_open", filePath), ex.Message, Gtk.MessageType.Error,null);
				ms.ShowDialog();
				return;
			}

			//Console.WriteLine(editor.Document.Text.Replace("\r","^").Replace("\n","$"));
			
			(control as Gtk.ScrolledWindow).Add(editor);
			control.ShowAll();
			fileName = filePath;

			fileSeting = MainClass.Workspace.WorkspaceUserSetting.FilesSetting.Find(x=> x.FileName == fileName);

			if(fileSeting == null)
				fileSeting = new FileSetting( fileName );
			
			editor.TextViewMargin.ButtonPressed += OnTextMarginButtonPress;
			editor.IconMargin.ButtonPressed += OnIconMarginButtonPress;
			//editor.KeyPressEvent += OnKeyPressEvent;
			editor.KeyReleaseEvent += OnKeyReleaseEvent;
			editor.ButtonPressEvent += OnButtonPressEvent;
			
			bookmarkActions = new IdeBookmarkActions(editor,fileSeting);
			breakpointActions = new IdeBreakpointActions(editor);
			autoCompleteActions = new IdeAutocompleteAction(editor, editor);
			editAction = new IdeEditAction(editor);

			Gtk.Action act = new Gtk.Action("sourceeditor_togglebookmark", "");
			act.Activated += bookmarkActions.ToggleBookmark;
			editorAction.Add(act);
			
			Gtk.Action act2 = new Gtk.Action("sourceeditor_clearbookmark", "");
			act2.Activated += bookmarkActions.ClearBookmarks;
			editorAction.Add(act2);
			
			Gtk.Action act3 = new Gtk.Action("sourceeditor_nextbookmark", "");
			act3.Activated += bookmarkActions.NextBookmark;
			editorAction.Add(act3);
			
			Gtk.Action act4 = new Gtk.Action("sourceeditor_prevbookmark", "");
			act4.Activated += bookmarkActions.PreviousBookmark;
			editorAction.Add(act4);
			
			Gtk.Action act5 = new Gtk.Action("sourceeditor_addbreakpoint", "");
			act5.Activated += breakpointActions.AddBreakpoints;
			editorAction.Add(act5);
			
			Gtk.Action act6 = new Gtk.Action("sourceeditor_inserttemplate", "");
			act6.Activated += autoCompleteActions.InsertTemplate;
			editorAction.Add(act6);
			
			Gtk.Action act7 = new Gtk.Action("sourceeditor_insertautocomplete", "");
			act7.Activated += autoCompleteActions.InsertCompletion;
			editorAction.Add(act7);

			Gtk.Action act8 = new Gtk.Action("sourceeditor_pasteClipboard", "");
			act8.Activated += editAction.PasteText;
			editorAction.Add(act8);

			Gtk.Action act9 = new Gtk.Action("sourceeditor_copyClipboard", "");
			act9.Activated += editAction.CopyText;
			editorAction.Add(act9);

			Gtk.Action act10 = new Gtk.Action("sourceeditor_cutClipboard", "");
			act10.Activated += editAction.CutText;
			editorAction.Add(act10);

			Gtk.Action act11 = new Gtk.Action("sourceeditor_gotoDefinition", "");
			act11.Activated += editAction.GoToDefinition;
			editorAction.Add(act11);

			Gtk.Action act12 = new Gtk.Action("sourceeditor_commentUncomment", "");
			act12.Activated += editAction.CommentUncomment;
			editorAction.Add(act12);

			List<FoldSegment> list = editor.GetFolding();

			foreach(SettingValue sv in fileSeting.Folding){
				FoldSegment foldS = list.Find(x=>x.Offset.ToString() == sv.Display);
				if(foldS != null){
					bool isfolding = false;
					if( Boolean.TryParse(sv.Value, out isfolding))
						foldS.IsFolded = isfolding;
				}
			}

			this.editor.Document.UpdateFoldSegments(list,true);

			//foreach (int bm in fileSeting.Bookmarks){
			foreach (MyBookmark bm in fileSeting.Bookmarks2){
				LineSegment ls = this.editor.Document.GetLine(bm.Line);
				if(ls != null)
					ls.IsBookmarked = true;
				//this.editor.Document.Lines[bm].IsBookmarked = true;
			}
		}
		
		/*
			int j = 1;
			for (int i = 1; i <= insertObject.Document.LineCount; i++) {
				LineSegment ls = insertObject.Document.GetLine(i);
				if (ls != null) {

					if (j == 1) {
						ErrorMarker er = new ErrorMarker(ls);
						er.AddToLine(insertObject.Document);
						j++;
					} else if (j == 2) {
						TextMarker bm = new BreakpointTextMarker(insertObject, false);
						//DebugTextMarker tm = new DebugTextMarker((insertObject as TextEditor));

						insertObject.Document.AddMarker(ls, bm);
						insertObject.QueueDraw();
						j++;
					} else {
						if (ls.IsBookmarked != true) {
							//int lineNumber = insertObject.Document.OffsetToLineNumber (ls.Offset);
							ls.IsBookmarked = true;
							insertObject.Document.RequestUpdate(new LineUpdate(i));
							insertObject.Document.CommitDocumentUpdate();
						}
						j = 1;
					}
				}
			}*/

		void OnTextMarginButtonPress(object s, MarginMouseEventArgs args)
		{
			if (args.Button == 3){
				Selection sel =  editor.MainSelection;

				editor.Caret.Line = args.LineNumber;
				DocumentLocation dl = editor.VisualToDocumentLocation(args.X, args.Y);
				editor.Caret.Location = dl;

				if((sel!= null) && (args.LineNumber >= sel.MinLine && args.LineNumber <= sel.MaxLine))
					editor.MainSelection = sel;

				if (args.LineSegment != null) {
					Gtk.Menu popupMenu = (Gtk.Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/textMarginPopup");
					if (popupMenu != null) {
						Gtk.Action act = MainClass.MainWindow.ActionUiManager.FindActionByName("gotodefinition");

						if (act!= null){
							act.Visible = false;

							string caretWord =  editor.GetCarretWord();
							//Console.WriteLine("caretWord ->"+caretWord );
							if(!String.IsNullOrEmpty(caretWord)){
								int indx = MainClass.CompletedCache.ListDataTypes.FindIndex(x=>x.DisplayText == caretWord);
								if(indx>-1){
									act.Visible = true;
								}

							}
							//act.Visible = false;
							//act.Sensitive = false;
						}
						//popupMenu.ShowAll();
						popupMenu.Popup();
					}
				}
			}
		}

		//private TaskList tl = new TaskList();

		[GLib.ConnectBefore]
		void OnKeyReleaseEvent(object s, Gtk.KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Left || args.Event.Key == Gdk.Key.Right || args.Event.Key == Gdk.Key.Up || args.Event.Key == Gdk.Key.Down || args.Event.Key == Gdk.Key.Control_L || args.Event.Key == Gdk.Key.Control_R)
				return;

			//return;
			if (MainClass.Settings.PreCompile && isCompileExtension ) {
				DateTime now = DateTime.Now;
				TimeSpan ts = now.Subtract(lastPrecompile);

					if (ts.TotalMilliseconds > 5500) {

					//MainClass.MainWindow.ErrorOutput.Clear();
					TaskList tl = new TaskList();
					tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
					
					PrecompileTask pt = new PrecompileTask();
					pt.Initialize(new PrecompileData(editor.Document.Text, fileName));
					
					tl.TasksList.Clear();
					tl.TasksList.Add(pt);
					
					MainClass.MainWindow.RunSecondaryTaskList(tl, EndTaskWritte,true);
					lastPrecompile = DateTime.Now;
					timer.Enabled = false;
				}
			}
		}


		private void OnTimeElapsed(object o, ElapsedEventArgs args)
		{
			DateTime now = DateTime.Now;
			TimeSpan ts = now.Subtract(lastPrecompile);
			
			if (ts.TotalMilliseconds > 5000) {
				timer.Enabled = false;
				TaskList tl = new TaskList();
				tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
				
				PrecompileTask pt = new PrecompileTask();
				pt.Initialize(new PrecompileData(editor.Document.Text, fileName));
				
				tl.TasksList.Clear();
				tl.TasksList.Add(pt);
				
				MainClass.MainWindow.RunSecondaryTaskList(tl, EndTaskWritte,true);
				lastPrecompile = DateTime.Now;
			}
		}

		
		public void OnBookmarkUpdate ()
		{
			//FileSetting fs = MainClass.Workspace.WorkspaceUserSetting.FilesSetting.Find(x=> x.FileName == fileName);

			fileSeting.Bookmarks2 = new List<MyBookmark>();//new List<int>();

			foreach (LineSegment ls in this.editor.Document.Lines){
				if(ls.IsBookmarked){
					int lineNumber = this.editor.Document.OffsetToLineNumber(ls.Offset);
					string text = this.editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);
					fileSeting.Bookmarks2.Add(new MyBookmark(lineNumber,text.Trim()));
				}
			}

			MainClass.MainWindow.BookmarkOutput.RefreshBookmark();
		}

		public void EndTaskWritte(object sender, string name, string status, List<TaskMessage> taskMessage)
		{

			Gtk.Application.Invoke(delegate
			{
				try {
					foreach (ErrorMarker er in errors)
						er.RemoveFromLine(editor.Document);//editor.Document.RemoveMarker(er.Line,typeof(ErrorMarker));

					errors.Clear();
					
					if (taskMessage != null) {
						foreach (TaskMessage tm in taskMessage) {
							LineSegment ls = editor.Document.GetLine(Convert.ToInt32(tm.Line) - 1);
							ErrorMarker er = new ErrorMarker(ls);
							er.AddToLine(editor.Document);
							errors.Add(er);
						}
					} else {
						Console.WriteLine("Output NULL");

					}
				} catch (Exception ex) {
					//Tool.Logger.Error(ex.Message, null);
					Tool.Logger.Error(ex.Message);
					Tool.Logger.Error(ex.StackTrace);
					Tool.Logger.Error(ex.Source);
				}
			});
			//MainClass.MainWindow.TaskOutput.WriteTask_II(sender, name, status, taskMessage);
			
			//MainClass.MainWindow.EndTaskWritte(sender,name,status,taskMessage);
		}


		private void LoadSetting(){

			TextEditorOptions options = new TextEditorOptions();
			options.ColorScheme = "Moscrif";
			options.ShowInvalidLines = true;
			options.ShowLineNumberMargin = true;
			options.AutoIndent = true;
			options.TabSize = MainClass.Settings.SourceEditorSettings.TabSpace;//8;
			options.TabsToSpaces= MainClass.Settings.SourceEditorSettings.TabsToSpaces;

			options.ShowEolMarkers =MainClass.Settings.SourceEditorSettings.ShowEolMarker;
			options.ShowRuler =MainClass.Settings.SourceEditorSettings.ShowRuler;
			options.RulerColumn = MainClass.Settings.SourceEditorSettings.RulerColumn;
			options.ShowTabs=MainClass.Settings.SourceEditorSettings.ShowTab;
			options.ShowSpaces=MainClass.Settings.SourceEditorSettings.ShowSpaces;

			options.EnableAnimations=MainClass.Settings.SourceEditorSettings.EnableAnimations;
			options.ShowLineNumberMargin =MainClass.Settings.SourceEditorSettings.ShowLineNumber;

			options.HighlightCaretLine = true;

			options.DefaultEolMarker = "\n";
			options.OverrideDocumentEolMarker= true;

			if (!String.IsNullOrEmpty(MainClass.Settings.SourceEditorSettings.EditorFont))
				options.FontName = MainClass.Settings.SourceEditorSettings.EditorFont;

			try{
				Mono.TextEditor.Highlighting.Style style = SyntaxModeService.GetColorStyle(null, "Moscrif");
			}catch(Exception ex){
				options.ColorScheme = "";

				//MessageDialogs msd = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_load_styles"),ex.Message, Gtk.MessageType.Error,null);
				//msd.ShowDialog();
				Tool.Logger.Error(MainClass.Languages.Translate("error_load_styles"));
				Tool.Logger.Error(ex.Message);
			}
			editor.Options = options;
			//
			editor.Options.ShowFoldMargin = true;
		}

		/*
		public void RunSecondaryTaskList(TaskList tasklist){

			MainClass.TaskServices.RunSecondaryTastList(tasklist,EndTaskWritte);//);null
			//tasklist.ExecuteTask();
		}*/

		/*	[GLib.ConnectBefore]
		void OnKeyPressEvent(object s, KeyPressEventArgs args)
		{

		}*/

		void OnButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			//if (currentCompletionContext != null) {
			CompletionWindowManager.HideWindow();
			//}
		}

		void OnIconMarginButtonPress(object s, MarginMouseEventArgs args)
		{
			if (args.Button == 3) {
				editor.Caret.Line = args.LineNumber;
				editor.Caret.Column = 1;
				//IdeApp.CommandService.ShowContextMenu ("/MonoDevelop/SourceEditor2/IconContextMenu/Editor");
				Gtk.Menu popupMenu = (Gtk.Menu)MainClass.MainWindow.ActionUiManager.GetWidget("/iconMarginPopup");
				if (popupMenu != null) {
					popupMenu.ShowAll();
					popupMenu.Popup();
				}
				
			} else if (args.Button == 1)
				if (!string.IsNullOrEmpty(FileName)) {
					if (args.LineSegment != null) {
						//DebuggingService.Breakpoints.Toggle (this.Document.FileName, args.LineNumber + 1);
						//AddBreakpoints(args.LineSegment);
					}
				}
		}


		private void AddBreakpoints(LineSegment line)
		{
			if (line != null) {
				TextMarker bm = new BreakpointTextMarker(editor, false);
				editor.Document.AddMarker(line, bm);
				editor.QueueDraw();
			}
			
		}

		private void SetSearchPattern(SearchPattern sp)
		{
			searchPattern = sp;
			Mono.TextEditor.SearchRequest sr = new Mono.TextEditor.SearchRequest();
			sr.CaseSensitive = searchPattern.CaseSensitive;
			sr.SearchPattern = searchPattern.Expresion.ToString();
			sr.WholeWordOnly = searchPattern.WholeWorlds;
			
			this.editor.SearchEngine.SearchRequest = sr;
		}

		void GotoResult(SearchResult result)
		{
			try {
				if (result == null) {
					this.editor.ClearSelection();
					return;
				}
				this.editor.Caret.Offset = result.Offset;
				this.editor.SetSelection(result.Offset, result.EndOffset);
				this.editor.CenterToCaret();
				this.editor.AnimateSearchResult(result);
				
				//this.editor.QueueDraw ();
				
			} catch (System.Exception) {
			}
		}


		#region IEditor

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
			get { return modified; }
		}


		public bool RefreshSettings(){

			LoadSetting();

			return true;
		}

		public void Rename(string newName){

			fileName = newName;
		}

		public void GoToPosition(object position)
		{
			int pos = 0;
			if( position.GetType() == typeof(string)){ // offset
				Console.WriteLine("string");
				if (Int32.TryParse(position.ToString(), out pos)) {
					position = this.editor.Document.OffsetToLineNumber(pos)+1;
					//position=position+1;
				}
			}
			// line
			if (Int32.TryParse(position.ToString(), out pos)) {
				int line = Convert.ToInt32(pos);
				
				if (line < 0)
					line = 0;
				if (line > editor.Document.LineCount)
					line = editor.Document.LineCount;
				
				Caret caret = editor.Caret;
				
				DocumentLocation dl = new DocumentLocation(line - 1, 0);
				caret.Location = dl;
				editor.ScrollToCaret();
			}
			
		}

		public object GetSelected(){
			string selTxt = this.editor.SelectedText;
			if(String.IsNullOrEmpty(selTxt))
				return null;
			else return selTxt;
			
		}		

		public bool SearchExpression(SearchPattern expresion)
		{
			this.editor.HighlightSearchPattern = true;
			this.editor.TextViewMargin.RefreshSearchMarker();
			
			SetSearchPattern(expresion);

			//Mono.TextEditor.SearchResult ser = this.editor.FindNext(true);
			SearchResult sr = this.editor.SearchForward(this.editor.Document.LocationToOffset(this.editor.Caret.Location));
			
			GotoResult(sr);
			//this.editor.GrabFocus ();
			return true;
			
		}

		public bool SearchNext(SearchPattern expresion)
		{
			if (expresion != null) {
				SetSearchPattern(expresion);
				this.editor.FindNext(true);
				return true;
			}
			return false;
		}

		public bool SearchPreviu(SearchPattern expresion)
		{
			if (expresion != null) {
				SetSearchPattern(expresion);
				this.editor.FindPrevious(true);
				return true;
			}
			return false;
		}

		public bool Replace(SearchPattern expresion)
		{
			if (expresion != null) {
				SetSearchPattern(expresion);
				return this.editor.Replace(expresion.ReplaceExpresion.ToString());
				
			}
			return false;
		}

		public bool ReplaceAll(SearchPattern expresion)
		{
			if (expresion != null) {
				SetSearchPattern(expresion);
				
				int number = this.editor.ReplaceAll(expresion.ReplaceExpresion.ToString());
				this.editor.QueueDraw();
				if (number == 0) {
					return false;
				}
				return true;
			}
			return false;
		}

		public List<FindResult> FindReplaceAll(SearchPattern searchPattern)
		{
			return this.editor.FindReplaceAll(searchPattern);
		}


		/*			if((fa & FileAttributes.Directory) == FileAttributes.ReadOnly)
				editor.Document.ReadOnly= true;*/
		public bool Save()
		{
			if (onlyRead)
				return false;
			
			if (!modified)
				return true;
			try {
				using (StreamWriter file = new StreamWriter(fileName)) {
					file.Write(editor.Document.Text);
					file.Close();
					file.Dispose();
				}
				OnModifiedChanged(false);
				
			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file",fileName), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
			// do save
			return true;
			// alebo true ak je OK
		}


		public bool SaveAs(string newPath)
		{
			
			if (File.Exists(newPath)) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, "", MainClass.Languages.Translate("overwrite_file", newPath), Gtk.MessageType.Question);
				int result = md.ShowDialog();
				
				if (result != (int)Gtk.ResponseType.Yes)
					return false;
			}
			
			try {
				using (StreamWriter file = new StreamWriter(newPath)) {
					file.Write(editor.Document.Text);
					file.Close();
					file.Dispose();
				}
				OnModifiedChanged(false);
				
				fileName = newPath;
				onlyRead = false;
				
			} catch (Exception ex) {
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", newPath), ex.Message, Gtk.MessageType.Error);
				ms.ShowDialog();
				return false;
			}
			// do save
			return true;
			// alebo true ak je OK
		}

		public void Close()
		{

			if(MainClass.Workspace != null){

				if(MainClass.Workspace.WorkspaceUserSetting.FilesSetting == null)
					MainClass.Workspace.WorkspaceUserSetting.FilesSetting = new List<FileSetting>();

				//FileSetting fs = MainClass.Workspace.WorkspaceUserSetting.FilesSetting.Find(x=> x.FileName == fileName);

				if(fileSeting!=null)
					MainClass.Workspace.WorkspaceUserSetting.FilesSetting.Remove(fileSeting);

				fileSeting = new FileSetting( fileName );

				foreach (FoldSegment fold in this.editor.Document.FoldSegments){
					SettingValue sv = new SettingValue(fold.IsFolded.ToString(),fold.Offset.ToString());
					fileSeting.Folding.Add(sv);
				}

				fileSeting.Bookmarks2 = new List<MyBookmark>(); //new List<int>();

				foreach (LineSegment ls in this.editor.Document.Lines){
					if(ls.IsBookmarked){
						int lineNumber = this.editor.Document.OffsetToLineNumber(ls.Offset);
						string text = this.editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);
						fileSeting.Bookmarks2.Add(new MyBookmark(lineNumber,text.Trim()));

					}
				}

				MainClass.Workspace.WorkspaceUserSetting.FilesSetting.Add(fileSeting);
				//this.editor.IconMargin.
			}

		}

		public bool Undo()
		{
			if (!editor.Document.CanUndo)
				return true;
			try {
				editor.Document.Undo();
			} catch (Exception ex) {
				throw ex;
				//return false;
			}
			// do save
			return true;
			// alebo true ak je OK
		}

		public bool Redo()
		{
			if (!editor.Document.CanRedo)
				return true;
			try {
				editor.Document.Redo();
			} catch (Exception ex) {
				throw ex;
				//return false;
			}
			// do save
			return true;
			// alebo true ak je OK
		}

		public Gtk.Widget Control
		{
			get { return control; }
		}

		public Gtk.ActionGroup EditorAction
		{
			get { return editorAction; }
		}

		public event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;
		public event EventHandler<WriteStatusEventArgs> WriteStatusChange;			

		void OnModifiedChanged(bool newModified)
		{
			if (newModified != modified)
				modified = newModified;
			else
				return;
			
			ModifiedChangedEventArgs mchEventArg = new ModifiedChangedEventArgs(modified);
			
			if (ModifiedChanged != null)
				ModifiedChanged(this, mchEventArg);
		}
		
		void OnWriteToStatusbar(string message)
		{
			WriteStatusEventArgs mchEventArg = new WriteStatusEventArgs(message);
			if (WriteStatusChange != null)
				WriteStatusChange(this, mchEventArg);
		}		

		public void ActivateEditor(bool updateStatus){
			editor.ShowWidget();
			if(updateStatus){
				OnWriteToStatusbar(String.Format(statusFormat,editor.Caret.Location.Line+1,editor.Caret.Location.Column,editor.Caret.Offset));
			}
		}

		#endregion
	}
}

