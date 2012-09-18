using System;
using System.Collections.Generic;
using Gtk;
using System.Reflection;
using Moscrif.IDE.Components;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;


namespace Moscrif.IDE.Actions
{
	public class NewProjectAction : Gtk.Action
	{
		public NewProjectAction() : base("newproject", MainClass.Languages.Translate("menu_new_project"), MainClass.Languages.Translate("menu_title_new_project"),"project-new.png")
		{

		}


		private void CreateProject(string projectName, string skin, string theme){
			string projectDir = MainClass.Tools.RemoveDiacritics(projectName).Replace(" ","_");
			string projectLocat = MainClass.Workspace.RootDirectory;

			string projectFile = System.IO.Path.Combine(projectLocat, projectDir + ".msp");
			string appFile = System.IO.Path.Combine(projectLocat, projectDir + ".app");

			MainClass.MainWindow.CreateProject(projectFile,projectName,projectLocat,projectDir,appFile,skin,theme);
		}


		protected override void OnActivated()
		{
			base.OnActivated();
			string projectName = String.Empty;

			if (String.IsNullOrEmpty(MainClass.Workspace.FilePath))
			{
				NewWorkspaceDialog nwd = new NewWorkspaceDialog(true);
				int result = nwd.Run();

				if (result == (int)ResponseType.Ok) {
					string workspaceName = nwd.WorkspaceName;
					string workspaceOutput = nwd.WorkspaceOutput;
					string workspaceRoot =nwd.WorkspaceRoot;
					bool copyLibs = nwd.CopyLibs;
					//projectName = nwd.ProjectName;
					projectName = MainClass.Tools.RemoveDiacritics(nwd.ProjectName).Replace(" ","_");

					string workspaceFile = System.IO.Path.Combine(workspaceRoot, workspaceName + ".msw");

					MainClass.MainWindow.CreateWorkspace(workspaceFile,workspaceName,workspaceOutput,workspaceRoot,copyLibs);

					if (!String.IsNullOrEmpty(projectName))
						CreateProject(projectName,nwd.Skin,nwd.Theme);
				}
				nwd.Destroy();
			} else {
				NewProjectDialog npd = new NewProjectDialog();
				int result = npd.Run();

				if (result == (int)ResponseType.Ok){
					if (!String.IsNullOrEmpty(npd.ProjectName) ){
						//projectName = npd.ProjectName;
						projectName = MainClass.Tools.RemoveDiacritics(npd.ProjectName).Replace(" ","_");
						CreateProject(projectName,npd.Skin,npd.Theme);
					}
				}
				npd.Destroy();

				/*EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_project_name"));
				int result = ed.Run();
				if (result == (int)ResponseType.Ok){
					if (!String.IsNullOrEmpty(ed.TextEntry) ){
						projectName = ed.TextEntry;
						CreateProject(projectName);
					}
				}
				ed.Destroy();*/
			}

			//projectName

			/*
			NewProjectDialog nfd = new NewProjectDialog();
			int result = nfd.Run();
			
			if (result == (int)ResponseType.Ok) {
				string projectName = nfd.ProjectName;
				string projectDir = MainClass.Tools.RemoveDiacritics(nfd.ProjectName).Replace(" ","_");
				//string projectLocat = MainClass.Tools.RemoveDiacritics(nfd.ProjectName).Replace(" ","_");
				string projectLocat = MainClass.Workspace.RootDirectory;

				string projectFile = System.IO.Path.Combine(projectLocat, projectName + ".msp");
				string appFile = System.IO.Path.Combine(projectLocat, projectName + ".app");

				MainClass.Workspace.CreateProject(projectFile,projectName,projectLocat,projectDir,appFile);
				//MainClass.MainWindow.CreateProject(projectName);
			}
			nfd.Destroy();*/
		}
				
	}
}

