using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;

namespace Moscrif.IDE.Task
{
	public class LogMonitor : Gtk.ScrolledWindow,ITaskOutput
	{
		private ListStore outputModel = new ListStore(typeof(string), typeof(string), typeof(string));
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

		public LogMonitor()
		{
			this.ShadowType = ShadowType.Out;
			treeView = new TreeView();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			
			treeView.Model = outputModel;

			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);

			Gtk.CellRendererText fileNameRenderer = new Gtk.CellRendererText();

			TreeViewColumn tvcName = new TreeViewColumn ("Category",  fileNameRenderer, "text", 0);
			tvcName.MinWidth = 100;
			tvcName.Resizable = true;
			treeView.AppendColumn(tvcName);
			TreeViewColumn tvcMessage = new TreeViewColumn ( "Message",  fileNameRenderer, "text", 1);
			tvcMessage.MinWidth = 100;
			tvcMessage.Resizable = true;
			treeView.AppendColumn(tvcMessage);

			treeView.HeadersVisible = true;
			treeView.EnableTreeLines = true;

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
					string category = outputModel.GetValue(ti, 0).ToString();
					string message = outputModel.GetValue(ti, 1).ToString();

					Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
					string txt =category +"\t"+message;

					clipboard.Text=txt;

				};
				popupMenu.Append(miCopy);

				if(selRow.Length<1){
					miCopy.Sensitive = false;
				}
				popupMenu.Popup();
				popupMenu.ShowAll();
			};

			treeView.RowActivated += new RowActivatedHandler(OnRowActivate);
			treeView.EnableSearch =false;
			treeView.HasFocus = false;
			
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
				//TreeIter iter = outputModel.AppendValues(name, status, "", "");
				
				if (errors != null) {
					foreach (string str in errors)
						outputModel.AppendValues("ERROR", str, "");
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

		//
		protected void UnsafeAddText(TaskResult task)
		{
			try {
				TreeIter iter = TreeIter.Zero;
				//TreeIter iter = outputModel.AppendValues(task.name, task.status, "", "");
				if (task.errors != null) {
					foreach (TaskMessage tm in task.errors){
						bool find = false;
						outputModel.Foreach((model, path, iterr) => {
								string category = outputModel.GetValue(iterr, 0).ToString();
								if (category == tm.File){
										outputModel.SetValue(iterr,1,tm.Message);
										find = true;
										return true;
								}
									return false;
							});
						if(!find)
							iter = outputModel.AppendValues(tm.File, tm.Message, tm.Line);
					}
				}
				ScrolledWindow window = treeView.Parent as ScrolledWindow;
				bool scrollToEnd = true;
				if (window != null) {
					scrollToEnd = window.Vadjustment.Value >= window.Vadjustment.Upper - 2 * window.Vadjustment.PageSize;
				}

				if (scrollToEnd) {
					//it.LineOffset = 0;
					if(!iter.Equals(TreeIter.Zero)){
						TreePath path = outputModel.GetPath(iter);
						treeView.ScrollToCell(path, null, false, 0, 0);
					}

				}
			} catch(Exception ex) {
				Logger.Error(ex.Message,null);
			}
		}

		private abstract class QueuedUpdate
		{
			public abstract void Execute(LogMonitor pad);
		}

		private class QueuedTextWrite : QueuedUpdate
		{
			private TaskResult task;

			public override void Execute(LogMonitor pad)
			{
				pad.UnsafeAddText(task);
			}

			public QueuedTextWrite(TaskResult task)
			{
				this.task = task;
			}

			public void Write(string s)
			{
				Console.WriteLine("ERROR Write  LogMonitor");
				//Text.Append(s);
				//if (Text.Length > MAX_BUFFER_LENGTH)
				///	Text.Remove(0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}
		
	}

}

