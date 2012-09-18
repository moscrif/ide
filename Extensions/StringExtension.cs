using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Moscrif.IDE.Extensions
{
	public static class StringExtension
	{

		public static IEnumerable<string> SplitAndKeep(this string s, string delims)
		    {
		        int start = 0;
		        int index = 0;

		        while ((index = s.IndexOf(delims, start)) != -1)
		        {
		            index=index+delims.Length ;
		            index = Interlocked.Exchange(ref start, index);

			     yield return s.Substring(index, start-index);
		            //yield return s.Substring(index, start-index-delims.Length);
		            //yield return s.Substring(start-delims.Length, delims.Length);
		        }

		        if (start < s.Length)
		        {
		            yield return s.Substring(start);
		        }
		    }


	}
}

