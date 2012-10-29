using System;
using Mono.TextEditor;
using Moscrif.IDE.Completion;
using Moscrif.IDE.Controls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Moscrif.IDE.Iface.Entities;


namespace Moscrif.IDE.Components
{
	public class TextEdit : Mono.TextEditor.TextEditor,ICompletionWidget
	{
		public TextEdit()
		{
			 this.Caret.PositionChanged += delegate {
				FireCompletionContextChanged ();
			};
			this.CanFocus = true;

			this.TooltipProviders.Add(new ScriptTooltipProvider());

			// 
			this.Caret.PositionChanged+= HandleCarethandlePositionChanged;

		}

		//Dictionary<int, HoverMarker> markers = new Dictionary<int, HoverMarker> ();
		List<HoverMarker> markers = new List<HoverMarker> ();

		void HandleCarethandlePositionChanged (object sender, DocumentLocationEventArgs e)
		{
			//Console.WriteLine("HandleCarethandlePositionChanged ->"+DateTime.Now);
			TextEditorData ted = new TextEditorData(this.Document);
			//if (!ted.IsSomethingSelected && markers.Values.Any (m => m.Contains (ted.Caret.Offset)))
			//	return;
			RemoveMarkers (ted.IsSomethingSelected);
			RemoveTimer ();
			if (!ted.IsSomethingSelected)
				popupTimer = GLib.Timeout.Add (1000, DelayedTooltipShow);
		}

		bool DelayedTooltipShow ()
		{
			//Console.WriteLine("DelayedTooltipShow ->"+DateTime.Now);
			try {
				int caretOffset = this.Caret.Offset;
				int start = Math.Min (caretOffset, this.Document.Length - 1);
				while (start > 0) {
					char ch = this.Document.GetCharAt (start);
					if (!char.IsLetterOrDigit (ch) && ch != '_' && ch != '#') { //.
						start++;
						break;
					}
					start--;
				}
				
				int end = Math.Max (caretOffset, 0);
				while (end < this.Document.Length) {
					char ch = this.Document.GetCharAt (end);
					if (!char.IsLetterOrDigit (ch) && ch != '_')
						break;
					end++;
				}
				
				if (start < 0 || start >= end)
					return false;

				string expression = this.Document.GetTextBetween (start, end);


				int i = MainClass.CompletedCache.ListDataKeywords.FindIndex(x=>x.DisplayText ==expression);

				if(i>-1){
					popupTimer = 0;
					return false;
				}

				SearchPattern searchPattern = new SearchPattern();
				searchPattern.CaseSensitive = true;
				searchPattern.WholeWorlds = true;
				searchPattern.Expresion =expression;
				searchPattern.ReplaceExpresion = null;

				List<FindResult> list = FindReplaceAll(searchPattern);

				foreach (var r in list) {
					GetMarker (r);
					//UsageMarker marker = GetMarker (r);//GetMarker (Convert.ToInt32(r.Key));
				}

				/*ResolveResult resolveResult = textEditorResolver.GetLanguageItem (caretOffset, expression);
				if (resolveResult == null)
					return false;
				if (resolveResult is AggregatedResolveResult) {
					foreach (var curResult in ((AggregatedResolveResult)resolveResult).ResolveResults) {
						var references = GetReferences (curResult);
						if (references.Any (r => r.Position <= caretOffset && caretOffset <= r.Position  + r.Name.Length )) {
							ShowReferences (references);
							break;
						}
					}
				} else {
					ShowReferences (GetReferences (resolveResult));
				}*/


			} catch (Exception e) {
				Tool.Logger.Error("Unhandled Exception in HighlightingUsagesExtension",null);
				Tool.Logger.Error(e.Message,null);
			} finally {
				popupTimer = 0;
			}
			return false;
		}

		uint popupTimer = 0;
		void RemoveTimer ()
		{
			if (popupTimer != 0) {
				GLib.Source.Remove (popupTimer);
				popupTimer = 0;
			}
		}

		public bool IsWordSeparator (char ch)
		{
			return !Char.IsLetterOrDigit (ch) && ch != '_';
		}
	
		private bool IsWholeWordAt (string text, int offset, int length)
		{
			return (offset  <= 0 || IsWordSeparator (text[offset - 1]))  &&
				   (offset + length >= text.Length || IsWordSeparator (text[offset + length]));
		}

