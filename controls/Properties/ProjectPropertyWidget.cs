using System;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Controls.Properties
{
	public class ProjectPropertyWidget : Gtk.HBox
	{

		private Project project;
		public ProjectPropertyWidget(Project project)
		{
			this.project = project;

			Table mainTable = new Table(2,1,false);
			Table propertyTable = new Table(5,2,false);
			
			Label lblFN = new Label(System.IO.Path.GetFileName(project.ProjectName));
			lblFN.UseUnderline = false;
			lblFN.Selectable = true;
			lblFN.Xalign = 0;

			/*prjDir.LabelProp = this.project.RelativeAppFilePath;
			prjFullPath.LabelProp  = this.project.AbsolutProjectDir;*/

			Entry entr = new Entry(this.project.RelativeAppFilePath);
			entr.ModifyBg(StateType.Normal,this.Style.Background(StateType.Normal));
			entr.IsEditable = false;
			
			Entry entrFullPath = new Entry(this.project.AbsolutProjectDir);
			entrFullPath.IsEditable = false;

			Entry entrFacebookApi = new Entry(this.project.FacebookAppID);
			entrFacebookApi.Changed+= delegate(object sender, EventArgs e)
			{
				this.project.FacebookAppID = entrFacebookApi.Text;
			};

			Entry entrTitle = new Entry(this.project.AppFile.Title);
			entrTitle.Changed+= delegate(object sender, EventArgs e)
			{
				try{
					this.project.AppFile.Title = entrTitle.Text;
				} catch (Exception ex){
					Moscrif.IDE.Tool.Logger.Error(ex.Message);
				}
			};

			ComboBox cbType = new ComboBox();
			ListStore projectModel = new ListStore(typeof(string), typeof(string));
			CellRendererText textRenderer = new CellRendererText();
			cbType.PackStart(textRenderer, true);
			cbType.AddAttribute(textRenderer, "text", 0);
			cbType.Model= projectModel;
			TreeIter ti = new TreeIter();
			foreach(SettingValue ds in MainClass.Settings.ApplicationType){// MainClass.Settings.InstallLocations){
				if(ds.Value == this.project.ApplicationType){
					ti = projectModel.AppendValues(ds.Display,ds.Value);
					cbType.SetActiveIter(ti);
				} else  projectModel.AppendValues(ds.Display,ds.Value);
			}
			if(cbType.Active <0)
				cbType.Active =0;
			cbType.Changed+= delegate(object sender, EventArgs e) {
				TreeIter tiSelect = new TreeIter();
				cbType.GetActiveIter(out tiSelect);
				string text = cbType.Model.GetValue(tiSelect,1).ToString();
				project.ApplicationType =text;
			};

			AddControl(ref propertyTable,0,lblFN,"Project ");
			AddControl(ref propertyTable,1,entr,"Project App ");
			AddControl(ref propertyTable,2,entrFullPath,"Project Path ");
			AddControl(ref propertyTable,3,entrTitle,"Title ");
			AddControl(ref propertyTable,4,cbType,"Type ");
			AddControl(ref propertyTable,5,entrFacebookApi,"Facebook ID ");

			mainTable.Attach(propertyTable,0,1,0,1,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			
			/*int rowCount = project.ConditoinsDefine.Count;
			Table conditionsTable = new Table((uint)(rowCount + 3),(uint)2,false);
			
			GenerateContent(ref conditionsTable, MainClass.Settings.Platform.Name, 1, MainClass.Settings.Platform,false);//tableSystem
			GenerateContent(ref conditionsTable, MainClass.Settings.Resolution.Name, 2,MainClass.Settings.Resolution,true); //project.Resolution);//tableSystem
			int i = 3;
			foreach (Condition cd in project.ConditoinsDefine) {
				GenerateContent(ref conditionsTable, cd.Name, i, cd,false);
				i++;
			}
			mainTable.Attach(conditionsTable,0,1,1,2,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Fill,0,0);
			*/
			this.PackStart(mainTable,true,true,0);
			this.ShowAll();
		}

		private void AddControl(ref Table tbl, int top, Widget ctrl, string label){
			Label lbl = new Label(label);
			lbl.Xalign = 1;
			lbl.Yalign = 0.5F;
			lbl.WidthRequest = 100;
			
			tbl.Attach(lbl,0,1,(uint)top,(uint)(top+1),AttachOptions.Shrink,AttachOptions.Shrink,2,2);
			tbl.Attach(ctrl,1,2,(uint)top,(uint)(top+1),AttachOptions.Fill|AttachOptions.Expand,AttachOptions.Fill,2,2);
		}
	}
}

