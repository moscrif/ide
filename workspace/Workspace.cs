using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Editors;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;
using System.Threading;
using System.Diagnostics;

namespace Moscrif.IDE.Workspace
{
	public class Workspace
	{
		#region Public Property

		private string filePath;

		[XmlIgnore]
		public string FilePath
		{
			get { return filePath; }
			set {
				filePath = value;
				if (!String.IsNullOrEmpty(filePath))
					RootDirectory = System.IO.Path.GetDirectoryName(filePath);
			}
		}

		[XmlIgnore]
		public List<Project> Projects;
		[XmlIgnore]
		public string RootDirectory;

		[XmlIgnore]
		public Project ActualProject
		{
			get;
			set;
		}

		[XmlArray("projects")]
		[XmlArrayItem("project")]
		public List<string> ProjectsFile;
		[XmlArray("opened")]
		[XmlArrayItem("file")]
		public List<string> OpenFiles;

		//public List<string> PublishDevices;

		[XmlAttribute("fileExplorerPath")]
		public string FileExplorerPath;

		[XmlAttribute("vertBodyHeight")]
		public int VpBodyHeight;

		[XmlAttribute("horzMiddleBodyHeight")]
		public int HpBodyMiddleWidth;

		[XmlAttribute("vertOutputWidth")]
		public int HpOutputWidth;

		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("outputDir")]
		public string OutputDirectory;

		[XmlAttribute("tag")]
		public int Tag;

		[XmlAttribute("signapp")]
		public bool SignApp;

		[XmlAttribute("showLeftPane")]
		public bool ShowLeftPane = true;

		[XmlAttribute("showRightPane")]
		public bool ShowRightPane = true;

		[XmlAttribute("showBottomPane")]
		public bool ShowBottomPane = true;

		[XmlAttribute("selectedProject")]
		public String ActualProjectPath
		{
			get;
			set;
		}

		[XmlAttribute("showalllibs")]
		public bool ShowAllLibs
		{
			get;
			set;
		}

		[XmlAttribute("selectedResolution")]
		public string ActualResolution
		{
			get;
			set;
		}

		[XmlAttribute("selectedDevices")]
		public int ActualDevice
		{
			get;
			set;
		}

		public void SetActualProject(string projectFile)
		{
			ActualProject = FindProject(projectFile);
			if(ActualProject!=null)
				ActualProjectPath = ActualProject.FilePath;
		}

		[XmlIgnore]
		public string OutputMaskToFullPath{
			get{
				return ConvertOutputMaskToFullPath(OutputDirectory);
			}
		}


		// nazovprojektu.msp.nazovPc-Username.user
		private WorkspaceUserFile workspaceUserSetting;

		[XmlIgnore]
		public WorkspaceUserFile WorkspaceUserSetting{
			get{
				if(workspaceUserSetting == null){
					if (string.IsNullOrEmpty(FilePath)){
						workspaceUserSetting =new WorkspaceUserFile();
						return workspaceUserSetting;
					}

					string mspFile= System.IO.Path.GetFileName(FilePath);
					string mspPath = System.IO.Path.GetDirectoryName(FilePath);

					string userName = String.Format("{0}.{1}-{2}.user",Name,Environment.MachineName,Environment.UserName);
					string userPath = System.IO.Path.Combine(mspPath,userName);

					if(System.IO.File.Exists(userPath)){
						workspaceUserSetting = WorkspaceUserFile.OpenWorkspaceUserFile(userPath);
					} else {
						workspaceUserSetting =new WorkspaceUserFile(userPath);
					}
				}
				return workspaceUserSetting;
			}
		}

		 public string ConvertOutputMaskToFullPath(string mask){

			if(String.IsNullOrEmpty(mask)) return "";

			mask = mask.Replace('/',Path.DirectorySeparatorChar);
			mask = mask.Replace('\\',Path.DirectorySeparatorChar);

			string path = mask.Replace("$(workspace_dir)", RootDirectory);
			path = path.Replace("$(workspace_work)", MainClass.Paths.WorkDir);
			return path;
		}

		#endregion

		//private Tools tools = new Tools();