		private void Replace (ref string text, int offset, int length, string replacement)
		{
			text = text.Remove (offset, length);
			text = text.Insert (offset, replacement);
			//if (this.editor != null) {
				Gtk.Application.Invoke (delegate {
					this.Replace (offset, length, replacement);
				});
				return;
			//}
		}

		public List<FindResult> FindReplaceAll(SearchPattern searchPattern)
		{
			List<FindResult> result = new List<FindResult>();

			string expresion = searchPattern.Expresion.ToString();
			string replaceExpresion = null;
			if(searchPattern.ReplaceExpresion!= null)
				replaceExpresion = searchPattern.ReplaceExpresion.ToString();

			string text = this.Document.Text;
			var comparison = searchPattern.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			int idx = 0;
			int delta = 0;

			while ((idx = text.IndexOf (expresion, idx, text.Length - idx, comparison)) >= 0) {
				if (!searchPattern.WholeWorlds || IsWholeWordAt (text, idx, expresion.Length)) {
					int localDelta = 0;
					if (replaceExpresion != null) {
	
						Replace (ref text,idx + delta, expresion.Length, replaceExpresion);

						delta += replaceExpresion.Length - expresion.Length;
						localDelta = replaceExpresion.Length - expresion.Length;
					}

					int lineNumber =  this.Document.OffsetToLineNumber(idx);
					LineSegment ls = this.Document.GetLine(lineNumber);
					int lengthLine =ls.EndOffset-ls.Offset +localDelta;
					string line = text.Substring(ls.Offset,lengthLine);
					//string line = this.editor.Document.GetTextBetween(ls.Offset,ls.EndOffset);
					result.Add(new FindResult((object)(lineNumber+1),(object)line,(object)idx,(object)(idx+expresion.Length) ));
				}
				idx += expresion.Length;
			}

			Gtk.Application.Invoke (delegate {
				this.QueueDraw();
			});

			return result;
		}

		void RemoveMarkers (bool updateLine)
		{
			if (markers.Count == 0)
				return;

			this.TextViewMargin.AlphaBlendSearchResults = false;
			foreach (var pair in markers) {
				pair.RemoveFromLine(this.Document);
				//this.Document.RemoveMarker (pair.Value, true);
			}
			markers.Clear ();
		}
		
		HoverMarker GetMarker (FindResult fr)//(int line)
		{
			int line = Convert.ToInt32(fr.Key);
			int offset1 = Convert.ToInt32(fr.StartOffset);
			int offset2 = Convert.ToInt32(fr.EndOffset);
			HoverMarker result;
			//if (!markers.TryGetValue (line, out result)) {
			//
				LineSegment ls = this.Document.GetLine(line-1);
				//ls = this.Document.GetLine(line-1);
				//ls = new LineSegment(offset1,offset2);

				result = new HoverMarker(ls,offset1,offset2);
				result.AddToLine(this.Document);
				//result = new HoverMarker ();
				//this.Document.AddMarker (line, result);
				markers.Add (result);//line

				this.Document.CommitLineUpdate (line);
			//}
			return result;
			//return new UsageMarker();
		}

		private List<string> keyvord = new List<string>( new string[]{"var","type","class"});
		private CompletionTyp completionType = CompletionTyp.allType;


		[GLib.ConnectBefore]
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			char charKey = (char)Gdk.Keyval.ToUnicode (evnt.KeyValue);

			bool result = base.OnKeyReleaseEvent (evnt);

			List<FoldSegment> list = GetFolding();
			//this.Document.ClearFoldSegments();
			this.Document.UpdateFoldSegments(list,true);

			return result ;
		}

		public string GetCarretWord(){

			int posCaret = this.Caret.Offset;

			int startWord =FindPrevWordOffset(posCaret);
			int endtWord =FindNextWordOffset(posCaret);

			string  activeWord = this.Document.GetTextBetween(startWord,endtWord).Trim();
			return activeWord;
		}

		public string GetOffsetWord(int offset, bool withDot){

			int startWord;

			if(withDot)
				startWord=FindPrevWordOffsetWithoutDot(offset);
			else
				startWord=FindPrevWordOffset(offset);

			int endtWord =FindNextWordOffset(offset);

			string  activeWord = this.Document.GetTextBetween(startWord,endtWord).Trim();
			return activeWord;
		}


		private bool IsKeyvord(string word){

			int indx = keyvord.FindIndex(x=> x==word);
			if (indx>-1) return true;
			else return false;
		}

