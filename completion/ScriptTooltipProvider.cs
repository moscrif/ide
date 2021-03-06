using System;
using Mono.TextEditor;
using Mono.TextEditor.PopupWindow;
using Moscrif.IDE.Components;
using Moscrif.IDE.Extensions;

namespace Moscrif.IDE.Completion
{
	public class ScriptTooltipProvider: ITooltipProvider
	{
		public ScriptTooltipProvider()
		{
		}

		#region ITooltipProvider implementation 
		
		public TooltipItem GetItem (Mono.TextEditor.TextEditor editor, int offset)
		{
			string offsetWord = (editor as TextEdit).GetOffsetWord(offset,true);

			//if((string.IsNullOrEmpty(offsetWord)) || !offsetWord.Contains(".") ) return null;
			if((string.IsNullOrEmpty(offsetWord)) ) return null;


			// tooltip, len nad slovami s bodkou alebo Type
			if(!offsetWord.Contains(".") ){
				Completion.CompletionData cdType = MainClass.CompletedCache.ListDataTypes.Find(x=>x.CompletionText==offsetWord);
				if(cdType != null ){
					return new TooltipItem (cdType.Description);
				}
				else return null;
			}

			Completion.CompletionData cd = editor.GetCompletionData(offsetWord); //ParseString(writeWord,fullWord,completiontype);

			if(cd!= null){
				return new TooltipItem (cd.Description);
			}
			return null;
		}


		public Gtk.Window CreateTooltipWindow (Mono.TextEditor.TextEditor editor, int offset, Gdk.ModifierType modifierState, TooltipItem item)
		{
			Mono.TextEditor.PopupWindow.TooltipWindow window = new Mono.TextEditor.PopupWindow.TooltipWindow ();
			window.Markup = item.Item.ToString(); // Tooltip;
			return window;
		}
		
		public void GetRequiredPosition (Mono.TextEditor.TextEditor editor, Gtk.Window tipWindow, out int requiredWidth, out double xalign)
		{
			Mono.TextEditor.PopupWindow.TooltipWindow win = (Mono.TextEditor.PopupWindow.TooltipWindow)tipWindow;
			requiredWidth = win.SetMaxWidth (win.Screen.Width);
			xalign = 0.5;
		}
		
		public bool IsInteractive (Mono.TextEditor.TextEditor editor, Gtk.Window tipWindow)
		{
			return false;
		}
		
		#endregion 
		
	}
}

