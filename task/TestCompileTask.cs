using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;

namespace Moscrif.IDE.Task
{
	public class TestCompileTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();

		StateEnum stateTask = StateEnum.OK;

		public TestCompileTask()
		{
		}


		#region ITask implementation
		public void Initialize (object dataObject)
		{

		}

		public bool ExecuteTask ()
		{
			if (MainClass.Settings.ClearConsoleBeforRuning)
				MainClass.MainWindow.OutputConsole.Clear();

			if(MainClass.MainWindow.RunningEmulator){
				output.Add( new TaskMessage("Emulator is running."));
				stateTask = StateEnum.ERROR;
				return false;
			}


			if(String.IsNullOrEmpty(MainClass.Workspace.ActualResolution)){
				output.Add( new TaskMessage("Is not no device selected."));
				stateTask = StateEnum.ERROR;
				return false;
			}

			if(MainClass.Workspace.ActualProject == null){
				output.Add( new TaskMessage("Is not no Project selected."));
				stateTask = StateEnum.ERROR;
				return false;
			}

			string newPath =System.IO.Path.Combine(MainClass.Settings.EmulatorDirectory,"generic.ini");
			try{
				if (File.Exists(newPath))
					File.Delete(newPath);

				File.Copy(MainClass.Workspace.ActualResolution,newPath);

			}catch (Exception ex){
				//output.Add("I Can not create generic.ini");
				output.Add( new TaskMessage(ex.Message));
				stateTask = StateEnum.ERROR;
				return false;

			}

			string cmd = Path.Combine(MainClass.Settings.EmulatorDirectory,  "moscrif.exe");

			if(!System.IO.File.Exists(cmd)  ){
				output.Add( new TaskMessage("Emulator not exist."));
				stateTask = StateEnum.ERROR;
				return false;
			}
			MainClass.MainWindow.EditorNotebook.SaveCurentPage();
			string file =  MainClass.MainWindow.EditorNotebook.CurrentFile;

			if (System.IO.File.Exists(file))
			{
			string fileDir = System.IO.Path.GetDirectoryName(file);
			string extension =System.IO.Path.GetExtension(file);
			string fileName =System.IO.Path.GetFileName(file);

				if (extension != ".ms"){
					//Console.WriteLine("Invalid Extension");
					stateTask = StateEnum.ERROR;
					return false;

				}

			//Console.WriteLine("fileDir" +fileDir);
			//AppFile appFile = MainClass.Workspace.ActualProject.AppFile;
			//string projDir = Path.GetDirectoryName(appFile.ApplicationFile);
			//if (!projDir.EndsWith(Path.DirectorySeparatorChar.ToString())) projDir += Path.DirectorySeparatorChar;
			string args = String.Format("/d \"{0}\" /c {1} /o console", fileDir, fileName);

				//Console.WriteLine("cmd ->" +cmd);
				//Console.WriteLine("args ->" +args);

			try{
				MainClass.MainWindow.RunProcess(cmd, args, MainClass.Settings.EmulatorDirectory);
				output.Add( new TaskMessage(file));

			}catch (Exception ex){

				output.Add( new TaskMessage(ex.Message));
				stateTask = StateEnum.ERROR;
				return false;
			}
				return true;
			} else {
				stateTask = StateEnum.ERROR;
				return false;
			}
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
		public void OnErrorWrite(object sender, string name, string status, TaskMessage error){
			if(ErrorWrite!= null)
				ErrorWrite(sender, name,status, error);

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

		void ITask.StopTask()
		{
		}

		public string Name {
			get {
				return "COMPILE TEST Emulator";
			}
		}


		public StateEnum StateTask {
			get {
				return stateTask;
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

