using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Moscrif.IDE.Completion;
using Moscrif.IDE.Completion.Templates;
using Mono.TextEditor;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Extensions;

namespace Moscrif.IDE.Editors.SourceEditorActions
{
	public class AutoCompleteActions
	{
		private readonly TextEditor editor;
		private readonly ICompletionWidget widget;

		private CodeCompletionContext currentCompletionContext;

		public AutoCompleteActions(ICompletionWidget widget, TextEditor editor)
		{
			this.widget = widget;
			this.editor = editor;
		}

		public void InsertTemplate(object obj, EventArgs args)
		{
			ICompletionDataList completionList = new CompletionDataList();
			
			completionList = ShowCodeTemplatesCommand();
			currentCompletionContext = widget.CreateCodeCompletionContext();//this.editor.Caret.Offset
			CompletionWindowManager.IsTemplateModes = true;
			CompletionWindowManager.ShowWindow((char)0, completionList, widget, currentCompletionContext, OnCompletionWindowClosed);
			editor.GrabFocus();
		}

		public void InsertCompletion(object obj, EventArgs args)
		{
			if(widget.BanCompletion){
				return;
			}

			CompletionWindowManager.IsTemplateModes = false;
			ICompletionDataList completionList = new CompletionDataList();

			currentCompletionContext = widget.CreateCodeCompletionContext();

			string writeWord = widget.GetCompletionText(currentCompletionContext,false);
			string fullWord = widget.GetCompletionText(currentCompletionContext,true);

			CompletionTyp completiontype =  (CompletionTyp)widget.GetCompletionTyp();

			if(completiontype == CompletionTyp.includeType){
				completionList = editor.GetCompletionData(writeWord,fullWord);
			}else
				completionList = editor.GetCompletionData(writeWord,fullWord,completiontype); //ParseString(writeWord,fullWord,completiontype);

			CompletionWindowManager.ShowWindow((char)0, completionList, widget, currentCompletionContext, OnCompletionWindowClosed);
			editor.GrabFocus();
		}

		public virtual ICompletionDataList ShowCodeTemplatesCommand()
		{
			CompletionDataList list = new CompletionDataList();

			list.CompletionSelectionMode = CompletionSelectionMode.OwnTextField;


			foreach (CodeTemplate template in CodeTemplateService.Templates) //GetCodeTemplates("text/moscrif"))
				list.Add(new CodeTemplateCompletionData(template,this.editor));
			return list;
		}

		void OnCompletionWindowClosed()
		{
			currentCompletionContext = null;
		}
		
	}
}

