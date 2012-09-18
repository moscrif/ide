using System;
using Gdk;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using System.Linq;
using System.Collections.Generic;

namespace Moscrif.IDE.Components
{
	public class UsageMarker: TextMarker
	{
		protected UsageMarker ()
		{}
		
		/*public UsageMarker (string colorName, int start, int end)
		{
			this.ColorName = colorName;
			this.StartCol = start;
			this.EndCol = end;

		}*/
		//public UsageMarker (Gdk.Color color, int start, int end)
		public UsageMarker (int start, int end)
		{
			//this.Color = color;
			this.StartCol = start;
			this.EndCol = end;

		}
		
		//public string ColorName { get; set; }
		//public Gdk.Color Color { get; set; }
		public int StartCol { get; set; }
		public int EndCol { get; set; }



 		public override void Draw (TextEditor editor, Gdk.Drawable win,Pango.Layout layout, bool selected, int startOffset, int endOffset, int y, int startXPos, int endXPos)
            {
                  int markerStart = LineSegment.Offset + System.Math.Max (StartCol, 0);
                  int markerEnd   = LineSegment.Offset + (EndCol < 0? LineSegment.Length : EndCol);
                  if (markerEnd < startOffset || markerStart > endOffset)
                        return;

                  int from;
                  int to;
                  
                  if (markerStart < startOffset && endOffset < markerEnd) {
                        from = startXPos;
                        to   = endXPos;
                  } else {
                        int start = startOffset < markerStart ? markerStart : startOffset;
                        int end   = endOffset < markerEnd ? endOffset : markerEnd;
                        from = startXPos + editor.GetWidth (editor.Document.GetTextAt (startOffset, start - startOffset));
                        to   = startXPos + editor.GetWidth (editor.Document.GetTextAt (startOffset, end - startOffset));
                  }
                  from = System.Math.Max (from, editor.TextViewMargin.XOffset);
                  to   = System.Math.Max (to, editor.TextViewMargin.XOffset);
                  if (from >= to) {
                        return;
                  }
                  
                  using (Gdk.GC gc = new Gdk.GC (win)) {
                       // gc.RgbFgColor = ColorName == null ? Color : editor.ColorStyle.GetColorFromDefinition (ColorName);
                        int drawY    = y + editor.LineHeight - 1;

                        win.DrawLine (gc, from, drawY, to, drawY);
			if(@from<to){
				//gc.RgbFgColor = editor.ColorStyle.BracketHighlightRectangle.BackgroundColor;
				//win.DrawRectangle (gc, true, @from + 1, y + 1, to - @from - 1, editor.LineHeight - 2);
				gc.RgbFgColor = editor.ColorStyle.BracketHighlightRectangle.Color;
				win.DrawRectangle (gc, false, @from, y, to - @from, editor.LineHeight - 1);
			}

                  }



            }



	}
}

