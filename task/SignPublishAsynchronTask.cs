using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Execution;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Option;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Devices;
using System.Timers;
//using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
//using MessageDialogsUrl = Moscrif.IDE.Controls.MessageDialogUrl;
using System.Text;
using Moscrif.IDE.Iface;

namespace Moscrif.IDE.Task
{
	public class SignPublishAsynchronTask : ITaskAsyn
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask;
		Project project;
		private bool stopProcess = false;

		private List<CombinePublish> listCombinePublish;

		TaskMessage parentTask = new TaskMessage();
		//List<string> listFiles = new List<string>();
		public SignPublishAsynchronTask()
		{
		}

		public Gtk.Window ParentWindow
		{
			set;get;
		}

		#region ITask implementation
		public void Initialize(object dataObject)
		{
			if (dataObject.GetType()== typeof(List<CombinePublish>)){

				listCombinePublish =(List<CombinePublish>)dataObject;
			}
		}

		bool exitCompile = false;
		//bool isPublishError = false;
		bool devicePublishError = false;
		bool allPublishError = false;
		bool isJavaInstaled = false;
		StringBuilder allErrors =  new StringBuilder();

		private void SetChildError(string error,TaskMessage tm){

			allErrors.AppendLine(error);
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

		private void StopProces(){
			this.stateTask = StateEnum.CANCEL;
			ShowInfo(MainClass.Languages.Translate("Canceled")," ",-1);
		}

		private void SetError(string error){
			output.Add(new TaskMessage(error,null,null));
			stateTask = StateEnum.ERROR;
			allPublishError = true;
			//MainClass.MainWindow.OutputConsole.WriteError(String.Format("Platform {0} not found!", MainClass.Settings.Platform.Name));
			ShowError(error, "");
		}
		private void SetError(string error1,string error2){
			output.Add(new TaskMessage(error2,error1,null));
			stateTask = StateEnum.ERROR;
			allPublishError = true;
			//MainClass.MainWindow.OutputConsole.WriteError(String.Format("Platform {0} not found!", MainClass.Settings.Platform.Name));
			ShowError(error1, " ");
		}

		public bool ExecuteTask()
		{

			if (MainClass.Workspace.ActualProject == null) {
				SetError(MainClass.Languages.Translate("no_project_selected"));
				return false;
			}

			project = MainClass.Workspace.ActualProject;

			if (String.IsNullOrEmpty(project.ProjectOutput)){

				if (!String.IsNullOrEmpty(MainClass.Workspace.OutputDirectory)){
					project.ProjectOutput  = MainClass.Workspace.OutputDirectory;

				} else project.ProjectOutput  = project.AbsolutProjectDir;
			}

			if(!Directory.Exists(project.OutputMaskToFullPath)){
				try{
					Directory.CreateDirectory(project.OutputMaskToFullPath);
				}catch
				{
					SetError(MainClass.Languages.Translate("cannot_create_output"));
				return false;
				}
			}
			ShowInfo("Publish project" ,MainClass.Workspace.ActualProject.ProjectName);

			stateTask = StateEnum.OK;

			//################ uvodne kontroly
			if (MainClass.MainWindow.RunningEmulator) {
				SetError(MainClass.Languages.Translate("emulator_is_running"));
				return false;
			}
			
			string cmd = Path.Combine(MainClass.Settings.EmulatorDirectory, "moscrif.exe");

			if(MainClass.Platform.IsMac){
				//Console.WriteLine("EmulatorDirectory --> {0}",MainClass.Settings.EmulatorDirectory);

				//cmd = "open";// + MainClass.Settings.EmulatorDirectory,  "moscrif.app");
				string file = System.IO.Path.Combine( MainClass.Settings.EmulatorDirectory,  "Moscrif.app");//.app
				file = System.IO.Path.Combine(file,  "Contents");
				file = System.IO.Path.Combine(file,  "MacOS");
				file = System.IO.Path.Combine(file,  "Moscrif");
				cmd = file;
				Tool.Logger.LogDebugInfo(String.Format("command compile MAC ->{0}",cmd),null);
			}

			if(MainClass.Platform.IsWindows){

				if (!System.IO.File.Exists(cmd)) {
					SetError(MainClass.Languages.Translate("emulator_not_found"));
					return false;
				}
			}
			
			string tempDir =  MainClass.Paths.TempPublishDir;//System.IO.Path.Combine(MainClass.Settings.PublishDirectory,"_temp");

			if (!Directory.Exists(tempDir)){

				try{
					Directory.CreateDirectory(tempDir);
				} catch{
					SetError(MainClass.Languages.Translate("cannot_create_temp_f1"));
					return false;
				}
			}

			if ((listCombinePublish == null) || (listCombinePublish.Count <1)){
				SetError(MainClass.Languages.Translate("publish_list_is_empty"));
				return false;
				//project.GeneratePublishCombination();
			}

			//bool cancelled = false;
			bool isAndroid = false;

			foreach(CombinePublish ccc in  listCombinePublish){
				CombineCondition crPlatform = ccc.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Platform.Id);
				if(crPlatform != null){

					if ((crPlatform.RuleId == (int)DeviceType.Android_1_6)||
					    (crPlatform.RuleId == (int)DeviceType.Android_2_2)){
						Console.WriteLine("ANDROID FOUND");
						CheckJava();
					}
				}
			}

			if(!isJavaInstaled && isAndroid){
				ShowError(MainClass.Languages.Translate("java_missing"),MainClass.Languages.Translate("java_missing_title"));
				/*MessageDialogsUrl md = new MessageDialogsUrl(MessageDialogsUrl.DialogButtonType.Ok,MainClass.Languages.Translate("java_missing"), MainClass.Languages.Translate("java_missing_title"),"http://moscrif.com/java-requirement", Gtk.MessageType.Error,ParentWindow);
				md.ShowDialog();*/
			}


			project = MainClass.Workspace.ActualProject;

			if((MainClass.User == null) || (string.IsNullOrEmpty(MainClass.User.Token))){
				SetError(MainClass.Languages.Translate("invalid_login"));
				return false;
			}

			//########################## kompilovanie
			if(stopProcess){
				StopProces();
				return false;
			}

			try {
				List<string> list = new List<string>();
				
				GetComands(project.AbsolutProjectDir, ref list,true);
				string[] libs = project.AppFile.Libs;

				foreach (string lib in libs){
					if(string.IsNullOrEmpty(lib)) continue;
					string pathLib = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,lib);
					GetComands(pathLib, ref list,true);
				}


				if (list.Count > 0) {
					double step = 1 / (list.Count * 1.0);
						MainClass.MainWindow.ProgressStart(step, MainClass.Languages.Translate("compiling"));
					ShowInfo(MainClass.Languages.Translate("compiling") ,list.Count + " Files");
				}

				foreach (string f in list) {

					if (exitCompile) { // chyba koncim
						MainClass.MainWindow.ProgressEnd();
						SetError(MainClass.Languages.Translate("compiling_failed"));

						return false;
					}
					if(stopProcess){
						MainClass.MainWindow.ProgressEnd();
						StopProces();
						return false;
					}

					string fileUpdate = FileUtility.AbsoluteToRelativePath(MainClass.Workspace.RootDirectory ,f);

					if ( project.FilesProperty != null){

						FileItem fi =  project.FilesProperty.Find(x => x.SystemFilePath == fileUpdate);
						if (fi != null)
						{
							if (fi.IsExcluded) continue;
						}
					}

					
					string fdir = System.IO.Path.GetDirectoryName(f);
					string fname = System.IO.Path.GetFileName(f);

					string args = String.Format("/d \"{0}\" /c {1} /o console", fdir, fname);

					if(MainClass.Platform.IsMac){

						args = String.Format("-d {0} -c {1} -o console", fdir, fname);

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

					string a = args;
					//output.Add(new TaskMessage("args>>" + a));

					ProcessService ps = new ProcessService();

					MainClass.MainWindow.ProgressStep();
					ProcessWrapper pw = ps.StartProcess(cmd, a, fdir, ProcessOutputChange, ProcessOutputChange);
					pw.WaitForExit();
					//pw.WaitForOutput();

					pw.Exited += delegate(object sender, EventArgs e) {
						//Console.WriteLine("pw.Exited");
						ParseOutput(MainClass.Languages.Translate("exit_compiling"),pw.StartInfo.WorkingDirectory);
					};

				}

			} catch (Exception ex) {
				MainClass.MainWindow.ProgressEnd();
				//progressDialog.Destroy();
				SetError(MainClass.Languages.Translate("compiling_failed"),ex.Message);

				return false;
			} finally {
			}

			if(stateTask != StateEnum.OK){

				MainClass.MainWindow.ProgressEnd();
				//progressDialog.Destroy();

				SetError(MainClass.Languages.Translate("compiling_failed"));

				return false;
			}


			//#################### regenerate app file, backup, hash
			if(stopProcess){
				StopProces();
				return false;
			}
			parentTask = new TaskMessage("OK",MainClass.Languages.Translate("compiling"),null);
			//ShowInfo(" ",StateEnum.OK.ToString());
			output.Add(parentTask);		

			List<string> filesList = new List<string>();
			GetAllFiles(ref filesList,project.AbsolutProjectDir );

			//progressDialog.Reset(filesList.Count,MainClass.Languages.Translate("generate_app"));

			string bakAppPath =project.AbsolutAppFilePath+".bak";
			string hashAppPath =project.AbsolutAppFilePath+".hash";

			if(System.IO.File.Exists(bakAppPath)){
				try{
					File.Delete(bakAppPath);
				} catch {
					//progressDialog.Destroy();
					SetError(MainClass.Languages.Translate("cannot_create_backup"));

					return false;
				}
			}
			if(System.IO.File.Exists(hashAppPath)){
				try{
					File.Delete(hashAppPath);
				} catch {
					//progressDialog.Destroy();
					SetError(MainClass.Languages.Translate("cannot_create_hash"));
					return false;
				}
			}

			try{
				File.Copy(project.AbsolutAppFilePath,bakAppPath);
				File.Copy(project.AbsolutAppFilePath,hashAppPath);
			} catch {
				//progressDialog.Destroy();
				SetError(MainClass.Languages.Translate("cannot_create_backup"));
				return false;
			}

			using (StreamWriter stream = File.AppendText(hashAppPath)) {
				stream.WriteLine();

				foreach(string file in filesList){

					if (System.IO.Path.GetExtension(file)==".ms") continue;

					string fileUpdate = FileUtility.AbsoluteToRelativePath(project.AbsolutProjectDir,file);
					fileUpdate = FileUtility.TrimStartingDotCharacter(fileUpdate);
					fileUpdate = FileUtility.TrimStartingDirectorySeparator(fileUpdate);
/*
					stream.WriteLine("file : {0}",fileUpdate);


					string fileNameHash = Cryptographer.SHA1HashBase64(fileUpdate);
					stream.WriteLine("hash : {0}",fileNameHash);
					*/
					//progressDialog.Update(MainClass.Languages.Translate("create_app"));
					using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read)) {
						int size = (int)fs.Length;
						byte[] data = new byte[size];
						fs.Read(data, 0, size);
						fs.Close();
						stream.WriteLine("file: {0}",fileUpdate);
						stream.WriteLine("hash: {0}", Cryptographer.SHA1HashBase64(data));
					}
				}
				
