using System;
using System.IO;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Task;

namespace  Moscrif.IDE.Actions
{
	public class CompileProjectAction : Gtk.Action
	{
		public CompileProjectAction() : base("compileproject", MainClass.Languages.Translate("menu_compile_project"), MainClass.Languages.Translate("menu_title_compile_project"), "compile.png")
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			MainClass.MainWindow.ProcessOutput.Clear();
			MainClass.MainWindow.ErrorOutput.Clear();

			if (MainClass.Settings.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();
			
			TaskList tl = new TaskList();
			tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
			CompileTask ct = new CompileTask();

			tl.TasksList.Add(ct );

			MainClass.MainWindow.RunTaskList(tl,false);

			return;
		}
	}
}

