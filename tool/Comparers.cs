using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Moscrif.IDE.Tool
{


	public class CompareDirByDate :IComparer{
		int IComparer.Compare(object a, object b)
		{
			DirectoryInfo fia =(DirectoryInfo)a ;//new DirectoryInfo((string)a);
			DirectoryInfo fib = (DirectoryInfo)b;//new DirectoryInfo((string)b);

			DateTime cta = fia.CreationTime;
			DateTime ctb = fib.CreationTime;
			return DateTime.Compare(cta, ctb);

		}
	}
}

