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
		public StepEventArgs (string message1,string message2){
			this.Message1 = message1;
			this.Message2 = message2;
			this.IsError = false;
		}

		public StepEventArgs (string message1,string message2,bool isError){
			this.Message1 = message1;
			this.Message2 = message2;
			this.IsError = isError;
		}

		public string Message1 { get; set; }
		public string Message2 { get; set; }
		public bool IsError { get; set; }
	}

}



