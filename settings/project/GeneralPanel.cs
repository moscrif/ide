using System;
using Gtk;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Workspace;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Components;
using Moscrif.IDE.Tool;


namespace Moscrif.IDE.Settings
{

	internal class GeneralPanel : OptionsPanel
	{
		GeneralWidget widget;
		Project project ;

		public override Widget CreatePanelWidget()
		{
			return widget = new GeneralWidget(project,ParentDialog);
		}
		  
		public override void ShowPanel()
		{
			widget.ReloadPanel();
		}

		public override void ApplyChanges()
		{
			widget.Store();
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
			get { return MainClass.Languages.Translate("global_prferencies_project"); }
		}

		public override string Icon
		{
			get { return "project-preferences.png"; }
		}

	}


	public partial class GeneralWidget : Gtk.Bin
	{
		Project project;
		private FileMaskEntry  feOutput;

		Gtk.Menu popupCondition = new Gtk.Menu();
		Gtk.Window parentWindow;
		Gtk.Button btnPopUp = new Button();

		private bool checkChange = true;
		private bool generatePublishList = false;
		private string projectArtefact = "";


		public GeneralWidget(Project project,Gtk.Window parent)
		{
			parentWindow =parent;
			this.Build();

			feOutput = new FileMaskEntry(MainClass.Settings.WorkspaceMaskDirectory,this.project,parentWindow);
			feOutput.IsFolder = true;


			Pango.FontDescription fd = PangoContext.FontDescription.Copy();
			fd.Weight = Pango.Weight.Bold;

			//Pango.FontDescription customFont = Pango.FontDescription.FromString("Courier 16");//Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			//customFont.Weight = Pango.Weight.Bold;

			//checkbutton1.ModifyFont(customFont );

			this.project = project;
			if(MainClass.Workspace !=null){
				worksName.LabelProp =MainClass.Workspace.Name;
				worksDir.LabelProp= MainClass.Workspace.RootDirectory;

				prjName.LabelProp =this.project.ProjectName;
				prjDir.LabelProp = this.project.RelativeAppFilePath;
				prjFullPath.LabelProp  = this.project.AbsolutProjectDir;

			}
			if(!this.project.NewSkin){
				foreach (Devices.Device d in this.project.DevicesSettings){
					d.Includes.Skin.Name="";
					d.Includes.Skin.Theme="";
				}
				this.project.NewSkin =true;
			}
			//checkbutton1.Active =this.project.NewSkin;
			//checkbutton1.Visible =false;
			//checkbutton1.HideAll();//ShowAll()
			Gdk.Pixbuf default_pixbuf = null;
			string file = System.IO.Path.Combine(MainClass.Paths.ResDir, "stock-menu.png");
			popupCondition = new Gtk.Menu();

			if (System.IO.File.Exists(file)) {
				default_pixbuf = new Gdk.Pixbuf(file);

				btnPopUp = new Gtk.Button(new Gtk.Image(default_pixbuf));
				btnPopUp.TooltipText = MainClass.Languages.Translate("insert_condition_name");
				btnPopUp.Relief = Gtk.ReliefStyle.None;
				btnPopUp.CanFocus = false;
				btnPopUp.WidthRequest = btnPopUp.HeightRequest = 19;

				btnPopUp.Clicked += delegate {
					popupCondition.Popup(null,null, new Gtk.MenuPositionFunc (GetPosition) ,3,Gtk.Global.CurrentEventTime);
					//popupCondition.Popup();
				};
				hbox1.PackEnd(btnPopUp,false,false,0);
				//table1.Attach(btnPopUp,2, 3, 3, 4, AttachOptions.Shrink, AttachOptions.Shrink,0,0);
			}
			ReloadPanel();

			/*AddMenuItem(MainClass.Settings.Platform.Name);
			AddMenuItem(MainClass.Settings.Resolution.Name);

			if (project.ConditoinsDefine != null){
				foreach (Condition cd in project.ConditoinsDefine) {

					AddMenuItem(cd.Name);
				}
			}

			popupCondition.ShowAll();*/

			table1.Attach(feOutput, 1, 3, 4,5, AttachOptions.Fill, AttachOptions.Fill, 0, 0);

			if (String.IsNullOrEmpty(project.ProjectOutput)){
				if (!String.IsNullOrEmpty(MainClass.Workspace.OutputDirectory)){
					string fullOutput = MainClass.Workspace.OutputMaskToFullPath;

					if (!System.IO.Directory.Exists(fullOutput)){
						try {
							System.IO.Directory.CreateDirectory(fullOutput);
						}catch{	}
					}

					feOutput.DefaultPath  = fullOutput;
					feOutput.VisiblePath  = MainClass.Workspace.OutputDirectory;
				}
				else feOutput.DefaultPath  = project.AbsolutProjectDir;
			} else {
				string fullProjectOutput = project.OutputMaskToFullPath;
				feOutput.DefaultPath  = fullProjectOutput;
				feOutput.VisiblePath  = project.ProjectOutput;
			}

			if (String.IsNullOrEmpty(project.ProjectArtefac)) {

				string name = System.IO.Path.GetFileNameWithoutExtension(project.RelativeAppFilePath);
				projectArtefact = String.Format("{0}_$({1})_$({2})", name, MainClass.Settings.Platform.Name, MainClass.Settings.Resolution.Name);
				project.ProjectArtefac = projectArtefact;

			} else
				projectArtefact = project.ProjectArtefac;

			entrName.Text= projectArtefact;
			entrFacebookApi.Text= project.FacebookAppID ;

			entrName.Changed+= delegate(object sender, EventArgs e) {
				if(checkChange)
					CheckMessage();
			};

		}

