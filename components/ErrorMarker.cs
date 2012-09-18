using System;
using System.Collections.Generic;
using Gtk;
using Mono.TextEditor;
using Mono.TextEditor.Theatrics;

namespace  Moscrif.IDE.Components
{
	class ErrorMarker
	{
		//public Error Info { get; private set; }
		public LineSegment Line { get; private set; }
		
		UnderlineMarker marker;
		
		public ErrorMarker (LineSegment line)//(MonoDevelop.Projects.Dom.Error info, LineSegment line)
		{
			//this.Info = info;
			this.Line = line; // may be null if no line is assigned to the error.
			string underlineColor;
			//if (info.ErrorType == ErrorType.Warning)
			//	underlineColor = Mono.TextEditor.Highlighting.Style.WarningUnderlineString;
			//else
			underlineColor = Mono.TextEditor.Highlighting.Style.ErrorUnderlineString;

			marker = new UnderlineMarker (underlineColor, - 1, line.Length-1);

			//if (Info.Region.Start.Line == info.Region.End.Line)
			//	marker = new UnderlineMarker (underlineColor, Info.Region.Start.Column - 1, info.Region.End.Column - 1);
			//else
			//	marker = new UnderlineMarker (underlineColor, - 1, - 1);
		}

		public void AddToLine (Mono.TextEditor.Document doc)
		{
			if (Line != null) {
				doc.AddMarker (Line, marker);
			}
		}
		
		public void RemoveFromLine (Mono.TextEditor.Document doc)
		{
			doc.RemoveMarker (marker);
		}
	}
}

