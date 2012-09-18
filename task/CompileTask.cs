using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Execution;

namespace Moscrif.IDE.Task
{
	public class CompileTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask;
		Project project;

		//List<string> listFiles = new List<string>();
		public CompileTask()
		{
		}

		#region ITask implementation
		public void Initialize(object dataObject)
		{
			if (dataObject.GetType() == typeof(Project)) {
				project = (Project)dataObject;
				
			}
		}

		bool exitCompile = false;

		public bool ExecuteTask()
	{
	    stateTask = StateEnum.OK;
			
	    if (MainClass.MainWindow.RunningEmulator) {
		output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_is_running")));
		Tool.Logger.Error(MainClass.Languages.Translate("emulator_is_running"),null);
		stateTask = StateEnum.ERROR;
		return false;
	    }

	    string cmd = Path.Combine(MainClass.Settings.EmulatorDirectory,"moscrif.exe");
	    if (MainClass.Platform.IsMac) {

		string file = System.IO.Path.Combine(MainClass.Settings.EmulatorDirectory,  "Moscrif.app");//.app

			if(!System.IO.Directory.Exists(file) ){
				output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
				stateTask = StateEnum.ERROR;
				Tool.Logger.Error(MainClass.Languages.Translate("emulator_not_found"),null);
				return false;
			}

			file = System.IO.Path.Combine(file,  "Contents");
			file = System.IO.Path.Combine(file,  "MacOS");
			file = System.IO.Path.Combine(file,  "Moscrif");
			cmd = file;

			Tool.Logger.LogDebugInfo(String.Format("command MAC ->{0}",cmd),null);
			if(!System.IO.File.Exists(file) ){
				output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
				stateTask = StateEnum.ERROR;
				Tool.Logger.Error(MainClass.Languages.Translate("emulator_not_found"),null);
				return false;
			}

		} else {
			Tool.Logger.LogDebugInfo(String.Format("command WIN ->{0}",cmd),null);
			if (!System.IO.File.Exists(cmd)) {
				output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
				stateTask = StateEnum.ERROR;
				return false;
			}
		}


			if (MainClass.Workspace.ActualProject == null) {
				output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
				stateTask = StateEnum.ERROR;
				return false;
			}
			project = MainClass.Workspace.ActualProject;
			try {
				List<string> list = new List<string>();
				GetComands(project.AbsolutProjectDir, ref list);
				
				if (list.Count > 0) {
					double step = 1 / (list.Count * 1.0);
					MainClass.MainWindow.ProgressStart(step, MainClass.Languages.Translate("compiling"));
				}
				
				foreach (string f in list) {
					
					if (exitCompile) {
						MainClass.MainWindow.ProgressEnd();
						return false;
					}
					
					string fdir = System.IO.Path.GetDirectoryName(f);
					string fname = System.IO.Path.GetFileName(f);

					string args = String.Format("/d \"{0}\" /c {1} /o console", fdir, fname);

					if (MainClass.Platform.IsMac){		
						args = String.Format("-d \"{0}\" -c {1} -o console", fdir, fname);
					} 

					string a = args;
					ProcessService ps = new ProcessService();
					ProcessWrapper pw = ps.StartProcess(cmd, a, fdir, ProcessOutputChange, ProcessOutputChange);

					MainClass.MainWindow.ProgressStep();

					//pw.WaitForOutput();
					pw.Exited += delegate(object sender, EventArgs e) {
						//Console.WriteLine("pw.Exited");
						ParseOutput("Exit Compilation",pw.StartInfo.WorkingDirectory);
					};
					//return true;
				}
				//output.Add(file);
				
			} catch (Exception ex) {
				output.Add(new TaskMessage(">>" + ex.Message));
				stateTask = StateEnum.ERROR;
				return false;
			} finally {
				MainClass.MainWindow.ProgressEnd();
			}
			//Console.WriteLine("0 >>"+stateTask);
			return true;
		}

		public void OnEndTaskWrite(object sender, string name, string status, List<TaskMessage> errors){
			if(EndTaskWrite!= null)
				EndTaskWrite(sender, name,status, errors);

		}		

