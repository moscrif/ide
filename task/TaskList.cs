using System;
using System.Collections.Generic;

namespace Moscrif.IDE.Task
{
	public class TaskList
	{
		//public delegate void ProcessTaskHandler(object sender, string name, string status, List<TaskMessage> errors);

		public TaskList()
		{
			this.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
		}
		public List<ITask> TasksList;

		public void ExecuteTaskOnlineWrite()
		{
			foreach (ITask task in TasksList){
				/*if (ErrorTaskWritte != null )
					task.ProcessErrorHandler += ErrorTaskWritte;
				else{
					Console.WriteLine("ErrorTaskWritte is null");
				}
				if (LogTaskWritte != null )
					task.ProcessLogHandler += LogTaskWritte;*/


				if (task.ExecuteTask()) {

					task.OnEndTaskWrite(task, task.Name,task.StateTask.ToString(), task.Output);
					/*if (EndTaskWritte != null){
						EndTaskWritte(task, task.Name,task.StateTask.ToString(), task.Output);
					} else {

						//Console.WriteLine("EndTaskWritte IS null");
					}*/
				} else {

					List<string> errorList = new List<string>();
					task.OnEndTaskWrite(task, task.Name, task.StateTask.ToString(), task.Output);
					/*if (EndTaskWritte != null) {
						//Console.WriteLine(EndTaskWritte.ToString());
						EndTaskWritte(task, task.Name, task.StateTask.ToString(), task.Output);
						//Console.WriteLine("END TASK");
					}else {

						//Console.WriteLine("EndTaskWritte IS null");
					}*/
					return;
				}
			}
		}

		public void ExecuteTask()
		{
			foreach (ITask task in TasksList){

				if (task.ExecuteTask()) {
					task.OnEndTaskWrite(task, task.Name,task.StateTask.ToString(), task.Output);
					/*if (EndTaskWritte != null){
						EndTaskWritte(task, task.Name,task.StateTask.ToString(), task.Output);
					} else {
						//Console.WriteLine("EndTaskWritte IS null");
					}*/
				} else {

					List<string> errorList = new List<string>();
					task.OnEndTaskWrite(task, task.Name, task.StateTask.ToString(), task.Output);
					/*if (EndTaskWritte != null) {
						EndTaskWritte(task, task.Name, task.StateTask.ToString(), task.Output);
					}else {

						//Console.WriteLine("EndTaskWritte IS null");
					}*/
					return;
				}
			}
		}

		//public event ProcessTaskHandler EndTaskWritte;
		//public event ProcessErrorHandler ErrorTaskWritte;
		//public event ProcessErrorHandler LogTaskWritte;
	}
}

