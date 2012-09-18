using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Execution;
using Moscrif.IDE.Tool;
using System.Text.RegularExpressions;

namespace Moscrif.IDE.Task
{
	public class TodoTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask = StateEnum.OK;
		Project project;

		public TodoTask()
		{
		}

		#region ITask implementation
		public void Initialize (object dataObject)
		{
			if (dataObject.GetType() == typeof(Project) ){
				project = (Project)dataObject;
			}
		}

		public bool ExecuteTask ()
		{
			string regEx = @"//\s*?{0}\s*:.*|/\*\n?\s*?\n?\s*?{0}\s*:.*\n?\s*?\*/|";
			string[] expres = new string[]{"TODO","FIX","FIXME","HACK","HACKME","WARN","WARNING"};

			string regExpresion ="";

			foreach (string str in expres){
				regExpresion = regExpresion+String.Format(regEx,str);
			}
			regExpresion=regExpresion.Remove(regExpresion.Length-1,1);

			//Console.WriteLine("regExpresion - > {0}",regExpresion);

			if(project == null) return false;

			output.Clear();

			List<string> files = new List<string>();

			project.GetAllFiles(ref files,project.AbsolutProjectDir,".ms");
			//if(MainClass.MainWindow.EditorNotebook == null) return false;


			List<string> openFiles = MainClass.MainWindow.EditorNotebook.OpenFiles;
			foreach (string file in openFiles){
				if(System.IO.Path.GetExtension(file)==".ms"){
					int indx = files.FindIndex(x=> x== file);
					if(indx <0){
						files.Add(file);
					}
				}

			}


			List<TaskMessage> listOfTask = new List<TaskMessage>();

			foreach (string file in files){
				MatchCollection mc  = Tool.FileUtility.FindInFileRegEx(file,regExpresion);
				if (mc == null){ 
					//Console.WriteLine("mc null");
					continue;
				}

				//MatchCollection mc  = Tool.FileUtility.FindInFile(file,@"(//\s*?(TODO)|(FIX)|(FIXME).*)|(/\*\s*?TODO.*\*/)");

				for(int i=0; i<mc.Count;i++){
					//Console.WriteLine("file {0} --> {1}",file,mc[i].Index);

					TaskMessage tm =new TaskMessage(mc[i].Value, file, mc[i].Index.ToString());
					listOfTask.Add(tm);
				}
			}
			if(EndTaskWrite != null){
				EndTaskWrite(this,this.Name,this.StateTask.ToString(),listOfTask);
			}
			//MainClass.MainWindow.OutputConsole.WriteText("message");

			//ProcessWrapper pw =MainClass.ProcessService.StartProcess("cmd.exe","/c dir *.*", MainClass.Tools.AppPath, ProcessOutputChange, ProcessErrorChange);
			//MainClass.MainWindow.RunProcess("cmd.exe", "/c dir *.*", MainClass.Tools.TempDir);
			//Console.WriteLine("teST TASk BEZI");
			//stateTask = StateEnum.ERROR;
			return true;
		}


		void ProcessOutputChange(object sender, string message)
		{
			Console.WriteLine(message);
		}

		void ProcessErrorChange(object sender, string message)
		{
			Console.WriteLine(message);
		}

		public string Name {
			get {
				return "TODO TASK";
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

		public void OnEndTaskWrite(object sender, string name, string status, List<TaskMessage> errors){
			//if(EndTaskWrite!= null)
			//	EndTaskWrite(sender, name,status, errors);

		}		


		// not use
		public void OnTaskOutputChanged(object sender, string name, string status, List<TaskMessage> errors){
			if(TaskOutputChanged!= null)
				TaskOutputChanged(sender, name,status, errors);

		}

		// not use
		public void OnErrorWrite(object sender, string name, string status,TaskMessage error){
			if(ErrorWrite!= null)
				ErrorWrite(sender, name,status, error);

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

		#endregion
	}

}

