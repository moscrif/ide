using System;
using Mono.TextEditor;
using Moscrif.IDE.Components;

namespace Moscrif.IDE.Editors.SourceEditorActions
{
	internal class BreakpointActions
	{
		private readonly TextEditor editor;

		public BreakpointActions(TextEditor editor)
		{
			this.editor = editor;
		}

		//public void AddBreakpoints (LineSegment ls){
			//TextMarker bm = new BreakpointTextMarker(editor, false);
			//editor.Document.AddMarker(ls, bm);
			//editor.QueueDraw();
		//}
		public void AddBreakpoints(object obj, EventArgs args)
		{
			LineSegment line = editor.Document.GetLine(editor.Caret.Line);
			if (line != null) {
				TextMarker bm = new BreakpointTextMarker(editor, false);
				editor.Document.AddMarker(line, bm);
				editor.QueueDraw();
			}
		}

	}
}

