using System;
using System.Threading;
namespace Moscrif.IDE.Task
{
	public class TaskServices
	{

		public TaskServices()
		{
			secondTaskThread = new Thread(new ThreadStart(SecondTaskThreadRun));
			//secondTaskThread.Priority = ThreadPriority.Normal;
			secondTaskThread.Name = "Second Task";
			secondTaskThread.IsBackground = true;
			secondTaskThread.Start();
		}

		private bool play = true;

		//private int i = 0;
		//private object lock_stl = new object();


		private Thread secondTaskThread;
		private bool isBussy;
		private bool isPrimary;

		private TaskList secondTaskList;//

		public void SecondTaskThreadRun()
		{
			try {
				while (play){
					if ((!isPrimary) && (!isBussy) && (secondTaskList != null)) {
						isBussy = true;
						lock (secondTaskList) {
							//Console.WriteLine("secondTaskList THREAD RUNING");
							//secondTaskList.ExecuteTask();
							secondTaskList.ExecuteTaskOnlineWrite();
							secondTaskList = null;
						}
						//Thread.Sleep (1000);
						isBussy = false;
					}
					Thread.Sleep (500);
				}
			} catch(ThreadAbortException){
				Thread.ResetAbort ();
			}

		}

		public void RunSecondaryTastList(TaskList taskList, ProcessTaskHandler processTaskHandler,ProcessErrorHandler errorTaskHandler)
		{
			if (isBussy || isPrimary)
				return;

			foreach (ITask task in taskList.TasksList){
				//task.EndTaskWritte += processTaskHandler;
				task.ErrorWrite+=errorTaskHandler;
				task.EndTaskWrite +=processTaskHandler;
			}


			//secondTaskList = taskList;
			if(secondTaskList== null)
				secondTaskList = new TaskList();

			secondTaskList.TasksList.AddRange(taskList.TasksList);
		}

		public void RunPrimaryTastListOnlineWrite(TaskList taskList, ProcessTaskHandler processTaskHandler,ProcessErrorHandler errorTaskHandler,ProcessErrorHandler logTaskHandler)
		{
			isPrimary = true;
			//taskList.EndTaskWritte += processTaskHandler;
			//taskList.ErrorTaskWritte+=errorTaskHandler;
			//if (logTaskHandler != null)
			//	taskList.LogTaskWritte+= logTaskHandler;

			foreach (ITask task in taskList.TasksList){
				task.ErrorWrite+=errorTaskHandler;
				task.EndTaskWrite +=processTaskHandler;
				if (logTaskHandler != null)
					task.LogWrite+= logTaskHandler;
			}


			taskList.ExecuteTaskOnlineWrite();
			isPrimary = false;
		}

		public void RunPrimaryTastList(TaskList taskList, ProcessTaskHandler processTaskHandler)
		{
			isPrimary = true;
			//taskList.EndTaskWritte += processTaskHandler;
			foreach (ITask task in taskList.TasksList){
				task.EndTaskWrite +=processTaskHandler;
			}


			taskList.ExecuteTask();
			isPrimary = false;
		}

		public void Stop()
		{
			play = false;
			//secondTaskThread.Interrupt();
		}
		
	}
}

