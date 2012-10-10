using System;
using System.IO;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Controls.Wizard;
using Moscrif.IDE.Task;
using Gtk;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;

namespace  Moscrif.IDE.Actions
{
	public class PublishAction : Gtk.Action
	{
		public PublishAction() : base("publish", MainClass.Languages.Translate("menu_publish"), MainClass.Languages.Translate("menu_title_publish"), "publish.png")
		{

		}

		protected override void OnActivated ()
		{
			base.OnActivated();

			if (MainClass.Settings.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();

			MainClass.MainWindow.ProcessOutput.Clear();
			MainClass.MainWindow.ErrorOutput.Clear();
			MainClass.MainWindow.LogOutput.Clear();

			if(MainClass.Workspace.ActualProject == null){
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", MainClass.Languages.Translate("no_project_selected"), MessageType.Error);
				ms.ShowDialog();
				return;
			}

			PublishDialogWizzard npw = new PublishDialogWizzard();
			int result = npw.Run();
			if (result == (int)ResponseType.Ok) {
				
			}
			npw.Destroy();

			/*PublishDialog pd = new PublishDialog();
			if(pd.Run() == (int)ResponseType.Ok){

			}
			pd.Destroy();*/

			/*TaskList tl = new TaskList();
			tl.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
			tl.TasksList.Add(new PublishTask() );

			MainClass.MainWindow.RunTaskList(tl,false);
			 */
		}
	}
}

