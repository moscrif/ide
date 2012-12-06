using System;
using System.Collections;
using System.Collections.Generic;
using Moscrif.IDE.Extensions;
using Mono.TextEditor;

namespace Moscrif.IDE.Completion
{
	public class ParameterDataProvider : IParameterDataProvider
	{
		//private Mono.TextEditor.TextEditorData editor;
		//private TextEditor editor;
		//private List<Function> functions = new List<Function> ();

		private List<CompletionData> list =  new List<CompletionData> ();
		public ParameterDataProvider (Mono.TextEditor.TextEditor editor, string functionName)
		{
			//this.editor = editor;
			List<CompletionData> cd = editor.GetCompletionMemberData(functionName);
			if(cd!= null)
				list.AddRange(cd);
		}
		
		// Returns the number of methods
		public int OverloadCount {
			get { return list.Count; }
		}
		
		// Returns the index of the parameter where the cursor is currently positioned.
		// -1 means the cursor is outside the method parameter list
		// 0 means no parameter entered
		// > 0 is the index of the parameter (1-based)
		public int GetCurrentParameterIndex (ICompletionWidget widget, CodeCompletionContext ctx)
		{
			int cursor = widget.CreateCodeCompletionContext().TriggerOffset;
			int i = ctx.TriggerOffset;
			//if (i < 0 || i >= editor.Length || editor.GetCharAt (i) == ')')
			//	return -1;
			
			if (i > cursor)
				return -1;
			else if (i == cursor)
				return 0;
			
			int parameterIndex = 1;
			
			while (i++ < cursor) {
				if (i >= widget.TextLength)
					break;
				char ch = widget.GetChar (i);
				if (ch == ',')
					parameterIndex++;
				else if (ch == ')')
					return -1;
			}
			
			return parameterIndex;
		}
		
		// Returns the markup to use to represent the specified method overload
		// in the parameter information window.
		public string GetMethodMarkup (int overload, string[] parameterMarkup, int currentParameter)
		{
			/*Function function = functions[overload];
			string paramTxt = string.Join (", ", parameterMarkup);
			
			int len = function.FullName.LastIndexOf ("::");
			string prename = null;
			
			if (len > 0)
				prename = GLib.Markup.EscapeText (function.FullName.Substring (0, len + 2));
			
			string cons = string.Empty;
			
			if (function.IsConst)
				cons = " const";
			
			return prename + "<b>" + function.Name + "</b>" + " (" + paramTxt + ")" + cons;
			*/
			if(list.Count>overload)
				return "<b>"+list[overload].Signature+ "</b>" +Environment.NewLine+list[overload].Description;
			return "";
		}
		
		// Returns the text to use to represent the specified parameter
		public string GetParameterMarkup (int overload, int paramIndex)
		{
			/*Function function = functions[overload];

			return GLib.Markup.EscapeText (function.Parameters[paramIndex]);*/
			if(list.Count>overload)
				return list[overload].Description;
			return "";

		}
		
		// Returns the number of parameters of the specified method
		public int GetParameterCount (int overload)
		{
			return 1;
			//return functions[overload].Parameters.Length;
		}
	}

}

