using System;
using System.IO;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Task;

namespace Moscrif.IDE.Actions
{
	public class RunEmulatorDebugAction : Gtk.Action
	{
		public RunEmulatorDebugAction() : base("rundebug", MainClass.Languages.Translate("menu_rundebug"), MainClass.Languages.Translate("menu_title_rundebug"), "run.png")
		{

		}
		protected override void OnActivated ()
		{
			base.OnActivated();

			if (MainClass.Settings.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();

			if(MainClass.Settings.SaveChangesBeforeRun){
				Tool.Logger.LogDebugInfo("SAVE PAGES");
				MainClass.MainWindow.EditorNotebook.SaveAllPage();
				MainClass.MainWindow.SaveWorkspace();
			}			

			MainClass.MainWindow.ClearOutput();

			TaskList tl = new TaskList();
			tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
			RunEmulatorDebugTask ret = new RunEmulatorDebugTask();
			ret.LogMonitor +=  new ProcessErrorHandler(MainClass.MainWindow.MonitorTaskWritte);
			ret.LogGarbageCollector += new ProcessErrorHandler(MainClass.MainWindow.GcTaskWritte);

			tl.TasksList.Add(ret);

			MainClass.MainWindow.RunTaskList(tl,true);

			return;
		}
	}
}

