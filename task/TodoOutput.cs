using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;
using System.Linq;

namespace Moscrif.IDE.Task
{
	public class TodoOutput : Gtk.ScrolledWindow,ITaskOutput
	{
		private ListStore outputModel = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(TaskMessage));
		private TreeView treeView = null;

		private object lock_Write = new object();

		//
		Queue<QueuedUpdate> updates = new Queue<QueuedUpdate>();
		QueuedTextWrite lastTextWrite;
		bool outputDispatcherRunning = false;
		GLib.TimeoutHandler outputDispatcher;
		private List<TaskMessage> oldError;


		#region ITaskOutput implementation
		void ITaskOutput.ClearOutput ()
		{
			Clear();
		}
		#endregion

		public TodoOutput()
		{
			this.ShadowType = ShadowType.Out;
			treeView = new TreeView();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			
			treeView.Model = outputModel;

			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);

			Gtk.CellRendererText fileNameRenderer = new Gtk.CellRendererText();

			TreeViewColumn tvcName = new TreeViewColumn (MainClass.Languages.Translate("description"),  fileNameRenderer, "text", 0);
			tvcName.MinWidth = 350;
			tvcName.Resizable = true;
			treeView.AppendColumn(tvcName);

			TreeViewColumn tvcState = new TreeViewColumn (MainClass.Languages.Translate("file"),  fileNameRenderer, "text", 3);
			tvcState.MinWidth = 50;
			tvcState.Resizable = true;
			treeView.AppendColumn(tvcState);

			TreeViewColumn tvcMessage = new TreeViewColumn ( MainClass.Languages.Translate("line"),  fileNameRenderer, "text", 2);
			tvcMessage.MinWidth = 25;
			tvcMessage.Resizable = true;
			tvcMessage.Visible = false;
			treeView.AppendColumn(tvcMessage);

			treeView.HeadersVisible = true;
			treeView.EnableTreeLines = true;

			treeView.RowActivated += new RowActivatedHandler(OnRowActivate);
			treeView.EnableSearch =false;
			treeView.HasFocus = false;

			treeView.ButtonReleaseEvent += delegate(object o, ButtonReleaseEventArgs args) {
				if (args.Event.Button != 3) return;

				Menu popupMenu = new Menu();
				/*
				MenuItem miClear = new MenuItem( MainClass.Languages.Translate("clear" ));
				miClear.Activated+= delegate(object sender, EventArgs e) {
					Clear();
				};
				popupMenu.Append(miClear);*/

				TreeSelection ts = treeView.Selection;
				Gtk.TreePath[] selRow = ts.GetSelectedRows();

				MenuItem miCopy = new MenuItem( MainClass.Languages.Translate("copy_f2" ));
				miCopy.Activated+= delegate(object sender, EventArgs e) {

					Gtk.TreePath tp = selRow[0];
					TreeIter ti = new TreeIter();
					outputModel.GetIter(out ti, tp);
					string type = outputModel.GetValue(ti, 0).ToString();
					string timeStamp = outputModel.GetValue(ti, 1).ToString();
					//string message = outputModel.GetValue(ti, 2).ToString();

					Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
					string txt =type +"\t"+timeStamp;//+"\t"+message;

					clipboard.Text=txt;

				};
				popupMenu.Append(miCopy);

				if(selRow.Length<1){
					miCopy.Sensitive = false;
				}
				popupMenu.Popup();
				popupMenu.ShowAll();
			};

			outputDispatcher = new GLib.TimeoutHandler(OutputDispatchHandler);
			
			this.Add(treeView);

			oldError = new List<TaskMessage>();
			
