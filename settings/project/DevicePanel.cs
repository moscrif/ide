using System;
using System.IO;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Components;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Execution;
using System.Text.RegularExpressions;

namespace Moscrif.IDE.Settings
{
	internal class DevicePanel : OptionsPanel
	{
		DeviceWidget widget;
		//Project project ;
		DevicePropertyData dpd;
		private string label;
		private string ico; 

		public override Widget CreatePanelWidget()
		{
			return widget = new DeviceWidget(dpd,ParentDialog);
		}

		public override void ShowPanel()
		{
			widget.ShowWidget();
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
			if (dataObject.GetType() == typeof(DevicePropertyData)) {
				dpd = (DevicePropertyData)dataObject;

				Rule plarform = dpd.Device.Platform;//MainClass.Settings.Platform.Rules.Find(x=>x.Id ==dpd.Device.TargetPlatformId);

				if (plarform != null) {

					label = plarform.Name;//dpd.Device.TargetPlatform;

					switch (dpd.Device.Devicetype) {
					//switch ((DeviceType)dpd.Device.MobileOs) {
						case DeviceType.Android_1_6:{
							ico = "android.png";
							break;}
						case DeviceType.Android_2_2:{
							ico = "android.png";
							break;}
						case DeviceType.Bada_1_0:
						case DeviceType.Bada_1_1:
						case DeviceType.Bada_1_2:
						case DeviceType.Bada_2_0:{
							ico = "bada.png";
							break;}
						case DeviceType.Symbian_9_4:{
							ico = "symbian.png";
							break;}
						case DeviceType.iOS_5_0:{
							ico = "apple.png";
							break;}
						case DeviceType.PocketPC_2003SE:
						case DeviceType.WindowsMobile_5:
						case DeviceType.WindowsMobile_6:{
							ico = "windows.png";
							break;}
						case DeviceType.Windows:{
							ico = "win32.png";
							break;}
						case DeviceType.MacOs:{
							ico = "macos.png";
							break;}
						default:{
							ico = "empty.png";
							break;}
					}
				} else
					label = "INVALIDATE";
			}


		}

		public override string Label
		{
			get { return label; }
		}

		public override string Icon
		{
			get { return ico; }
		}

	}

	public partial class DeviceWidget : Gtk.Bin
	{
		// ked n ieje adresar vpublis-dir tak znepristupnit !!!!!!
		private DevicePropertyData dpd;

		//displey name, full path, is selected, location (workspace,app), publish path
		Gtk.ListStore fontListStore = new Gtk.ListStore(typeof(string), typeof(string), typeof(bool),typeof(string),typeof(string));
		//Gtk.ListStore skinListStore = new Gtk.ListStore(typeof(string), typeof(string));
		//Gtk.ListStore themeListStore = new Gtk.ListStore(typeof(string), typeof(string));
		Gtk.Window parentWindow;
		List<string> fonts = new List<string>();

		//Gtk.Menu popupCondition = new Gtk.Menu();

		private PublishProperty FindPublishProperty(List<PublishProperty> ppArray, string name)
		{
			PublishProperty pp  = ppArray.Find(x=>x.PublishName == name);
			return pp;
		}

		/*private void GenerateFileEntry(ref Table table, string name, string label, string val,int xPos){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			FileEntry fEntry = new FileEntry();
			fEntry.Name = name;
			fEntry.IsFolder = false;
			if (!String.IsNullOrEmpty(val))
				fEntry.DefaultPath = val;
			else {
				string projectPlatform = System.IO.Path.Combine( dpd.Project.AbsolutProjectDir,"Platform");

				if (!Directory.Exists(projectPlatform))
					fEntry.SetDefaultPathInvisible(dpd.Project.AbsolutProjectDir);
				else fEntry.SetDefaultPathInvisible(projectPlatform);
			}

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(fEntry,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Expand,0,0);
		}*/

