using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using System.Linq;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Settings
{
	internal class ProjectConditionsPanel : OptionsPanel
	{
		ProjectConditionsWidget widget;
		Project project ;

		public override Widget CreatePanelWidget()
		{
			return widget = new ProjectConditionsWidget(project,ParentDialog);
		}

		public override void ApplyChanges()
		{
			widget.Store();
		}

		public override void ShowPanel()
		{

		}

		public override bool ValidateChanges()
		{
			return true;
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
			if (dataObject.GetType() == typeof(Project))
				project=(Project)dataObject;
		}

		public override string Label {
			get { return MainClass.Languages.Translate("conditions"); }
		}

		public override string Icon {
			get { return "conditions.png"; }
		}
	}

	public partial class ProjectConditionsWidget : Gtk.Bin
	{
		private Project project;
		private List<Condition> conditions;
		//private Condition resolution;
		private bool generatePublishList = false;
		private int maxCond = 0;
		Gtk.Window parentWindow;

		Gtk.ListStore conditionStore = new Gtk.ListStore(typeof(int), typeof(string),typeof(Condition));
		Gtk.ListStore ruleStore = new Gtk.ListStore(typeof(int), typeof(string),typeof(Rule));

		public ProjectConditionsWidget(Project project,Gtk.Window parent)
		{
			parentWindow = parent;
			this.Build();
			this.project = project;

			tvConditions.AppendColumn(MainClass.Languages.Translate("id"), new Gtk.CellRendererText(), "text", 0);
			tvConditions.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 1);

			tvRules.AppendColumn(MainClass.Languages.Translate("id"), new Gtk.CellRendererText(), "text", 0);
			tvRules.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 1);


			/*if(project.SelectResolution != null)
				resolutions = new List<int>( project.SelectResolution);
			else
				resolutions = new List<int>();*/

			tvConditions.Model = conditionStore;
			tvRules.Model = ruleStore;
			//tvResolution.Selection.Mode  = Gtk.SelectionMode.Multiple;

			if ( project.ConditoinsDefine == null) return;

			this.conditions = new List<Condition>();

			//this.resolution = project.Resolution.Clone();
			foreach (Rule rl in MainClass.Settings.Resolution.Rules ){

				//Gtk.TreeIter tir = resolStore.AppendValues(rl.Id,rl.Name,rl.Specific,rl);
				//if (this.project.Resolution != null) {
					int indx = MainClass.Settings.Resolution.Rules.FindIndex(x=> x.Id == rl.Id);

					//if (indx > -1)
					//	tvResolution.Selection.SelectIter(tir);
				//}
			}


			//conditions = new List<Condition>(project.ConditoinsDefine.ToArray());
			conditions =MainClass.Tools.Clone(project.ConditoinsDefine);

			TreeIter ti = new TreeIter();

			foreach (Condition cd in conditions){
					 ti = conditionStore.AppendValues(cd.Id,cd.Name,cd);
					if (maxCond < cd.Id) maxCond = cd.Id;
			}

			tvConditions.Selection.Changed += delegate(object sender, EventArgs e)
			{
				ruleStore.Clear();
				Condition cd =GetSelected();
				if (cd == null ) return;

				foreach (Rule rl in cd.Rules){
					ruleStore.AppendValues(rl.Id,rl.Name,rl);
				}
			};
			if (conditions.Count>0)
				tvConditions.Selection.SelectIter(ti);
			//project.ConditoinsDefine

			/*tvResolution.Selection.Changed+= delegate(object sender, EventArgs e) {

				generatePublishList = true;
			};*/
		}

		private Condition GetSelected()
		{
			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return null;

			return  (Condition)tvConditions.Model.GetValue(ti, 2);
		}

		public void Store()
		{
			if(project.ConditoinsDefine!= null)
				project.ConditoinsDefine.Clear();

			project.ConditoinsDefine = new List<Condition>(conditions.ToArray());

			//project.Resolution.Rules.Clear();

			/*project.SelectResolution = new List<int>();

			project.SelectResolution = new List<int>(resolutions);

			string name = MainClass.Settings.Resolution.Name;
			tvResolution.Selection.SelectedForeach(delegate (Gtk.TreeModel model, Gtk.TreePath path, Gtk.TreeIter iter){

				Rule rule = (Rule)model.GetValue(iter, 3);
				//project.Resolution.Rules.Add(rule);
				project.SelectResolution.Add(rule.Id);
			});

			string name2 = MainClass.Settings.Resolution.Name;*/

			if (generatePublishList)
				project.GeneratePublishCombination();
		}

		protected virtual void OnBtnAddCondClicked (object sender, System.EventArgs e)
		{
			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_conditions"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					Condition cdFind = conditions.Find(x=>x.Name.ToUpper() ==newStr.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("conditions_is_exist", cdFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					Condition cd= new Condition();
					cd.Name =newStr;
					maxCond ++;
					cd.Id = maxCond;
					conditionStore.AppendValues(maxCond,cd.Name,cd);
					conditions.Add(cd);
				}
			}
			ed.Destroy();
		}
		
		protected virtual void OnBtnDeleteCondClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			Condition cd = (Condition)tvConditions.Model.GetValue(ti, 2);
			if (cd == null) return;

			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_conditions", cd.Name), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;

			conditions.Remove(cd);
			ruleStore.Clear();
			conditionStore.Remove(ref ti);

		}
		
		protected virtual void OnBtnEditCondClicked (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			Condition cd = (Condition)tvConditions.Model.GetValue(ti, 2);
			if (cd == null) return;

			EntryDialog ed = new EntryDialog(cd.Name,MainClass.Languages.Translate("new_conditions"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					if (newStr == cd.Name) return;

					Condition cdFind = conditions.Find(x=>x.Name.ToUpper() ==newStr.ToUpper());
					if (cdFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("conditions_is_exist", cdFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					Condition cdEdited = conditions.Find(x => x.Id == cd.Id);

					if (cdEdited == null){
						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("unspecified_error"), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}

					cdEdited.Name=newStr;

					conditionStore.SetValues(ti,cdEdited.Id,cdEdited.Name,cdEdited);

					//conditions.Find(cd).Name =ed.TextEntry;
				}
			}
			ed.Destroy();
		}

		protected virtual void OnBtnAddRulesClicked  (object sender, System.EventArgs e)
		{
			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;

			Condition cd = (Condition)tvConditions.Model.GetValue(ti, 2);
			if (cd == null) return;

			EntryDialog ed = new EntryDialog("",MainClass.Languages.Translate("new_rule"),parentWindow);
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					int maxCountRule = 0;
					foreach (Rule rlf in cd.Rules){
						if (maxCountRule < rlf.Id) maxCountRule = rlf.Id;
					}

					Rule rlFind = cd.Rules.Find(x=>x.Name.ToUpper() ==newStr.ToUpper());
					if (rlFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("rule_is_exist", rlFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}


					Rule rl= new Rule();
					rl.Name =newStr;
					maxCountRule++;
					rl.Id = maxCountRule;
					ruleStore.AppendValues(maxCountRule,rl.Name,rl);

					Condition cd2 = conditions.Find(x => x.Id == cd.Id);
					cd2.Rules.Add(rl);
					conditionStore.SetValues(ti,cd2.Id,cd2.Name,cd2);

				}
			}
			ed.Destroy();
		}


		protected virtual void OnBtnDeleteCond1Clicked(object sender, System.EventArgs e)
		{
			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			Condition cd = (Condition)tvConditions.Model.GetValue(ti, 2);
			if (cd == null) return;

			TreeSelection tsR = tvRules.Selection;
			TreeIter tiR = new TreeIter();
			tsR.GetSelected(out tiR);

			TreePath[] tpR = tsR.GetSelectedRows();
			if (tpR.Length < 1)
				return ;

			Rule rl = (Rule)tvRules.Model.GetValue(tiR, 2);

			if (rl == null) return;


			MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("delete_rule", rl.Name), "", Gtk.MessageType.Question,parentWindow);
			int result = md.ShowDialog();
			if (result != (int)Gtk.ResponseType.Yes)
				return;
			ruleStore.Remove(ref tiR);

			Condition cd2 = conditions.Find(x => x.Id == cd.Id);
			cd2.Rules.Remove(rl);
			conditionStore.SetValues(ti,cd2.Id,cd2.Name,cd2);
		}

		
		protected virtual void OnBtnEditCond1Clicked (object sender, System.EventArgs e)
		{

			TreeSelection ts = tvConditions.Selection;

			TreeIter ti = new TreeIter();
			ts.GetSelected(out ti);

			TreePath[] tp = ts.GetSelectedRows();
			if (tp.Length < 1)
				return ;
			Condition cd = (Condition)tvConditions.Model.GetValue(ti, 2);
			if (cd == null) return;

			TreeSelection tsR = tvRules.Selection;
			TreeIter tiR = new TreeIter();
			tsR.GetSelected(out tiR);

			TreePath[] tpR = tsR.GetSelectedRows();
			if (tpR.Length < 1)
				return ;

			Rule rl = (Rule)tvRules.Model.GetValue(tiR, 2);

			EntryDialog ed = new EntryDialog(rl.Name,MainClass.Languages.Translate("new_name"));
			int result = ed.Run();
			if (result == (int)ResponseType.Ok){
				string newStr = ed.TextEntry;
				if (!String.IsNullOrEmpty(newStr) ){

					Rule rlFind = cd.Rules.Find(x=>x.Name.ToUpper() ==newStr.ToUpper());
					if (rlFind != null){

						MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("rule_is_exist", rlFind.Name), "", Gtk.MessageType.Error,parentWindow);
						md.ShowDialog();
						ed.Destroy();
						return;
					}


					Condition cd2 = conditions.Find(x => x.Id == cd.Id);
					cd2.Rules.Remove(rl);

					rl.Name =newStr;
					ruleStore.SetValues(tiR,rl.Id,rl.Name,rl);

					cd2.Rules.Add(rl);
					conditionStore.SetValues(ti,cd2.Id,cd2.Name,cd2);

					//cd.Rules.Add(rl);
				}
			}
			ed.Destroy();
		}
		
		protected virtual void OnBtnTestClicked (object sender, System.EventArgs e)
		{

			//Console.WriteLine("project.Conditions>>"+project.ConditoinsDefine.Count);

		}
		
		






	}
}

