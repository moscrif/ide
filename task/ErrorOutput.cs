using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;

namespace Moscrif.IDE.Task
{
	public class ErrorOutput : Gtk.ScrolledWindow,ITaskOutput
	{
		private TreeStore outputModel = new TreeStore(typeof(string), typeof(string), typeof(string), typeof(string));
		private TreeView treeView = null;

		Queue<QueuedUpdate> updates = new Queue<QueuedUpdate>();
		QueuedTextWrite lastTextWrite;
		bool outputDispatcherRunning = false;
		GLib.TimeoutHandler outputDispatcher;

		#region ITaskOutput implementation
		void ITaskOutput.ClearOutput ()
		{
			Clear();
		}
		#endregion

	public ErrorOutput()
		{
			this.ShadowType = ShadowType.Out;
			treeView = new TreeView();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			
			treeView.Model = outputModel;

			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);

			//treeView.AppendColumn("Name", new CellRendererText(), "text", 0);
			//treeView.AppendColumn("Stat", new CellRendererText(), "text", 1);
			//treeView.AppendColumn("Message", new CellRendererText(), "text", 2);
			//treeView.AppendColumn("Place", new CellRendererText(), "text", 3);

			TreeViewColumn tvcName = new TreeViewColumn (MainClass.Languages.Translate("file"),  new CellRendererText(), "text", 0);
			tvcName.MinWidth = 100;
			tvcName.Resizable = true;
			treeView.AppendColumn(tvcName);
			TreeViewColumn tvcState = new TreeViewColumn (MainClass.Languages.Translate("Line"),  new CellRendererText(), "text", 1);
			tvcState.MinWidth = 25;
			tvcState.Resizable = true;
			treeView.AppendColumn(tvcState);
			TreeViewColumn tvcMessage = new TreeViewColumn (MainClass.Languages.Translate("message"),  new CellRendererText(), "text", 2);
			tvcMessage.MinWidth = 100;
			tvcMessage.Resizable = true;
			treeView.AppendColumn(tvcMessage);
			TreeViewColumn tvcPlace = new TreeViewColumn (MainClass.Languages.Translate("location"),  new CellRendererText(), "text", 3);
			tvcPlace.MinWidth = 100;
			tvcPlace.Resizable = true;
			treeView.AppendColumn(tvcPlace);

			treeView.HeadersVisible = true;
			treeView.EnableTreeLines = true;
			
			treeView.RowActivated += new RowActivatedHandler(OnRowActivate);
			treeView.EnableSearch =false;
			treeView.HasFocus = false;


			treeView.ButtonReleaseEvent += delegate(object o, ButtonReleaseEventArgs args) {
				if (args.Event.Button != 3) return;

				Menu popupMenu = new Menu();

				MenuItem miClear = new MenuItem( MainClass.Languages.Translate("clear" ));
				miClear.Activated+= delegate(object sender, EventArgs e) {
					Clear();
				};
				popupMenu.Append(miClear);

				TreeSelection ts = treeView.Selection;
				Gtk.TreePath[] selRow = ts.GetSelectedRows();

				MenuItem miCopy = new MenuItem( MainClass.Languages.Translate("copy_f2" ));
				miCopy.Activated+= delegate(object sender, EventArgs e) {

					Gtk.TreePath tp = selRow[0];
					TreeIter ti = new TreeIter();
					outputModel.GetIter(out ti, tp);
					string type = outputModel.GetValue(ti, 0).ToString();
					string timeStamp = outputModel.GetValue(ti, 1).ToString();
					string message = outputModel.GetValue(ti, 2).ToString();

					Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
					string txt =type +"\t"+timeStamp+"\t"+message;

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
				
				string pFile = outputModel.GetValue(ti, 3).ToString();
				
				string pLine = outputModel.GetValue(ti, 1).ToString();


				if (!String.IsNullOrEmpty(pFile)) {
					int line = 0;
					if(!Int32.TryParse(pLine,out line) ){
						line = 0;
					}
					MainClass.MainWindow.GoToFile(pFile, (object)line);
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
				//TreeIter iter = outputModel.AppendValues(name, status, "", "");
				
				if (errors != null) {
					foreach (string str in errors)
						outputModel.AppendValues("ERROR", "", str);
				}
			}
		}

		public void WriteTask_II(object task, string name, string status, List<TaskMessage> error, bool clearOutput)
		{
			if (clearOutput)
				Clear();
			TaskResult tr = new TaskResult(name, status, error);
			//raw text has an extra optimisation here, as we can append it to existing updates
						/*lock (updates) {
				if (lastTextWrite != null) {
					if (lastTextWrite.Tag == null) {
						lastTextWrite.Write(tr);
						return;
					}
				}
			}*/
			QueuedTextWrite qtw = new QueuedTextWrite(tr);
			AddQueuedUpdate(qtw);
		}

		public void WriteTask_II(object sender, string name, string status,TaskMessage error)
		{
			List<TaskMessage> list = new List<TaskMessage>();
			list.Add(error);
			WriteTask_II(sender, name, status, list, false);
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

		private void AppendChild(TreeIter parentIter, TaskMessage tm){

			/*if ( tm != null){
				TreeIter ti = outputModel.AppendValues(parentIter,System.IO.Path.GetFileName(tm.File), tm.Line, tm.Message, tm.File);
				if (tm.Child != null)
					AppendChild(parentIter,tm.Child);
			}*/
			if ( tm != null){
				outputModel.AppendValues(parentIter,System.IO.Path.GetFileName(tm.File), tm.Line, tm.Message, tm.File);
				if (tm.Child != null){
					AppendChild(parentIter,tm.Child);
				}
			}
		}

		//
		protected void UnsafeAddText(TaskResult task)
		{
			try {
				
				//TreeIter iter = outputModel.AppendValues(task.name, task.status, "", "");
				if (task.errors != null) {
					
					foreach (TaskMessage tm in task.errors){

						if (tm.Child != null){
							//TaskMessage tmChild = tm.Child;
							TreeIter ti = outputModel.AppendValues( System.IO.Path.GetFileName(tm.File), tm.Line, tm.Message, tm.File);
							AppendChild(ti,tm.Child);

						} else {
							if (!String.IsNullOrEmpty(tm.File))
								outputModel.AppendValues( System.IO.Path.GetFileName(tm.File), tm.Line, tm.Message, tm.File);
							else
								outputModel.AppendValues("", tm.Line, tm.Message, "");
						}
					}
				}
				ScrolledWindow window = treeView.Parent as ScrolledWindow;
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
				}
			} catch(Exception ex) {
				Logger.Error(ex.Message,null);
			}
		}

		private abstract class QueuedUpdate
		{
			public abstract void Execute(ErrorOutput pad);
		}

		private class QueuedTextWrite : QueuedUpdate
		{
			private TaskResult task;

			public override void Execute(ErrorOutput pad)
			{
				pad.UnsafeAddText(task);
			}

			public QueuedTextWrite(TaskResult task)
			{
				this.task = task;
			}

			public void Write(string s)
			{
				Console.WriteLine("ERROR Write  ErrorOutput");
				//Text.Append(s);
				//if (Text.Length > MAX_BUFFER_LENGTH)
				///	Text.Remove(0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}
		
	}

}