		private void GenerateFileMaskEntry(ref Table table, string name, string label, string val,int xPos){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			FileMaskEntry fmEntry = new FileMaskEntry(MainClass.Settings.ProjectMaskDirectory,dpd.Project,parentWindow);
			fmEntry.Name = name;
			fmEntry.IsFolder = false;

			if (!String.IsNullOrEmpty(val)){
				fmEntry.DefaultPath = dpd.Project.ConvertProjectMaskPathToFull(val);
				fmEntry.VisiblePath = val;
			}else {
				string projectPlatform = System.IO.Path.Combine( dpd.Project.AbsolutProjectDir,"Platform");

				if (!Directory.Exists(projectPlatform))
					fmEntry.SetDefaultPathInvisible(dpd.Project.AbsolutProjectDir);
				else fmEntry.SetDefaultPathInvisible(projectPlatform);
			}

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(fmEntry,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}

		private void GenerateEntry(ref Table table, string name, string label, string val,int xPos){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			Entry application = new Entry();
			application.Name = name;
			if (!String.IsNullOrEmpty(val))
				application.Text = val;
			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(application,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}


		private void GenerateBundleEntry(ref Table table, string name, string label, string val,int xPos){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			Entry application = new Entry();
			application.Name = name;

			if (!String.IsNullOrEmpty(val))
				application.Text = val.ToLower();
			else{
				val = "com";
				if(dpd.Project.AppFile!= null){
					if(!string.IsNullOrEmpty(dpd.Project.AppFile.Author))
						val = val +"."+dpd.Project.AppFile.Author;
					if(!string.IsNullOrEmpty(dpd.Project.AppFile.Title))
						val = val +"."+dpd.Project.AppFile.Title;

					string txt = MainClass.Tools.RemoveDiacritics(val);
					val = Regex.Replace(txt, @"[^A-Za-z\.]", "");
				}
				//val = "com."+Environment.UserName+"."+dpd.Project.ProjectName;
				application.Text = val.ToLower();
			}
			application.Changed+= delegate(object sender, EventArgs e) {
				string txt =application.Text;
				application.Text = Regex.Replace(txt, @"[^A-Za-z\.]", "").ToLower();
			};

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(application,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}

		private void GenerateComboBox(ref Table table, string name, string label, string selectVal,int xPos,List<SettingValue> list){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			CellRendererText textRenderer = new CellRendererText();

			ComboBox cbe = new ComboBox();//(val);

			ListStore cbModel = new ListStore(typeof(string), typeof(string));

			cbe.PackStart(textRenderer, true);
			cbe.AddAttribute(textRenderer, "text", 0);

			cbe.Name = name;
			cbe.Model= cbModel;
			cbe.Active = 0;

			TreeIter ti = new TreeIter();

			foreach(SettingValue ds in list){// MainClass.Settings.InstallLocations){
				if(ds.Value == selectVal){
					ti = cbModel.AppendValues(ds.Display,ds.Value);
					cbe.SetActiveIter(ti);
				} else  cbModel.AppendValues(ds.Display,ds.Value);
			}
			if(cbe.Active <0)
				cbe.Active =0;


			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(cbe,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}

		private void GenerateComboBoxSigning(ref Table table, string name, string label, string selectVal,int xPos,List<SettingValue> list){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			CellRendererText textRenderer = new CellRendererText();

			ComboBox cbe = new ComboBox();//(val);

			ListStore cbModel = new ListStore(typeof(string), typeof(string));

			cbe.PackStart(textRenderer, true);
			cbe.AddAttribute(textRenderer, "text", 0);

			cbe.Name = name;
			cbe.Model= cbModel;
			cbe.Active = 0;

			if(MainClass.Platform.IsMac){
				TreeIter ti = new TreeIter();

				foreach(SettingValue ds in list){// MainClass.Settings.InstallLocations){
					if(ds.Value == selectVal){
						ti = cbModel.AppendValues(ds.Display,ds.Value);
						cbe.SetActiveIter(ti);
					} else  cbModel.AppendValues(ds.Display,ds.Value);
				}
				if(cbe.Active <0)
					cbe.Active =0;
			} else {
				cbe.Sensitive = false;
				if(!String.IsNullOrEmpty(selectVal)){
					cbModel.AppendValues(selectVal,selectVal);
					cbe.Active =0;
				} else {
					Pango.FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
					customFont.Weight = Pango.Weight.Bold;
					cbe.ModifyFont(customFont);

					cbModel.AppendValues("Please, don´t forget set the provisioning","");
					cbe.Active =0;
				}
			}

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(cbe,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}

		private void GenerateEntryWithMenu(ref Table table, string name, string label, string val,int xPos){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			if (String.IsNullOrEmpty(val))
				val = "";


			EntryMenu application = new EntryMenu(MainClass.Settings.GetUnprotectedRange(),"Unprotected Range",MainClass.Settings.GetProtectedRange(),"Protected Range",val);
			application.Name = name;

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(application,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Expand,0,0);
		}


		private void GeneratePermissionEditor(ref Table table, string name, string label, string selectVal,int xPos){//,List<SettingValue> list){
			xPos = xPos+3;
			Label lblApp = new Label(label);
			lblApp.Xalign = 1;
			lblApp.Yalign = 0.5F;
			lblApp.WidthRequest = 115;
			if(table.Name != "table1")
				lblApp.WidthRequest = 114;

			PermissionButton button = new PermissionButton(selectVal,parentWindow);
			button.Name = name;
			button.Label = MainClass.Languages.Translate("permisions_edit");
			button.Permission = selectVal;
			button.HeightRequest = 24;
			button.WidthRequest = 95;

			HBox hb = new  HBox();
			hb.PackStart(button,false,false,0);

			table.Attach(lblApp,0,1,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Shrink,0,0);
			table.Attach(hb,1,2,(uint)(xPos-1),(uint)xPos,AttachOptions.Fill,AttachOptions.Expand,0,0);
		}

		public void GetIdentify ()
		{
			string cmd ="security";
			string args = "find-identity -v -p codesigning";
			//string cmd =MainClass.Settings.JavaCommand;
			//string args = MainClass.Settings.JavaArgument;
			try{
				//ProcessIdentifyChange(null, "message");
				ProcessWrapper pw = MainClass.ProcessService.StartProcess(cmd,args, "", ProcessIdentifyChange, ProcessIdentifyChange);
				pw.WaitForExit();

			}catch {//(Exception ex){
				//output.Add(new TaskMessage(ex.Message));
				//parentTask.Child =new TaskMessage(ex.Message);
				//stateTask = StateEnum.ERROR;
				//isPublishError = true;
				//return false;
			}
		}

		void ProcessIdentifyChange(object sender, string message)
		{
			while(message.IndexOf("\"")>-1){
				int idx =message.IndexOf("\"");
				string tmp = message.Remove(0,idx+1);
				int idx2 =tmp.IndexOf("\"");
				if(idx2 <=0) break;

				string displDev=tmp.Substring(0,idx2);

				securityIOs.Add(new SettingValue(displDev,displDev));

				tmp = tmp.Remove(0,idx2+1);

				message = tmp;
			}

			/*foreach(string s in securityIOs){
				Console.WriteLine(s);
			}*/
			//if(message.Contains("java version") )
		}

		/*
		string dirPublish = MainClass.Tools.GetPublishDirectory(rl.Specific);
		if (!System.IO.Directory.Exists(dirPublish)){
			validDevice = false;
		}
		 */

		List<SettingValue> securityIOs = new List<SettingValue>();
		public DeviceWidget(DevicePropertyData dpd,Gtk.Window parent)
		{
			parentWindow = parent;
			this.dpd = dpd;
			this.Build();

			/*this.WidthRequest = 650;
			this.HeightRequest = 500;*/

			if(dpd.Device.Devicetype == DeviceType.iOS_5_0)
				GetIdentify();

			skinThemeControl.SetLabelWidth(115);
			skinThemeControl.SetDevice(dpd.Device);

			this.dpd.Project.GenerateDevices();

			switch (dpd.Device.Devicetype) {
			//switch ((DeviceType)dpd.Device.TargetPlatformId) {
			case DeviceType.Android_1_6:{

				table2.NRows = table2.NRows+5;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//GenerateFileEntry

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_BUNDLEIDENTIFIER);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_BUNDLEIDENTIFIER));
				GenerateBundleEntry(ref table2,Project.KEY_BUNDLEIDENTIFIER,MainClass.Languages.Translate("bundleIdentifier"),pp.PublishValue,3);

				Gtk.Expander expanderAndr16 = new Expander("Android signing");

				Table tblAndr16 = new Table(4,2,false);
				tblAndr16.RowSpacing = 3;

				expanderAndr16.Add(tblAndr16);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_KEYSTORE);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_KEYSTORE));
				GenerateFileMaskEntry(ref tblAndr16,Project.KEY_KEYSTORE,MainClass.Languages.Translate("keystore")+" ",pp.PublishValue,0);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_STOREPASSWORD);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_STOREPASSWORD));
				GenerateEntry(ref tblAndr16,Project.KEY_STOREPASSWORD,MainClass.Languages.Translate("storepassword")+" ",pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ALIAS);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_ALIAS));
				GenerateEntry(ref tblAndr16,Project.KEY_ALIAS,MainClass.Languages.Translate("alias")+" ",pp.PublishValue,2);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_KEYPASSWORD);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_KEYPASSWORD));
				GenerateEntry(ref tblAndr16,Project.KEY_KEYPASSWORD,MainClass.Languages.Translate("keypassword")+" ",pp.PublishValue,3);
				//
				table2.Attach(expanderAndr16,0,2,6,7,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderAndr16.ShowAll();

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SUPPORTEDDEVICES);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_SUPPORTEDDEVICES));
				GenerateComboBox(ref table2,Project.KEY_SUPPORTEDDEVICES,MainClass.Languages.Translate("supportedDevices"),pp.PublishValue,5,MainClass.Settings.AndroidSupportedDevices);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_PERMISSION);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_PERMISSION));
				GeneratePermissionEditor(ref table2,Project.KEY_PERMISSION,MainClass.Languages.Translate("permisions"),pp.PublishValue,6);

				break;
				}
			case DeviceType.Android_2_2:{

				if ((MainClass.Settings.InstallLocations == null) || ((MainClass.Settings.InstallLocations.Count <1 )) ){
					MainClass.Settings.GenerateInstallLocation();
				}

				table2.NRows = table2.NRows+6;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//GenerateFileEntry

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_BUNDLEIDENTIFIER);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_BUNDLEIDENTIFIER));
				GenerateBundleEntry(ref table2,Project.KEY_BUNDLEIDENTIFIER,MainClass.Languages.Translate("bundleIdentifier"),pp.PublishValue,3);

				Gtk.Expander expanderAndr22 = new Expander("Android signing");
				
				Table tblAndr22 = new Table(4,2,false);
				tblAndr22.RowSpacing = 3;
				
				expanderAndr22.Add(tblAndr22);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_KEYSTORE);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_KEYSTORE));
				GenerateFileMaskEntry(ref tblAndr22,Project.KEY_KEYSTORE,MainClass.Languages.Translate("keystore")+" ",pp.PublishValue,0);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_STOREPASSWORD);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_STOREPASSWORD));
				GenerateEntry(ref tblAndr22,Project.KEY_STOREPASSWORD,MainClass.Languages.Translate("storepassword")+" ",pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ALIAS);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_ALIAS));
				GenerateEntry(ref tblAndr22,Project.KEY_ALIAS,MainClass.Languages.Translate("alias")+" ",pp.PublishValue,2);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_KEYPASSWORD);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_KEYPASSWORD));
				GenerateEntry(ref tblAndr22,Project.KEY_KEYPASSWORD,MainClass.Languages.Translate("keypassword")+" ",pp.PublishValue,3);

				table2.Attach(expanderAndr22,0,2,6,7,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderAndr22.ShowAll();

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SUPPORTEDDEVICES);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_SUPPORTEDDEVICES));
				GenerateComboBox(ref table2,Project.KEY_SUPPORTEDDEVICES,MainClass.Languages.Translate("supportedDevices"),pp.PublishValue,5,MainClass.Settings.AndroidSupportedDevices);


				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_INSTALLOCATION);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_INSTALLOCATION));
				GenerateComboBox(ref table2,Project.KEY_INSTALLOCATION,MainClass.Languages.Translate("installLocation"),pp.PublishValue,6,MainClass.Settings.InstallLocations);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_PERMISSION);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_PERMISSION));
				GeneratePermissionEditor(ref table2,Project.KEY_PERMISSION,MainClass.Languages.Translate("permisions"),pp.PublishValue,7);

				break;
				}


			case DeviceType.Bada_1_0:
			case DeviceType.Bada_1_1:
			case DeviceType.Bada_1_2:
			case DeviceType.Bada_2_0:{

				table2.NRows = table2.NRows+5;

				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);

				/*PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON_BADA1);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON_BADA1));
				GenerateFileMaskEntry(ref table1,Project.KEY_ICON_BADA1,MainClass.Languages.Translate("icon_f1", "100x96"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH_BADA1);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH_BADA1));
				GenerateFileMaskEntry(ref table1,Project.KEY_SPLASH_BADA1,MainClass.Languages.Translate("splash_f1","480x800"),pp.PublishValue,2);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON_BADA2);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON_BADA2));
				GenerateFileMaskEntry(ref table1,Project.KEY_ICON_BADA2,MainClass.Languages.Translate("icon_f1","50x47"),pp.PublishValue,3);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH_BADA2);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH_BADA2));
				GenerateFileMaskEntry(ref table1,Project.KEY_SPLASH_BADA2,MainClass.Languages.Translate("splash_f1","240x400"),pp.PublishValue,4);


				 pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON_BADA3);
				 if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON_BADA3));
				GenerateFileMaskEntry(ref table1,Project.KEY_ICON_BADA3,MainClass.Languages.Translate("icon_f1","50x50"),pp.PublishValue,xx);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH_BADA3);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH_BADA3));
				GenerateFileMaskEntry(ref table1,Project.KEY_SPLASH_BADA3,MainClass.Languages.Translate("splash_f1","320x480"),pp.PublishValue,xx);
				 */
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_MANIFEST);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_MANIFEST));
				GenerateFileMaskEntry(ref table2,Project.KEY_MANIFEST,MainClass.Languages.Translate("manifest"),pp.PublishValue,3);
				//

				break;}
			case DeviceType.Symbian_9_4:{

				table2.NRows = table2.NRows+7;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_APPLICATIONID);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_APPLICATIONID));
				GenerateEntryWithMenu(ref table2,Project.KEY_APPLICATIONID,MainClass.Languages.Translate("application_id"),pp.PublishValue,3);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_CERTIFICATE);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_CERTIFICATE));
				GenerateFileMaskEntry(ref table2,Project.KEY_CERTIFICATE,MainClass.Languages.Translate("certificate"),pp.PublishValue,4);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_KEY);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_KEY));
				GenerateFileMaskEntry(ref table2,Project.KEY_KEY,MainClass.Languages.Translate("key"),pp.PublishValue,5);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_PASSWORD);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_PASSWORD));
				GenerateEntry(ref table2,Project.KEY_PASSWORD,MainClass.Languages.Translate("password_f1"),pp.PublishValue,6);
				//
				break;}
			case DeviceType.iOS_5_0:{

				table2.NRows = table2.NRows+10;
//
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_BUNDLEIDENTIFIER);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_BUNDLEIDENTIFIER));
				GenerateBundleEntry(ref table2,Project.KEY_BUNDLEIDENTIFIER,MainClass.Languages.Translate("bundleIdentifier"),pp.PublishValue,1);


				 pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_CODESIGNINGIDENTITY);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_CODESIGNINGIDENTITY));
				GenerateComboBoxSigning(ref table2,Project.KEY_CODESIGNINGIDENTITY,MainClass.Languages.Translate("codeSigningIdentity"),pp.PublishValue,2,securityIOs);
				//GenerateComboBox(ref table1,Project.KEY_CODESIGNINGIDENTITY,MainClass.Languages.Translate("codeSigningIdentity"),pp.PublishValue,2,securityIOs);

				//GenerateEntry(ref table1,Project.KEY_CODESIGNINGIDENTITY,MainClass.Languages.Translate("codeSigningIdentity"),pp.PublishValue,1);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SUPPORTEDDEVICES);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp = new PublishProperty(Project.KEY_SUPPORTEDDEVICES));
				GenerateComboBox(ref table2,Project.KEY_SUPPORTEDDEVICES,MainClass.Languages.Translate("supportedDevices"),pp.PublishValue,3,MainClass.Settings.OSSupportedDevices);

				// iPhone4
				Gtk.Expander expanderiPh4 = new Expander("iPhone 4(S)");
				Table tbliPh4 = new Table(2,2,false);	
				tbliPh4.RowSpacing = 3;
				expanderiPh4.Add(tbliPh4);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IP4ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IP4ICON));
				GenerateFileMaskEntry(ref tbliPh4,Project.KEY_IP4ICON,"iPhone 4 icon : ",pp.PublishValue,0);

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IP4SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IP4SPLASH));
				GenerateFileMaskEntry(ref tbliPh4,Project.KEY_IP4SPLASH,"iPhone 4 splash : ",pp.PublishValue,1);

				table2.Attach(expanderiPh4,0,2,6,7,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderiPh4.ShowAll();

				// iPhone5
				Gtk.Expander expanderiPh5 = new Expander("iPhone 5");
				Table tbliPh5 = new Table(4,2,false);
				tbliPh5.RowSpacing = 3;
				expanderiPh5.Add(tbliPh5);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IP5ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IP5ICON));
				GenerateFileMaskEntry(ref tbliPh5,Project.KEY_IP5ICON,"iPhone 5 icon : ",pp.PublishValue,0);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IP5SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IP5SPLASH));
				GenerateFileMaskEntry(ref tbliPh5,Project.KEY_IP5SPLASH,"iPhone 5 splash : ",pp.PublishValue,1);
				
				table2.Attach(expanderiPh5,0,2,7,8,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderiPh5.ShowAll();

				// iPad2
				Gtk.Expander expanderiPd2 = new Expander("iPad 2");
				Table tbliPd2 = new Table(4,2,false);
				tbliPd2.RowSpacing = 3;
				expanderiPd2.Add(tbliPd2);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IPADICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IPADICON));
				GenerateFileMaskEntry(ref tbliPd2,Project.KEY_IPADICON,"iPad 2 icon : ",pp.PublishValue,0);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_IPADSPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_IPADSPLASH));
				GenerateFileMaskEntry(ref tbliPd2,Project.KEY_IPADSPLASH,"iPad 2 splash : ",pp.PublishValue,1);
				
				table2.Attach(expanderiPd2,0,2,8,9,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderiPh5.ShowAll();

				// new iPad
				Gtk.Expander expanderNewiPd = new Expander("the new iPad");
				Table tbliNewPd = new Table(4,2,false);	
				tbliNewPd.RowSpacing = 3;
				expanderNewiPd.Add(tbliNewPd);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_INEWPADICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_INEWPADICON));
				GenerateFileMaskEntry(ref tbliNewPd,Project.KEY_INEWPADICON,"new iPad icon : ",pp.PublishValue,0);
				
				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_INEWPADSPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_INEWPADSPLASH));
				GenerateFileMaskEntry(ref tbliNewPd,Project.KEY_INEWPADSPLASH,"new iPad splash : ",pp.PublishValue,1);
				
				table2.Attach(expanderNewiPd,0,2,9,10,AttachOptions.Expand|AttachOptions.Fill,AttachOptions.Shrink,0,0);
				expanderiPh5.ShowAll();

				break;
			}
			case DeviceType.PocketPC_2003SE:
			case DeviceType.WindowsMobile_5:
			case DeviceType.WindowsMobile_6:{

				table2.NRows = table2.NRows+3;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//
				break;
			}
			case DeviceType.Windows:{

				table2.NRows = table2.NRows+3;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//
				break;
			}
			case DeviceType.MacOs:{

				table2.NRows = table2.NRows+3;
				PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_ICON);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_ICON));
				GenerateFileMaskEntry(ref table2,Project.KEY_ICON,MainClass.Languages.Translate("icon"),pp.PublishValue,1);
				//

				pp = FindPublishProperty(dpd.Device.PublishPropertisMask, Project.KEY_SPLASH);
				if (pp == null)
					dpd.Device.PublishPropertisMask.Add(pp =new PublishProperty(Project.KEY_SPLASH));
				GenerateFileMaskEntry(ref table2,Project.KEY_SPLASH,MainClass.Languages.Translate("splash"),pp.PublishValue,2);
				//
				break;
			}

			default:{
				break;
			}

			}

			DirectoryInfo dir = new DirectoryInfo(MainClass.Paths.DisplayDir);

			nvFonts.Model = fontListStore;

			CellRendererToggle crt = new CellRendererToggle();
			crt.Activatable = true;
			crt.Toggled += delegate(object o, ToggledArgs args) {
					TreeIter iter;
					if (fontListStore.GetIter (out iter, new TreePath(args.Path))) {
						bool old = (bool) fontListStore.GetValue(iter,2);
					 	string font = (string) fontListStore.GetValue(iter,4);
						//CombinePublish cp =(CombinePublish) fontListStore.GetValue(iter,2);
						//cp.IsSelected = !old;
						if(old)
							fonts.Remove(font);
						else fonts.Add(font);

						fontListStore.SetValue(iter,2,!old);
				}
			};

			nvFonts.AppendColumn("", crt , "active", 2);
			nvFonts.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			//nvFonts.AppendColumn(MainClass.Languages.Translate("path"), new Gtk.CellRendererText(), "text", 1);
			nvFonts.AppendColumn(MainClass.Languages.Translate("location"), new Gtk.CellRendererText(), "text", 3);

			//nvFonts.Columns [1].Visible = false;

			string[] listFont = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.ttf");
			// fonty z projektoveho adresara
			string[] listFontProject = Directory.GetFiles(dpd.Project.AbsolutProjectDir, "*.ttf");

			if(MainClass.Platform.IsMac){ // for Mac UpperCase
				string[]  listFont2 = Directory.GetFiles(MainClass.Workspace.RootDirectory, "*.TTF");				

				var list = new List<string>();
				list.AddRange(listFont);
				list.AddRange(listFont2);
				listFont = list.ToArray();

				string[] listFontProject2 = Directory.GetFiles(dpd.Project.AbsolutProjectDir, "*.TTF");

				list = new List<string>();
				list.AddRange(listFontProject);
				list.AddRange(listFontProject2);
				listFontProject = list.ToArray();
			}


			if(dpd.Device != null && (dpd.Device.Includes!= null) && (dpd.Device.Includes.Fonts != null)){
				fonts = new List<string>(dpd.Device.Includes.Fonts);

				// odobratie neplatnich fontov - ak je nastaveny a nieje vo workspace - alebo v projektovom adresary
				List<string> fontsTmp = new List<string>(listFont);

				List<string> fontsTmpPrj = new List<string>(listFontProject);

				foreach (string fi in dpd.Device.Includes.Fonts) {

					int strFontWorks = fontsTmp.FindIndex(x=>System.IO.Path.GetFileName(x) == fi);

					string tmp =fi;
					tmp = tmp.Replace('/',System.IO.Path.DirectorySeparatorChar);
					tmp = tmp.Replace('\\',System.IO.Path.DirectorySeparatorChar);

					int strFontPrj =fontsTmpPrj.FindIndex(x=>
						System.IO.Path.Combine(dpd.Project.ProjectName,System.IO.Path.GetFileName(x)) == tmp);

					if (strFontWorks<0 && strFontPrj<0) fonts.Remove(fi);

				}
			} else fonts = new List<string>();

			Gtk.TreeIter firstSelectedTI = new Gtk.TreeIter();
			bool scrollToIter = false;
			foreach (string fi in listFont) {

				string fontname = System.IO.Path.GetFileName(fi);

				// ak obsahuje font v nazve medzeru nepouzije sa
				if(fontname.Contains(" ") ){
					MainClass.MainWindow.OutputConsole.WriteError("IGNORE FONT : '"+fontname+"' Invalid name contains a spacenvalid name contains a space \n");
					continue;
				}

				bool isSelect = false;

				int strNumber = fonts.FindIndex(x=>x == fontname);
				if (strNumber>-1) isSelect = true;
				if(isSelect && !scrollToIter){
					scrollToIter = true;
					firstSelectedTI =fontListStore.AppendValues(System.IO.Path.GetFileName(fi), fi,isSelect,MainClass.Languages.Translate("location_workspace"),System.IO.Path.GetFileName(fi));
				} else {
					fontListStore.AppendValues(System.IO.Path.GetFileName(fi), fi,isSelect,MainClass.Languages.Translate("location_workspace"),System.IO.Path.GetFileName(fi));
				}
			}

			foreach (string fi in listFontProject) {

				string fontname = System.IO.Path.GetFileName(fi);

				// ak obsahuje font v nazve medzeru nepouzije sa
				if(fontname.Contains(" ") ){
					MainClass.MainWindow.OutputConsole.WriteError("IGNORE FONT : '"+fontname+"' Invalid name contains a spacenvalid name contains a space");
					continue;
				}

				bool isSelect = false;
				string fontPath = System.IO.Path.Combine(dpd.Project.ProjectName,System.IO.Path.GetFileName(fi));
				//string fontPath = System.IO.Path.GetFileName(fi);
				string tmp =fi;
				tmp = tmp.Replace('/',System.IO.Path.DirectorySeparatorChar);
				tmp = tmp.Replace('\\',System.IO.Path.DirectorySeparatorChar);

				int strNumber = fonts.FindIndex(x=>
					x.Replace('/',System.IO.Path.DirectorySeparatorChar).Replace('\\',System.IO.Path.DirectorySeparatorChar)
					== fontPath);
				if (strNumber>-1) isSelect = true;

				if(isSelect && !scrollToIter){
					scrollToIter = true;
					firstSelectedTI =fontListStore.AppendValues(System.IO.Path.GetFileName(fi), fi,isSelect,MainClass.Languages.Translate("location_application"),fontPath);
				} else{
					fontListStore.AppendValues(System.IO.Path.GetFileName(fi), fi,isSelect,MainClass.Languages.Translate("location_application"),fontPath);
				}
			}
			skinThemeControl.ShowAll();
			if(scrollToIter){
				TreePath path = fontListStore.GetPath(firstSelectedTI);
				nvFonts.ScrollToCell(path,null, false, 0, 0);
			}

			//ShowWidget();
			/*
			Gtk.CellRendererText textRenderer2 = new Gtk.CellRendererText();
			cbTheme.PackStart(textRenderer2, true);
			//cbTheme.AddAttribute(textRenderer2, "text", 0);
			cbTheme.Model = themeListStore;


			Gtk.CellRendererText textRenderer = new Gtk.CellRendererText();
			cbSkin.PackStart(textRenderer, true);
			//cbSkin.AddAttribute(textRenderer, "text", 0);
			cbSkin.Model = skinListStore;

			cbSkin.Changed += new EventHandler(OnComboSkinChanged);

			if(dpd.Project.NewSkin){
				string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
				//cbSkin.Changed += new EventHandler(OnComboSkinChangedNew);
				FillSkinComboNew(skinDir);

			}*/
		}


		public void ShowWidget(){

			// kvoli tomu ze to islo dvoma verziamy
			//string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
			//skinThemeControl.RefreshSkin(skinDir);

			/*skinListStore.Clear();
			  if(dpd.Project.NewSkin){
				string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
				FillSkinComboNew(skinDir);

			} else {
				string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory, "skin");
				FillSkinComboOld(skinDir);
			}*/

		}
		/*
		private void FillSkinComboNew(string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);


			if (di == null) return;

			int i = 0;
			Gtk.TreeIter iter2 = skinListStore.AppendValues("", "");
			cbSkin.SetActiveIter(iter2);

			try {
				di.GetDirectories();
			}catch(Exception ex){
				Tool.Logger.Error(ex.Message,null);
				Tool.Logger.Error("SKIN directory {0} in Workspace {1} is invalid!",path,MainClass.Workspace.Name);
				return;
			}


			foreach (DirectoryInfo d in di.GetDirectories()) {

				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				//if (!d.Name.StartsWith(".")) {
				if (indx > -1)
					continue;//if (!d.Name.StartsWith("."))


				if (dpd.Device.Includes.Skin.Name == d.Name) {
					Gtk.TreeIter iter = skinListStore.AppendValues(d.Name, d.FullName);
					cbSkin.SetActiveIter(iter);
					i++;
				} else
					skinListStore.AppendValues(d.Name, d.FullName);
			}
			if (i == 0)
				cbSkin.Active = i;
		}


		private void FillSkinComboOld(string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo di = new DirectoryInfo(path);


			if (di == null) return;

			int i = 0;
			Gtk.TreeIter iter2 = skinListStore.AppendValues("", "");
			cbSkin.SetActiveIter(iter2);

			try {
				di.GetDirectories();
			}catch(Exception ex){
				Tool.Logger.Error(ex.Message,null);
				Tool.Logger.Error("SKIN directory {0} in Workspace {1} is invalid!",path,MainClass.Workspace.Name);
				return;
			}


			foreach (DirectoryInfo d in di.GetDirectories()) {

				int indx = -1;
				indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);

				//if (!d.Name.StartsWith(".")) {
				if (indx > -1)
					continue;//if (!d.Name.StartsWith("."))


				if (dpd.Device.Includes.Skin.Name == d.Name) {
					Gtk.TreeIter iter = skinListStore.AppendValues(d.Name, d.FullName);
					cbSkin.SetActiveIter(iter);
					i++;
				} else
					skinListStore.AppendValues(d.Name, d.FullName);
			}
			if (i == 0)
				cbSkin.Active = i;
		}

		void OnComboSkinChanged(object o, EventArgs args)
		{
			ComboBox combo = o as ComboBox;
			if (o == null)
				return;

			TreeIter iter;
			if (combo.GetActiveIter(out iter)) {
				string skin = (string)combo.Model.GetValue(iter, 0);

				themeListStore.Clear();

				if (String.IsNullOrEmpty(skin)) return;

				string stringTheme = "";
				if(dpd.Project.NewSkin){
					string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
					stringTheme = System.IO.Path.Combine(skinDir,skin);
					stringTheme = System.IO.Path.Combine(stringTheme, "themes");//".themes");

				} else {
					stringTheme = System.IO.Path.Combine(dpd.Project.AbsolutProjectDir, MainClass.Settings.ThemeDir);//".themes");

				}

				if (!Directory.Exists(stringTheme))
					return;

				DirectoryInfo di = new DirectoryInfo(stringTheme);

				int i = 0;
				bool isSelected = false;

				DirectoryInfo[] dirInfos;// = di.GetDirectories(skin + ".*");
				if(dpd.Project.NewSkin){
					dirInfos = di.GetDirectories();
				} else {

					Gtk.TreeIter iter2 = themeListStore.AppendValues("", "");
					cbTheme.SetActiveIter(iter2);

					// po starom vybereme "nic"
					isSelected = true;

					dirInfos = di.GetDirectories(skin + ".*");
				}


				foreach (DirectoryInfo d in dirInfos) {

					int indx = -1;
					indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);
	
					//if (!d.Name.StartsWith(".")) {
					if (indx > -1)
						continue;

					// po novom predvybereme default
					if(!isSelected && dpd.Project.NewSkin){
						if(d.Name == "default"){
							Gtk.TreeIter iter3 = themeListStore.AppendValues(d.Name, d.FullName);
							cbTheme.SetActiveIter(iter3);
							continue;
						}
					}

					if (dpd.Device.Includes.Skin.Theme == d.Name) {

						Gtk.TreeIter iter4 = themeListStore.AppendValues(d.Name, d.FullName);
						cbTheme.SetActiveIter(iter4);
						isSelected = true;
					} else
						themeListStore.AppendValues(d.Name, d.FullName);

					i++;
				}


			}
		}
		*/
		/*
		void OnComboSkinChangedOld(object o, EventArgs args)
		{
			ComboBox combo = o as ComboBox;
			if (o == null)
				return;

			TreeIter iter;
			if (combo.GetActiveIter(out iter)) {
				string skin = (string)combo.Model.GetValue(iter, 0);
				themeListStore.Clear();

				if (String.IsNullOrEmpty(skin)) return;

				string stringTheme = System.IO.Path.Combine(dpd.Project.AbsolutProjectDir, MainClass.Settings.ThemeDir);//".themes");

				if (!Directory.Exists(stringTheme))
					return;

				DirectoryInfo di = new DirectoryInfo(stringTheme);

				int i = 0;
				Gtk.TreeIter iter2 = themeListStore.AppendValues("", "");
				cbTheme.SetActiveIter(iter2);

				foreach (DirectoryInfo d in di.GetDirectories(skin + ".*")) {

					if (dpd.Device.Includes.Skin.Theme == d.Name) {

						Gtk.TreeIter iter3 = themeListStore.AppendValues(d.Name, d.FullName);
						cbTheme.SetActiveIter(iter3);
					} else
						themeListStore.AppendValues(d.Name, d.FullName);
					i++;
				}

			}
		}
		*/


		public void StoreTable(Gtk.Container parent){

			foreach (Widget w in parent.Children ){
				//Console.WriteLine(w.Name);
				if (w is Gtk.Container) {
					Gtk.Container container = w as Gtk.Container; 
					StoreTable(container);
				}

				switch (w.Name)
				{
					case Project.KEY_SPLASH:
					case Project.KEY_SPLASH_BADA1:
					case Project.KEY_SPLASH_BADA2:
					case Project.KEY_SPLASH_BADA3:
					case Project.KEY_MANIFEST:
					case Project.KEY_APPLICATIONFILE:
					case Project.KEY_ICON:
					case Project.KEY_ICON_BADA1 :
					case Project.KEY_ICON_BADA2 :
					case Project.KEY_ICON_BADA3 :
					case Project.KEY_CERTIFICATE:
					case Project.KEY_KEYSTORE:
					case Project.KEY_KEY:
					case Project.KEY_IP4ICON :
					case Project.KEY_IP4SPLASH  :
					case Project.KEY_IP5ICON  :
					case Project.KEY_IP5SPLASH  :
					case Project.KEY_IPADICON  :
					case Project.KEY_IPADSPLASH  :
					case Project.KEY_INEWPADICON  :
					case Project.KEY_INEWPADSPLASH   :
					/*case Project.KEY_ICON3GS:
					case Project.KEY_ICON4:
					case Project.KEY_ICONPAD:
					case Project.KEY_SPLASH3GS:
					case Project.KEY_SPLASH4:
					case Project.KEY_SPLASHPAD:*/
					{
						string file = (w as FileMaskEntry).Path;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);//dpd.Device.PublishPropertis.Find(x=>x.PublishName == Project.KEY_ICON);//
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						}
						pp.PublishValue = file;
						break;
					}
						/*case Project.KEY_SPLASH:
						case Project.KEY_MANIFEST:
	   					case Project.KEY_APPLICATIONFILE:
						case Project.KEY_ICON:{
							string file = (w as FileEntry).Path;
							PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);//dpd.Device.PublishPropertis.Find(x=>x.PublishName == Project.KEY_ICON);//
							if (pp == null){
								pp = new PublishProperty(w.Name);
								dpd.Device.PublishPropertisMask.Add(pp);
							}
							pp.PublishValue = file;
							break;
						}*/
					case Project.KEY_PASSWORD:
					case Project.KEY_STOREPASSWORD:
					case Project.KEY_ALIAS:
					case Project.KEY_KEYPASSWORD:
					{
						string text = (w as Entry).Text;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						}
						pp.PublishValue = text;
						break;
					}
						
					case Project.KEY_PERMISSION:
					{
						string text = (w as PermissionButton).Permission;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						}
						pp.PublishValue = text;
						break;
					}
					case Project.KEY_BUNDLEIDENTIFIER:{
						string text = (w as Entry).Text;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						}
						string txt = MainClass.Tools.RemoveDiacritics(text);
						txt = Regex.Replace(txt, @"[^A-Za-z\.]", "");
						pp.PublishValue = txt;
						break;
					}
					case Project.KEY_INSTALLOCATION:
					case Project.KEY_SUPPORTEDDEVICES:
					case Project.KEY_CODESIGNINGIDENTITY:
					{
						//string text = (w as ComboBox). ActiveText;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						};
						
						TreeIter ti = new TreeIter();
						
						if( !(w as ComboBox).GetActiveIter(out ti) ) {
							pp.PublishValue = "";
							break;//return;
						}
						
						string text = (w as ComboBox).Model.GetValue(ti,1).ToString();
						
						pp.PublishValue = text;
						break;
					}
					case Project.KEY_APPLICATIONID:{
						string text = (w as EntryMenu).LabelText;
						PublishProperty pp = FindPublishProperty(dpd.Device.PublishPropertisMask,w.Name);
						if (pp == null){
							pp = new PublishProperty(w.Name);
							dpd.Device.PublishPropertisMask.Add(pp);
						}
						pp.PublishValue = text;
						break;
					}
				}
			}
		}

		public void Store()
		{
			StoreTable(table2);

			/*Gtk.TreeIter iterSkin;
			string skinName = string.Empty;
			if (cbSkin.GetActiveIter(out iterSkin))
				skinName = (string)cbSkin.Model.GetValue(iterSkin, 0);
			else
				skinName = "";

			dpd.Device.Includes.Skin.Name = skinName;

			Gtk.TreeIter iterTheme;
			string themeName = string.Empty;

			if (cbTheme.GetActiveIter(out iterTheme))
				themeName = (string)cbTheme.Model.GetValue(iterTheme, 0);
			else
				themeName = "";

			dpd.Device.Includes.Skin.Theme = themeName;*/

			dpd.Device.Includes.Skin.Name = skinThemeControl.GetSkin();
			dpd.Device.Includes.Skin.Theme = skinThemeControl.GetTheme();

			string[] selectedFont = fonts.ToArray();//new string[nvFonts.Selection.CountSelectedRows()];

			/*int i = 0;

			nvFonts.Selection.SelectedForeach(delegate (Gtk.TreeModel model, Gtk.TreePath path, Gtk.TreeIter iter){

				string font = model.GetValue(iter, 0).ToString();

				selectedFont.SetValue(font, i);
				i++;
			});*/

			dpd.Device.Includes.Fonts = selectedFont;

			//
			//dpd.Device.Root = MainClass.Workspace.RootDirectory;
			//dpd.Device.Publish = MainClass.Settings.PublishDirectory;
		}
	}
}

