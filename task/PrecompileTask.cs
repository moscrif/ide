using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Execution;

namespace  Moscrif.IDE.Task
{
	public class PrecompileData{

		public string Text;
		public string Filepath;

		public PrecompileData(string text, string filepath){

			this.Text = text;
			this.Filepath = filepath;
		}
	}

	public class PrecompileTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask ;
		PrecompileData pd;

		public PrecompileTask()
		{
		}

		#region ITask implementation
		public void Initialize (object dataObject)
		{
			if (dataObject.GetType() == typeof(PrecompileData) ){
				pd = (PrecompileData)dataObject;

			}
		}

		public bool ExecuteTask ()
		{
			stateTask = StateEnum.OK;
			string tmpFile = System.IO.Path.Combine(MainClass.Paths.TempPrecompileDir, "temp.ms");

			try {
				using (StreamWriter file = new StreamWriter(tmpFile)) {
					file.Write(pd.Text);
					file.Close();
					file.Dispose();
			}
			} catch {

			}

			if(MainClass.MainWindow.RunningEmulator){
				output.Add( new TaskMessage(MainClass.Languages.Translate("emulator_is_running")));
				stateTask = StateEnum.ERROR;
				return false;
			}

			string cmd = Path.Combine(MainClass.Paths.TempPrecompileDir,  "moscrif.exe");
	    
			if (MainClass.Platform.IsMac) {

				string file = System.IO.Path.Combine(MainClass.Paths.TempPrecompileDir,  "Moscrif.app");//.app
	
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
	
				if(!System.IO.File.Exists(file) ){
					output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
					stateTask = StateEnum.ERROR;
					Tool.Logger.Error(MainClass.Languages.Translate("emulator_not_found"),null);
					return false;
				}
	
			} else {
				if (!System.IO.File.Exists(cmd)) {
					output.Add(new TaskMessage(MainClass.Languages.Translate("emulator_not_found")));
					stateTask = StateEnum.ERROR;
					return false;
				}
			}

			if(!System.IO.File.Exists(tmpFile)  ){
				output.Add( new TaskMessage(MainClass.Languages.Translate("file_not_exist_f1", tmpFile)));
				stateTask = StateEnum.ERROR;
				return false;
			}


			//AppFile appFile = MainClass.Workspace.ActualProject.AppFile;
			//string args = String.Format("/d \"{0}\" /c {1} /o console", MainClass.Tools.TempDir, "temp.ms");

			//string args = String.Format("/c {0} /o console /d \"{1}\" ", "temp.ms",MainClass.Tools.TempDir);
			string args = String.Format(" /d \"{0}\" /c {1} /o console ", MainClass.Paths.TempPrecompileDir,"temp.ms");

			if(MainClass.Platform.IsMac){
				args = String.Format(" -d \"{0}\" -c {1} -o console ", MainClass.Paths.TempPrecompileDir,"temp.ms");
			}

			try{

				//MainClass.MainWindow.RunProcess(cmd, args, MainClass.Settings.EmulatorDirectory);

				//MainClass.MainWindow.RunProcess("cmd.exe", "/c dir *.*", MainClass.Tools.TempDir);
				//ProcessWrapper pw = MainClass.ProcessService.StartProcess("cmd.exe", "/c dir *.*", MainClass.Tools.TempDir,ProcessOutputChange, ProcessErrorChange);

				ProcessService ps = new ProcessService();
				ProcessWrapper pw = ps.StartProcess(cmd,
				 args,
				//"D:\\Work\\moscrift.Ide\\bin\\Debug\\Workspace4\\x000001"
                               MainClass.Paths.TempPrecompileDir
				// MainClass.Settings.EmulatorDirectory
				//                                     ""
				, ProcessOutputChange,ProcessOutputChange);// ProcessErrorChange);
				pw.Exited += delegate(object sender, EventArgs e) {
					//Console.WriteLine("pw.Exited");
					//ParseOutput("Exit Compilation");
				};
				pw.WaitForOutput(1000);
				//pw.WaitForOutput(1000);
				//output.Add(file);

			}catch (Exception ex){

				output.Add( new TaskMessage(ex.Message));
				stateTask = StateEnum.ERROR;
				return false;
			}
			finally{
				//ps.Dispose();
			}
				//Console.WriteLine("0 >>"+stateTask);
				return true;
		}

		void ProcessOutputChange(object sender, string message)
		{
			string msg = message;
			if (String.IsNullOrEmpty(msg))
				return;

			//this.output.Add( new TaskMessage(">> " + message.Trim() + "<<"));
			//MainClass.MainWindow.OutputConsole.WriteText(message);
			//Console.WriteLine(">> " + message.Trim() + "<<");

			ParseOutput(message);

		}

		void ProcessErrorChange(object sender, string message)
		{
			string msg = message;
			if (String.IsNullOrEmpty(msg))
				return;

			//this.output.Add( new TaskMessage(">> " + message.Trim() + "<<"));
			//MainClass.MainWindow.OutputConsole.WriteText(message);
			//Console.WriteLine(">> " + message.Trim() + "<<");

			ParseOutput(message);
		}

		private string messageError;