				stream.Flush();
				stream.Close();
				stream.Dispose();
			}
			ShowInfo(MainClass.Languages.Translate("create_app"),StateEnum.OK.ToString());
			//ShowInfo(" ",StateEnum.OK.ToString());
			//#################### podpisovanie

			//ShowInfo(MainClass.Languages.Translate("sign_app_f1") ," ");

			SignApp sa = new SignApp();
			string newAppdata = "";

			if(stopProcess){
				RestoreBackup(hashAppPath,bakAppPath);
				StopProces();
				return false;
			}

			try{
				if(!sa.PostFile(hashAppPath,MainClass.User.Token,out newAppdata)){
					RestoreBackup(hashAppPath,bakAppPath);

					output.Add(new TaskMessage(newAppdata,MainClass.Languages.Translate("expired_licence"),null));
					ShowError(MainClass.Languages.Translate("expired_licence")," ");

					stateTask = StateEnum.ERROR;
					Gtk.Application.Invoke(delegate{
						LicenceExpiredDialog md = new LicenceExpiredDialog(MainClass.Languages.Translate("expired_licence"));
						md.ShowDialog();
					});

					return false;
				}

				if(String.IsNullOrEmpty(newAppdata)){
					RestoreBackup(hashAppPath,bakAppPath);
					SetError(MainClass.Languages.Translate("sign_app_failed"));
					return false;
				}

				using (StreamWriter file = new StreamWriter(project.AbsolutAppFilePath)) {
					file.Write(newAppdata);
					file.Flush();
					file.Close();
				}

			}catch(Exception ex){

				SetError(MainClass.Languages.Translate("sign_app_failed"), ex.Message);
				RestoreBackup(hashAppPath,bakAppPath);
				return false;
			}

			//timer.Stop();
			parentTask = new TaskMessage("OK",MainClass.Languages.Translate("sign"),null);
			output.Add(parentTask);
			ShowInfo(MainClass.Languages.Translate("sign_app_f1"),StateEnum.OK.ToString());
			if(stopProcess){
				RestoreBackup(hashAppPath,bakAppPath);
				StopProces();
				return false;
			}

			//#################### publish
			double step2 = 1 / (listCombinePublish.Count * 1.0);
			MainClass.MainWindow.ProgressStart(step2, MainClass.Languages.Translate("publish"));

			foreach(CombinePublish ccc in  listCombinePublish){//listCC ){
				//if (!ccc.IsSelected) continue;
				//Console.WriteLine(ccc.ToString());

				if(stopProcess){
					break;
				}

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

				parentTask = new TaskMessage(MainClass.Languages.Translate("publishing"),fileName,null);
				devicePublishError = false;

				/*if (progressDialog != null)
					 progressDialog.SetLabel (fileName );*/
				ShowInfo( "Publishing",fileName);

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
					SetError(MainClass.Languages.Translate("platform_not_found", MainClass.Settings.Platform.Name), parentTask);
					continue;
				}

				Device dvc = project.DevicesSettings.Find(x=>x.TargetPlatformId == crPlatform.RuleId);

				if (dvc == null) {
					SetError(MainClass.Languages.Translate("device_not_found", crPlatform.ConditionName, crPlatform.RuleName), parentTask);
					continue;
				}

				//if ((crPlatform.RuleId == (int)DeviceType.Android_1_6) && (!isJavaInstaled)){
				if (((crPlatform.RuleId == (int)DeviceType.Android_1_6) ||
				  (crPlatform.RuleId == (int)DeviceType.Android_2_2))
				    && (!isJavaInstaled)){
					SetError(MainClass.Languages.Translate("java_missing"), parentTask);
					continue;
				}

				string dirPublish = MainClass.Tools.GetPublishDirectory(dvc.Platform.Specific);//System.IO.Path.Combine(MainClass.Settings.PublishDirectory,dvc.Platform.Specific);//crPlatform.RuleName);

				if (!Directory.Exists(dirPublish)){
					SetError(MainClass.Languages.Translate("publish_tool_not_found"), parentTask);
					continue;
				}

				if (String.IsNullOrEmpty(dirPublish)) {

					SetError(MainClass.Languages.Translate("publish_tool_not_found_f1"), parentTask);
					continue;
				}

				dvc.Application = project.AppFile.Name;

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
				// a vytiahnem z nich rezolution a spojim do do string odeleneho &
				if(dvc.Includes.Skin != null){
					dvc.Includes.Skin.ResolutionJson = dvc.Includes.Skin.Resolution;
					if(!String.IsNullOrEmpty(dvc.Includes.Skin.Name)){
						if(project.IncludeAllResolution){
							resolution = "";
							foreach(CombinePublish cp in  listCombinePublish){
								CombineCondition crPlatform2 = cp.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Platform.Id && x.RuleId== crPlatform.RuleId);
								if(crPlatform2 != null){
									CombineCondition crResolution2 = cp.combineRule.Find(x=>x.ConditionId==MainClass.Settings.Resolution.Id );
									if(crResolution2!= null){
										resolution =resolution+crResolution2.RuleName+"&";
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
					//if (System.IO.Path.GetExtension(file)==".msc") continue;
					if (System.IO.Path.GetExtension(file)==".ms") continue;

					string checkFile =file;

					if (System.IO.Path.GetExtension(file)==".msc")
						checkFile = System.IO.Path.ChangeExtension(file,".ms");

					string fileUpdate = FileUtility.AbsoluteToRelativePath(MainClass.Workspace.RootDirectory ,checkFile);

					if ( project.FilesProperty != null){

						// vyhldam property suborov a pozriem ci nemam nejake conditiony nastavene
						FileItem fi =  project.FilesProperty.Find(x => x.SystemFilePath == fileUpdate);
						if (fi != null)
						{
							if (fi.IsExcluded) continue;
							if(fi.ConditionValues == null)continue;
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

					nav:;
				}

				dvc.Includes.Files = filesForPublish.ToArray();
				dvc.Output_Name = fileName;

				dvc.PublishPropertisFull = new List<PublishProperty>();
				foreach(PublishProperty pp in dvc.PublishPropertisMask){
					PublishProperty ppFull = new PublishProperty(pp.PublishName);
					ppFull.PublishValue = project.ConvertProjectMaskPathToFull(pp.PublishValue);

					dvc.PublishPropertisFull.Add(ppFull);
				}

				/*if(dvc.Includes.Skin != null){
					dvc.Includes.Skin.ResolutionJson = dvc.Includes.Skin.Resolution;
					if(!String.IsNullOrEmpty(dvc.Includes.Skin.Name)){
						if(project.IncludeAllResolution){
							dvc.Includes.Skin.ResolutionJson = "*";
						}
					}
				}*/

				//dvc.Includes.Files
				dvc.LogDebug = MainClass.Settings.LogPublish;
				dvc.ApplicationType = project.ApplicationType;
				dvc.FacebookAppID = project.FacebookAppID;
				if(String.IsNullOrEmpty(project.FacebookAppID))
					dvc.FacebookAppID ="";

				string path = System.IO.Path.Combine(dirPublish,"settings.mso");//fileName+".mso"); //dvc.TargetPlatform + "_settings.mso");
				string json = dvc.GenerateJson();//GenerateJson(dvc);
				
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
					SetError(MainClass.Languages.Translate("cannot_generate_mso"), parentTask);
					//isPublishError = true;
					continue;
				}

				string appFile =dvc.Platform.Specific+ ".app"; /*dvc.TargetPlatform*///platformRule.Specific + ".app";
				string fullAppPath = System.IO.Path.Combine(MainClass.Settings.PublishDirectory,appFile);

				if (!System.IO.File.Exists(fullAppPath) ){
					SetError(MainClass.Languages.Translate("publish_tool_not_found_f2"), parentTask);
					continue;
				}


				RunPublishTool(appFile,parentTask);

				if(MainClass.Platform.IsMac){
					ExitPublish(null,null);
				}

				if(devicePublishError){
					allPublishError = true;
					parentTask.Message =MainClass.Languages.Translate("publish_error");
					//Console.WriteLine(parentTask.Child.Message);
					output.Add(parentTask);
					stateTask = StateEnum.ERROR;
					ShowError(StateEnum.ERROR.ToString()," ");
				}
				else{
					parentTask.Message = MainClass.Languages.Translate("publish_successfully_done");
					output.Add(parentTask);
					ShowInfo(" ",StateEnum.OK.ToString());
				}

				MainClass.MainWindow.ProgressStep();
			}


			MainClass.MainWindow.ProgressEnd();
			 
			RestoreBackup(hashAppPath,bakAppPath);

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
				ShowInfo(MainClass.Languages.Translate("Canceled")," ",-1);
				return false;
			}
			else {
				this.stateTask = StateEnum.OK;
				ShowInfo(MainClass.Languages.Translate("publish_successfully_done"), "",1);

				return true;
			}
		}

		private void GetAllFiles(ref List<string> filesList,string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);
			DirectoryInfo diOutput = new DirectoryInfo(MainClass.Workspace.ActualProject.OutputMaskToFullPath);

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
				//if (f.Extension == ".ms") continue;
				int indx = -1;
				indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForPublish);
				if(indx <0) 
					filesList.Add( f.FullName );
			}
		}

		private string RestoreBackup(string hashAppPath, string bakAppPath){
			try{
				File.Delete(project.AbsolutAppFilePath);
				File.Copy(bakAppPath,project.AbsolutAppFilePath);
				File.Delete(hashAppPath);
				File.Delete(bakAppPath);

			}catch(Exception ex){
				Moscrif.IDE.Tool.Logger.Error("Restore backup App file FAILED.");
				Moscrif.IDE.Tool.Logger.Error(ex.Message);
				Console.WriteLine(ex.Message);
				ShowError(ex.Message,"Restore backup App file FAILED.");
			}
			return "";
		}

		private string LastSeparator(string path){

			if(String.IsNullOrEmpty(path)) return path;

			if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) )
				path = path +System.IO.Path.DirectorySeparatorChar;

			return path;
		}

		public bool RunPublishTool (string appFile,TaskMessage parentTask)
		{

			//if (MainClass.Settings.ClearConsoleBeforRuning)
			//	MainClass.MainWindow.OutputConsole.Clear();

			if(MainClass.MainWindow.RunningEmulator){

				//output.Add( new TaskMessage(MainClass.Languages.Translate("emulator_is_running"),null,null));
				stateTask = StateEnum.ERROR;
				parentTask.Child =new TaskMessage(MainClass.Languages.Translate("emulator_is_running"),null,null);
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
				Tool.Logger.LogDebugInfo(String.Format("command MAC ->{0}",cmd),null);
			}

			if (MainClass.Platform.IsWindows){

				if (!System.IO.File.Exists(cmd)) {
					SetError(MainClass.Languages.Translate("emulator_not_found"));
					return false;
				}
			}

			string args = String.Format("/o console /w nowindow /t nowarm /d {0} /f {1}",MainClass.Settings.PublishDirectory+System.IO.Path.DirectorySeparatorChar, appFile);

			if(MainClass.Platform.IsMac){
				args = String.Format("-o console -w nowindow -t nowarm -d {0} -f {1} ", MainClass.Settings.PublishDirectory+System.IO.Path.DirectorySeparatorChar, appFile);
				}


			try{
				MainClass.MainWindow.RunProcessWait(cmd, args, MainClass.Settings.EmulatorDirectory,ProcessOutputPublishChange,ExitPublish);


			}catch (Exception ex){
				//output.Add(new TaskMessage(ex.Message));
				parentTask.Child =new TaskMessage(ex.Message);
				stateTask = StateEnum.ERROR;
				//isPublishError = true;
				return false;
			}

			return true;
		}

		private void ShowError(string error1, string error2){

			stateTask = StateEnum.ERROR;
			if(WriteStep!=null){
				WriteStep(this,new StepEventArgs(error2,error1,true,0));
			}
			/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok,error1, error2, Gtk.MessageType.Error,ParentWindow);
			md.ShowDialog();*/
		}

		private void ShowInfo(string error1, string error2){
			if(WriteStep!=null){
				WriteStep(this,new StepEventArgs(error1,error2,false,0));
			}
			/*MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok,error1, error2, Gtk.MessageType.Info,ParentWindow);
			md.ShowDialog();*/
		}

		private void ShowInfo(string error1, string error2,int state){
			if(WriteStep!=null){
				WriteStep(this,new StepEventArgs(error1,error2,false,state));
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
			//Console.WriteLine("spat-:StopTask");				
		}

		// not use
		public void OnTaskOutputChanged(object sender, string name, string status, List<TaskMessage> errors){
			if(TaskOutputChanged!= null)
				TaskOutputChanged(sender, name,status, errors);

		}

		// not use
		public void OnLogWrite(object sender, string name, string status, TaskMessage error){
			if(LogWrite!= null)
				LogWrite(sender, name,status, error);

		}

		public event ProcessTaskHandler TaskOutputChanged;
		public event ProcessErrorHandler ErrorWrite;
		public event ProcessErrorHandler LogWrite;
		public event ProcessTaskHandler EndTaskWrite;

		void ExitPublish(object sender, EventArgs e){
			if(lastMessage.Contains(MainClass.Languages.Translate("publishing_succesful"))){
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

					//ProcessErrorWrite(this,this.Name,this.StateTask.ToString(),tm2);
				}
			}
		}

		private string lastMessage = "";
		void ProcessOutputPublishChange(object sender, string message)
		{
			lastMessage = message;
			MainClass.MainWindow.OutputConsole.WriteText(message);
			//return;
			string msg = message.Trim();
			if (String.IsNullOrEmpty(msg))
				return;

			if(message.Contains(MainClass.Languages.Translate("publishing_succesful"))){
				devicePublishError = false;
				//this.stateTask = StateEnum.OK;
				return;
			}

			return;
		}


		public void CheckJava ()
		{
			string cmd =MainClass.Settings.JavaCommand;
			string args = MainClass.Settings.JavaArgument;
			try{
				ProcessWrapper pw = MainClass.ProcessService.StartProcess(cmd,args, "", ProcessOutputJavaChange, ProcessOutputJavaChange);
				pw.WaitForExit();

			}catch {
			}
		}

		void ProcessOutputJavaChange(object sender, string message)
		{
			Console.WriteLine(message);
			if(message.Contains("java version") )
				isJavaInstaled = true;

		}

		void ProcessOutputChange(object sender, string message)
		{
			//Gtk.Application.Invoke(delegate
			//{
			//this.output.Add( new TaskMessage(">> " + message.Trim() + "<<"));

			MainClass.MainWindow.OutputConsole.WriteText(message);

			string msg = message.Trim();
			if (String.IsNullOrEmpty(msg))
				return;

			
			string fileDitr = "";
			if (sender.GetType() == typeof(ProcessWrapper)) {
				ProcessWrapper pw = (ProcessWrapper)sender;
				if (pw.StartInfo != null) {
					fileDitr = pw.StartInfo.WorkingDirectory;
				}
				//(sender as ProcessWrapper).
			}
			
			ParseOutput(message, fileDitr);
			//Console.WriteLine(">> " + message.Trim() + "<<");
			//});
		}

		public StateEnum StateTask
		{
			get { return stateTask; }
		}


		public string Name
		{
			get { return "PUBLISH"; }
		}

		public List<TaskMessage> Output
		{
			get { return output; }
		}

		public ITask ChildrenTask
		{
			get { return null; }
		}
		#endregion

		#region ITaskAsyn implementation
		public event EventHandler<StepEventArgs> WriteStep;
		#endregion


		#region private
		private string messageError;
		
		private void ParseOutput(string message, string fileDir)
		{
			int indx = message.IndexOf("Error:");
			
			//Console.WriteLine("indx ->"+indx);
			//Console.WriteLine("message >>> "+message.Replace("\t","»") + "<<<");
			message = message.Replace("\r\n","\n"); //\r\n\r\n
			message = message.Replace("\n\r","\n");//\n\r\n\r
			
			if (indx > -1) {
				
				if (!String.IsNullOrEmpty(messageError)){
					TaskMessage tm =GetMessage(messageError,fileDir);
					if (tm == null) return;
					
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;
					
					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
				}
				
				messageError = message; //.Remove(0, indx + 6);
				
				int indxError = messageError.IndexOf("Error: ");
				if (indxError>0) messageError = messageError.Remove(0,indx);
				
				//message =message.Replace("\n\r","");// odstranim entery riadkov necham len \n
				//message =message.Replace("\r\n","");
				//message =message.Replace("\n","");
				//message =message.Replace("\r","");
				message= message.Replace("Compilation failed!","" );
				
				//int indxEmptyLine =message.IndexOf("\n");
				//Console.WriteLine("indxEmptyLine 1->"+indxEmptyLine);
				if (message.EndsWith("\n\n"))//\r\n\r\n // koniec erroru
				{
					TaskMessage tm =GetMessage(messageError,fileDir);
					if (tm == null) return;
					
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;
					
					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
					//if (MainClass.Settings.FirstErrorStopCompile)
					exitCompile = true;
					
				}
				
				return;
				//messageError = message.Remove(0,6) ;
				//this.output.Add( new TaskMessage(message));
			}
			
			if (message.StartsWith("\t") && (message.Trim().Length > 1 ) )
			{
				messageError  = messageError +message;
				
				//message =message.Replace("\n\r",""); // odstranim entery riadkov necham len \n
				//message =message.Replace("\r\n","");
				//message =message.Replace("\n","");
				//message =message.Replace("\r","");
				message= message.Replace("Compilation failed!","" );
				
				//int indxEmptyLine =message.IndexOf("\n");
				//Console.WriteLine("indxEmptyLine 2->"+indxEmptyLine);
				if (message.EndsWith("\n\n"))//\r\n\r\n  // koniec erroru
				{
					TaskMessage tm =GetMessage(messageError,fileDir);
					if (tm == null) return;
					
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;
					
					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
					//if (MainClass.Settings.FirstErrorStopCompile)
					exitCompile = true;
					
				}
				return;
			}else if (!String.IsNullOrEmpty(messageError) )//	if (!message.StartsWith("\t"))
			{
				TaskMessage tm =GetMessage(messageError,fileDir);
				if (tm == null) return;
				
				this.output.Add(tm);
				this.stateTask = StateEnum.ERROR;
				messageError = null;
				
				if (ErrorWrite!=null){
					ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
				}
				//if (MainClass.Settings.FirstErrorStopCompile)
				exitCompile = true;
			}
		}
		
		private TaskMessage GetMessage(string message, string fileDir){
			TaskMessage tm =new TaskMessage();
			try{
				
				messageError= messageError.Replace("Error:","" );
				
				
				messageError =messageError.Replace("\t","»");
				
				messageError =messageError.Replace("\n\r","");
				messageError =messageError.Replace("\r\n","");
				messageError =messageError.Replace("\n","");
				messageError =messageError.Replace("\r","");
				messageError= messageError.Replace("Compilation failed!","" );
				
				
				//Console.WriteLine("messageError -> "+ messageError);
				
				string[] msg = messageError.Split('»');
				
				//Console.WriteLine("match.Count -> "+ msg.Length);
				
				if (msg.Length <3){
					//Tool.Logger.Error("message ->"+messageError+"<");
					return null;
				}
				
				string error = msg[0];
				string filename = msg[1];
				string line =msg[2];
				
				if (msg[1].StartsWith("at") ){
					if (msg.Length <4){
						//Tool.Logger.Error("message ->"+messageError+"<");
						return null;
					}
					
					filename = msg[2];
					line =msg[3];
				}
				else{
					filename = msg[1];
					line =msg[2];
				}
				filename = filename.Replace('/',System.IO.Path.DirectorySeparatorChar);
				
				
				if (!String.IsNullOrEmpty(fileDir) ){
					filename=System.IO.Path.Combine(fileDir,filename);
					if(filename.EndsWith (".ms")){
						string tmp = System.IO.Path.ChangeExtension(filename, ".msc");
						if(File.Exists(tmp)){
							try{
								File.Delete(tmp);
							} catch{}
						}
					}
					
				}
				
				//Console.WriteLine("error -> "+ error);
				//Console.WriteLine("filename -> "+ filename);
				//Console.WriteLine("line -> "+ line);
				
				tm =new TaskMessage(error, filename, line);
				
			} catch {
				
			}
			return tm;
		}

		private void GetComands(string dir, ref List<string> list, bool ignoreFiles)
		{
			if (!Directory.Exists(dir))
				return;
			
			DirectoryInfo di = new DirectoryInfo(dir);
			
			foreach (DirectoryInfo d in di.GetDirectories()){
				
				int indx = -1;
				if(ignoreFiles){
					indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForPublish);
				}
				
				if(indx<0){
					//if (!d.Name.StartsWith(".")) {
					GetComands(d.FullName, ref list,ignoreFiles);
				}
			}
			
			foreach (FileInfo f in di.GetFiles("*.ms")){
				string fileCompile = System.IO.Path.ChangeExtension(f.FullName,".msc");
				if( File.Exists(fileCompile)){
					
					FileInfo fiCompile = new FileInfo(fileCompile);
					// len tie ms ktorych datum upravy je vetsi, ako datum upravy msc subory
					//if(f.LastWriteTime > fiCompile.LastWriteTime){
					int indx = -1;
					indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForPublish);
					if(indx <0) 
						list.Add(f.FullName);
					//}
					
				} else {
					int indx = -1;
					indx = MainClass.Settings.IgnoresFiles.FindIndex(x => x.Folder == f.Name && x.IsForPublish);
					if(indx <0) 
						list.Add(f.FullName);
				}
			}
		}


		#endregion
	}
	
}