			this.ShowAll();
		}

		private void OnRowActivate(object o, RowActivatedArgs args)
		{
			TreeIter ti = new TreeIter();
			//filter.GetIter(out ti, args.Path);
			//TreePath childTP = outputModel.ConvertPathToChildPath(args.Path);
			try {
				outputModel.GetIter(out ti, args.Path);
				//if (ti != TreeIter.Zero) {
				
				string pFile = outputModel.GetValue(ti, 1).ToString();
				string pLine = outputModel.GetValue(ti, 2).ToString();
				if (!String.IsNullOrEmpty(pFile)) {
					MainClass.MainWindow.GoToFile(pFile, (object)pLine);
				}
				//}
			} catch {
			}
		}

		public void SetFont(string fontname){
			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);
		}

		public void WriteTask(string name, string status, List<string> errors)
		{
			//raw text has an extra optimisation here, as we can append it to existing updates
			lock (updates) {
				
				if (errors != null) {
					foreach (string str in errors)
						outputModel.AppendValues("ERROR", str, "","");
				}
			}
		}

		public void WriteTask(object task, string name, string status, List<TaskMessage> error, bool clearOutput)
		{
			//if (clearOutput)
			//	Clear();

			//Console.WriteLine("WriteTask error ->{0}",error.Count);
			//TaskResult tr = new TaskResult(name, status, error);
			//raw text has an extra optimisation here, as we can append it to existing updates
						/*lock (updates) {
				if (lastTextWrite != null) {
					if (lastTextWrite.Tag == null) {
						lastTextWrite.Write(tr);
						return;
					}
				}
			}*/

			QueuedTextWrite qtw = new QueuedTextWrite(error);
			AddQueuedUpdate(qtw);

		}

		public void WriteTask(object sender, string name, string status,TaskMessage error)
		{
			List<TaskMessage> list = new List<TaskMessage>();
			list.Add(error);
			WriteTask(sender, name, status, list, false);
		}

		public void Clear()
		{
			
			lock (updates) {
				updates.Clear();
				lastTextWrite = null;
				outputDispatcherRunning = false;
			}
			outputModel.Clear();
		}

		//**//
		void AddQueuedUpdate(QueuedUpdate update)
		{
			lock (updates) {
				updates.Enqueue(update);
				if (!outputDispatcherRunning) {
					GLib.Timeout.Add(50, outputDispatcher);
					outputDispatcherRunning = true;
				}
				lastTextWrite = update as QueuedTextWrite;
			}
		}

		bool OutputDispatchHandler()
		{
			lock (updates) {
				lastTextWrite = null;
				if (updates.Count == 0) {
					outputDispatcherRunning = false;
					return false;
				} else if (!outputDispatcherRunning) {
					updates.Clear();
					return false;
				} else {
					while (updates.Count > 0) {
						QueuedUpdate up = updates.Dequeue();
						up.Execute(this);
					}
				}
			}
			return true;
		}

		//
		protected void UnsafeAddText(List<TaskMessage> list)
		{
			try {

				TreeSelection ts = treeView.Selection;
				Gtk.TreePath[] selRow = ts.GetSelectedRows();
				bool isSelected = false;
				Gtk.TreePath tpSelected = null;

				if(selRow.Length>0){
					tpSelected = selRow[0];
					isSelected = true;
				}


				List<TaskMessage> addRecord = new List<TaskMessage>();
				List<TaskMessage> removeRecord= new List<TaskMessage>();
				if(oldError.Count > 0){
					addRecord = new List<TaskMessage>(list.Except(oldError).ToArray());
					removeRecord = new List<TaskMessage>(oldError.Except(list).ToArray());

					foreach(TaskMessage ti in removeRecord)
						oldError.Remove(ti);

					oldError.AddRange(addRecord);

				} else {
					oldError.AddRange(list);
					addRecord = list;
				}

				if(removeRecord.Count()>0){
					List<TreeIter> listIterr = new List<TreeIter>();
					outputModel.Foreach((model, path, iterr) =>
					{
						TaskMessage tm = (TaskMessage)outputModel.GetValue(iterr, 4);
						//Console.WriteLine("tm.->"+tm.Message);
	
						if(removeRecord.Contains(tm)){
							//Console.WriteLine("Find TM - >"+tm.Message);
							listIterr.Add(iterr);
						}

						if(path == tpSelected)
							isSelected = false;
	
						return false;
					});
	
					foreach(TreeIter ti in listIterr){
						TreeIter ti2 =ti;
						outputModel.Remove(ref ti2);
					}
				}

				if (addRecord != null) {
					
					foreach (TaskMessage tm in addRecord){
						//Console.WriteLine("UnsafeAddText->"+tm.Message);
						string message = tm.Message.Replace("\r","");
						message = message.Replace("\n","");
						message = message.Replace("//","");
						message = message.Replace(@"/*","");
						message = message.Replace(@"*/","");
						message= message.TrimStart();

						outputModel.AppendValues(message,tm.File, tm.Line, System.IO.Path.GetFileName(tm.File),tm);
					}
				}
				if(isSelected)
					treeView.Selection.SelectPath(tpSelected);

				/*ScrolledWindow window = treeView.Parent as ScrolledWindow;
				bool scrollToEnd = true;
				if (window != null) {
					scrollToEnd = window.Vadjustment.Value >= window.Vadjustment.Upper - 2 * window.Vadjustment.PageSize;
				}
				//if (extraTag != null)
				//	buffer.InsertWithTags(ref it, text, tag, extraTag);
				//else
				//	buffer.InsertWithTags(ref it, text, tag);
				
				if (scrollToEnd) {
					//it.LineOffset = 0;

					//TreePath path = outputModel.GetPath(iter);
					//treeView.ScrollToCell(path, null, false, 0, 0);

					//outputModel.MoveBefore(iter,TreeIter.Zero);
					//buffer.MoveMark(endMark, it);
					//treeView.ScrollToCell() ScrollToMark(endMark, 0, false, 0, 0);
				}*/
			} catch(Exception ex) {
				Logger.Error(ex.Message,null);
			}
		}

		private abstract class QueuedUpdate
		{
			public abstract void Execute(TodoOutput pad);
		}

		private class QueuedTextWrite : QueuedUpdate
		{
			private List<TaskMessage> list;

			public override void Execute(TodoOutput pad)
			{
				pad.UnsafeAddText(list);
			}

			public QueuedTextWrite(List<TaskMessage> list)
			{
				this.list = list;
			}

			public void Write(string s)
			{
				Console.WriteLine("ERROR Write  TODOOutput");
				//Text.Append(s);
				//if (Text.Length > MAX_BUFFER_LENGTH)
				///	Text.Remove(0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}
		
	}

}

