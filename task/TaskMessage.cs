using System;
namespace  Moscrif.IDE.Task
{
	public class TaskMessage
	{
		public TaskMessage(string message,string file,string line)
		{
			this.Message= message;
			this.File= file;
			this.Line= line;
		}

		public TaskMessage(string message)
		{
			this.Message= message;
			this.File= "";
			this.Line= "";
		}

		public TaskMessage()
		{
			this.Message= "";
			this.File= "";
			this.Line= "";
		}

		public string Message;
		public string File;
		public string Line;

		public TaskMessage Child;

	}
}