		public Workspace()
		{
			FilePath = String.Empty;
			//System.IO.Path.Combine(tools.ConfingDir, "workspace.wsp");
			Projects = new List<Project>();
			ProjectsFile = new List<string>();
			OpenFiles = new List<string>();
		}

		/*	public Workspace(string filePath)
		{
			FilePath = filePath;
			Projects = new List<Project>();
			ProjectsFile = new List<string>();
			OpenFiles = new List<string>();
		}*/

		public void SaveWorkspace(List<string> openFile, int vpBodyHeight, int hpBodyMiddleWidth, int hpOutputWidth)
		{
			foreach (Project project in Projects)
				SaveProject(project);

			HpOutputWidth = hpOutputWidth;
			VpBodyHeight = vpBodyHeight;
			HpBodyMiddleWidth = hpBodyMiddleWidth;

			OpenFiles = openFile;

			SaveWorkspace();
		}

		public void SaveWorkspace()
		{

			if (String.IsNullOrEmpty(FilePath))
				return;

			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.NewLineChars = "\n";//Environment.NewLine;
			xmlSettings.Indent = true;
			xmlSettings.CloseOutput = true;

			XmlSerializer x_serial = new XmlSerializer( typeof( Workspace ) );
			using (XmlWriter wr = XmlWriter.Create( FilePath, xmlSettings )) {
			    x_serial.Serialize( wr, this );
			}

			XmlSerializer x_serial_WU = new XmlSerializer( typeof( WorkspaceUserFile ) );
			using (XmlWriter wr = XmlWriter.Create( this.WorkspaceUserSetting.FilePath, xmlSettings )) {
			    x_serial_WU.Serialize( wr, this.WorkspaceUserSetting );
			}

			/*using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(Workspace));

				serializer.Serialize(fs, this);
			}*/
		}

		public void SaveUserWorkspaceFile(){

			if (String.IsNullOrEmpty(FilePath))
				return;

			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.NewLineChars = "\n";//Environment.NewLine;
			xmlSettings.Indent = true;
			xmlSettings.CloseOutput = true;

			XmlSerializer x_serial_WU = new XmlSerializer( typeof( WorkspaceUserFile ) );
			using (XmlWriter wr = XmlWriter.Create( this.WorkspaceUserSetting.FilePath, xmlSettings )) {
			    x_serial_WU.Serialize( wr, this.WorkspaceUserSetting );
			}
		}

		static void OnExited(object sender, EventArgs e) {
			//Console.WriteLine("exit ");
			try{
				//System.IO.File.Delete(path);
			}catch{

			}
		}

		static internal Workspace CreateWorkspace(string filePath, string workspaceName, string workspaceOutput, string workspaceRoot, bool copyLibs)
		{
			Workspace workspace = new Workspace();

			workspace.FilePath = filePath;
			workspace.RootDirectory = workspaceRoot;
			workspace.OutputDirectory = workspaceOutput;
			workspace.Name = workspaceName;

			if (!Directory.Exists(workspace.RootDirectory)){
				try {
					Directory.CreateDirectory(workspace.RootDirectory);
				}catch(Exception ex){
					throw new Exception(MainClass.Languages.Translate("cannot_create_root",workspace.RootDirectory) + "\n"+ ex.Message); 

					/*MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_create_root",workspace.RootDirectory), ex.Message, Gtk.MessageType.Error);
					md.ShowDialog();
					return null;*/
				}
			}

			if (!Directory.Exists(workspace.OutputMaskToFullPath)){
				try {
					Directory.CreateDirectory(workspace.OutputMaskToFullPath);
				}catch{

				}
			}
			//Console.WriteLine("MainClass.Settings.LibDirectory -> {0}",MainClass.Settings.LibDirectory);
			if (!Directory.Exists(MainClass.Settings.LibDirectory)){
				throw new Exception(MainClass.Languages.Translate("invalid_lids_dir",MainClass.Settings.LibDirectory) ); 
					/*MessageDialogs md =
						new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("invalid_lids_dir",MainClass.Settings.LibDirectory),"", Gtk.MessageType.Error);
					md.ShowDialog();
					return null;*/
			}

			if (copyLibs)
				MainClass.Tools.CopyDirectory(MainClass.Settings.LibDirectory, workspace.RootDirectory, true, true);
			else
			{
				MainClass.Tools.CreateLinks(MainClass.Settings.LibDirectory, workspace.RootDirectory, true, false);
			}
			workspace.SaveWorkspace();

			return workspace;
		}