		// not uses
		public void OnTaskOutputChanged(object sender, string name, string status, List<TaskMessage> errors){
			if(TaskOutputChanged!= null)
				TaskOutputChanged(sender, name,status, errors);

		}
		// not uses
		public void OnLogWrite(object sender, string name, string status, TaskMessage error){
			if(LogWrite!= null)
				LogWrite(sender, name,status, error);

		}

		public event ProcessTaskHandler TaskOutputChanged;
		public event ProcessErrorHandler ErrorWrite;
		public event ProcessErrorHandler LogWrite;
		public event ProcessTaskHandler EndTaskWrite;

		/*	public void GetComands(string dir, ref List<string> list)
		{
			if (!Directory.Exists(dir))
				return;
			
			DirectoryInfo di = new DirectoryInfo(dir);
			
			foreach (DirectoryInfo d in di.GetDirectories())
				if (!d.Name.StartsWith(".")) {
					GetComands(d.FullName,ref list);
				}
			
			foreach (FileInfo f in di.GetFiles("*.ms")) {
				string fdir = System.IO.Path.GetDirectoryName(f.FullName) ;//+ System.IO.Path.DirectorySeparatorChar;
				string fname = System.IO.Path.GetFileName(f.FullName);
				string args = String.Format("/c {1} /o console /d \"{0}\"",fdir, fname);

				list.Add(args);
			}
		}*/
		public void GetComands(string dir, ref List<string> list)
		{
			if (!Directory.Exists(dir))
				return;
			
			DirectoryInfo di = new DirectoryInfo(dir);
			
			foreach (DirectoryInfo d in di.GetDirectories()){

				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForPublish);

				if(indx<0){
				//if (!d.Name.StartsWith(".")) {
					GetComands(d.FullName, ref list);
				}
			}

			foreach (FileInfo f in di.GetFiles("*.ms"))
				//string fdir = System.IO.Path.GetDirectoryName(f.FullName) ;//+ System.IO.Path.DirectorySeparatorChar;
				//string fname = System.IO.Path.GetFileName(f.FullName);
				//string args = String.Format("/c {1} /o console /d \"{0}\"",fdir, fname);
				
				list.Add(f.FullName);
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


		private string messageError;

		private void ParseOutput(string message, string fileDir)
		{
			int indx = message.IndexOf("Error:");

			message = message.Replace("\r\n","\n"); //\r\n\r\n
			message = message.Replace("\n\r","\n");//\n\r\n\r
			//Console.WriteLine("indx ->"+indx);
			//Console.WriteLine("message >>> "+message.Replace("\t","»") + "<<<");

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
					if (MainClass.Settings.FirstErrorStopCompile)
						exitCompile = true;

				}

				return;
				//messageError = message.Remove(0,6) ;
				//this.output.Add( new TaskMessage(message));
			}

			if (message.StartsWith("\t") && (message.Trim().Length > 1 ) )
			{
				messageError  = messageError +message;

				/*message =message.Replace("\n\r","@");
				message =message.Replace("\r\n","#");
				message =message.Replace("\n","$");
				message =message.Replace("\r","");
				 */

				//message =message.Replace("\n\r",""); // odstranim entery riadkov necham len \n
				//message =message.Replace("\r\n","");
				//message =message.Replace("\n","");
				//message =message.Replace("\r","");
				message= message.Replace("Compilation failed!","" );

				//int indxEmptyLine =message.IndexOf("\n");
				//Console.WriteLine("indxEmptyLine 2->"+indxEmptyLine);
				if (message.EndsWith("\n\n"))//\r\n\r\n// koniec erroru
				{
					TaskMessage tm =GetMessage(messageError,fileDir);
					if (tm == null) return;

					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;

					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
					if (MainClass.Settings.FirstErrorStopCompile)
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
				if (MainClass.Settings.FirstErrorStopCompile)
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


				if (!String.IsNullOrEmpty(fileDir) )
					filename=System.IO.Path.Combine(fileDir,filename);

				//Console.WriteLine("error -> "+ error);
				//Console.WriteLine("filename -> "+ filename);
				//Console.WriteLine("line -> "+ line);

				tm =new TaskMessage(error, filename, line);

			} catch {

			}
			return tm;
		}


		public StateEnum StateTask
		{
			get { return stateTask; }
		}


		public string Name
		{
			get { return "COMPILE"; }
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
	}
	
}


