using System;
using System.Collections.Generic;
using Mono.TextEditor.PopupWindow;

namespace Moscrif.IDE.Completion.Templates
{
	public class CodeTemplateListDataProvider : IListDataProvider<string>
	{
		List<CodeTemplateVariableValue> itemList;
		
		public CodeTemplateListDataProvider (List<CodeTemplateVariableValue> itemList)
		{
			this.itemList = itemList;
		}
		
		public CodeTemplateListDataProvider (string s)
		{
			itemList = new List<CodeTemplateVariableValue> ();
			itemList.Add (new CodeTemplateVariableValue (s, null));
		}

		#region IListDataProvider implementation
		public string GetText (int index)
		{
			return itemList[index].Text;
		}
		
		public string this [int index] {
			get {
				return GetText (index);
			}
		}
		
		public Gdk.Pixbuf GetIcon (int index)
		{
			string iconName = itemList[index].IconName;
			if (string.IsNullOrEmpty (iconName))
				return null;
			return null;  //ImageService.GetPixbuf (iconName, Gtk.IconSize.Menu);
		}
		
		public int Count {
			get {
				return itemList.Count;
			}
		}
		#endregion
		
	}
}