		public bool Validate()
		{
			if (String.IsNullOrEmpty(feOutput.Path) || String.IsNullOrEmpty(entrName.Text)) {

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
			}

			return true;
		}

		public void Store()
		{
			string nameApp = entrName.Text;

			project.ProjectOutput = feOutput.Path;
			project.ProjectArtefac = nameApp;

			project.FacebookAppID = entrFacebookApi.Text;

			if (generatePublishList){
				// bude to treba volat aj ked zmeny vybrane rozlisenia v projekte
				project.GeneratePublishCombination();
			}
		}

		protected void OnButton734Clicked(object sender, System.EventArgs e)
		{
			string path = worksDir.LabelProp;
			MainClass.Tools.OpenFolder(path);

		}

		protected void OnButton735Clicked(object sender, System.EventArgs e)
		{
			string path = prjFullPath.LabelProp;
			MainClass.Tools.OpenFolder(path);
		}

		protected void OnCheckbutton1Toggled (object sender, System.EventArgs e)
		{
			/*if(this.project.NewSkin != checkbutton1.Active){
				foreach (Devices.Device d in this.project.DevicesSettings){
					d.Includes.Skin.Name="";
					d.Includes.Skin.Theme="";
				}
				this.project.NewSkin =checkbutton1.Active;
			}*/
		}

		private void DetachWidget(Gtk.Widget attach_widget, Gtk.Menu menu){}

		public void ReloadPanel(){

			popupCondition = new Gtk.Menu(); 
			popupCondition.AttachToWidget(btnPopUp,new Gtk.MenuDetachFunc(DetachWidget));

			AddMenuItem(MainClass.Settings.Platform.Name);
			AddMenuItem(MainClass.Settings.Resolution.Name);

			if (project.ConditoinsDefine != null){
				foreach (Condition cd in project.ConditoinsDefine) {

					AddMenuItem(cd.Name);
				}
			}

			popupCondition.ShowAll();
		}

		static void GetPosition(Gtk.Menu menu, out int x, out int y, out bool push_in){

			menu.AttachWidget.GdkWindow.GetOrigin(out x, out y);

			x =menu.AttachWidget.Allocation.X+x;//+menu.AttachWidget.WidthRequest;
			y =menu.AttachWidget.Allocation.Y+y+menu.AttachWidget.HeightRequest;

			push_in = true;
		}

		private bool CheckMessage()
		{
			if (!generatePublishList) {

				MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.YesNo, MainClass.Languages.Translate("are_you_sure"),  MainClass.Languages.Translate("change_name_deleted_publish_list"), MessageType.Question,parentWindow);

				int result =ms.ShowDialog();


				if (result == (int)ResponseType.No){
					checkChange = false;
					entrName.Text=projectArtefact;
					checkChange = true;
					return false;
				}
				generatePublishList = true;

			}
			return true;
		}

		private void AddMenuItem(string condition)
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

		}
	}
}

