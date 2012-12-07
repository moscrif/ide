using System;
using Gtk;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Option
{
	internal class ApplicationPanel : OptionsPanel
	{
		ApplicationWidget widget;
		Project project ;
		 
		public override Widget CreatePanelWidget()
		{
			return widget = new ApplicationWidget(project,ParentDialog);
		} 

		public override void ApplyChanges()
		{
			widget.Store();
		}

		public override void ShowPanel()
		{
			widget.ReloadPanel();
		}

		public override bool ValidateChanges()
		{
			return widget.Validate();
		}

		public override void Initialize(PreferencesDialog dialog, object dataObject)
		{
			base.Initialize(dialog, dataObject);
			if (dataObject.GetType() == typeof(Project))
				project = (Project)dataObject;
		}

		public override string Label
		{
			get { return MainClass.Languages.Translate("application_prferencies_project"); }
		}

		public override string Icon
		{
			get { return "application.png"; }
		}

	}

	public partial class ApplicationWidget : Gtk.Bin
	{
		ApplicationFileControl afc;
		Project project;

		private bool generatePublishList = false;

		private ComboBox cbType;
		Gtk.Window parentWindow;

		public ApplicationWidget(Project project,Gtk.Window parent)
		{
			parentWindow =parent;
			this.Build();
			this.project = project;

			cbType = new ComboBox();

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

			tblGlobal.Attach(cbType, 1, 2, 0,1, AttachOptions.Fill|AttachOptions.Expand, AttachOptions.Fill|AttachOptions.Expand, 0, 0);

			afc = new ApplicationFileControl(project.AppFile,ApplicationFileControl.Mode.EditNoSaveButton,parentWindow);
			vbox2.PackEnd(afc, true, true, 0);

		}


		public void ReloadPanel(){

		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}

		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		/*private void AddMenuItem(string condition)
		{
			Gtk.MenuItem mi = new Gtk.MenuItem(condition);
			mi.Name = condition;
			mi.Activated += delegate(object sender, EventArgs e) {
					if (sender.GetType() == typeof(Gtk.MenuItem)){
						if (!CheckMessage()) return;

						entrName.Text = entrName.Text = String.Format("{0}$({1})", entrName.Text, (sender as Gtk.MenuItem).Name);

					}
				};
			popupCondition.Add(mi);

		}*/

		public bool Validate()
		{
		/*	if (String.IsNullOrEmpty(feOutput.Path) || String.IsNullOrEmpty(entrName.Text)) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"), MainClass.Languages.Translate("please_set_all_Folders"), MessageType.Error,parentWindow);
				ms.ShowDialog();
				return false;
			}
			//Console.WriteLine(feOutput.Path);
			//Console.WriteLine(project.ConvertProjectMaskPathToFull(feOutput.Path));
			//Console.WriteLine(project.AbsolutProjectDir);
			string outputPath =project.ConvertProjectMaskPathToFull(feOutput.Path);

			bool cointaint =  FileUtility.ContainsPath(project.AbsolutProjectDir,outputPath);

			if(cointaint){
				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("error"), MainClass.Languages.Translate("output_overlap_project"), MessageType.Error,parentWindow);
				ms.ShowDialog();
				return false;
			}*/
			return true;
		}

		public void Store()
		{
			TreeIter ti = new TreeIter();
			cbType.GetActiveIter(out ti);
			string text = cbType.Model.GetValue(ti,1).ToString();

			project.ApplicationType =text;
			if (generatePublishList){
				// bude to treba volat aj ked zmeny vybrane rozlisenia v projekte
				project.GeneratePublishCombination();
			}

			afc.Save();
		}

		private bool CheckMessage()
		{
			/*if (!generatePublishList) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("are_you_sure"),  MainClass.Languages.Translate("change_name_deleted_publish_list"), MessageType.Question,parentWindow);

				int result =ms.ShowDialog();


				if (result == (int)ResponseType.No){
					checkChange = false;
					entrName.Text=projectArtefact;
					checkChange = true;
					return false;
				}
				generatePublishList = true;

			}*/
			return true;
		}

	}
}

