using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Execution;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Editors;
using System.Text.RegularExpressions;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Task
{
	public class FindReplaceTask : ITask
	{
		List<TaskMessage> output = new List<TaskMessage>();
		StateEnum stateTask = StateEnum.OK;
		SearchPattern searchPatern;

		public FindReplaceTask()
		{
		}

		#region ITask implementation
		public void Initialize (object dataObject)
		{
			if (dataObject.GetType() == typeof(SearchPattern) ){
				searchPatern = (SearchPattern)dataObject;
			}
		}

		public bool ExecuteTask ()
		{
			output.Clear();

			List<string> files = new List<string>();

			List<TaskMessage> listOfTask = new List<TaskMessage>();
			int filesCount = 0;
			if(searchPatern.OpenFiles != null )
				filesCount +=searchPatern.OpenFiles.Count;
			if(searchPatern.CloseFiles != null )
				filesCount +=searchPatern.CloseFiles.Count;

			double step = 1 / (filesCount * 1.0);

			MainClass.MainWindow.ProgressStart(step, MainClass.Languages.Translate("searching"));


			foreach (string file in searchPatern.OpenFiles){
				MainClass.MainWindow.ProgressStepInvoke();

				IEditor editor = MainClass.MainWindow.EditorNotebook.FindEditor(file);
				if(editor!= null){
					List<FindResult> table  =  editor.FindReplaceAll(searchPatern);
					if(table == null) continue;

					foreach (FindResult pair in table)
				        {
						TaskMessage tm =new TaskMessage(pair.Value.ToString(), file, pair.Key.ToString());
						listOfTask.Add(tm);
				        }
				}
			}

			foreach (string file in searchPatern.CloseFiles){
				MainClass.MainWindow.ProgressStepInvoke();
				string replaceExpression = searchPatern.ReplaceExpresion == null ? null : searchPatern.ReplaceExpresion.ToString();


				List<FindResult> table  = Tool.FileUtility.FindInFile(file, searchPatern.Expresion.ToString(),searchPatern.CaseSensitive,searchPatern.WholeWorlds,replaceExpression);
				if (table == null) continue;

				foreach (FindResult pair in table)
			        {
					TaskMessage tm =new TaskMessage(pair.Value.ToString(), file, pair.Key.ToString());
					listOfTask.Add(tm);
			        }
			}
			if(EndTaskWrite != null){
				EndTaskWrite(this,this.Name,this.StateTask.ToString(),listOfTask);
			}
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

		void ITask.StopTask()
		{
		}

		#endregion
	}

}

