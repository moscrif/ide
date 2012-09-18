using System;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;

namespace  Moscrif.IDE.Settings
{

	internal class FilteringPanel : OptionsPanel
	{
		FilteringWidget widget;

		public override Widget CreatePanelWidget ()
		{
			return widget = new  FilteringWidget (ParentDialog);
		}

		public override void ShowPanel()
		{
		}

		public override void ApplyChanges ()
		{
			widget.Store ();
		}

		public override bool ValidateChanges ()
		{
			return true;
		}

		public override void Initialize (PreferencesDialog dialog, object dataObject)
		{
			base.Initialize (dialog, dataObject);
		}

		public override string Label {
			get { return MainClass.Languages.Translate("filtering"); }
		}

		public override string Icon {
			get { return "filter.png"; }
		}

	}

	public partial class FilteringWidget : Gtk.Bin
	{

		private List<LogicalSystem> conditions;

		Gtk.ListStore filterStore = new Gtk.ListStore( typeof(string),typeof(LogicalSystem));
		Gtk.ListStore maskStore = new Gtk.ListStore(typeof(string));
		Gtk.Window parentWindow;

		public FilteringWidget(Gtk.Window parent)
		{
			parentWindow = parent;
			this.Build();

			tvFilter.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			tvMask.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);


			tvFilter.Model = filterStore;
			tvMask.Model = maskStore;

			if ( MainClass.Settings == null) return;
			if(MainClass.Settings == null || MainClass.Settings.LogicalSort.Count<1)
				MainClass.Settings.LogicalSort = LogicalSystem.GetDefaultLogicalSystem();

			this.conditions = new List<LogicalSystem>();

			conditions =MainClass.Tools.Clone(MainClass.Settings.LogicalSort);

			TreeIter ti = new TreeIter();

			foreach (LogicalSystem cd in conditions){
					 ti = filterStore.AppendValues(cd.Display,cd);
			}

			tvFilter.Selection.Changed += delegate(object sender, EventArgs e)
			{
				maskStore.Clear();
				LogicalSystem cd =GetSelected();
				if (cd == null ) return;

				if(cd.Mask == null) cd.Mask = new List<string>();

				foreach (string rl in cd.Mask){
					maskStore.AppendValues(rl);
				}
			};
			if (conditions.Count>0)
				tvFilter.Selection.SelectIter(ti);
		}

		private LogicalSystem GetSelected()
		{
			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return null;

			return  (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
		}

		public void Store()
		{
			if(MainClass.Settings!= null)
				MainClass.Settings.LogicalSort.Clear();

			MainClass.Settings.LogicalSort = new List<LogicalSystem>(conditions.ToArray());
		}


		protected virtual void OnBtnAddFilterClicked (object sender, System.EventArgs e)
		{

			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_filter"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					LogicalSystem cdFind = conditions.Find(x=>x.Display.ToUpper() ==newStr.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("filter_is_exist", cdFind.Display), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}


					LogicalSystem cd= new LogicalSystem();
					cd.Display =newStr;
					filterStore.AppendValues(cd.Display,cd);
					conditions.Add(cd);
				}
			}
			ed.Destroy();	}
		
		protected virtual void OnBtnDeleteFilterClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			LogicalSystem cd = (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
			if (cd == null) return;

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_filter", cd.Display), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			conditions.Remove(cd);
			maskStore.Clear();
			filterStore.Remove(ref ti);
		}
		
		protected virtual void OnBtnEditFilterClicked (object sender, System.EventArgs e)
		{

			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			LogicalSystem cd = (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
			if (cd == null) return;

			EntryDialog ed = new EntryDialog(cd.Display,MainClass.Languages.Translate("new_filter"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					if (newStr == cd.Display) return;

					LogicalSystem cdFind = conditions.Find(x=>x.Display.ToUpper() ==newStr.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("filter_is_exist", cdFind.Display), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					LogicalSystem cdEdited = conditions.Find(x => x.Display.ToUpper() == cd.Display.ToUpper());

					if (cdEdited == null){
						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("unspecified_error"), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					cdEdited.Display=newStr;

					filterStore.SetValues(ti,cdEdited.Display,cdEdited);

					//conditions.Find(cd).Name =ed.TextEntry;
				}
			}
			ed.Destroy();	}
		
		protected virtual void OnBtnAddMaskClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			LogicalSystem cd = (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
			if (cd == null) return;

			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_mask"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					//int maxCountRule = 0;
					/*foreach (string rlf in cd.Mask){
						if (maxCountRule < rlf.Id) maxCountRule = rlf.Id;
					}*/

					string rlFind = cd.Mask.Find(x=>x.ToUpper() ==newStr.ToUpper());
					if (rlFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("mask_is_exist", rlFind), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					maskStore.AppendValues(newStr);

					LogicalSystem cd2 = conditions.Find(x => x.Display.ToUpper() == cd.Display.ToUpper());
					cd2.Mask.Add(newStr);
					filterStore.SetValues(ti,cd2.Display,cd2);

				}
			}
			ed.Destroy();
		}
		
		protected virtual void OnBtnDeleteMaskClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			LogicalSystem cd = (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
			if (cd == null) return;

			TreeSelection tsR = tvMask.Selection;
			TreeIter tiR = new TreeIter();
			tsR.GetSelected(out tiR);

			TreePath[] tpR = tsR.GetSelectedRows();
			if (tpR.Length < 1)
				return ;

			string rl = (string)tvMask.Model.GetValue(tiR, 0);

			if (String.IsNullOrEmpty(rl)) return;


			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_mask", rl), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;
			maskStore.Remove(ref tiR);

			LogicalSystem cd2 = conditions.Find(x => x.Display.ToUpper() == cd.Display.ToUpper());
			cd2.Mask.Remove(rl);
			filterStore.SetValues(ti,cd2.Display,cd2);
		}
		
		protected virtual void OnBtnEditMaskClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvFilter.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			LogicalSystem cd = (LogicalSystem)tvFilter.Model.GetValue(ti, 1);
			if (cd == null) return;

			TreeSelection tsR = tvMask.Selection;
			TreeIter tiR = new TreeIter();
			tsR.GetSelected(out tiR);

			TreePath[] tpR = tsR.GetSelectedRows();
			if (tpR.Length < 1)
				return ;

			string rl = (string)tvMask.Model.GetValue(tiR, 0);

			EntryDialog ed = new EntryDialog(rl,MainClass.Languages.Translate("new_name"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					if(rl.ToUpper() == newStr.ToUpper()){
						ed.Destroy();
						return;
					}

					string rlFind = cd.Mask.Find(x=>x.ToUpper() ==newStr.ToUpper());
					if (!String.IsNullOrEmpty(rlFind)){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("mask_is_exist", rlFind), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}


					LogicalSystem cd2 = conditions.Find(x => x.Display.ToUpper() == cd.Display.ToUpper());
					cd2.Mask.Remove(rlFind);

					maskStore.SetValues(tiR,0,newStr,newStr);

					cd2.Mask.Add(newStr);
					filterStore.SetValues(ti,cd2.Display,cd2);

				}
			}
			ed.Destroy();
		}
		
		





	}
}

