using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Moscrif.IDE.Tool;
using Pango;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;
using Moscrif.IDE.Iface;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.FileTemplates;

namespace Moscrif.IDE.Controls.Wizard
{
	public partial class NewProjectWizzard_New : Gtk.Dialog
	{
		const int COL_DISPLAY_NAME = 0; 
		const int COL_DISPLAY_TEXT = 1;
		const int COL_PIXBUF = 2;
		const int COL_IS_DIRECTORY = 3;

		const string KEY_CUSTOM = "Custom";

		ListStore storeTyp;
		ListStore storeWorkspace;
		ListStore storeTemplate;
		ListStore storeOrientation;

		ListStore storeOutput;
		ComboBoxEntry cbeWorkspace;

		string prjDefaultName = "";
		string worksDefaultName =  "";
		string templateDir = "";
		ProjectTemplate projectTemplate;

		string workspaceName ="";
		string workspaceRoot="";
		//bool copyLibs = false;
		string workspaceOutput="";
		string workspaceFile="";
		
		string projectName = "";
		string projectDir = "";
		TreePath selectedTypPrj ;

		int page = 0;
		public NewProjectWizzard_New(Window parent)
		{
			if (parent != null)
				this.TransientFor =parent;
			else
				this.TransientFor = MainClass.MainWindow;

			this.Build();		

			this.DefaultHeight = 390 ;
			this.Title = MainClass.Languages.Translate("moscrif_ide_title_f1");
			ntbWizzard.ShowTabs = false;

			Pango.FontDescription customFont = lblNewProject.Style.FontDescription.Copy();//  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			customFont.Size = 24;
			customFont.Weight = Pango.Weight.Bold;
			lblNewProject.ModifyFont(customFont);

			storeTyp = new ListStore (typeof (string), typeof (string), typeof (Gdk.Pixbuf), typeof (string),typeof(ProjectTemplate),typeof (bool));
			storeOrientation = new ListStore (typeof (string), typeof (string), typeof (Gdk.Pixbuf), typeof (string));

			storeOutput = new ListStore (typeof (string), typeof (string), typeof (Gdk.Pixbuf));

			nvOutput.Model = storeOutput;
			nvOutput.AppendColumn ("", new Gtk.CellRendererText (), "text", 0);
			nvOutput.AppendColumn ("", new Gtk.CellRendererText (), "text", 1);
			nvOutput.AppendColumn ("", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvOutput.Columns[1].Expand = true;


			ivSelectTyp.Model = storeTyp;
			ivSelectTyp.SelectionMode = SelectionMode.Single;
			ivSelectTyp.Orientation = Orientation.Horizontal;

			CellRendererText rendererSelectTyp = new CellRendererText();
			rendererSelectTyp.Ypad =0;
			ivSelectTyp.PackEnd(rendererSelectTyp,false);
			ivSelectTyp.SetCellDataFunc(rendererSelectTyp, new Gtk.CellLayoutDataFunc(RenderTypProject));
			ivSelectTyp.PixbufColumn = COL_PIXBUF;
			ivSelectTyp.TooltipColumn = COL_DISPLAY_TEXT;
			ivSelectTyp.AddAttribute(rendererSelectTyp, "sensitive", 5);

			Gdk.Pixbuf icon0 = MainClass.Tools.GetIconFromStock("file-new.png",IconSize.LargeToolbar);
			storeTyp.AppendValues ("New Empty Project", "Create empty application", icon0, "", null,true);

			DirectoryInfo[] diTemplates = GetDirectory(MainClass.Paths.FileTemplateDir);
			foreach (DirectoryInfo di in diTemplates) {

				string name = di.Name;

				string iconFile = System.IO.Path.Combine(di.FullName,"icon.png");
				string descFile = System.IO.Path.Combine(di.FullName,"description.xml");
				if(!File.Exists(iconFile) || !File.Exists(descFile))
					continue;

				string descr = name;
				ProjectTemplate pt = null;

				if(File.Exists(descFile)){
					pt = ProjectTemplate.OpenProjectTemplate(descFile);
					if((pt!= null))
						descr = pt.Description;
				}
				Gdk.Pixbuf icon = new Gdk.Pixbuf(iconFile);
				DirectoryInfo[] templates = di.GetDirectories();
				bool sensitive = true;

				if(templates.Length<1)
					sensitive = false;
				else 
					sensitive = true;


				storeTyp.AppendValues (name, descr, icon, di.FullName,pt,sensitive);
			}

			ivSelectTyp.SelectionChanged+= delegate(object sender, EventArgs e)
			{
				Gtk.TreePath[] selRow = ivSelectTyp.SelectedItems;
				if(selRow.Length<1){
					lblHint.Text = " ";
					btnNext.Sensitive = false;
					return;
				}

				Gtk.TreePath tp = selRow[0];
				TreeIter ti = new TreeIter();
				storeTyp.GetIter(out ti,tp);

				if(tp.Equals(TreeIter.Zero))return;
			
				//string typ = storeTyp.GetValue (ti, 3).ToString();
				string text1 = (string) storeTyp.GetValue (ti, 0);
				string text2 = (string) storeTyp.GetValue (ti, 1);
				bool sensitive = Convert.ToBoolean(storeTyp.GetValue (ti, 5));
				if(!sensitive){
					ivSelectTyp.SelectPath(selectedTypPrj);
					return;
				}
				selectedTypPrj = selRow[0];

				lblHint.Text = text1+" - "+text2;
				btnNext.Sensitive = true;
			};
			CellRendererText rendererOrientation = new CellRendererText();

			selectedTypPrj = new TreePath("0");
			ivSelectTyp.SelectPath(selectedTypPrj); 

			ivSelectOrientation.Model = storeOrientation;
			ivSelectOrientation.SelectionMode = SelectionMode.Single;
			ivSelectOrientation.Orientation = Orientation.Horizontal;

			ivSelectOrientation.PackEnd(rendererOrientation,false);
			ivSelectOrientation.SetCellDataFunc(rendererOrientation, new Gtk.CellLayoutDataFunc(RenderOrientationProject));
			ivSelectOrientation.PixbufColumn = COL_PIXBUF;
			ivSelectOrientation.TooltipColumn = COL_DISPLAY_TEXT;

			//Gdk.Pixbuf iconPort = MainClass.Tools.GetIconFromStock("file-new.png",IconSize.LargeToolbar);
			//Gdk.Pixbuf iconLL = MainClass.Tools.GetIconFromStock("file-ms.png",IconSize.LargeToolbar);
			//Gdk.Pixbuf iconLR = MainClass.Tools.GetIconFromStock("preferences.png",IconSize.LargeToolbar);
			foreach(SettingValue ds in MainClass.Settings.DisplayOrientations){
				storeOrientation.AppendValues (ds.Display,ds.Display,null,ds.Value);
			}
			ivSelectOrientation.SelectPath(new TreePath("0")); 
			storeWorkspace = new ListStore(typeof(string), typeof(string), typeof(int));
			cbeWorkspace = new ComboBoxEntry();
			cbeWorkspace.Model = storeWorkspace;
			cbeWorkspace.TextColumn = 0;
			cbeWorkspace.Changed+= OnCbeWorkspaceChanged;

				//
			table2.Attach(cbeWorkspace,1,2,1,2,AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,0,0);

			CellRendererText rendererWorkspace = new CellRendererText();
			cbeWorkspace.PackStart(rendererWorkspace, true);
			cbeWorkspace.SetCellDataFunc(rendererWorkspace, new Gtk.CellLayoutDataFunc(RenderWorkspacePath));
			cbeWorkspace.WidthRequest = 125;

			cbeWorkspace.SetCellDataFunc(cbeWorkspace.Cells[0], new Gtk.CellLayoutDataFunc(RenderWorkspaceName));

			string currentWorkspace ="";
			if((MainClass.Workspace!= null) && !string.IsNullOrEmpty(MainClass.Workspace.FilePath))
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(MainClass.Workspace.FilePath);
				storeWorkspace.AppendValues (name,MainClass.Workspace.FilePath,1);
				currentWorkspace = MainClass.Workspace.FilePath;
			}
			IList<RecentFile> lRecentProjects = MainClass.Settings.RecentFiles.GetWorkspace();

			foreach(RecentFile rf in lRecentProjects){

				if(rf.FileName == currentWorkspace) continue;
				if(File.Exists(rf.FileName)){
					string name = System.IO.Path.GetFileNameWithoutExtension(rf.FileName);
					storeWorkspace.AppendValues(name,rf.FileName,0);
				}
			}

				//storeWorkspace.AppendValues("","-------------",-1);

			worksDefaultName = "Workspace"+MainClass.Settings.WorkspaceCount.ToString();
			TreeIter tiNewW = storeWorkspace.AppendValues(worksDefaultName,MainClass.Paths.WorkDir,2);

			if(!String.IsNullOrEmpty(currentWorkspace)){
				cbeWorkspace.Active =0;
			}
			else {
				feLocation.DefaultPath = MainClass.Paths.WorkDir;
				cbeWorkspace.SetActiveIter(tiNewW);
				//storeWorkspace.AppendValues(worksDefaultName,MainClass.Paths.WorkDir,2);

			}
			prjDefaultName = "Project"+MainClass.Settings.ProjectCount.ToString();
			entrProjectName.Text = prjDefaultName;
			cbeWorkspace.ShowAll();

			CellRendererText rendererTemplate = new CellRendererText();
			cbTemplate.PackStart(rendererTemplate, true);

			storeTemplate = new ListStore(typeof(string), typeof(string));
			cbTemplate.Model = storeTemplate;

			cbTemplate.Changed+= delegate(object sender, EventArgs e) {

				if(cbTemplate.Active <0) return;

				if(cbTemplate.ActiveText != KEY_CUSTOM){

					tblLibraries.Sensitive = false;
					tblScreens.Sensitive = false;
					ivSelectOrientation.Sensitive = false;
				} else {
					ivSelectOrientation.Sensitive = true;
					tblLibraries.Sensitive = true;
					tblScreens.Sensitive = true;
				}

				TreeIter tiChb = new TreeIter();
				cbTemplate.GetActiveIter(out tiChb);

				if(tiChb.Equals(TreeIter.Zero))return;

				string name = storeTemplate.GetValue(tiChb, 0).ToString(); 
				string path = storeTemplate.GetValue(tiChb, 1).ToString();

				string appPath = System.IO.Path.Combine(path,"$application$.app");
				if(File.Exists(appPath)){
					AppFile app = new AppFile(appPath);
					List<string> libs = new List<string>(app.Libs);

					Widget[] widgets = tblLibraries.Children;
					foreach (Widget w in widgets ){
						int indx = libs.FindIndex(x=>x==w.Name);
						if(indx>-1) {
							(w as CheckButton).Active = true;
						} else {
							(w as CheckButton).Active = false;
						}
					}
				}
			};
			btnBack.Sensitive = false;
		}

