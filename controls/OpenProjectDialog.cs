using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;


namespace Moscrif.IDE.Controls
{
	public partial class OpenProjectDialog : Gtk.Dialog
	{
		Gtk.ListStore fileListStore = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(int));

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			OpenProject();

		}

		private bool loadAllDirectory = false;

		private void OpenProject(){

			TreeSelection ts = tvFiles.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			//string fileState = tvFiles.Model.GetValue(ti, 2).ToString();
			string filePath = tvFiles.Model.GetValue(ti, 3).ToString();
			string prjName = tvFiles.Model.GetValue(ti, 1).ToString();
			int fileStat = (int)tvFiles.Model.GetValue(ti, 4);

			if (fileStat == (int)StatEnum.Open ){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_opened", prjName), null, Gtk.MessageType.Error);
				md.ShowDialog();
				return ;
			} else if (fileStat == (int)StatEnum.Close ){
				MainClass.MainWindow.OpenProject(filePath,true);
				this.Respond(ResponseType.Ok);

			} else if (fileStat == (int)StatEnum.Not_Create ){

				AppFile appFile= new AppFile(filePath);

				string projectDir = appFile.Directory;
				if (!Directory.Exists(projectDir) )
				{
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("application_foldes_not_exist"), projectDir, Gtk.MessageType.Error);
					md.ShowDialog();
					return ;
				}

				MainClass.MainWindow.ConvertAppToProject(appFile);

				this.Respond(ResponseType.Ok);
			}

			this.Respond(ResponseType.Ok);
		}
		

		public OpenProjectDialog()
		{
			this.TransientFor = MainClass.MainWindow;
			this.Build();

			tvFiles.AppendColumn(MainClass.Languages.Translate("app"), new Gtk.CellRendererText(), "text", 0);
			tvFiles.AppendColumn(MainClass.Languages.Translate("project_f1"), new Gtk.CellRendererText(), "text", 1);
			tvFiles.AppendColumn(MainClass.Languages.Translate("state"),new Gtk.CellRendererText(), "text", 2);
			tvFiles.AppendColumn(MainClass.Languages.Translate("path"), new Gtk.CellRendererText(), "text", 3);
			tvFiles.AppendColumn(MainClass.Languages.Translate("StateInt"), new Gtk.CellRendererText(), "text", 4);
			tvFiles.Columns[4].Visible=false;
			tvFiles.Columns[0].Visible=false;

			string[] listApp ;
			string[] listMsp ;

			if(loadAllDirectory){
				 listApp = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.app",SearchOption.AllDirectories);
				 listMsp = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.msp",SearchOption.AllDirectories);

			}else {
			 	listApp = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.app");
			 	listMsp = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.msp");
			}

			List<Project> listAllProject= new List<Project>();

			foreach (string fi in listMsp){
				Project prj= Project.OpenProject(fi,false);
				if(prj!=null){
					listAllProject.Add(prj);
				} else{

					fileListStore.AppendValues(System.IO.Path.GetFileName(fi),"-",MainClass.Languages.Translate("missing_app"), fi,(int)StatEnum.Not_Create);
				}
			}

			List<Project> listOpenProject= MainClass.Workspace.Projects;

			foreach (string fi in listApp){
				//First Check Open Project
				Project openprj = listOpenProject.Find( x=> x.AbsolutAppFilePath == fi);
				if (openprj != null){
					fileListStore.AppendValues(System.IO.Path.GetFileName(fi),openprj.ProjectName,MainClass.Languages.Translate("opened"), openprj.FilePath,(int)StatEnum.Open);
					continue;
				}
				Project allprj = null;

			//Console.WriteLine("fi ->"+fi);

				try{
					allprj = listAllProject.Find( x=> x.AbsolutAppFilePath == fi);
				} catch {}

				if (allprj != null){
					fileListStore.AppendValues(System.IO.Path.GetFileName(fi),allprj.ProjectName,MainClass.Languages.Translate("closed"), allprj.FilePath,(int)StatEnum.Close);
				}
				else fileListStore.AppendValues(System.IO.Path.GetFileName(fi),"-",MainClass.Languages.Translate("missing_app"), fi,(int)StatEnum.Not_Create);

			}
			tvFiles.Model = fileListStore;
		}

		protected virtual void OnTvFilesRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			OpenProject();
		}
		
		

		enum StatEnum{
			Open,
			Close,
			
			Not_Create
		}

	}
}

