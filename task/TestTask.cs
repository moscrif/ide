using System;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Execution;

namespace Moscrif.IDE.Task
{
	public class TestTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask = StateEnum.OK;

		public TestTask()
		{
		}

		#region ITask implementation
		public void Initialize (object dataObject)
		{
			//throw new NotImplementedException ();
		}

		public bool ExecuteTask ()
		{
			output.Clear();
			output.Add(new TaskMessage("Test Task"));
			//MainClass.MainWindow.OutputConsole.WriteText("message");

			//ProcessWrapper pw =MainClass.ProcessService.StartProcess("cmd.exe","/c dir *.*", MainClass.Tools.AppPath, ProcessOutputChange, ProcessErrorChange);
			//MainClass.MainWindow.RunProcess("cmd.exe", "/c dir *.*", MainClass.Tools.TempDir);
			//Console.WriteLine("teST TASk BEZI");
			stateTask = StateEnum.ERROR;
			return false;
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
				return "TEST TASK";
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

		#endregion
	}
}

