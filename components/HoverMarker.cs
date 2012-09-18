using System;
using Gdk;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using System.Linq;


namespace Moscrif.IDE.Components
{

	class HoverMarker
	{
		//public Error Info { get; private set; }
		public LineSegment Line { get; private set; }
		
		//UnderlineMarker marker;
		UsageMarker marker;
		//StyleTextMarker marker;

		public HoverMarker (LineSegment line,int startOffset, int endOffset)//(MonoDevelop.Projects.Dom.Error info, LineSegment line)
		{
			this.Line = line; // may be null if no line is assigned to the error.

			//string underlineColor; = Mono.TextEditor.Highlighting.Style.WarningUnderlineString;

			marker = new UsageMarker (startOffset,endOffset);
		}

		public void AddToLine (Mono.TextEditor.Document doc)
		{
			if (Line != null) {

				DocumentLocation dl = doc.OffsetToLocation(marker.StartCol);
				marker.StartCol = dl.Column;

				dl = doc.OffsetToLocation(marker.EndCol);
				marker.EndCol = dl.Column;

				doc.AddMarker (Line, marker);
			}
		}
		
		public void RemoveFromLine (Mono.TextEditor.Document doc)
		{
			doc.RemoveMarker (marker);
		}
	}


}

