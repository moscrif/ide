using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;

namespace Moscrif.IDE.Task
{
	public class LogGarbageCollector : Gtk.ScrolledWindow,ITaskOutput
	{
		private ListStore outputModel = new ListStore(typeof(string));
		private TreeView treeView = null;

		public int CountItem = 0;

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

		public LogGarbageCollector()
		{
			this.ShadowType = ShadowType.Out;
			treeView = new TreeView();
			treeView.Selection.Mode = Gtk.SelectionMode.Single;
			
			treeView.Model = outputModel;

			FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			treeView.ModifyFont(customFont);

			treeView.AppendColumn(MainClass.Languages.Translate("message"), new CellRendererText(), "text", 0);

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
					string message = outputModel.GetValue(ti, 0).ToString();

					Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
					string txt =message;

					clipboard.Text=txt;

				};
				popupMenu.Append(miCopy);

				if(selRow.Length<1){
					miCopy.Sensitive = false;
				}
				popupMenu.Popup();
				popupMenu.ShowAll();
			};

			//treeView.RowActivated += new RowActivatedHandler(OnRowActivate);
			treeView.EnableSearch =false;
			treeView.HasFocus = false;
			
			outputDispatcher = new GLib.TimeoutHandler(OutputDispatchHandler);
			
			this.Add(treeView);
			
			this.ShowAll();
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
			CountItem = 0;
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
						CountItem++;
						iter= outputModel.AppendValues(tm.Message);
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
			public abstract void Execute(LogGarbageCollector pad);
		}

		private class QueuedTextWrite : QueuedUpdate
		{
			private TaskResult task;

			public override void Execute(LogGarbageCollector pad)
			{
				pad.UnsafeAddText(task);
			}

			public QueuedTextWrite(TaskResult task)
			{
				this.task = task;
			}

			public void Write(string s)
			{
				Console.WriteLine("ERROR Write  LogGarbageCollector");
				//Text.Append(s);
				//if (Text.Length > MAX_BUFFER_LENGTH)
				///	Text.Remove(0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}
		
	}

}

