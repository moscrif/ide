using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Controls;


namespace Moscrif.IDE.Actions
{
	public class NewWorkspaceAction : Gtk.Action
	{
		public NewWorkspaceAction(): base("newworkspace", MainClass.Languages.Translate("menu_new_workspace"), MainClass.Languages.Translate("menu_title_new_workspace"),"workspace.png")
		{
		}

		protected override void OnActivated()
		{
			base.OnActivated();

			NewWorkspaceDialog nwd = new NewWorkspaceDialog(false);
			int result = nwd.Run();

			if (result == (int)ResponseType.Ok) {
				string workspaceName = nwd.WorkspaceName;
				string workspaceOutput = nwd.WorkspaceOutput;
				string workspaceRoot =nwd.WorkspaceRoot;
				bool copyLibs = nwd.CopyLibs;

				string workspaceFile = System.IO.Path.Combine(workspaceRoot, workspaceName + ".msw");

				MainClass.MainWindow.CreateWorkspace(workspaceFile,workspaceName,workspaceOutput,workspaceRoot,copyLibs);

			}
			nwd.Destroy();
		}
	}
}

