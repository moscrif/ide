using System;
using System.Collections.Generic;

namespace Moscrif.IDE.Task
{
	public interface ITask
	{
		void Initialize (object dataObject);

		string Name { get; }
		bool ExecuteTask ();

		//bool ExecuteTask (ProcessTaskHandler TaskWrite);

		StateEnum StateTask { get; }
		List<TaskMessage> Output  { get; }
		ITask ChildrenTask { get; }

		event ProcessTaskHandler TaskOutputChanged;
		event ProcessErrorHandler ErrorWrite;
		event ProcessErrorHandler LogWrite;
		event ProcessTaskHandler EndTaskWrite;

		void OnEndTaskWrite(object sender, string name, string status, List<TaskMessage> errors);
	}

	public enum StateEnum{
		OK,
		ERROR,
		DONE

	}

	public delegate void ProcessTaskHandler(object sender, string name, string status, List<TaskMessage> errors);
	public delegate void ProcessErrorHandler(object sender, string name, string status, TaskMessage error);
}



