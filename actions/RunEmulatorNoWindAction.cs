using System;
using System.IO;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Task;

namespace Moscrif.IDE.Actions
{
	public class RunEmulatorNoWindAction  : Gtk.Action
	{
		public RunEmulatorNoWindAction() : base("runnowindow", MainClass.Languages.Translate("menu_run_as_console"), MainClass.Languages.Translate("menu_title_run_as_console"), "run-in-console.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			if (MainClass.Settings.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();

			if(MainClass.Settings.SaveChangesBeforeRun){
				MainClass.MainWindow.EditorNotebook.SaveAllPage();
				MainClass.MainWindow.SaveWorkspace();
			}

			MainClass.MainWindow.ClearOutput();

			TaskList tl = new TaskList();
			tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
			RunEmulatorConsoleTask ret = new RunEmulatorConsoleTask();

			tl.TasksList.Add(ret);

			MainClass.MainWindow.RunTaskList(tl,true);

			//
			return;

			/*if(MainClass.Workspace.ActualProject == null){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", "Please select Project", Gtk.MessageType.Error);
				md.ShowDialog();
				return;
			}

			if (MainClass.Workspace.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();

			string cmd = Path.Combine(MainClass.Settings.EmulatorDirectory, "moscrif.exe");

			AppFile appFile = MainClass.Workspace.ActualProject.AppFile;
			string projDir = Path.GetDirectoryName(appFile.ApplicationFile);
			if (!projDir.EndsWith(Path.DirectorySeparatorChar.ToString())) projDir += Path.DirectorySeparatorChar;
			//string args = String.Format("/f {0} /d {1} /o console-win /w nowindow", Path.GetFileName(appFile.ApplicationFile), projDir);
			string args = String.Format("/f {0} /d {1} /o console /w nowindow", Path.GetFileName(appFile.ApplicationFile), projDir);
			//string args = String.Format("/f {0} /d {1} /o console ", Path.GetFileName(appFile.ApplicationFile), projDir);
			MainClass.MainWindow.RunProcess(cmd, args,MainClass.Settings.EmulatorDirectory);
			 */
		}
	}
}



