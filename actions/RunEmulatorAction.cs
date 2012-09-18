using System;
using System.IO;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Task;

namespace Moscrif.IDE.Actions
{
	public class RunEmulatorAction : Gtk.Action
	{
		public RunEmulatorAction() : base("run", MainClass.Languages.Translate("menu_run"), MainClass.Languages.Translate("menu_title_run"), "run.png")
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
			RunEmulatorTask ret = new RunEmulatorTask();

			tl.TasksList.Add(ret);

			MainClass.MainWindow.RunTaskList(tl,true);

			return;
		}
	}
}