		private void ParseOutput(string message)
		{
			int indx = message.IndexOf("Error:");

			message = message.Replace("\r\n","\n"); //\r\n\r\n
			message = message.Replace("\n\r","\n");//\n\r\n\r

			//Console.WriteLine("indx ->"+indx);
			//Console.WriteLine("message >>> "+message.Replace("\t","»") + "<<<");

			if (indx > -1) {

				if (!String.IsNullOrEmpty(messageError)){
					TaskMessage tm =GetMessage(messageError);
					if (tm == null) return;
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;

					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
				}


				messageError = message; //.Remove(0, indx + 6);

				//int indxError = messageError.IndexOf("Error: ");
				//if (indxError>0) messageError = messageError.Remove(0,indx);

				if (indx>0) messageError = messageError.Remove(0,indx);

				message= message.Replace("Compilation failed!","" );

				//Console.WriteLine("compled message ->"+message);
				//int indxEmptyLine =message.IndexOf("\n");
				//Console.WriteLine("indxEmptyLine 1->"+indxEmptyLine);
				if (message.EndsWith("\n\n"))//\r\n\r\n // koniec erroru
				{
					TaskMessage tm =GetMessage(messageError);
					if (tm == null) return;
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;

					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}
				}

				return;
				//messageError = message.Remove(0,6) ;
				//this.output.Add( new TaskMessage(message));
			}

			if (message.StartsWith("\t") && (message.Trim().Length > 1 ) )
			{
				messageError  = messageError +message;

				message= message.Replace("Compilation failed!","" );
				//Console.WriteLine("compled message ->"+message);
				//int indxEmptyLine =message.IndexOf("\n");

				//Console.WriteLine("indxEmptyLine ->"+indxEmptyLine);
				//Console.WriteLine("message.EndsWith(\\n) ->"+message.EndsWith("\n"));

				//if (message.EndsWith("\n")) // koniec erroru
				if (message.EndsWith("\n\n"))//\r\n\r\n
				{
					TaskMessage tm =GetMessage(messageError);
					if (tm == null) return;
					this.output.Add(tm);
					this.stateTask = StateEnum.ERROR;
					messageError = null;

					if (ErrorWrite!=null){
						ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
					}

				}
				return;
			}else if (!String.IsNullOrEmpty(messageError))//(!message.StartsWith("\t"))
			{
				TaskMessage tm =GetMessage(messageError);
				if (tm == null) return;

				this.output.Add(tm);
				this.stateTask = StateEnum.ERROR;
				messageError = null;

				if (ErrorWrite!=null){
					ErrorWrite(this,this.Name,this.StateTask.ToString(),tm);
				}
			}
		}

		private TaskMessage GetMessage(string message){

			if(String.IsNullOrEmpty(message)) return null;

			TaskMessage tm =new TaskMessage();

			try{
				message= message.Replace("Compilation failed!","" );

				message =message.Replace("\t","»");

				message =message.Replace("\n\r","");
				message =message.Replace("\r\n","");
				message =message.Replace("\n","");
				message =message.Replace("\r","");

				string[] msg = message.Split('»');

				if (msg.Length <3){
					//Tool.Logger.Error("message ->"+message+"<");
					return null;
				}

				string error = msg[0];
				string line =msg[2];

				if (msg[1].StartsWith("at") ){
					//filename = msg[2];
					if (msg.Length <4){
						//Tool.Logger.Error("message ->"+message+"<");
						return null;
					}
					line =msg[3];
				}
				else{
					//filename = msg[1];
					line =msg[2];
				}
				string filename = pd.Filepath;

				tm =new TaskMessage(error, filename, line);
			} catch (Exception ex){
					//Tool.Logger.Error("message ->"+message+"<");
					Tool.Logger.Error(ex.Message);
					Tool.Logger.Error(ex.StackTrace);
					Tool.Logger.Error(ex.Source);
				//throw ex;
				return null;
			}
			return tm;
		}

		public void OnEndTaskWrite(object sender, string name, string status, List<TaskMessage> errors){
			if(EndTaskWrite!= null){
				EndTaskWrite(sender, name,status, errors);
			}

		}		

		void ITask.StopTask()
		{
		}

		/*
		private void ParseOutput(string message){

			int indx = message.IndexOf("Error: ");

			//if (message.StartsWith("Error:")){
			if (indx>-1){
				messageError = message.Remove(0,indx+6);
				//messageError = message.Remove(0,6) ;
				//this.output.Add( new TaskMessage(message));
			}
			int indxFS = message.IndexOf(Convert.ToChar(28).ToString());
			//if (message.StartsWith(Convert.ToChar(28).ToString())){
			if (indxFS >-1){
				//int indxFS = message.IndexOf(Convert.ToChar(28));
				int indxRS = message.IndexOf(Convert.ToChar(30));
				int indxGS = message.IndexOf(Convert.ToChar(29));
				int indxUS = message.IndexOf(Convert.ToChar(31));

				string filename =message.Substring(indxFS+1,indxRS-1);
				string line =message.Substring(indxRS+1,indxGS-indxRS-1);
				string position =message.Substring(indxGS+1);
				filename = pd.Filepath;

				this.output.Add( new TaskMessage(messageError,filename,line));

				this.stateTask = StateEnum.ERROR;
				messageError = null;
			}
		}
		*/

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

		public StateEnum StateTask {
			get {
				return stateTask;
			}
		}


		public string Name {
			get {
				return "Precompiling";
			}
		}

		public List<TaskMessage> Output {
			get {
				return output;
			}
		}

		public ITask ChildrenTask {
			get {
				return null;
			}
		}
		#endregion
	}

}

