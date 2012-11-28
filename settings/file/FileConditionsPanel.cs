using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Iface.Entities;

namespace  Moscrif.IDE.Option
{
	internal class FileConditionsPanel : OptionsPanel
	{
		FileConditionsWidget widget;
		FilePropertisData fcd ;

		public override Widget CreatePanelWidget()
		{
			return widget = new FileConditionsWidget(fcd);
		}

		public override void ShowPanel()
		{
		}

		public override void ApplyChanges()
		{
			widget.Store();
		}

		public override bool ValidateChanges()
		{
			return true;
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
			if (dataObject.GetType() == typeof(FilePropertisData))
				fcd = (FilePropertisData)dataObject;
		}

		public override string Label
		{
			get { return MainClass.Languages.Translate("file_condition"); }
		}

		public override string Icon
		{
			get { return "file-conditions.png"; }
		}
	}

	public partial class FileConditionsWidget : Gtk.Bin
	{
		FilePropertisData fcd ;
		Project project;
		FileItem fi_old;
		string filepath;
		List<ConditionRule> conditionRules;
		List<ConditionRule> conditionRules_Old;

		public FileConditionsWidget(FilePropertisData fcd)
		{
			this.Build();
			this.fcd = fcd;
			project = fcd.Project;
			filepath = fcd.Filename;

			fi_old = project.FilesProperty.Find(x => x.SystemFilePath == filepath);

			conditionRules = new List<ConditionRule>();
			conditionRules_Old = new List<ConditionRule>();

			if (fi_old != null)
			if (fi_old.ConditionValues != null)
				conditionRules_Old = fi_old.ConditionValues;

			int rowCount = project.ConditoinsDefine.Count;
			//Table tableSystem = new Table((uint)(2),(uint)2,false);
			Table table = new Table((uint)(rowCount + 3),(uint)2,false);

			GenerateContent(ref table, MainClass.Settings.Platform.Name, 1, MainClass.Settings.Platform,false);//tableSystem
			GenerateContent(ref table, MainClass.Settings.Resolution.Name, 2,MainClass.Settings.Resolution,true); //project.Resolution);//tableSystem
			int i = 3;//1;

			foreach (Condition cd in project.ConditoinsDefine) {

				GenerateContent(ref table, cd.Name, i, cd,false);

				i++;
			}
			vbox2.PackEnd(table, true, true, 0);
			this.ShowAll();
		}

		private void GenerateContent(ref Gtk.Table tableSystem, string label, int xPos, Condition cd,bool isResolution)
		{

			ListStore lstorePlatform = new ListStore(typeof(int),typeof(int),typeof(string));

			int selectRule = 0;
			if (conditionRules_Old.Count > 0) {
				ConditionRule cr = conditionRules_Old.Find(x => x.ConditionId == cd.Id);
				if (cr != null)
					selectRule = cr.RuleId;
			}

			Label lblPlatform = new Label(label);
			lblPlatform.Name = "lbl_" + label;
			lblPlatform.Xalign = 1;
			lblPlatform.Yalign = 0.5F;

			ComboBox cboxPlatform = new ComboBox();
			cboxPlatform.Name = "cd_" + label;

			CellRendererText textRenderer = new CellRendererText();
			cboxPlatform.PackStart(textRenderer, true);
			cboxPlatform.AddAttribute(textRenderer, "text", 2);

			cboxPlatform.WidthRequest = 200;
			cboxPlatform.Model = lstorePlatform;
			cboxPlatform.Changed += delegate(object sender, EventArgs e) {

					if (sender == null)
						return;

					ComboBox combo = sender as ComboBox;

					TreeIter iter;
					if (combo.GetActiveIter(out iter)) {
						int ruleId = (int)combo.Model.GetValue(iter, 0);
						int condId = (int)combo.Model.GetValue(iter, 1);
							if (ruleId != 0){
							ConditionRule cr = conditionRules.Find(x => x.ConditionId == condId);//cd.Id);
							if (cr != null)
								cr.RuleId = ruleId;
							else
								conditionRules.Add(new ConditionRule(condId,ruleId));
						}
						else {
							ConditionRule cr = conditionRules.Find(x => x.ConditionId == condId);//cd.Id);
							if (cr != null){
								conditionRules.Remove(cr);
							}
						}
					}

				};

			tableSystem.Attach(lblPlatform, 0, 1, (uint)(xPos - 1), (uint)xPos, AttachOptions.Shrink, AttachOptions.Shrink, 2, 2);
			tableSystem.Attach(cboxPlatform, 1, 2, (uint)(xPos - 1), (uint)xPos, AttachOptions.Shrink, AttachOptions.Shrink, 2, 2);

			TreeIter selectIter = lstorePlatform.AppendValues(0, cd.Id, "Unset");

			foreach (Rule rl in cd.Rules){

				if(!isResolution){

					if (rl.Id == selectRule)
						selectIter = lstorePlatform.AppendValues(rl.Id, cd.Id, rl.Name);
					else
						lstorePlatform.AppendValues(rl.Id, cd.Id, rl.Name);
				} else {
					string name  = String.Format("{0} ({1}x{2})",rl.Name,rl.Width,rl.Height);
					if (rl.Id == selectRule)
						selectIter = lstorePlatform.AppendValues(rl.Id, cd.Id, name);
					else
						lstorePlatform.AppendValues(rl.Id, cd.Id, name);
				}
			}

			cboxPlatform.SetActiveIter(selectIter);
		}

		public void Store()
		{
			FileItem fi = project.FilesProperty.Find(x => x.SystemFilePath == filepath);
			if (fi != null)
				fi.ConditionValues = conditionRules;
		}
	}
}

