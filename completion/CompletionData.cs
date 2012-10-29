// 
// CompletionData.cs
// 
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
//using MonoDevelop.Core;

namespace Moscrif.IDE.Completion
{
	public class CompletionData : ICloneable
	{

		protected CompletionData () {}
		
		public virtual string Icon { get; set; }
		public virtual string DisplayText { get; set; }
		public virtual string Description { get; set; }
		public virtual string CompletionText { get; set; }
		public virtual string DisplayDescription { get; set; }
	//	public virtual CompletionCategory CompletionCategory { get; set; }
		public virtual DisplayFlags DisplayFlags { get; set; }

		public virtual string Signature { get; set; }

		public virtual string Parent { get; set; }
		public virtual string ReturnType { get; set; }
		public virtual CompletionDataTyp ComTypes { get; set; }

		private bool isOverloaded = false;
		public virtual bool IsOverloaded { 
			get {
				return isOverloaded;
				//return false;
			}
			set {
				isOverloaded = value;
			}
		}

		private  List<CompletionData> overloadedData = new List<CompletionData>();//IEnumerable
		public virtual List<CompletionData> OverloadedData {
			get {
				return overloadedData;
				//throw new System.InvalidOperationException ();
			}
		}
		public CompletionData (string text) : this (text, null, null) {}
		public CompletionData (string text, string icon) : this (text, icon, null) {}
		public CompletionData (string text, string icon, string description)  : this (text, icon, description,0,null,null) {} //this (text, icon, description, text) {}

		public CompletionData (string text, string icon, string description,int cct, string parent,string returnType) : this (text, icon, description, text,cct,parent,returnType) {}

		public CompletionData (string displayText, string icon, string description, string completionText)
		{
			this.DisplayText = displayText;
			this.Icon = icon;
			this.Description = description;
			this.CompletionText = completionText;
			this.ComTypes = (CompletionDataTyp)0;
			this.Parent = "";
		}

		public CompletionData (string displayText, string icon, string description, string completionText,int cct, string parent,string returnType)
		{
			this.DisplayText = displayText;
			this.Icon = icon;
			this.Description = description;
			this.CompletionText = completionText;
			this.ComTypes = (CompletionDataTyp)cct;
			this.Parent = parent;
			this.ReturnType = returnType;
		}
		
		public  string GetCurrentWord (CompletionListWindow window)//static
		{
			//return window.PartialWord;

			int partialWordLength = window.PartialWord != null ? window.PartialWord.Length : 0;
			int replaceLength = window.CodeCompletionContext.TriggerWordLength + partialWordLength - window.InitialWordLength;
			string temp =window.CompletionWidget.GetText (window.CodeCompletionContext.TriggerOffset, window.CodeCompletionContext.TriggerOffset + replaceLength);
			return temp;

			//return window.CompletionWidget.GetText (window.CodeCompletionContext.TriggerOffset, window.CodeCompletionContext.TriggerOffset + replaceLength);
		}


		public virtual void InsertCompletionText (CompletionListWindow window)
		{
			if (CompletionText == GetCurrentWord (window))
				return;
			window.CompletionWidget.SetCompletionText (window.CodeCompletionContext, GetCurrentWord (window), CompletionText);
		}
		
		public override string ToString ()
		{
			return string.Format ("[CompletionData: Parent={0}, DisplayText={1}, Description={2}, CompletionText={3}, DisplayFlags={4},ComTypes={5}]", Parent, DisplayText, Description, CompletionText, DisplayFlags,ComTypes);
		}

		#region ICloneable implementation
		object ICloneable.Clone()
		{
			CompletionData cd = (CompletionData)MemberwiseClone();
			cd.OverloadedData.Clear(); //AddRange(new List<CompletionData>(cd.OverloadedData.ToArray()));
			return cd;

		}

		#endregion
		public CompletionData Clone()
		{
			CompletionData cd = (CompletionData)MemberwiseClone();
			cd.OverloadedData.Clear(); //AddRange(new List<CompletionData>(cd.OverloadedData.ToArray()));
			return cd;

			//return (CompletionData)this.MemberwiseClone();
		}


	}



}
