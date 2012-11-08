using System;
using System.Collections.Generic;

namespace Moscrif.IDE.Task
{
	public interface ITaskAsyn:ITask
	{
		event EventHandler<StepEventArgs> WriteStep;
	}

	public class StepEventArgs: EventArgs
	{
		public StepEventArgs (string message1,string message2,int status){
			this.Message1 = message1;
			this.Message2 = message2;
			this.IsError = false;
			this.Status = 0;
		}

		public StepEventArgs (string message1,string message2,bool isError,int status){
			this.Message1 = message1;
			this.Message2 = message2;
			this.Status = status;
		}

		public string Message1 { get; set; }
		public string Message2 { get; set; }
		public bool IsError { get; set; }
		public int Status { get; set; }
	}

}