		private bool IsIdentifier(string word){

			Regex regex = new Regex("([a-zA-Z_#])([a-zA-Z0-9_#-])", RegexOptions.Compiled);
			return regex.IsMatch(word);
		}

		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			CompletionWindowManager.HideWindow ();
			ParameterInformationWindowManager.HideWindow (this);
			return base.OnFocusOutEvent (evnt); 
		}

		[GLib.ConnectBefore]
		protected override bool OnButtonReleaseEvent(Gdk.EventButton e)
		{
			int offset =this.Document.LocationToOffset(this.Caret.Location);
			int lineNumber= this.Document.OffsetToLineNumber(offset);
			LineSegment lineSegment = this.Document.GetLine(lineNumber);
			
			string lineText =this.Document.GetTextBetween(lineSegment.Offset,offset);
			
			
			int countBrackets1 = lineText.Count(c => c == '(');
			int countBrackets2 = lineText.Count(c => c == ')');
			if(countBrackets1<=countBrackets2){
				ParameterInformationWindowManager.HideWindow(this);
			}
			return base.OnButtonReleaseEvent(e);
		}

		[GLib.ConnectBefore]
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{

			char charKey = (char)Gdk.Keyval.ToUnicode (evnt.KeyValue);
			if ((evnt.State == Gdk.ModifierType.ControlMask) &&(charKey=='v')) {
				PasteClipboard();
				return true;
			};
			if ((evnt.State == Gdk.ModifierType.ControlMask) &&(charKey=='c')) {
				CopyClipboard();
				return true;
			};

			KeyActions ka = KeyActions.None;
			bool isLetterOrDigit = char.IsLetterOrDigit((char) evnt.Key);

			//if (currentCompletionContext != null) {
				if (CompletionWindowManager.PreProcessKeyEvent (evnt.Key,(char)evnt.Key, evnt.State, out ka)) {
					CompletionWindowManager.PostProcessKeyEvent (ka);

					//if ((!isLetterOrDigit)||
					if(evnt.Key == Gdk.Key.Up || evnt.Key == Gdk.Key.Down || evnt.Key == Gdk.Key.Return
					|| evnt.Key == Gdk.Key.Left || evnt.Key == Gdk.Key.Right  ) //)
						return true;
				}
			//}

			if (ParameterInformationWindowManager.IsWindowVisible) {
				if (ParameterInformationWindowManager.ProcessKeyEvent (this, evnt.Key, evnt.State))
					return false;
			}

			if(CompletionWindowManager.IsTemplateModes){
				return true;
			}


			bool result = base.OnKeyPressEvent (evnt);

			if(!MainClass.Settings.SourceEditorSettings.AggressivelyTriggerCL)
				return result;

			completionType = CompletionTyp.allType;

			if ((evnt.State != Gdk.ModifierType.None) && charKey!='(' && charKey!=')') return result;

			int offset =this.Document.LocationToOffset(this.Caret.Location);
			int lineNumber= this.Document.OffsetToLineNumber(offset);
			LineSegment lineSegment = this.Document.GetLine(lineNumber);
			
			string lineText =this.Document.GetTextBetween(lineSegment.Offset,offset);


			int countBrackets1 = lineText.Count(c => c == '(');
			int countBrackets2 = lineText.Count(c => c == ')');
			if(countBrackets1<=countBrackets2){
				ParameterInformationWindowManager.HideWindow(this);
			}

			if( ( (charKey == '\0') || (char.IsPunctuation(charKey)) &&
						( char.IsSymbol(charKey) )
						// ||( char.IsWhiteSpace(charKey) )
				)
						&& ( charKey!= '#')
						&& ( charKey!= '_')
						&& ( charKey!= '.'))
			{

				return result;
			}


			int endOffset = offset;
			offset = FindPrevWordOffset(offset);

			if ( offset > 0 || endOffset > 0 || offset < endOffset)
			{
				int offset2 =FindPrevWordOffsetStartSpace(offset);
				string  previousWord = this.Document.GetTextBetween(offset2,offset).Trim();

				int offset3 = FindPrevWordOffsetWithoutDot(offset);
				string  previousWordDot = this.Document.GetTextBetween(offset3,offset).Trim();

				//Console.WriteLine("previousWord-> {0}",previousWord);
				//Console.WriteLine("previousWord-> {0}",previousWordDot);

				/*int lineNumber= this.Document.OffsetToLineNumber(endOffset);
				LineSegment lineSegment = this.Document.GetLine(lineNumber);

				string lineText =this.Document.GetTextBetween(lineSegment.Offset,endOffset);
*/

				if (!string.IsNullOrEmpty(lineText)){ // som za komentarom
					if (lineText.Contains("//")) return result;
				}

				string docText =this.Document.GetTextBetween(0,endOffset);

				int countCommentStart = CountExpresion("/*",docText);//
				int countCommentEnd = CountExpresion("*/",docText);//
				//int countComment = CountExpresion(""",docText);

				int countSem = docText.Count(c => c == '"');

				if(charKey=='('){
					int offsetB = FindPrevWordOffsetWithoutBrackets(offset);
					string  previousWordB = this.Document.GetTextBetween(offsetB,offset-1).Trim();
					if ((charKey == '('  ) || (previousWordB.Trim().Contains('(')   )){
						ParameterDataProvider pdp = new ParameterDataProvider(this,previousWordB.Trim());
						/*IParameterDataProvider cp = null;
					CodeCompletionContext ctx = CreateCodeCompletionContext (cpos);
					cp = ParameterCompletionCommand (ctx);
*/
						ParameterInformationWindowManager.ShowWindow(this,pdp);
						
						return result;
					}					
					return result;					
				}
				if(charKey==')'){
					ParameterInformationWindowManager.HideWindow(this);
					return result;
				}

				/*if(lineText.Trim().StartsWith("include")){
					completionType = CompletionTyp.includeType;

					Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_insertautocomplete");
					if (act!=null){
						act.Activate();
					}
					return result;
				}*/

				// komentar alebo string "" /**/
				if((countCommentStart>countCommentEnd) ||(!ItEven(countSem)) ) return result;

				if ((charKey == '.'  ) || (previousWordDot.Trim().Contains('.')   ))
					completionType = CompletionTyp.dotType;

				if (previousWord.Trim()=="new")
					completionType = CompletionTyp.newType;



				string word = this.Document.GetTextAt (offset, endOffset - offset).Trim();
				if ((!IsKeyvord(previousWord.Trim()) && IsIdentifier(word)) ||
					(previousWord.Trim()=="new") ||
					( charKey == '.'  ))
				{
					Gtk.Action act = MainClass.MainWindow.EditorNotebook.EditorAction.GetAction("sourceeditor_insertautocomplete");
					if (act!=null){
						act.Activate();
					}
				}
			}

			return result;
		}

		int CountExpresion(string expresion, string text)
		{
		    return (text.Length - text.Replace(expresion,"").Length) / expresion.Length;
		}

		bool ItEven (int number){
			if(number % 2 == 0)
			{
			 	return true;
			}
			else
			{
				return false;
			}
		}

		/*private int FindPrevWordOffset ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;

			while(--offset >= 0 && ( ( (!char.IsPunctuation(this.Document.GetCharAt (offset))) &&
						( !char.IsSymbol(this.Document.GetCharAt (offset)) ) &&
						( !char.IsWhiteSpace(this.Document.GetCharAt (offset)) )
						|| ( this.Document.GetCharAt (offset)== '#')
						|| ( this.Document.GetCharAt (offset)== '_')
						//|| ( this.Document.GetCharAt (offset)== '.')
				) ))
				;

			return ++offset;
		}*/

		private int FindPrevWordOffset ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;

			while(--offset >= 0 && ( ( (!char.IsPunctuation(this.Document.GetCharAt (offset))) &&
						( !char.IsSymbol(this.Document.GetCharAt (offset)) ) &&
						( !char.IsWhiteSpace(this.Document.GetCharAt (offset)) )
						|| ( this.Document.GetCharAt (offset)== '#')
						|| ( this.Document.GetCharAt (offset)== '_')
				) ))
				;

			return ++offset;
		}

		private int FindNextWordOffset ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			if (offset >=this.Document.Length)
				return offset;

			while(++offset < this.Document.Length && ( ( (!char.IsPunctuation(this.Document.GetCharAt (offset))) &&
						( !char.IsSymbol(this.Document.GetCharAt (offset)) ) &&
						( !char.IsWhiteSpace(this.Document.GetCharAt (offset)) )
						|| ( this.Document.GetCharAt (offset)== '#')
						|| ( this.Document.GetCharAt (offset)== '_')
				) ))
				;

			return offset;
		}

		private int FindPrevWordOffsetWithoutDot ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;

			while(--offset >= 0 && ( ( (!char.IsPunctuation(this.Document.GetCharAt (offset))) &&
						( !char.IsSymbol(this.Document.GetCharAt (offset)) ) &&
						( !char.IsWhiteSpace(this.Document.GetCharAt (offset)) )
						|| ( this.Document.GetCharAt (offset)== '#')
						|| ( this.Document.GetCharAt (offset)== '_')
						|| ( this.Document.GetCharAt (offset)== '.')
				) ))
				;

			return ++offset;
		}

		private int FindPrevWordOffsetWithoutBrackets ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;
			
			while(--offset >= 0 && ( ( (!char.IsPunctuation(this.Document.GetCharAt (offset))) &&
			                          ( !char.IsSymbol(this.Document.GetCharAt (offset)) ) &&
			                          ( !char.IsWhiteSpace(this.Document.GetCharAt (offset)) )
			                          || ( this.Document.GetCharAt (offset)== '#')
			                          || ( this.Document.GetCharAt (offset)== '_')
			                          || ( this.Document.GetCharAt (offset)== '(')
			                          ) ))
				;
			
			return ++offset;
		}


		private int FindPrevWordOffsetStartSpace ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;

			while(--offset >= 0 && ( char.IsWhiteSpace(this.Document.GetCharAt (offset)) ))
				;

			return FindPrevWordOffset(++offset);
		}

		private string FindPrevWord ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;
			offset = FindPrevWordOffset(offset);
			if (offset < 0 || endOffset < 0 || offset > endOffset)
				return "";
			
			return this.Document.GetTextAt (offset, endOffset - offset).Trim();
		}

		private string FindPrevWordWithoutDot ( int offset)// MonoDevelop.Ide.Gui.TextEditor editor,
		{
			int endOffset = offset;
			offset = FindPrevWordOffsetWithoutDot(offset);
			if (offset < 0 || endOffset < 0 || offset > endOffset)
				return "";
			
			return this.Document.GetTextAt (offset, endOffset - offset).Trim();
		}

		private void ParseFoldingRegion(MatchCollection mc, ref List<FoldSegment> list, int start){

			int needStart =1;
			int findEnd =0;

			if(mc == null || mc.Count<1) return;

			if(start >= mc.Count) return;

			Match startMatch = mc[start];
			if(startMatch.Value.Contains("@endregion")){
				start++;
				ParseFoldingRegion(mc, ref list, start);
				return;
			}
			for(int i=start+1; i<mc.Count;i++){
				if(!mc[i].Value.Contains("@endregion")){
					needStart++;
				} else {
					findEnd++;

					if(needStart == findEnd){

						int startIndex =startMatch.Index;
						int endIndex =mc[i].Index- startIndex + mc[i].Value.Length;

						Regex regex = new Regex(@"//\s*?@region", RegexOptions.Compiled);
						string text = regex.Replace(startMatch.Value, "");

						text = text.Trim();
						if(String.IsNullOrEmpty(text)) text ="....";

						FoldSegment fs = new FoldSegment(text,startIndex,endIndex,FoldingType.Region);
						list.Add(fs);
						break;
					}
				}

			}
			start++;
			ParseFoldingRegion(mc, ref list, start);
		}

		private void ParseFolding(MatchCollection mc, ref List<FoldSegment> list, int start, string startExpresion,string endExpresion){

			int needStart =1;
			int findEnd =0;

			if(mc == null || mc.Count<1) return;

			if(start >= mc.Count) return;

			Match startMatch = mc[start];

			if(startMatch.Value.Contains(endExpresion)){//"}"
				start++;
				ParseFolding(mc, ref list, start,startExpresion,endExpresion);
				return;
			}

			for(int i=start+1; i<mc.Count;i++){
				if(mc[i].Value.Contains(startExpresion)){//"}"
					needStart++;
				} else if (mc[i].Value.Contains(endExpresion)){
					findEnd++;

					if(needStart == findEnd){

						int startIndex =startMatch.Index;
						int endIndex =mc[i].Index- startIndex + mc[i].Value.Length;

						FoldSegment fs = new FoldSegment("....",startIndex,endIndex,FoldingType.Region);
						list.Add(fs);
						break;
					}
				}

			}
			start++;
			ParseFolding(mc, ref list, start,startExpresion,endExpresion);
		}


		public  List<FoldSegment> GetFolding(){

			List<FoldSegment> result = new List<FoldSegment> ();
		        Stack<FoldSegment> foldSegments = new Stack<FoldSegment> ();

			string allRegex =@"//\s*?@region.*|//\s*?@endregion\s*?";
			Regex regexAll = new Regex(allRegex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.CultureInvariant);


			MatchCollection mcAll = regexAll.Matches(this.Document.Text);

			ParseFoldingRegion(mcAll,ref result, 0);

			allRegex =@"{|}";
			Regex regexBar = new Regex(allRegex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.CultureInvariant);

			mcAll = regexBar.Matches(this.Document.Text);

			ParseFolding(mcAll,ref result, 0,"{","}");

			allRegex =@"/\*|\*/";
			Regex regexComentary = new Regex(allRegex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.CultureInvariant);

			mcAll = regexComentary.Matches(this.Document.Text);

			ParseFolding(mcAll,ref result, 0,"/*","*/");

			return result;

		}



		public void ShowWidget(){
			this.GrabFocus();
		}

		public void CopyClipboard(){

			if(String.IsNullOrEmpty(this.SelectedText)) return;

			Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
			clipboard.Text=this.SelectedText;
		}

		public void PasteClipboard(){
			Gtk.Clipboard clipboard = this.GetClipboard(Gdk.Selection.Clipboard);
			clipboard.RequestText (new Gtk.ClipboardTextReceivedFunc (PasteReceived));
		}

		void PasteReceived (Gtk.Clipboard clipboard, string text)
		{
			ISegment sg = this.SelectionRange;

			if((sg != null) && sg.Length>0){

				this.Replace(sg.Offset, sg.Length,text);
				int newPositionCaret = sg.Offset + text.Length;

				DocumentLocation dl =this.Document.OffsetToLocation(newPositionCaret);

				Caret caret = this.Caret;
				caret.Location = dl;

				//editor.ScrollToCaret();

			}else{
				this.InsertAtCaret(text);
			}
		}

	#region ICompletionWidget implementation
		public event EventHandler CompletionContextChanged;

		void FireCompletionContextChanged ()
		{
			if (CompletionContextChanged != null)
				CompletionContextChanged (this, EventArgs.Empty);
		}

		string ICompletionWidget.GetText (int startOffset, int endOffset)
		{
			if (startOffset < 0 || endOffset < 0 || startOffset > endOffset)
				return "";
			return this.Document.GetTextAt (startOffset, endOffset - startOffset);
		}

		char ICompletionWidget.GetChar (int offset)
		{
			return this.Document.GetCharAt (offset);
		}

		void ICompletionWidget.Replace (int offset, int count, string text)
		{
			this.Replace (offset, count, text);
			if (this.Caret.Offset >= offset) {
				this.Caret.Offset -= count;
				this.Caret.Offset += text.Length;
			}
		}

		int ICompletionWidget.GetCompletionTyp ()
		{
			return	(int)completionType;
		}

		string ICompletionWidget.GetCompletionText (CodeCompletionContext ctx, bool full)
		{
			/*if (ctx == null)
				return null;
			int min = System.Math.Min (ctx.TriggerOffset, this.Caret.Offset);
			int max = System.Math.Max (ctx.TriggerOffset, this.Caret.Offset);
			return this.Document.GetTextBetween (min, max);
			 */
			string str="";
			if(full)
				str = FindPrevWordWithoutDot(ctx.TriggerOffset);
			else
				str = FindPrevWord(ctx.TriggerOffset);

			//string str =  FindPrevWord(ctx.TriggerOffset);//this.editor.Caret.Offset
			return str;
		}
		void ICompletionWidget.SetCompletionText (CodeCompletionContext ctx, string partial_word, string complete_word)
		{
			TextEditorData data = this.GetTextEditorData ();
			if (data == null || data.Document == null)
				return;
			int triggerOffset = ctx.TriggerOffset;
			int length = String.IsNullOrEmpty (partial_word) ? 0 : partial_word.Length;

			bool blockMode = false;
			if (data.IsSomethingSelected) {
				blockMode = data.MainSelection.SelectionMode == Mono.TextEditor.SelectionMode.Block;
				if (blockMode) {
					data.Caret.PreserveSelection = true;
					triggerOffset = data.Caret.Offset - length;
				} else {
					if (data.SelectionRange.Offset < ctx.TriggerOffset)
						triggerOffset = ctx.TriggerOffset - data.SelectionRange.Length;
						data.DeleteSelectedText ();
				}
				length = 0;
			}
			// | in the completion text now marks the caret position
			int idx = complete_word.IndexOf ('|');

			if (idx >= 0) {
				complete_word = complete_word.Remove (idx, 1);
			} else {
				idx = complete_word.Length;
			}

			triggerOffset += data.EnsureCaretIsNotVirtual ();

			data.Document.EndAtomicUndo ();
			if (blockMode) {
				data.Document.BeginAtomicUndo ();

				int minLine = data.MainSelection.MinLine;
				int maxLine = data.MainSelection.MaxLine;
				int column = triggerOffset - data.Document.GetLineByOffset (triggerOffset).Offset;
				for (int lineNumber = minLine; lineNumber <= maxLine; lineNumber++) {
					LineSegment lineSegment = data.Document.GetLine (lineNumber);
					if (lineSegment == null)
						continue;
					int offset = lineSegment.Offset + column;
					data.Replace (offset, length, complete_word);
				}
				data.Caret.Offset = triggerOffset + idx;
				int minColumn = System.Math.Min (data.MainSelection.Anchor.Column, data.MainSelection.Lead.Column);
				data.MainSelection.Anchor = new DocumentLocation (data.Caret.Line == minLine ? maxLine : minLine, minColumn);
				data.MainSelection.Lead = new DocumentLocation (data.Caret.Line, this.Caret.Column);
				
				data.Document.CommitMultipleLineUpdate (data.MainSelection.MinLine, data.MainSelection.MaxLine);
				data.Caret.PreserveSelection = false;
			} else {

				//string word = GetWordBeforeCaret (editor).Trim ();
				//if (word.Length > 0)
				//	offset = DeleteWordBeforeCaret (editor);

				//int triggerOffset2 =FindPrevWordOffset(triggerOffset);
				int triggerOffset2 =FindPrevWordOffset(this.Caret.Offset);
				string word = FindPrevWord(this.Caret.Offset);
				length =word.Length;

				/*if (triggerOffset2 != triggerOffset){
					this.Remove (triggerOffset2, triggerOffset - triggerOffset2);
					this.Caret.Offset = triggerOffset2;
				}*/

				data.Replace (triggerOffset2, length, complete_word);
				data.Caret.Offset = triggerOffset2 + idx;

				//data.Replace (triggerOffset, length, complete_word);
				//data.Caret.Offset = triggerOffset + idx;
				data.Document.BeginAtomicUndo ();
			}
			
			data.Document.CommitLineUpdate (data.Caret.Line);
		}

	/*	CodeCompletionContext ICompletionWidget.CurrentCodeCompletionContext {
			get {
				return ICompletionWidget.CreateCodeCompletionContext ();
			}
		}*/

		CodeCompletionContext ICompletionWidget.CreateCodeCompletionContext() {

			int triggerOffset = this.Caret.Offset;
			CodeCompletionContext result = new CodeCompletionContext ();
			result.TriggerOffset = triggerOffset;
			DocumentLocation loc = this.Document.OffsetToLocation (triggerOffset);

			result.TriggerLine   = loc.Line + 1;

			result.TriggerLineOffset = loc.Column + 1;

			Gdk.Point p = this.DocumentToVisualLocation (loc);

			int tx = 0; int ty = 0;

			this.ParentWindow.GetOrigin (out tx, out ty);
			tx += this.Allocation.X;
			ty += this.Allocation.Y;

			result.TriggerXCoord = tx + p.X + this.TextViewMargin.XOffset - (int)this.HAdjustment.Value;
			result.TriggerYCoord = ty + p.Y - (int)this.VAdjustment.Value + this.LineHeight;
			result.TriggerTextHeight = this.LineHeight;

			return result;
		}


		int ICompletionWidget.TextLength {
			get {
				return this.Document.Length;
			}
		}

		int ICompletionWidget.SelectedLength {
			get {
				if (this.IsSomethingSelected) {
					if (this.MainSelection.SelectionMode == Mono.TextEditor.SelectionMode.Block)
						return System.Math.Abs (this.MainSelection.Anchor.Column - this.MainSelection.Lead.Column);
					return this.SelectionRange.Length;
				}
				return 0;
			}
		}

		Gtk.Style ICompletionWidget.GtkStyle {
			get {
				return this.Style.Copy ();
			}
		}
	#endregion
}
}

