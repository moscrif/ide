using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Controls;
//using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
//using MessageDialogsUrl = Moscrif.IDE.Controls.MessageDialogUrl;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Execution;
using System.Text;

namespace  Moscrif.IDE.Task
{
	public class PublishAsynchronTask : ITaskAsyn
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask = StateEnum.OK;
		//private List<Condition> fullCondition;
		//private ProgressDialog progressDialog;

		private Project project;
		private List<CombinePublish> listCombinePublish;
		private bool stopProcess = false;
		//bool isPublishError = false;
		bool devicePublishError = false;
		bool allPublishError = false;
		bool isJavaInstaled = false;
		StringBuilder allErrors =  new StringBuilder();

		TaskMessage parentTask = new TaskMessage();


		public Gtk.Window ParentWindow
		{
			set;get;
		}


		public PublishAsynchronTask()
		{
		}

		#region ITask implementation
		public void Initialize(object dataObject)
		{
			if (dataObject.GetType()== typeof(List<CombinePublish>)){

				listCombinePublish =(List<CombinePublish>)dataObject;
			}
		}

		private string LastSeparator(string path){

			if(String.IsNullOrEmpty(path)) return path;

			if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) )
				path = path +System.IO.Path.DirectorySeparatorChar;

			return path;
		}

		private void SetChildError(string error,TaskMessage tm){

			//isPublishError = true;
			tm.Child =new TaskMessage(error);
			this.stateTask = StateEnum.ERROR;
			allPublishError = true;
		}


		private void SetError(string error,TaskMessage tm){
			tm.Message =error;

			allErrors.AppendLine(error);
			output.Add(tm);
			this.stateTask = StateEnum.ERROR;
			allPublishError = true;
			ShowError(error, "");
		}

		private void SetError(string error){
			output.Add(new TaskMessage(error,null,null));
			stateTask = StateEnum.ERROR;
			allPublishError = true;
			ShowError(error, "");
		}

		public bool ExecuteTask()
		{
			//MessageDialogs md =  new MessageDialogs(MessageDialogs.DialogButtonType.Cancel, "Cancel Publish.", filename, Gtk.MessageType.Question);
			Tool.Logger.LogDebugInfo("Publish no sign",null);

			/*if (MainClass.Settings.ClearConsoleBeforRuning){
				Tool.Logger.LogDebugInfo("CLEAR CONSOLE");
				MainClass.MainWindow.OutputConsole.Clear();
			}*/

			if (MainClass.Workspace.ActualProject == null) {
				SetError(MainClass.Languages.Translate("no_project_selected"));
				Tool.Logger.LogDebugInfo(MainClass.Languages.Translate("no_project_selected"),null);
				return false;
			}

			project = MainClass.Workspace.ActualProject;
			Tool.Logger.LogDebugInfo(String.Format("Project -> {0}",MainClass.Workspace.ActualProject),null);

			ShowInfo("Publish project" ,MainClass.Workspace.ActualProject.ProjectName);

			if (String.IsNullOrEmpty(project.ProjectOutput)){

				if (!String.IsNullOrEmpty(MainClass.Workspace.OutputDirectory)){
					project.ProjectOutput  = MainClass.Workspace.OutputDirectory;

				} else project.ProjectOutput  = project.AbsolutProjectDir;
				Tool.Logger.LogDebugInfo(String.Format("Project Output-> {0}",project.ProjectOutput),null);

			}

			if(!Directory.Exists(project.OutputMaskToFullPath)){
				try{
					Tool.Logger.LogDebugInfo(String.Format("CREATE DIR Output-> {0}",project.OutputMaskToFullPath),null);
					Directory.CreateDirectory(project.OutputMaskToFullPath);
				}catch
				{
					SetError(MainClass.Languages.Translate("cannot_create_output"));
					return false;
				}
			}

			List<string> filesList = new List<string>();

			//project.GetAllFiles(ref filesList);
			GetAllFiles(ref filesList,project.AbsolutProjectDir );

			string tempDir =  MainClass.Paths.TempPublishDir;//System.IO.Path.Combine(MainClass.Settings.PublishDirectory,"_temp");
			Tool.Logger.LogDebugInfo(String.Format("Temp Directory-> {0}",tempDir),null);

			if (!Directory.Exists(tempDir)){

				try{
					Tool.Logger.LogDebugInfo(String.Format("CREATE Temp Directory-> {0}",tempDir),null);
					Directory.CreateDirectory(tempDir);
				} catch{
					SetError(MainClass.Languages.Translate("cannot_create_temp_f1",tempDir));
					return false;
				}
			}

			if ((listCombinePublish == null) || (listCombinePublish.Count <1)){
				Tool.Logger.LogDebugInfo("Publish list empty",null);
				SetError(MainClass.Languages.Translate("publish_list_is_empty"));
				return false;
				//project.GeneratePublishCombination();
			}

			bool isAndroid = false;

			foreach(CombinePublish ccc in  listCombinePublish){
				CombineCondition crPlatform = ccc.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Platform.Id);
				if(crPlatform != null){

					if ((crPlatform.RuleId == (int)DeviceType.Android_1_6) ||
					    (crPlatform.RuleId == (int)DeviceType.Android_2_2)){
						isAndroid = true;
						//Console.WriteLine("ANDROID FOUND");
						CheckJava();
					}
				}
			}

			if(!isJavaInstaled && isAndroid){
				ShowError(MainClass.Languages.Translate("java_missing"),MainClass.Languages.Translate("java_missing_title"));
				/*MessageDialogsUrl md = new MessageDialogsUrl(MessageDialogsUrl.DialogButtonType.Ok,MainClass.Languages.Translate("java_missing"), MainClass.Languages.Translate("java_missing_title"),"http://moscrif.com/java-requirement", Gtk.MessageType.Error,ParentWindow);
				md.ShowDialog();*/
			}

			/*if (listCombinePublish.Count > 0) {

				double step = 1 / (listCombinePublish.Count * 1.0);
				MainClass.MainWindow.ProgressStart(step, MainClass.Languages.Translate("publish"));
				progressDialog = new ProgressDialog(MainClass.Languages.Translate("publishing"),ProgressDialog.CancelButtonType.Cancel,listCombinePublish.Count,ParentWindow);//MainClass.MainWindow
			}*/

			foreach(CombinePublish ccc in  listCombinePublish){//listCC ){
				//if (!ccc.IsSelected) continue;
				//Console.WriteLine(ccc.ToString());

				if (stopProcess) break;

				if(String.IsNullOrEmpty(project.ProjectArtefac) ){
					project.ProjectArtefac = System.IO.Path.GetFileNameWithoutExtension(project.RelativeAppFilePath);
				}
				string fileName =  project.ProjectArtefac;

				List<ConditionDevice> condList = new List<ConditionDevice>();


				foreach(CombineCondition cr in ccc.combineRule){
					ConditionDevice cd = new ConditionDevice (cr.ConditionName,cr.RuleName);
					//condList.Add(new ConditionDevice(cr.ConditionName,cr.RuleName));
					if(cr.ConditionId == MainClass.Settings.Resolution.Id){
						Rule rl = MainClass.Settings.Resolution.Rules.Find(x=>x.Id == cr.RuleId);
						if(rl!= null){
							cd.Height = rl.Height;
							cd.Width = rl.Width;
						}

					}
					condList.Add(cd);

					fileName = fileName.Replace(String.Format("$({0})",cr.ConditionName),cr.RuleName);
				}

				parentTask = new TaskMessage("Publishing",fileName,null);
				devicePublishError = false;

				ShowInfo( "Publishing",fileName);

				/*if (progressDialog != null)
					 progressDialog.SetLabel (fileName );*/

				if (Directory.Exists(tempDir)) {
					try{
						DirectoryInfo di = new DirectoryInfo(tempDir);
						foreach (DirectoryInfo d in di.GetDirectories()){
							d.Delete(true);
						}
						foreach (FileInfo f in di.GetFiles()){
							f.Delete();
						}

					} catch {
					}
				}

				CombineCondition crPlatform = ccc.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Platform.Id);
				CombineCondition crRsolution = ccc.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Resolution.Id);

				if (crPlatform == null) {
					SetError(MainClass.Languages.Translate("platform_not_found", MainClass.Settings.Platform.Name),parentTask);
					continue;
				}

				Device dvc = project.DevicesSettings.Find(x=>x.TargetPlatformId == crPlatform.RuleId);

				if (dvc == null) {
					SetError(MainClass.Languages.Translate("device_not_found", crPlatform.ConditionName, crPlatform.RuleName),parentTask);
					continue;
				}

				if (((crPlatform.RuleId == (int)DeviceType.Android_1_6) ||
				  (crPlatform.RuleId == (int)DeviceType.Android_2_2))
				    && (!isJavaInstaled)){
					SetError(MainClass.Languages.Translate("java_missing"),parentTask);
					ShowError(MainClass.Languages.Translate("java_missing"),fileName);
					continue;
				}

				string dirPublish = MainClass.Tools.GetPublishDirectory(dvc.Platform.Specific);//System.IO.Path.Combine(MainClass.Settings.PublishDirectory,dvc.Platform.Specific);//crPlatform.RuleName);

				if (!Directory.Exists(dirPublish)){
					SetError(MainClass.Languages.Translate("publish_tool_not_found"),parentTask);
					continue;
				}

				if (String.IsNullOrEmpty(dirPublish)) {
					SetError(MainClass.Languages.Translate("publish_tool_not_found_f1"),parentTask);
					continue;
				}

				dvc.Application = project.AppFile.Name; // TTT

				if (String.IsNullOrEmpty(project.ProjectOutput)){

					if (!String.IsNullOrEmpty(MainClass.Workspace.OutputDirectory)){

						project.ProjectOutput  = MainClass.Workspace.OutputDirectory;

					} else project.ProjectOutput  = project.AbsolutProjectDir;
				}

				dvc.Output_Dir = project.OutputMaskToFullPath;

				dvc.Temp = tempDir;

				dvc.Temp = LastSeparator(dvc.Temp);
				dvc.Publish = LastSeparator(MainClass.Settings.PublishDirectory);
				dvc.Root = LastSeparator(MainClass.Workspace.RootDirectory);
				dvc.Output_Dir = LastSeparator(dvc.Output_Dir);

				dvc.Conditions =condList.ToArray();

				List<int> mergeResolutions = new List<int>();
				string resolution = crRsolution.RuleName;
				mergeResolutions.Add(crRsolution.RuleId);

				// Ak je zaskrtnute Merge All Resolution
				// Najdem vsetky publishovatelne combinacie danej platformy (napr. vsetky publishovane andrroidy)
				// a vytiahnem z nich rezolution a spojim do string odeleneho &
				if(dvc.Includes.Skin != null){
					dvc.Includes.Skin.ResolutionJson = dvc.Includes.Skin.Resolution;
					if(!String.IsNullOrEmpty(dvc.Includes.Skin.Name)){
						if(project.IncludeAllResolution){
							resolution = "";
							mergeResolutions.Clear();
							foreach(CombinePublish cp in  listCombinePublish){
								CombineCondition crPlatform2 = cp.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Platform.Id && x.RuleId== crPlatform.RuleId);
								if(crPlatform2 != null){
									CombineCondition crResolution2 = cp.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Resolution.Id );
									if(crResolution2!= null){
										resolution =resolution+crResolution2.RuleName+"&";
										mergeResolutions.Add(crResolution2.RuleId);
									}
								}

							}
							resolution = resolution.Remove(resolution.Length - 1, 1);
							ConditionDevice cd = condList.Find(x=>x.Name == MainClass.Settings.Resolution.Name);
							if(cd != null){
								cd.Value = resolution;
								dvc.Conditions =condList.ToArray();
							}

						}
					}
				}

				List<string> filesForPublish = new List<string>();

				foreach(string file in filesList){
					if (System.IO.Path.GetExtension(file)==".msc") continue;

					string fileUpdate = FileUtility.AbsoluteToRelativePath(MainClass.Workspace.RootDirectory ,file);

					if ( project.FilesProperty != null){

						// vyhldam property suborov a pozriem ci nemam nejake conditiony nastavene
						FileItem fi =  project.FilesProperty.Find(x => x.SystemFilePath == fileUpdate);
						if (fi != null)
						{
							if (fi.IsExcluded) continue;

							if(fi.ConditionValues == null){ // nema ziadne konditions tak ho pridam
								goto nav1;
								//continue;
							}
							foreach(CombineCondition cr in ccc.combineRule){

								ConditionRule conRile = fi.ConditionValues.Find(x=>x.ConditionId == cr.ConditionId);
								if (conRile != null){
									//if ((conRile.RuleId != cr.RuleId)) // subor ma condition daneho typu, ale nastavenu na inu hodnotu
									//goto nav;

									int resolutionId = -1;

									if(conRile.ConditionId == MainClass.Settings.Resolution.Id){
										resolutionId=mergeResolutions.FindIndex(x=>x == conRile.RuleId);
										//continue;
									}

									// mam merge resolution a subor patri do niektoreho mergnuteho resolution
									if((conRile.ConditionId == MainClass.Settings.Resolution.Id) && (resolutionId>-1)){
										goto nav1;
									}

									// subor ma condition daneho typu, ale nastavenu na inu hodnotu
									if ((conRile.RuleId != cr.RuleId)){
										string msg1 =String.Format("File {0} REMOVE {1}-> {0}",fileUpdate,conRile.RuleId);
										Tool.Logger.LogDebugInfo(msg1,null);
										Console.WriteLine(msg1);
										goto nav;
									}
								}
							}
						}
					}
					nav1:
					fileUpdate = FileUtility.AbsoluteToRelativePath(project.AbsolutProjectDir,file);
					fileUpdate = FileUtility.TrimStartingDotCharacter(fileUpdate);
					fileUpdate = FileUtility.TrimStartingDirectorySeparator(fileUpdate);
					filesForPublish.Add(fileUpdate);

						/*string msg =String.Format("Add File to Publish-> {0}",fileUpdate);
						Tool.Logger.LogDebugInfo(msg,null);
						Console.WriteLine(msg);*/

					nav:;
				}
				dvc.Includes.Files = filesForPublish.ToArray();

				List<string> fontForPublish = new List<string>();
				foreach(string fnt in dvc.Includes.Fonts){
					string tmp =fnt;
					tmp = tmp.Replace('/',System.IO.Path.DirectorySeparatorChar);
					tmp = tmp.Replace('\\',System.IO.Path.DirectorySeparatorChar);

					fontForPublish.Add(tmp);

				}
				dvc.Includes.Fonts =fontForPublish.ToArray();

				dvc.Output_Name = fileName;

				dvc.PublishPropertisFull = new List<PublishProperty>();

				foreach(PublishProperty pp in dvc.PublishPropertisMask){

					PublishProperty ppFull = new PublishProperty(pp.PublishName);
					ppFull.PublishValue = pp.PublishValue;
					dvc.PublishPropertisFull.Add(ppFull);

					if (pp.PublishName == Project.KEY_PERMISSION) continue;
					if (pp.PublishName == Project.KEY_CODESIGNINGIDENTITY) continue;
					if (pp.PublishName == Project.KEY_STOREPASSWORD) continue;
					if (pp.PublishName == Project.KEY_KEYPASSWORD) continue;
					if (pp.PublishName == Project.KEY_SUPPORTEDDEVICES) continue;
					if (pp.PublishName == Project.KEY_INSTALLOCATION) continue;
					if (pp.PublishName == Project.KEY_BUNDLEIDENTIFIER) continue;
					if (pp.PublishName == Project.KEY_ALIAS) continue;
					if (pp.PublishName == Project.KEY_APPLICATIONID) continue;
					if (pp.PublishName == Project.KEY_PASSWORD) continue;

					ppFull.PublishValue = project.ConvertProjectMaskPathToFull(pp.PublishValue);
				}

				//

				/*if(dvc.Includes.Skin != null){
					dvc.Includes.Skin.ResolutionJson = dvc.Includes.Skin.Resolution;
					if(!String.IsNullOrEmpty(dvc.Includes.Skin.Name)){
						if(project.IncludeAllResolution){
							dvc.Includes.Skin.ResolutionJson = "*";
						}
					}
				}*/
				dvc.LogDebug = MainClass.Settings.LogPublish;
				dvc.ApplicationType = project.ApplicationType;

				dvc.FacebookAppID = project.FacebookAppID;

				if(String.IsNullOrEmpty(project.FacebookAppID))
					dvc.FacebookAppID ="";

				string path = System.IO.Path.Combine(dirPublish,"settings.mso");//fileName+".mso"); //dvc.TargetPlatform + "_settings.mso");
				string json = dvc.GenerateJson();

				if(String.IsNullOrEmpty(json)){
					SetError(MainClass.Languages.Translate("cannot_generate_mso"),parentTask);
					continue;
				}

				try {
					using (StreamWriter file = new StreamWriter(path)) {
						file.Write(json);
						file.Close();
					}

				} catch {
					SetError(MainClass.Languages.Translate("cannot_generate_mso"),parentTask);
					continue;
				}

				//var platformRule = MainClass.Settings.Platform.Rules.Find(x => x.Id == dvc.TargetPlatformId);

				string appFile =dvc.Platform.Specific+ ".app"; /*dvc.TargetPlatform*///platformRule.Specific + ".app";
				string fullAppPath = System.IO.Path.Combine(MainClass.Settings.PublishDirectory,appFile);

				if (!System.IO.File.Exists(fullAppPath) ){

					SetError(MainClass.Languages.Translate("publish_tool_not_found_f2"),parentTask);
					continue;
				}

				RunPublishTool(appFile,parentTask);
				/*if (RunPublishTool(appFile,parentTask) ){
					parentTask.Message =MainClass.Languages.Translate("publish_successfully_done");
					output.Add(parentTask);

					//output.Add(new TaskMessage(MainClass.Languages.Translate("publish_successfully_done"),dvc.Platform.Specific,null));
				} else {
					parentTask.Message =MainClass.Languages.Translate("publish_error");
					Console.WriteLine(parentTask.Child.Message);
					output.Add(parentTask);
					//output.Add(new TaskMessage(MainClass.Languages.Translate("publish_error"),dvc.Platform.Specific,null));
				}*/
				if(MainClass.Platform.IsMac){
					ExitPublish(null,null);
				}

				if(devicePublishError){
					parentTask.Message = StateEnum.ERROR.ToString();
					allPublishError = true;
					ShowError(StateEnum.ERROR.ToString()," ");
					//ShowError(lastMessage.Trim(),fileName);
				}else {
					parentTask.Message = StateEnum.OK.ToString();
					ShowInfo(" ",StateEnum.OK.ToString());
				}
				//parentTask.Message = //this.StateTask.ToString();
				//Console.WriteLine(parentTask.Child.Message);
				output.Add(parentTask);

				MainClass.MainWindow.ProgressStep();
				/*if (progressDialog != null)
					cancelled = progressDialog.Update (fileName );*/
			}

			MainClass.MainWindow.ProgressEnd();

			/*if(progressDialog!= null){
				progressDialog.Destroy();
			}*/
			//Console.WriteLine("allPublishError -> {0}",allPublishError);
			if(allPublishError){
				this.stateTask = StateEnum.ERROR;
				string s = allErrors.ToString();

				if(s.Length > 120){
					s = s.Substring(0,120);
					s= s+ " ... and more.";
				}

				ShowError(MainClass.Languages.Translate("publish_error")," ");
				return false;
			} if(stopProcess){
				this.stateTask = StateEnum.CANCEL;
				ShowInfo(MainClass.Languages.Translate("Canceled")," ");

				return false;
			}
			else {
				this.stateTask = StateEnum.OK;

				ShowInfo(" ",MainClass.Languages.Translate("publish_successfully_done"));

				/*if(MainClass.Settings.OpenOutputAfterPublish){
					if (!String.IsNullOrEmpty(project.ProjectOutput)){
						MainClass.Tools.OpenFolder(project.OutputMaskToFullPath);
					}
				}*/
				return true;
			}
		}


		private void GetAllFiles(ref List<string> filesList,string path)
		{
			if (!Directory.Exists(path)){
				Tool.Logger.LogDebugInfo(String.Format("Directory Not Exist-> {0}",path),null);
				return;
			}

			DirectoryInfo di = new DirectoryInfo(path);

			DirectoryInfo diOutput = new DirectoryInfo(MainClass.Workspace.ActualProject.OutputMaskToFullPath) ;

			foreach (DirectoryInfo d in di.GetDirectories()){
				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForPublish);

				if(di.FullName == diOutput.FullName)
					continue;

				if(indx<0){
					GetAllFiles(ref filesList, d.FullName);
				}
			}

			foreach (FileInfo f in di.GetFiles()) {

				if (f.Extension == ".msc") continue;

				filesList.Add( f.FullName );
				//string msg =String.Format("Add File all Files-> {0}",f.FullName);
				//Tool.Logger.LogDebugInfo(msg,null);
				//Console.WriteLine(msg);
			}

		}

		public bool RunPublishTool (string appFile,TaskMessage parentTask)
		{
			if(MainClass.MainWindow.RunningEmulator){

				//output.Add( new TaskMessage(MainClass.Languages.Translate("emulator_is_running"),null,null));
				SetChildError(MainClass.Languages.Translate("emulator_is_running"),parentTask);
				return false;
			}

			string cmd = Path.Combine(MainClass.Settings.EmulatorDirectory,  "moscrif.exe");

			if(MainClass.Platform.IsMac){
				//Console.WriteLine("EmulatorDirectory --> {0}",MainClass.Settings.EmulatorDirectory);

				//cmd = "open";// + MainClass.Settings.EmulatorDirectory,  "moscrif.app");
				string file = System.IO.Path.Combine( MainClass.Settings.EmulatorDirectory,  "Moscrif.app");//.app
				file = System.IO.Path.Combine(file,  "Contents");
				file = System.IO.Path.Combine(file,  "MacOS");
				file = System.IO.Path.Combine(file,  "Moscrif");
				cmd = file;
			}

			if (MainClass.Platform.IsWindows){

				if (!System.IO.File.Exists(cmd)) {
					SetError(MainClass.Languages.Translate("emulator_not_found"));
					return false;
				}
			}

			string args = String.Format("/o console /w nowindow /t nowarn /d {0} /f {1}",MainClass.Settings.PublishDirectory+System.IO.Path.DirectorySeparatorChar, appFile);
			if(MainClass.Platform.IsMac){
				args = String.Format("-o console -w nowindow -t nowarn -d {0} -f {1} ", MainClass.Settings.PublishDirectory+System.IO.Path.DirectorySeparatorChar, appFile);
				/*Process []pArry = Process.GetProcesses();
				foreach(Process p in pArry)
				{
					if(p != null){
						try {
							if(p.ProcessName == "Moscrif"){
								p.Kill();
								MainClass.MainWindow.RunningEmulator= false;
							}
							//string s = p.ProcessName;
							//s = s.ToLower();
							//Console.WriteLine("\t"+s);
						} catch (Exception ex){
							Console.WriteLine(ex.Message);
						}
					}
				}*/
				}


			try{
				Console.WriteLine("cmd ->> {0}",cmd);
				Console.WriteLine("args ->> {0}",args);

				Tool.Logger.Info( String.Format("cmd ->> {0}",cmd) );
				Tool.Logger.Info( String.Format("args ->> {0}",args) );

				MainClass.MainWindow.RunProcessWait(cmd, args, MainClass.Settings.EmulatorDirectory,ProcessOutputChange,ExitPublish);


			}catch (Exception ex){
				//output.Add(new TaskMessage(ex.Message));
				SetChildError(ex.Message,parentTask);
				//isPublishError = true;
				return false;
			}
			return true;
		}

		public void CheckJava ()
		{
			string cmd =MainClass.Settings.JavaCommand;
			string args = MainClass.Settings.JavaArgument;
			Console.WriteLine("cmd -->"+cmd);
			try{
				ProcessWrapper pw = MainClass.ProcessService.StartProcess(cmd,args, "", ProcessOutputJavaChange, ProcessOutputJavaChange);
				pw.WaitForExit();

			}catch {//(Exception ex){
				//output.Add(new TaskMessage(ex.Message));
				//parentTask.Child =new TaskMessage(ex.Message);
				//stateTask = StateEnum.ERROR;
				//isPublishError = true;
				//return false;
			}
		}


		void ProcessOutputJavaChange(object sender, string message)
		{
			Console.WriteLine(message);
			if(message.Contains("java version") )
				isJavaInstaled = true;

		}

		private void ShowError(string error1, string error2){
			if(WriteStep!=null){
				WriteStep(this,new StepEventArgs(error2,error1,true));
			}
			/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok,error1, error2, Gtk.MessageType.Error,ParentWindow);
			md.ShowDialog();*/

		}

		private void ShowInfo(string error1, string error2){
			if(WriteStep!=null){
				WriteStep(this,new StepEventArgs(error1,error2,false));
			}
			/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok,error1, error2, Gtk.MessageType.Info,ParentWindow);
			md.ShowDialog();*/

		}

		public void OnEndTaskWrite(object sender, string name, string status, List<TaskMessage> errors){
			if(EndTaskWrite!= null)
				EndTaskWrite(sender, name,status, errors);
		}

		public void StopTask(){
			stopProcess = true;
		}

		// not use
		public void OnTaskOutputChanged(object sender, string name, string status, List<TaskMessage> errors){
			if(TaskOutputChanged!= null)
				TaskOutputChanged(sender, name,status, errors);

		}

		public event ProcessTaskHandler TaskOutputChanged;
		public event ProcessErrorHandler ErrorWrite;
		public event ProcessErrorHandler LogWrite;
		public event ProcessTaskHandler EndTaskWrite;

		public string Name
		{
			get {
				return "Publish";
			}
		}

		public StateEnum StateTask
		{
			get {
				return stateTask;
			}
		}

		public List<TaskMessage> Output
		{
			get {
				return output;
			}
		}

		public ITask ChildrenTask
		{
			get {
				return null;
			}
		}

		private static List<T> CloneList<T>(List<T> listToClone) where T: ICloneable
		{
			List<T> newList = new List<T>(listToClone.Count);

			listToClone.ForEach((item) =>
    				{
        				newList.Add((T)item.Clone());
    				});
			return newList;
		}


		#endregion

		#region ITaskAsyn implementation
		public event EventHandler<StepEventArgs> WriteStep;
		#endregion

		#region private
		void ExitPublish(object sender, EventArgs e){
			if(lastMessage.Contains("Publishing succesful")){
				devicePublishError = false;
				return;
			} else{
				if (ErrorWrite!=null){
					devicePublishError = true;
					//TaskMessage tm2 = new TaskMessage(lastMessage,"","");
					TaskMessage tm =new TaskMessage(lastMessage.Trim(), "","" );
					allErrors.AppendLine(lastMessage.Trim());
					parentTask.Child = tm;
					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,StateEnum.ERROR.ToString(),tm);
						this.stateTask = StateEnum.ERROR;
					}
				}
			}
		}
		
		void ProcessOutputChange(object sender, string message)
		{
			//Console.WriteLine("-->"+message);
			MainClass.MainWindow.OutputConsole.WriteText(message);
			//return;
			string msg = message.Trim();
			if (String.IsNullOrEmpty(msg))
				return;
			//if (string.Compare(msg,"OK",true)>-1 )
			//	return;
			//isPublishError = true;
			ParseOutput(message);
		}
		
		
		private string lastMessage = "";
		private void ParseOutput(string message)
		{
			lastMessage = message;
			
			if (String.IsNullOrEmpty(message))
				return;
			
			if(message.Contains("Publishing succesful")){
				devicePublishError = false;
				//this.stateTask = StateEnum.OK;
				return;
			}
			GetLog(message);
			return;
			//Publishing succesful
			/*devicePublishError = true;
			TaskMessage tm =new TaskMessage(message.Trim(), "","" );
			allErrors.AppendLine(message.Trim());

			parentTask.Child = tm;

			this.stateTask = StateEnum.ERROR;

			if (ErrorWrite!=null){
					ErrorWrite(this,this.Name,StateEnum.ERROR.ToString(),tm);
			}

			Console.WriteLine("devicePublishError ParseOutput2->{0}",devicePublishError);*/
		}
		
		
		private TaskMessage GetLog(string message){
			
			TaskMessage tm =new TaskMessage();
			try{
				//Log-I: Log-W: Log-E:
				message= message.Replace("LOG-I","" );
				message= message.Replace("LOG-W","" );
				message= message.Replace("LOG-E","" );
				
				message =message.Replace("\t","»");
				
				message =message.Replace("\n\r","");
				message =message.Replace("\r\n","");
				message =message.Replace("\n","");
				message =message.Replace("\r","");
				
				//Console.WriteLine("messageError -> "+ message);
				
				string[] msg = message.Split('»');
				
				//Console.WriteLine("match.Count -> "+ msg.Length);
				
				if (msg.Length <3){
					//Tool.Logger.Error("message ->"+message+"<");
					return null;
				}
				
				string  filename= msg[0];
				string  line= msg[1];
				string  error=msg[2];
				
				filename = filename.Replace('/',System.IO.Path.DirectorySeparatorChar);
				
				/*Console.WriteLine("error -> "+ error);
				Console.WriteLine("filename -> "+ filename);
				Console.WriteLine("line -> "+ line);
				Console.WriteLine("this.Name -> "+ this.Name);*/
				
				tm =new TaskMessage(error, filename, line);
				
				//this.output.Add(tm);
				this.stateTask = StateEnum.ERROR;
				//messageError = null;
				
				if(filename.Contains("Log-E:")){
					//Console.WriteLine("YES LOGE");
					if (ErrorWrite!=null){
						TaskMessage tm2 = new TaskMessage(error,"","");
						
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm2);
					}
				} //else {
				
				// this if
				if( filename.Contains("Log-E:") || filename.Contains("Log-W:") || filename.Contains("Log-I:") ){
					if (LogWrite!=null){
						LogWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
				}
				
				
			} catch {
				
			}
			return tm;
		}
		#endregion

	}
}