		private DirectoryInfo[] GetDirectory(string parentDirectory){
			if(!Directory.Exists(parentDirectory) ) return new DirectoryInfo[0];

			DirectoryInfo dirParent = new DirectoryInfo(parentDirectory);
			DirectoryInfo[] dirChild = dirParent.GetDirectories("*",SearchOption.TopDirectoryOnly);
			return  dirChild;

		}

		private void RenderWorkspaceName(CellLayout column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string) model.GetValue (iter, 0);
			string text2 = (string) model.GetValue (iter, 1);
			int type = Convert.ToInt32(storeWorkspace.GetValue(iter, 2));

			Pango.FontDescription fd = new Pango.FontDescription();

			if(type == 2)
				fd.Weight = Pango.Weight.Bold;
		
			(cell as Gtk.CellRendererText).FontDesc = fd;

		}

		private void RenderWorkspacePath(CellLayout column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string) model.GetValue (iter, 0);
			string text2 = (string) model.GetValue (iter, 1);
			int type = Convert.ToInt32(storeWorkspace.GetValue(iter, 2));
			
			Pango.FontDescription fd = new Pango.FontDescription();
			
			if(type == 2)
				fd.Weight = Pango.Weight.Bold;
			
			(cell as Gtk.CellRendererText).FontDesc = fd;
			(cell as Gtk.CellRendererText).Markup = "<small><i>"+text2+"</i></small>";
		}


		private void RenderOrientationProject(CellLayout column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string) model.GetValue (iter, 0);
			string text2 = (string) model.GetValue (iter, 1);

			Pango.FontDescription fd = new Pango.FontDescription();
			
			(cell as Gtk.CellRendererText).FontDesc = fd;
			(cell as Gtk.CellRendererText).Markup = "<b >"+text + "</b>"+Environment.NewLine+"<small>" +  text2+"</small>";
			
			/*string pathTemplates = (string) model.GetValue (iter, 3);

				string[] templates =  System.IO.Directory.GetDirectories(pathTemplates);
				if(templates.Length<1)
					(cell as Gtk.CellRendererText).Sensitive = false;*/
		}

		private void RenderTypProject(CellLayout column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string) model.GetValue (iter, 0);
			string text2 = (string) model.GetValue (iter, 1);


			Pango.FontDescription fd = new Pango.FontDescription();

			(cell as Gtk.CellRendererText).FontDesc = fd;
			(cell as Gtk.CellRendererText).Markup = "<b >"+text + "</b>"+Environment.NewLine+"<small>" +  text2+"</small>";

			/*string pathTemplates = (string) model.GetValue (iter, 3);
			if(!string.IsNullOrEmpty(pathTemplates)){
				string[] templates =  System.IO.Directory.GetDirectories(pathTemplates);
				if(templates.Length<1)
					(cell as Gtk.CellRendererText).Sensitive = false;
				else 
					(cell as Gtk.CellRendererText).Sensitive = true;
			}*/
		}

		protected void OnCbeWorkspaceChanged (object sender, EventArgs e)
		{
			if(cbeWorkspace.Active >-1){
				feLocation.Sensitive = false;
			} else {
				feLocation.Sensitive = true;
				//feLocation.DefaultPath = MainClass.Paths.WorkDir;
				return;
			}
			TreeIter tiChb;
			cbeWorkspace.GetActiveIter(out tiChb);
			workspaceName = storeWorkspace.GetValue(tiChb, 0).ToString(); 
			string workspacePath = storeWorkspace.GetValue(tiChb, 1).ToString();

			string dir = System.IO.Path.GetDirectoryName(workspacePath);

			if(dir.ToLower().EndsWith(workspaceName.ToLower()) ){
				int indx =dir.ToLower().LastIndexOf(workspaceName.ToLower());
				dir = dir.Remove(indx-1,workspaceName.Length);
			}

			int type = Convert.ToInt32(storeWorkspace.GetValue(tiChb, 2));
			if(type == -1){
				cbeWorkspace.Active = cbeWorkspace.Active+1;
			} else if(type == 1 || type == 0){
				feLocation.DefaultPath = dir;
				feLocation.Sensitive = false;
			} else if(type == 2) {
				feLocation.Sensitive = true;
				feLocation.DefaultPath = dir;
			}


		}

		protected void OnBtnBackClicked (object sender, EventArgs e)
		{
			if(page ==1){
				page--;
				ntbWizzard.Page = 0;
				btnBack.Sensitive = false;
				btnNext.Label = "_Next";
				this.DefaultHeight = 390 ;
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{	
			if(page ==0){

				if(!CheckPage0()) return;

				Gtk.TreePath[] selRow = ivSelectTyp.SelectedItems;
				if(selRow.Length<1) return;

				Gtk.TreePath tp = selRow[0];
				TreeIter ti = new TreeIter();
				storeTyp.GetIter(out ti,tp);

				if(tp.Equals(TreeIter.Zero))return;

				templateDir = storeTyp.GetValue(ti, 3).ToString();
				projectTemplate = (ProjectTemplate)storeTyp.GetValue(ti, 4);

				projectName = MainClass.Tools.RemoveDiacritics(entrProjectName.Text).Replace(" ","_");
				projectDir = projectName;

				if(String.IsNullOrEmpty(templateDir)){ //Create Empty project 
					ntbWizzard.Page = 2;
					btnBack.Sensitive = false;
					btnNext.Sensitive = false;

					if(!CreateWorkspace(out workspaceName,out workspaceRoot,out workspaceOutput,out workspaceFile)){
						return;
					}

					string projectFile = System.IO.Path.Combine(MainClass.Workspace.RootDirectory, projectName + ".msp");
					string appFile = System.IO.Path.Combine(MainClass.Workspace.RootDirectory, projectName + ".app");

					TreeIter tiPrj =  AddMessage("Create Project ","....",null);
					Project prj = null;
					try{
						prj = MainClass.Workspace.CreateProject(projectFile, projectName, MainClass.Workspace.RootDirectory, projectDir, appFile,null,null);
					} catch(Exception ex){
						UpdateMessage(tiPrj,1,ex.Message);
						return ;
					}
					UpdateMessage(tiPrj,1,"OK");
					MainClass.MainWindow.AddAndShowProject(prj,true);
					AddMessage("Finish ","",null);

				} else { //Inicialize next page
					lblCustom.LabelProp = projectTemplate.Custom;

					storeTemplate.Clear();

					while (tblLibraries.Children.Length > 0)
						tblLibraries.Remove(tblLibraries.Children[0]);

					while (tblScreens.Children.Length > 0)
						tblScreens.Remove(tblScreens.Children[0]);

					btnBack.Sensitive = true;
					ntbWizzard.Page = 1;

					entrPage2PrjName.Text= projectName;
					DirectoryInfo[] diTemplates = GetDirectory(templateDir);

					List<DirectoryInfo> customTemplate = new List<DirectoryInfo>();

					storeTemplate.Clear();
					storeTemplate.AppendValues(KEY_CUSTOM,"");

					foreach (DirectoryInfo di in diTemplates) {
						string name = System.IO.Path.GetFileNameWithoutExtension(di.FullName);
						string ex = System.IO.Path.GetExtension(di.FullName);

						if(ex == ".custom"){
							customTemplate.Add(di);
						} else {
							storeTemplate.AppendValues(name,di.FullName);
						}
					}
					cbTemplate.Active = 0;

					int x = 0;
					if(projectTemplate!= null){
						foreach(SettingValue sv in projectTemplate.Libs){
							CheckButton chb = new CheckButton(sv.Display);
							chb.Name = sv.Value;
							tblLibraries.Attach(chb,0,1,(uint)x,(uint)(x+1),AttachOptions.Fill,AttachOptions.Fill,0,0);
							x++;
						}
						tblLibraries.ShowAll();
					}

					GenerateCustomTemplate(customTemplate);

					btnNext.Label = "Finish";
					page++;
			
				} 
			}
			else if(page == 1){

				ntbWizzard.Page = 2;

				while (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();

				page++;
				btnNext.Sensitive = false;
				btnBack.Sensitive = false;

				if(cbTemplate.ActiveText != KEY_CUSTOM){ // Select App

					if(!CreateWorkspace(out workspaceName,out workspaceRoot,out workspaceOutput,out workspaceFile)){
						return;
					}
				
					btnNext.Sensitive = false;

					TreeIter tiChb = new TreeIter();
					cbTemplate.GetActiveIter(out tiChb);

					string path = storeTemplate.GetValue(tiChb, 1).ToString();				
					string appPath = System.IO.Path.Combine(path,"$application$.app");

					if(File.Exists(appPath)){
						Project prj =  ImportProject(appPath,projectName,String.Empty,String.Empty);
						MainClass.MainWindow.AddAndShowProject(prj,true);
						AddMessage("Finish ","",null);
					}
				} else { // SELECT Custom
					if(!CreateWorkspace(out workspaceName,out workspaceRoot,out workspaceOutput,out workspaceFile)){
						return;
					}

					Widget[] widgets = tblScreens.Children;
					string path = "";
					string appPath = "";
					foreach (Widget w in widgets ){
						if((w as RadioButton).Active){
							path = (w as RadioButton).TooltipMarkup;
						}
					}

					widgets = tblLibraries.Children;
					string libs = "core";
					string orientation = "";

					foreach (Widget w in widgets ){	
						if((w as CheckButton).Active){
							libs = libs +" "+(w as CheckButton).Name;
						}
					}

					Gtk.TreePath[] selRow = ivSelectOrientation.SelectedItems;
					if(selRow.Length>0) {				
						Gtk.TreePath tp = selRow[0];
						TreeIter ti = new TreeIter();
						storeOrientation.GetIter(out ti,tp);
						
						if(!tp.Equals(TreeIter.Zero)){						
							orientation = storeOrientation.GetValue(ti, 3).ToString();
						}
					}

					if(!String.IsNullOrEmpty(path)){

						appPath = System.IO.Path.Combine(path,"$application$.app");
					}
					if(File.Exists(appPath)){
						Project prj = ImportProject(appPath,projectName,libs, orientation.Trim());
						MainClass.MainWindow.AddAndShowProject(prj,true);
						AddMessage("Finish ","",null);
					}
				}			
			}
		}

		private bool CheckPage0(){
			if(cbeWorkspace.ActiveText.Contains(" ")){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_work"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}
			if(String.IsNullOrEmpty(cbeWorkspace.ActiveText)){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_workspace_name"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}

			if  (String.IsNullOrEmpty(entrProjectName.Text)){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("please_set_project_name"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}

			if(entrProjectName.Text.Contains(" ")){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_whitespace_proj"),"", Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}
			if(cbeWorkspace.Active <0){
				string workspaceName = cbeWorkspace.ActiveText;
				string workspaceRoot =System.IO.Path.Combine(feLocation.Path,workspaceName);
				string workspaceFile = System.IO.Path.Combine(workspaceRoot, workspaceName + ".msw");
				if(File.Exists(workspaceFile)){
					MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("workspace_exist"),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return false;
				}
			}
			return true;
		}

		private bool CreateWorkspace(out string  workspaceName,out string workspaceRoot,out string workspaceOutput,out string workspaceFile){

			if(entrProjectName.Text == prjDefaultName)
				MainClass.Settings.ProjectCount = MainClass.Settings.ProjectCount +1;

			if(cbeWorkspace.ActiveText == worksDefaultName)
				MainClass.Settings.WorkspaceCount = MainClass.Settings.WorkspaceCount +1;

			if(cbeWorkspace.Active <0){

				workspaceName = cbeWorkspace.ActiveText;
				workspaceRoot =System.IO.Path.Combine(feLocation.Path,workspaceName);
				bool copyLibs = false;
				workspaceOutput =System.IO.Path.Combine("$(workspace_dir)","output");
				workspaceFile = System.IO.Path.Combine(workspaceRoot, workspaceName + ".msw");

				TreeIter ti =  AddMessage("Create Workspace ","....",null);

				Workspace.Workspace workspace = null;
				try{
					workspace = Workspace.Workspace.CreateWorkspace(workspaceFile,workspaceName,workspaceOutput,workspaceRoot,copyLibs);
				}catch(Exception ex){
					UpdateMessage(ti,1 ,ex.Message);
					return false;
				}
				if (workspace != null){
					MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
				}

				UpdateMessage(ti,1 ,"OK");

			} else {
				TreeIter tiChb = new TreeIter();
				cbeWorkspace.GetActiveIter(out tiChb);
				workspaceName = storeWorkspace.GetValue(tiChb, 0).ToString(); 
				string workspacePath = storeWorkspace.GetValue(tiChb, 1).ToString();
				workspaceRoot = System.IO.Path.GetDirectoryName(workspacePath);
				workspaceOutput ="";
				workspaceFile ="";

				Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(workspacePath);
				if (workspace != null){
					if(workspacePath != MainClass.Workspace.FilePath){
						MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
					}
					workspaceOutput = workspace.OutputDirectory;
					workspaceFile =workspace.FilePath;
				}
			}
			return true;
		}


		private Project ImportProject(string appPath, string projectName,string newLibs, string newOrientation)
		{
			TreeIter ti =  AddMessage("Create Project ","....",null);
			
			if(String.IsNullOrEmpty(MainClass.Workspace.FilePath)) return null;
			
			string fileName = projectName;//System.IO.Path.GetFileNameWithoutExtension(destinationFile);
			string destinationDir = System.IO.Path.GetDirectoryName(appPath); // aka workspace from
			string projectDir = System.IO.Path.Combine(destinationDir,"$application$"); // aka project dir from
		
			string mspPath =  System.IO.Path.Combine(destinationDir,"$application$.msp");
			
			if (!File.Exists(appPath) || !File.Exists(mspPath)){
				UpdateMessage(ti,1,MainClass.Languages.Translate("invalid_zip"));
				return null;
			}
			if(!System.IO.Directory.Exists(projectDir)){
				UpdateMessage(ti,1,MainClass.Languages.Translate("invalid_project"));
				return null;
			}
			
			string newApp = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,projectName+".app");
			string newMsp = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,projectName+".msp");
			
			if(File.Exists(newApp) || File.Exists(newMsp)){
				UpdateMessage(ti,1,MainClass.Languages.Translate("project_exist"));
				return null;
			}
			
			FileInfo fi = new FileInfo(appPath);
			FileTemplate ft= new FileTemplate(fi);
			int indx = ft.Attributes.FindIndex(x=>x.Name=="application");
			if(indx>-1)
				ft.Attributes[indx].Value = projectName;
			
			string contentApp = FileTemplateUtilities.Apply(ft.Content, ft.GetAttributesAsDictionary());
			
			string contentMsp="";
			if(System.IO.File.Exists(mspPath)){
				try {
					using (StreamReader file = new StreamReader(mspPath)) {
						string text = file.ReadToEnd();
						contentMsp = text;
					}
				} catch {
				}						
			}
			
			contentMsp = contentMsp.Replace("$application$",projectName);
			Project prj = null;
			try {
				FileUtility.CreateFile(newApp,contentApp); 
				AppFile app = null;
				if(!String.IsNullOrEmpty(newLibs)){
					app = new AppFile(newApp);
					app.Uses = newLibs;
					app.Orientation = newOrientation;
					app.Save();
				}
				
				FileUtility.CreateFile(newMsp,contentMsp); 
				
				string newPrjDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,projectName);			
				MainClass.Tools.CopyDirectory(projectDir,newPrjDir,true,true);
				prj = MainClass.Workspace.OpenProject(newMsp,false,true); //Project.OpenProject(newMsp,false,true);

				LoggingInfo log = new LoggingInfo();
				log.LoggWebThread(LoggingInfo.ActionId.IDENewProject,prj.ProjectName);
			
			} catch {
				UpdateMessage(ti,1,MainClass.Languages.Translate("error_creating_project"));
				return null;
			}
			UpdateMessage(ti,1,"OK");
			return prj;
		}


		private void GenerateCustomTemplate(List<DirectoryInfo> customTemplate){
			int x = 0;
			RadioButton rbGroup = new RadioButton("test");
			
			foreach(DirectoryInfo di in customTemplate){
				
				string name = System.IO.Path.GetFileNameWithoutExtension(di.FullName);
				string ex = System.IO.Path.GetExtension(di.FullName);
				
				string appPath = System.IO.Path.Combine(di.FullName,"$application$.app");
				
				RadioButton rb = new RadioButton(name);
				rb.Group = rbGroup.Group;
				rb.Name = name;
				rb.TooltipMarkup = di.FullName;
				rb.Events = Gdk.EventMask.AllEventsMask;
				rb.Clicked+= delegate(object sndr, EventArgs evt) {
					if(File.Exists(appPath)){
						AppFile app = new AppFile(appPath);
						List<string> libs = new List<string>(app.Libs);
						Widget[] widgets = tblLibraries.Children;
						
						foreach (Widget w in widgets ){
							int indx = libs.FindIndex(y=>y==w.Name);
							if(indx>-1) {
								(w as CheckButton).Active = true;
							} else {
								(w as CheckButton).Active = false;
							}
						}
					}
				};
				rb.Active = true;
				tblScreens.Attach(rb,0,1,(uint)x,(uint)(x+1),AttachOptions.Fill,AttachOptions.Fill,0,0);
				x++;
			}
			tblScreens.ShowAll();
		}

		private TreeIter AddMessage(string msg1,string msg2,Gdk.Pixbuf icon){
			TreeIter ti =  storeOutput.AppendValues(msg1,msg2,icon);
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
			return ti;
		}

		private TreeIter UpdateMessage(TreeIter ti,int col,string msg1){
			storeOutput.SetValue(ti,col,msg1);
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
			return ti;
		}	
	}
}