	static internal Workspace OpenWorkspace(string filePath){
		if (System.IO.File.Exists(filePath))

		try {
			using (FileStream fs = File.OpenRead(filePath)) {
					XmlSerializer serializer = new XmlSerializer(typeof(Workspace));
					Workspace w = (Workspace)serializer.Deserialize(fs);
					w.FilePath = filePath;
					return w;
				}
			} catch (Exception ex) {
				MessageDialogs md =
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("workspace_is_corrupted"), ex.Message, Gtk.MessageType.Error,null);
				md.ShowDialog();
				return null;

			}
		else {
			//throw new FileNotFoundException();
			MessageDialogs md = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "", MainClass.Languages.Translate("workspace_is_corrupted"), Gtk.MessageType.Error,null);
			md.ShowDialog();
			return null;
		}
	}

	private static string ShellExec(string cmd,string path,string parms,out int exit_code){

		System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(path + cmd, parms);
            	psi.RedirectStandardOutput = true;
            	psi.UseShellExecute = false;

            	System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            	string tool_output = p.StandardOutput.ReadToEnd();
            	p.WaitForExit();

		exit_code = p.ExitCode;
        	    return tool_output;
        }

	#region Project

		public string GetFullPath(string path)
		{
			if (String.IsNullOrEmpty(path))
				return "";

			return FileUtility.RelativeToAbsolutePath(RootDirectory, path);
		}

		public string GetRelativePath(string path)
		{
			if (String.IsNullOrEmpty(path))
				return "";

			return FileUtility.AbsoluteToRelativePath(RootDirectory, path);
		}

		public Project CreateProject(Project prj, AppFile appFile,string skin, string theme,string[] fonts){

			string projectLocation = this.RootDirectory;
			string projectDir = System.IO.Path.GetFileNameWithoutExtension(prj.FilePath);

			Project p = FindProject(prj.FilePath);
			if (p == null) {
				p = new Project();
				if (!System.IO.File.Exists(prj.FilePath)) {
					string projectWorkingDir = System.IO.Path.Combine(projectLocation, projectDir);

					if (!System.IO.File.Exists(appFile.ApplicationFile)) {

						try{
							appFile.Save();	
						} catch(Exception ex){
							MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", appFile.ApplicationFile), ex.Message, Gtk.MessageType.Error);
							ms.ShowDialog();
							return null;

						}
					}

					string mainFile = System.IO.Path.Combine(projectWorkingDir,"main.ms");
					try {
						Directory.CreateDirectory(projectWorkingDir);

						using (StreamWriter file = new StreamWriter(mainFile)) {
							file.Write("// TODO: Add your code here.");
							file.Close();
							file.Dispose();
						}
						//Directory.CreateDirectory(System.IO.Path.Combine(projectWorkingDir, "core"));
						//Directory.CreateDirectory(System.IO.Path.Combine(projectWorkingDir, "ui"));
					} catch {
					}

					//p = new Project(prj.FilePath);
					prj.NewSkin=true;
					//p = new Project(filename);
					//p.ProjectDir = GetRelativePath(projectWorkingDir);
					//p.ProjectName = projectName;
					prj.AppFilePath = GetRelativePath(appFile.ApplicationFile);
					prj.StartFile = GetRelativePath(mainFile);

					prj.GenerateDevices(skin,theme,fonts);

					SaveProject(prj);
					Projects.Add(prj);
					ProjectsFile.Add(GetRelativePath(prj.FilePath));
					return prj;
				} else {
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_exist",prj.FilePath), "", Gtk.MessageType.Error);
					md.ShowDialog();
					return null;
				}
			} else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_opened",p.ProjectName), null, Gtk.MessageType.Error);
				md.ShowDialog();
				return null;
			}
		}

		public Project CreateProject(string filename, string projectName, string projectLocation, string projectDir, string aplicationFile,string skin, string theme)
		{
			Project p = FindProject(filename);
			if (p == null) {
				p = new Project();
				if (!System.IO.File.Exists(filename)) {
					string projectWorkingDir = System.IO.Path.Combine(projectLocation, projectDir);

					if (!System.IO.File.Exists(aplicationFile)) {
						AppFile appFile = new AppFile(aplicationFile, projectName);//, projectDir);
						appFile.Uses = "core ui uix uix-skin media sqlite net game2d crypto box2d graphics sensor";
						appFile.Main = "main.ms";
						appFile.Author = "Generated by Moscrif-Ide";
						appFile.Title = projectName;
						appFile.Copyright = " ";
						appFile.Version = "1.0";
						appFile.Orientation = "portrait";
						try{
							appFile.Save();	
						} catch(Exception ex){
							throw new Exception(MainClass.Languages.Translate("cannot_save_file", appFile.ApplicationFile)+"\n"+ ex.Message);
							/*MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", appFile.ApplicationFile), ex.Message, Gtk.MessageType.Error);
							ms.ShowDialog();
							return null;*/
						}
					}

					string mainFile = System.IO.Path.Combine(projectWorkingDir,"main.ms");
					try {
						Directory.CreateDirectory(projectWorkingDir);

						using (StreamWriter file = new StreamWriter(mainFile)) {
							file.Write("// TODO: Add your code here.");
							file.Close();
							file.Dispose();
						}
						//Directory.CreateDirectory(System.IO.Path.Combine(projectWorkingDir, "core"));
						//Directory.CreateDirectory(System.IO.Path.Combine(projectWorkingDir, "ui"));
					} catch {
					}

					p = new Project(filename);
					//p.ProjectDir = GetRelativePath(projectWorkingDir);
					//p.ProjectName = projectName;
					p.AppFilePath = GetRelativePath(aplicationFile);
					p.StartFile = GetRelativePath(mainFile);

					p.NewSkin=true;
					p.GenerateDevices(skin,theme);

					SaveProject(p);
					Projects.Add(p);
					ProjectsFile.Add(GetRelativePath(filename));
					return p;
				} else {
					throw new Exception(MainClass.Languages.Translate("project_is_exist",filename));
					/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_exist",filename), "", Gtk.MessageType.Error);
					md.ShowDialog();
					return null;*/
				}
			} else {
				throw new Exception(MainClass.Languages.Translate("project_is_opened",p.ProjectName));
				/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_opened",p.ProjectName), null, Gtk.MessageType.Error);
				md.ShowDialog();
				return null;*/
			}
		}

		public Project ConvertAppToProject(AppFile aplicationFile)
		{
			string mspDir = System.IO.Path.GetDirectoryName(aplicationFile.ApplicationFile);
			string projectDir = System.IO.Path.Combine(mspDir, aplicationFile.Name);//Id
			string filename = System.IO.Path.Combine(mspDir, aplicationFile.Name + ".msp");

			Project p = FindProject(filename);
			if (p == null) {
				p = new Project();
				if (!System.IO.File.Exists(filename)) {
					p = new Project(filename);

					//p.ProjectDir = GetRelativePath(projectDir);
					//p.ProjectName = aplicationFile.Name;
					p.AppFilePath = GetRelativePath(aplicationFile.ApplicationFile);
					p.NewSkin=true;
					p.GenerateDevices("","");
					SaveProject(p);
					Projects.Add(p);
					ProjectsFile.Add(GetRelativePath(p.FilePath));
					return p;
				} else {
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_exist",filename), "", Gtk.MessageType.Error);
					md.ShowDialog();
					return null;
				}
			} else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_opened", p.ProjectName), null, Gtk.MessageType.Error);
				md.ShowDialog();
				return null;
			}
		}

		public Project OpenProject(string filename)
		{
			return OpenProject(filename,true,false);
		}

		public Project OpenProject(string filename,bool showError,bool throwError)
		{
			Project p = FindProject(filename);
			if (p == null) {
				p = Project.OpenProject(filename,showError,throwError);

				if (p != null) {
					Projects.Add(p);
					ProjectsFile.Add(GetRelativePath(p.FilePath));
					return p;
				}
			} else {
				if(showError){
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_is_opened", p.ProjectName), null, Gtk.MessageType.Error,null);
					md.ShowDialog();
				} else if (throwError){
					throw new Exception(MainClass.Languages.Translate("project_is_opened", p.ProjectName));
				}
				return null;
			}
			return null;
		}

		public Project CloseProject(string appFile)
		{
			Project p = FindProject_byApp(appFile, true);
			if (p != null) {
				Projects.Remove(p);
				ProjectsFile.Remove(GetRelativePath(p.FilePath));
				SaveProject(p);
				return p;
			} else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("project_not_exit"), null, Gtk.MessageType.Error);
				md.ShowDialog();
				return null;
			}
		}

		internal Project FindProject(string filename)
		{
			return Projects.Find(x => x.FilePath == filename);
		}

		internal Project FindProject_byApp(string appname, bool fullpath)
		{
			if (fullpath){
				//Console.WriteLine("GetRelativePath appname 1->{0}",appname);
				appname = GetRelativePath(appname);
				//Console.WriteLine("GetRelativePath appname 2->{0}",appname);
			}
			return Projects.Find(x => x.RelativeAppFilePath == appname);//AppFilePath
		}

		internal void SaveProject(Project p)
		{

			try {
				/*XmlWriterSettings xmlSettings = new XmlWriterSettings();

				xmlSettings.NewLineChars = "\n";//Environment.NewLine;
				xmlSettings.Indent = true;
				xmlSettings.CloseOutput = true;
	
				XmlSerializer x_serial = new XmlSerializer( typeof( Project ) );
				using (XmlWriter wr = XmlWriter.Create( p.FilePath, xmlSettings )) {
				    x_serial.Serialize( wr, p );
				}

				XmlSerializer x_serial_PU = new XmlSerializer( typeof( ProjectUserFile ) );
				using (XmlWriter wr = XmlWriter.Create( p.ProjectUserSetting.FilePath, xmlSettings )) {
				    x_serial_PU.Serialize( wr, p.ProjectUserSetting );
				}*/
				p.Save();
			} catch (Exception ex) {
				MessageDialogs md = 
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_saving"), ex.Message, Gtk.MessageType.Error);
				md.ShowDialog();
			}
		}


		public bool RenameProject(Project prj, string newName){

			string newDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,newName);
			string newApp = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,newName+".app");
			string newPrj = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,newName+".msp");
			string oldPrjName =prj.ProjectName;

			if(File.Exists(newApp) || File.Exists(newPrj) || Directory.Exists(newDir)){
				MessageDialogs md =
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_rename_project",prj.ProjectName),MainClass.Languages.Translate("project_exist"), Gtk.MessageType.Error);
				md.ShowDialog();
				return false;
			}

			string oldPrjFile =prj.FilePath;

			System.IO.File.Move(prj.AbsolutAppFilePath, newApp);
			System.IO.File.Move(prj.FilePath ,newPrj);
			System.IO.Directory.Move(prj.AbsolutProjectDir,newDir);

			prj.AppFilePath = GetRelativePath(newApp);
			prj.FilePath =newPrj;

			foreach(FileItem fi in prj.FilesProperty){
				fi.FilePath = fi.FilePath.Replace("./"+oldPrjName+"/","./"+newName+"/");
				fi.FilePath = fi.FilePath.Replace(".\\"+oldPrjName+"\\",".\\"+newName+"\\");
				//MainClass.Workspace.GetRelativePath(d.FullName);
			}

			SaveProject(prj);

			ProjectsFile.Remove(GetRelativePath(oldPrjFile));
			ProjectsFile.Add(GetRelativePath(prj.FilePath));

			Devices.Device dev = prj.DevicesSettings.Find(x=>x.Devicetype == DeviceType.iOS_5_0);
			if(dev != null){
				Devices.PublishProperty pp =  dev.PublishPropertisMask.Find(x=>x.PublishName == Project.KEY_BUNDLEIDENTIFIER);
				if(pp != null)
					pp.PublishValue.Replace(oldPrjName,newName);
			}
			return true;
		}

		#endregion


		#region File/Directory
		public TypeFile? ToggleExcludeFile(string filename, string appFile)
		{
			if (String.IsNullOrEmpty(appFile))
				return null;

			Project prj = FindProject_byApp(appFile, true);

			if (prj == null)
				return null;
			string relativeFile = GetRelativePath(filename);

			FileItem fi = prj.ToggleExcludeFile(relativeFile);

			if (fi.IsExcluded)
				return TypeFile.ExcludetFile;
			else
				return TypeFile.SourceFile;

			/*
			FileItem fi = prj.ExcludesFiles.Find(x => x.FilePath == relativeFile);
			if (fi == null) {
				prj.ExcludesFiles.Add(new FileItem(relativeFile, true));
				return TypeFile.ExcludetFile;
			} else {
				prj.ExcludesFiles.Remove(fi);
				return TypeFile.SourceFile;
			}*/
		}

		public void SetAsStartup(string filename, string appFilePath)
		{
			if (String.IsNullOrEmpty(appFilePath))
				return;

			Project prj = FindProject_byApp(appFilePath, true);

			if (prj == null)
				return;

			if (!String.IsNullOrEmpty(filename)) {

				if (!String.IsNullOrEmpty(prj.StartFile)) {
					// change old file
					Gtk.TreeIter tiOld = MainClass.MainWindow.WorkspaceTree.GetStartUpIter(GetFullPath(prj.StartFile));
					MainClass.MainWindow.WorkspaceTree.UpdateIter(tiOld, String.Empty, String.Empty, (int)TypeFile.SourceFile);
				}
				AppFile appFile = prj.AppFile;
				appFile.Main = System.IO.Path.GetFileName(filename);
				try{
					appFile.Save();	
				} catch(Exception ex){
					MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("cannot_save_file", appFile.ApplicationFile), ex.Message, Gtk.MessageType.Error);
					ms.ShowDialog();
				}

				//prj.AppFile.Main = System.IO.Path.GetFileName(filename);
				//prj.AppFile.Save();
			}
			prj.StartFile = GetRelativePath(filename);
			SaveProject(prj);
		}

		public string AddDirectory(string dirPath, string selectedDir, int selectedTyp)
		{
			//string filename = System.IO.Path.GetFileName(Filename);
			string dirName = System.IO.Path.GetFileName(dirPath);
			string newPath = "";

			if (selectedTyp == (int)TypeFile.AppFile) {
				Project prj = FindProject_byApp(selectedDir, true);
				if (prj == null)
					return String.Empty;

				selectedDir = prj.AbsolutProjectDir;
				newPath = System.IO.Path.Combine(prj.AbsolutProjectDir, dirName);

			} else
				newPath = System.IO.Path.Combine(selectedDir, dirName);

			if (System.IO.File.Exists(newPath))
				newPath = System.IO.Path.Combine(selectedDir, "Copy_" + dirName);

			try {

				MainClass.Tools.CopyDirectory(dirPath, newPath, true, true);
			} catch {
				return String.Empty;
			}
			return newPath;
		}

		public string AddTheme(string selectedProj, int selectedTyp)
		{
			string themeName = null;
			if (selectedTyp != (int)TypeFile.AppFile)
				return null;

			Project prj = FindProject_byApp(selectedProj, true);
			if (prj == null)
				return String.Empty;

			CreateThemeDialog ctd = new CreateThemeDialog(prj);
			int result = ctd.Run();
			if (result == (int)Gtk.ResponseType.Ok) {

				themeName = ctd.ThemePath;
				string skinName = ctd.SkinPath;

				try {
					MainClass.Tools.CopyDirectory(skinName, themeName, true, true);
				} catch {
					themeName = null;
				}

			}
			ctd.Destroy();
			return themeName;
		}

		public string AddFile(string filePath, string selectedDir, int selectedTyp)
		{
			string filename = System.IO.Path.GetFileName(filePath);
			string newFile = "";

			if (selectedTyp == (int)TypeFile.AppFile) {
				Project prj = FindProject_byApp(selectedDir, true);
				if (prj == null)
					return String.Empty;
				newFile = System.IO.Path.Combine(prj.AbsolutProjectDir, filename);
				selectedDir = prj.AbsolutProjectDir;

			} else

				newFile = System.IO.Path.Combine(selectedDir, filename);
			if (System.IO.File.Exists(newFile))
				newFile = System.IO.Path.Combine(selectedDir, "Copy_" + filename);

			try {
				System.IO.File.Copy(filePath, newFile);
			} catch {
				return String.Empty;
			}

			return newFile;
		}

		public string CreateFile(string filename, string selectedDir, int selectedTyp,string content)
		{
			//string filename = System.IO.Path.GetFileName(Filename);
			string newFile = "";

			if (selectedTyp == (int)TypeFile.AppFile) {
				Project prj = FindProject_byApp(selectedDir, true);
				if (prj == null)
					return String.Empty;

				selectedDir = prj.AbsolutProjectDir;
			}

			if(Directory.Exists(selectedDir))
				newFile = System.IO.Path.Combine(selectedDir, filename);
			else return String.Empty;

			try {
				FileUtility.CreateFile(newFile,content); //CreateDirectory(newFile);

			} catch {
				MessageDialogs md = 
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_file"), filename, Gtk.MessageType.Error);
				md.ShowDialog();
				return String.Empty;
			}
			return newFile;
		}

		public string CreateDirectory(string Filename, string selectedDir, int selectedTyp)
		{
			string filename = System.IO.Path.GetFileName(Filename);
			string newFile = "";
			if (selectedTyp == (int)TypeFile.AppFile) {
				Project prj = FindProject_byApp(selectedDir, true);
				if (prj == null)
					return String.Empty;

				newFile = System.IO.Path.Combine(prj.AbsolutProjectDir, filename);
				selectedDir = prj.AbsolutProjectDir;

			} else
				newFile = System.IO.Path.Combine(selectedDir, filename);

			try {
				FileUtility.CreateDirectory(newFile);
			} catch {
				MessageDialogs md = 
				new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error_creating_dir"), filename, Gtk.MessageType.Error);
				md.ShowDialog();
				return String.Empty;
			}
			return newFile;
		}

		/*public void DeleteFile()
		{
			string filename = "";
			int typ = -1;
			Gtk.TreeIter ti = new Gtk.TreeIter();
			MainClass.MainWindow.WorkspaceTree.GetSelectedFile(out filename, out typ, out ti);

			if (typ == (int)TypeFile.Directory) {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, string.Format("Are you sure you want to permanently delete folder {0} ?", filename), "", Gtk.MessageType.Question);
				int result = md.ShowDialog();

				if (result != (int)Gtk.ResponseType.Yes)
					return;

				if (!System.IO.Directory.Exists(filename))
					return;

				try {
					System.IO.Directory.Delete(filename, true);
				} catch {
					MessageDialogs mdd = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Cannot delete File.",filename, Gtk.MessageType.Error);
					mdd.ShowDialog();
				}

			} else {
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, string.Format("Are you sure you want to permanent delete file {0} ?", filename), "", Gtk.MessageType.Question);
				int result = md.ShowDialog();

				if (result != (int)Gtk.ResponseType.Yes)
					return;

				if (!System.IO.File.Exists(filename))
					return;

				try {

					System.IO.File.Delete(filename);
				} catch {
					MessageDialogs mdd = 
					new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Cannot rename File.", filename, Gtk.MessageType.Error);
					mdd.ShowDialog();
				}
			}
			if (typ == (int)TypeFile.StartFile) {
				string appFile = MainClass.MainWindow.WorkspaceTree.GetSelectedProjectApp();
				if (String.IsNullOrEmpty(appFile))
					return;
				Project prj = FindProject_byApp(appFile, true);
				if (prj != null)
					prj.StartFile = String.Empty;
			}

			MainClass.MainWindow.WorkspaceTree.RemoveTree(ti);
		}
		*/
		#endregion
	}
}

