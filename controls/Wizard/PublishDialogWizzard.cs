using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Components;
using Moscrif.IDE.Task;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using System.Threading;

namespace Moscrif.IDE.Controls.Wizard
{
	public partial class PublishDialogWizzard : Gtk.Dialog
	{

		private Notebook notebook;
		private Project project;
		private CheckButton chbOpenOutputDirectory;
		private CheckButton chbSignApp;
		private CheckButton chbIncludeAllResolution;
		private CheckButton chbDebugLog;
		Thread secondTaskThread;
		TaskList tlpublish;

		//PublishAsynchronTask pt;
		ListStore storeOutput;

		private DropDownButton ddbTypPublish = new DropDownButton();
		private DropDownButton ddbTypRemote = new DropDownButton();
		DropDownButton.ComboItemSet publishItems = new DropDownButton.ComboItemSet ();
		DropDownButton.ComboItemSet remoteItems = new DropDownButton.ComboItemSet ();
		Label lblRemote = new Label("Remote: ");

		DropDownButton.ComboItem ciDeviceTesting ;
		DropDownButton.ComboItem ciDeviceDistribution;

		private bool runningPublish = false;
		int status = 0;

		public PublishDialogWizzard()
		{
			project  = MainClass.Workspace.ActualProject;
			this.TransientFor = MainClass.MainWindow;
			this.Build();

			ciDeviceTesting = new DropDownButton.ComboItem(MainClass.Languages.Translate("device_testing"),0);
			ciDeviceDistribution = new DropDownButton.ComboItem(MainClass.Languages.Translate("market_distribution"),1);

			btnResetMatrix.Label = MainClass.Languages.Translate("reset_matrix");

			chbSignApp= new CheckButton( MainClass.Languages.Translate("sign_app"));
			chbSignApp.Active = MainClass.Workspace.SignApp;
			chbSignApp.Toggled += new EventHandler(OnChbSignAppToggled);
			chbSignApp.Sensitive = true;//MainClass.Settings.SignAllow;

			notebook1.ShowTabs = false;
			notebook1.ShowBorder = false;
			notebook1.Page = 0;

			Table tblHeader = new Table(1,4,false);
			
			ddbTypPublish = new DropDownButton();
			ddbTypPublish.Changed+= delegate(object sender, DropDownButton.ChangedEventArgs e)
			{
				if(e.Item !=null){
					int selTyp = (int)e.Item;
					if(selTyp == 0){
						lblRemote.Visible = true;
						ddbTypRemote.Visible = true;
						chbSignApp.Sensitive= false;
					} else {
						if(!MainClass.LicencesSystem.CheckFunction("marketdistribution",this)){
							ddbTypPublish.SelectItem(publishItems,ciDeviceTesting);
							return;
						}

						lblRemote.Visible = false;
						ddbTypRemote.Visible = false;
						chbSignApp.Sensitive= true;
					}

					MainClass.Workspace.ActualProject.TypPublish = selTyp;
				}
			};
			ddbTypPublish.WidthRequest = 175;
			ddbTypPublish.SetItemSet(publishItems);
			
			ddbTypRemote = new DropDownButton();
			ddbTypRemote.Changed+= delegate(object sender, DropDownButton.ChangedEventArgs e)
			{
				if(e.Item !=null){
					string ipAdress = (string)e.Item;
					MainClass.Settings.RemoteIpAdress = ipAdress;
				}
			};

			ddbTypRemote.WidthRequest = 175;
			ddbTypRemote.SetItemSet(remoteItems);

			publishItems.Add(ciDeviceTesting);
			publishItems.Add(ciDeviceDistribution);

			ddbTypPublish.SelectItem(publishItems,ciDeviceTesting);
			/*if(MainClass.Workspace.ActualProject.TypPublish ==1)
				ddbTypPublish.SelectItem(publishItems,ciDeviceDistribution);
			else 
				ddbTypPublish.SelectItem(publishItems,ciDeviceTesting);*/


			DropDownButton.ComboItem addRemote0 = new DropDownButton.ComboItem(MainClass.Languages.Translate("remote_none"),"0");
			remoteItems.Add(addRemote0);


			bool findSelect = false;
			List<string> listIp = Moscrif.IDE.Tool.Network.GetIpAdress();
			foreach (string ip in listIp){
				DropDownButton.ComboItem addIP = new DropDownButton.ComboItem(ip,ip);
				remoteItems.Add(addIP);
				if(ip== MainClass.Settings.RemoteIpAdress){
					ddbTypRemote.SelectItem(remoteItems,addIP);
					findSelect = true;
				}
			}
			if(!findSelect){
				ddbTypRemote.SelectItem(remoteItems,addRemote0);;
			}

			
			tblHeader.Attach(new Label("Publish: "),0,1,0,1,AttachOptions.Fill,AttachOptions.Fill,5,0);
			tblHeader.Attach(ddbTypPublish,1,2,0,1,AttachOptions.Fill,AttachOptions.Fill,0,0);
			
			tblHeader.Attach(lblRemote,2,3,0,1,AttachOptions.Fill,AttachOptions.Fill,5,0);
			tblHeader.Attach(ddbTypRemote,3,4,0,1,AttachOptions.Fill,AttachOptions.Fill,0,0);
			
			this.vbox2.PackStart(tblHeader,false,false,0);


			storeOutput = new ListStore (typeof (string), typeof (string), typeof (Gdk.Pixbuf),typeof (bool));
			
			nvOutput.Model = storeOutput;
			Gtk.CellRendererText collumnRenderer = new Gtk.CellRendererText();

			//nvOutput.AppendColumn ("", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvOutput.AppendColumn ("", collumnRenderer, "text", 0);
			nvOutput.AppendColumn ("", collumnRenderer, "text", 1);
			nvOutput.Columns[0].FixedWidth = 200;
			nvOutput.Columns[1].Expand = true;

			//nvOutput.Columns[0].SetCellDataFunc(collumnRenderer, new Gtk.TreeCellDataFunc(RenderOutput));
			//nvOutput.Columns[1].SetCellDataFunc(collumnRenderer, new Gtk.TreeCellDataFunc(RenderOutput));

			this.Title = MainClass.Languages.Translate("publish_title" , project.ProjectName);
			
			if(project.ProjectUserSetting.CombinePublish == null || project.ProjectUserSetting.CombinePublish.Count==0){
				project.GeneratePublishCombination();
			}
			
			if(project.DevicesSettings == null || project.DevicesSettings.Count == 0)
				project.GenerateDevices();
			
			foreach (Rule rl in MainClass.Settings.Platform.Rules){
				
				if( (rl.Tag == -1 ) && !MainClass.Settings.ShowUnsupportedDevices) continue;
				if( (rl.Tag == -2 ) && !MainClass.Settings.ShowDebugDevices) continue;
				
				Device dvc = project.DevicesSettings.Find(x => x.TargetPlatformId == rl.Id);
				if (dvc == null) {
					Console.WriteLine("generate device -{0}",rl.Id);
					dvc = new Device();
					dvc.TargetPlatformId = rl.Id;
					dvc.PublishPropertisMask = project.GeneratePublishPropertisMask(rl.Id);
					project.DevicesSettings.Add(dvc);
				}
			}

			project.Save();
			notebook = new Notebook();
			GenerateNotebookPages();
			
			this.vbox2.PackStart(notebook,true,true,0);//PackEnd
			
			VBox vbox1 = new VBox();
			
			chbOpenOutputDirectory = new CheckButton( MainClass.Languages.Translate("open_open_directory_after_publish"));
			chbOpenOutputDirectory.Toggled += new EventHandler(OnChbOpenOutputDirectoryToggled);
			
			chbIncludeAllResolution = new CheckButton( MainClass.Languages.Translate("include_all_resolution"));
			chbIncludeAllResolution.Active = project.IncludeAllResolution;
			chbIncludeAllResolution.Sensitive = false;
			chbIncludeAllResolution.Toggled+= delegate {
				project.IncludeAllResolution =chbIncludeAllResolution.Active;
			};
			
			vbox1.PackStart(chbIncludeAllResolution,false,false,0);
			vbox3.PackEnd(chbOpenOutputDirectory,false,false,0);
			
			chbDebugLog = new Gtk.CheckButton(MainClass.Languages.Translate("debug_log_publish"));
			chbDebugLog.Active = MainClass.Settings.LogPublish;
			chbDebugLog.Toggled+= delegate {
				MainClass.Settings.LogPublish =  chbDebugLog.Active;
			};
			
			vbox1.PackEnd(chbDebugLog,false,false,0);
			
			this.vbox2.PackEnd(vbox1,false,false,0);


			VBox hbox = new VBox();
			hbox.PackStart(chbSignApp,false,false,0);
			
			this.vbox2.PackEnd(hbox,false,false,0);
			
			this.ShowAll();
			
			int cpage = project.ProjectUserSetting.PublishPage;
			
			notebook.SwitchPage += delegate(object o, SwitchPageArgs args) {
				project.ProjectUserSetting.PublishPage = notebook.CurrentPage;
				
				NotebookLabel nl = (NotebookLabel)notebook.GetTabLabel(notebook.CurrentPageWidget);
				chbIncludeAllResolution.Sensitive = false;
				
				if(nl.Tag == null) return;
				
				Device d = project.DevicesSettings.Find(x=>(int)x.Devicetype==(int)nl.Tag);
				if(d!=null){
					if(d.Includes != null){
						if(d.Includes.Skin!=null){
							if(!String.IsNullOrEmpty(d.Includes.Skin.Name))
								chbIncludeAllResolution.Sensitive = true;
						}
					}
				}				
			};
			
			chbOpenOutputDirectory.Active = MainClass.Settings.OpenOutputAfterPublish;

			
			notebook.CurrentPage =cpage;
			btnNext.GrabFocus();
		}

		private void GenerateNotebookPages(){
			
			string platformName = MainClass.Settings.Platform.Name;
			
			foreach(Rule rl in MainClass.Settings.Platform.Rules){
				
				bool iOsNoMac = false;
				
				if( (rl.Tag == -1 ) && !MainClass.Settings.ShowUnsupportedDevices) continue;
				if( (rl.Tag == -2 ) && !MainClass.Settings.ShowDebugDevices) continue;
				
				bool validDevice = true;
				
				if(!Device.CheckDevice(rl.Specific) ){
					Tool.Logger.Debug("Invalid Device " + rl.Specific);
					validDevice = false;
				}
				
				ScrolledWindow sw= new ScrolledWindow();
				sw.ShadowType = ShadowType.EtchedOut;
				
				TreeView tvList = new TreeView();
				
				List<CombinePublish> lcp =  project.ProjectUserSetting.CombinePublish.FindAll(x=> x.combineRule.FindIndex(y=>y.ConditionName==platformName && y.RuleId == rl.Id) >-1);
				List<CombinePublish> lcpDennied = new List<CombinePublish>();
				string deviceName = rl.Name;
				int deviceTyp = rl.Id;
				
				ListStore ls = new ListStore(typeof(bool),typeof(string),typeof(CombinePublish),typeof(string),typeof(bool));
				
				string ico="empty.png";
				switch (deviceTyp) {
				case (int)DeviceType.Android_1_6:{
					ico = "android.png";
					break;}
				case (int)DeviceType.Android_2_2:{
					ico = "android.png";
					break;}
				case (int)DeviceType.Bada_1_0:
				case (int)DeviceType.Bada_1_1:
				case (int)DeviceType.Bada_1_2:
				case (int)DeviceType.Bada_2_0:{
					ico = "bada.png";
					break;}
				case (int)DeviceType.Symbian_9_4:{
					ico = "symbian.png";
					break;}
				case (int)DeviceType.iOS_5_0:{
					ico = "apple.png";
					if(!MainClass.Platform.IsMac){
						iOsNoMac = true;					
					}
					
					break;
				}
				case (int)DeviceType.PocketPC_2003SE:
				case (int)DeviceType.WindowsMobile_5:
				case (int)DeviceType.WindowsMobile_6:{
					ico = "windows.png";
					break;}
				case (int)DeviceType.Windows:{
					ico = "win32.png";
					break;}
				case (int)DeviceType.MacOs:{
					ico = "macos.png";
					break;}
				}
				
				List<CombinePublish> tmp =  lcp.FindAll(x=>x.IsSelected == true);
				
				NotebookLabel nl = new NotebookLabel(ico,String.Format("{0} ({1})",deviceName,tmp.Count ));
				nl.Tag=deviceTyp;
				
				
				if(iOsNoMac){
					Label lbl=new Label(MainClass.Languages.Translate("ios_available_Mac"));
					
					Pango.FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
					customFont.Weight = Pango.Weight.Bold;
					lbl.ModifyFont(customFont);
					
					notebook.AppendPage(lbl, nl);
					continue;
				}
				if(!validDevice){
					Label lbl=new Label(MainClass.Languages.Translate("publish_tool_missing"));
					
					Pango.FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
					customFont.Weight = Pango.Weight.Bold;
					lbl.ModifyFont(customFont);
					
					notebook.AppendPage(lbl, nl);
					continue;
				}
				
				;
				CellRendererToggle crt = new CellRendererToggle();
				crt.Activatable = true;
				crt.Sensitive = true;
				tvList.AppendColumn ("", crt, "active", 0);
				
				Gtk.CellRendererText fileNameRenderer = new Gtk.CellRendererText();
				Gtk.CellRendererText collumnResolRenderer = new Gtk.CellRendererText();
				
				tvList.AppendColumn(MainClass.Languages.Translate("file_name"),fileNameRenderer, "text", 1);
				tvList.AppendColumn(MainClass.Languages.Translate("resolution_f1"), collumnResolRenderer, "text", 1);
				
				tvList.Columns[1].SetCellDataFunc(fileNameRenderer, new Gtk.TreeCellDataFunc(RenderCombine));
				tvList.Columns[2].SetCellDataFunc(collumnResolRenderer, new Gtk.TreeCellDataFunc(RenderResolution));
				
				// povolene resolution pre danu platformu
				PlatformResolution listPR = MainClass.Settings.PlatformResolutions.Find(x=>x.IdPlatform ==deviceTyp);
				
				Device dvc  = project.DevicesSettings.Find(x=>x.TargetPlatformId ==deviceTyp);
				
				string stringTheme = "";
				List<System.IO.DirectoryInfo> themeResolution = new List<System.IO.DirectoryInfo>(); // resolution z adresara themes po novom
				
				if((project.NewSkin) && (dvc != null)){
					Skin skin =dvc.Includes.Skin;
					if((skin != null) && ( !String.IsNullOrEmpty(skin.Name)) && (!String.IsNullOrEmpty(skin.Theme)) ){
						string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
						stringTheme = System.IO.Path.Combine(skinDir,skin.Name);
						stringTheme = System.IO.Path.Combine(stringTheme, "themes");
						stringTheme = System.IO.Path.Combine(stringTheme, skin.Theme);
						
						if (System.IO.Directory.Exists(stringTheme)){
							System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(stringTheme);
							themeResolution = new List<System.IO.DirectoryInfo>(di.GetDirectories());
						}
					}
				}
				
				crt.Toggled += delegate(object o, ToggledArgs args) {
					if((deviceTyp == (int)DeviceType.Windows)||(deviceTyp == (int)DeviceType.MacOs)){
						if(!MainClass.LicencesSystem.CheckFunction("windowsandmac",this)){
							return;
						} 
					}


					TreeIter iter;
					if (ls.GetIter (out iter, new TreePath(args.Path))) {
						bool old = (bool) ls.GetValue(iter,0);
						CombinePublish cp =(CombinePublish) ls.GetValue(iter,2);
						cp.IsSelected = !old;
						ls.SetValue(iter,0,!old);
						
						List<CombinePublish> tmp2 =  lcp.FindAll(x=>x.IsSelected == true);
						nl.SetLabel (String.Format("{0} ({1})",deviceName,tmp2.Count ));
						
						//if(dvc == null) return;
						//if(dvc.Includes == null) return;
						if(dvc.Includes.Skin == null) return;
						if(String.IsNullOrEmpty(dvc.Includes.Skin.Name) || String.IsNullOrEmpty(dvc.Includes.Skin.Theme)) return;
						
						
						if(cp.IsSelected){
							// Najdem ake je rozlisenie v danej combinacii
							CombineCondition cc = cp.combineRule.Find(x=>x.ConditionId == MainClass.Settings.Resolution.Id);
							
							if(cc == null) return; /// nema ziadne rozlisenie v combinacii
							
							int indxResol = themeResolution.FindIndex(x=>x.Name.ToLower() == cc.RuleName.ToLower());
							if(indxResol<0){
								// theme chyba prislusne rozlisenie
								string error =String.Format("Invalid {0} Skin and {1} Theme, using in {2}. Missing resolutions: {3}. ",dvc.Includes.Skin.Name,dvc.Includes.Skin.Theme,deviceName,cc.RuleName.ToLower());
								MainClass.MainWindow.OutputConsole.WriteError(error+"\n");
								List<string> lst = new List<string>();
								lst.Add(error);
								MainClass.MainWindow.ErrorWritte("","",lst);
							}
						}
					}
				};
				
				int cntOfAdded = 0;
				foreach (CombinePublish cp in lcp){
					bool isValid = cp.IsSelected;
					
					if (!validDevice) isValid = false;
					
					// Najdem ake je rozlisenie v danej combinacii
					CombineCondition cc = cp.combineRule.Find(x=>x.ConditionId == MainClass.Settings.Resolution.Id);
					
					if(cc == null) continue; /// nema ziadne rozlisenie v combinacii
					
					int indx = MainClass.Settings.Resolution.Rules.FindIndex(x=> x.Id == cc.RuleId );
					if(indx<0) continue; /// rozlisenie pouzite v danej combinacii nexistuje
					
					if(cc!= null){
						bool isValidResolution = false;
						
						//ak nema definovane ziadne povolenia, tak povolene su vsetky
						if((listPR==null) || (listPR.AllowResolution == null) ||
						   (listPR.AllowResolution.Count<1)){
							isValidResolution = true;
						} else {
							isValidResolution = listPR.IsValidResolution(cc.RuleId);
						}
						
						if(isValidResolution){
							// po novom vyhodom aj tie ktore niesu v adresaru themes - pokial je thema definovana
							if((project.NewSkin) && (themeResolution.Count > 0)){
								//cntResolution = 0;
								int indxResol = themeResolution.FindIndex(x=>x.Name.ToLower() == cc.RuleName.ToLower());
								if(indxResol>-1){
									ls.AppendValues(isValid,cp.ProjectName,cp,cp.ProjectName,true);
									cntOfAdded++;
								} else {
									lcpDennied.Add(cp);
								}
							} else {
								ls.AppendValues(isValid,cp.ProjectName,cp,cp.ProjectName,true);
								cntOfAdded++;
							}
							
						} else {
							lcpDennied.Add(cp);
						}
					}
					//}
				}
				// pridam tie zakazane, ktore su vybrate na publish
				foreach (CombinePublish cp in lcpDennied){
					if(cp.IsSelected){
						ls.AppendValues(cp.IsSelected,cp.ProjectName,cp,cp.ProjectName,false);
						cntOfAdded++;
					}
				}
				
				if(cntOfAdded == 0){
					MainClass.MainWindow.OutputConsole.WriteError(String.Format("Missing publish settings for {0}.\n",deviceName));
				}
				
				bool showAll = false;
				tvList.ButtonReleaseEvent += delegate(object o, ButtonReleaseEventArgs args){
					
					if (args.Event.Button == 3) {
						TreeSelection ts = tvList.Selection;
						Gtk.TreePath[] selRow = ts.GetSelectedRows();
						
						if(selRow.Length<1){
							TreeIter tiFirst= new TreeIter();
							ls.GetIterFirst(out tiFirst);
							
							tvList.Selection.SelectIter(tiFirst);
							selRow = ts.GetSelectedRows();
						}
						if(selRow.Length<1) return;
						
						Gtk.TreePath tp = selRow[0];
						TreeIter ti = new TreeIter();
						
						ls.GetIter(out ti,tp);
						
						CombinePublish combinePublish= (CombinePublish)ls.GetValue(ti,2);
						
						if(combinePublish!=null){
							
							Menu popupMenu = new Menu();
							if(!showAll){
								MenuItem miShowDenied = new MenuItem( MainClass.Languages.Translate("show_denied" ));
								miShowDenied.Activated+= delegate(object sender, EventArgs e) {
									
									// odoberem zakazane, ktore sa zobrazuju kedze su zaceknute na publish
									List<TreeIter> lst= new List<TreeIter>();
									ls.Foreach((model, path, iterr) => {
										
										bool cp =(bool) ls.GetValue(iterr,4);
										bool selected =(bool) ls.GetValue(iterr,0);
										if(!cp && selected){
											lst.Add(iterr);
										}
										return false;
									});
									
									foreach(TreeIter ti2 in lst){
										TreeIter ti3 =ti2;
										ls.Remove(ref ti3);
									}
									
									// pridam zakazane
									if( (lcpDennied==null) || (lcpDennied.Count<1))
										return;
									
									foreach (CombinePublish cp in lcpDennied){
										ls.AppendValues(cp.IsSelected,cp.ProjectName,cp,cp.ProjectName,false);
									}
									showAll = true;
								};
								popupMenu.Append(miShowDenied);
							} else {
								MenuItem miHideDenied = new MenuItem( MainClass.Languages.Translate("hide_denied" ));
								miHideDenied.Activated+= delegate(object sender, EventArgs e) {
									
									List<TreeIter> lst= new List<TreeIter>();
									ls.Foreach((model, path, iterr) => {
										
										bool cp =(bool) ls.GetValue(iterr,4);
										bool selected =(bool) ls.GetValue(iterr,0);
										if(!cp && !selected){
											lst.Add(iterr);
										}
										return false;
									});
									
									foreach(TreeIter ti2 in lst){
										TreeIter ti3 =ti2;
										ls.Remove(ref ti3);
									}
									
									showAll = false;
								};
								popupMenu.Append(miHideDenied);
							}
							popupMenu.Append(new SeparatorMenuItem());
							
							
							MenuItem miCheckAll = new MenuItem( MainClass.Languages.Translate("check_all" ));
							miCheckAll.Activated+= delegate(object sender, EventArgs e) {
								if((deviceTyp == (int)DeviceType.Windows)||(deviceTyp == (int)DeviceType.MacOs)){
									if(!MainClass.LicencesSystem.CheckFunction("windowsandmac",this)){
										return;
									} 
								}

								int cnt = 0;
								ls.Foreach((model, path, iterr) => {
									CombinePublish cp =(CombinePublish) ls.GetValue(iterr,2);
									cp.IsSelected = true;
									ls.SetValue(iterr,0,true);
									cnt ++;
									return false;
								});
								nl.SetLabel (String.Format("{0} ({1})",deviceName,cnt ));
								
							};
							popupMenu.Append(miCheckAll);
							
							MenuItem miUnCheckAll = new MenuItem( MainClass.Languages.Translate("uncheck_all" ));
							miUnCheckAll.Activated+= delegate(object sender, EventArgs e) {
								ls.Foreach((model, path, iterr) => {
									CombinePublish cp =(CombinePublish) ls.GetValue(iterr,2);
									cp.IsSelected = false;
									ls.SetValue(iterr,0,false);
									return false;
								});
								nl.SetLabel (String.Format("{0} ({1})",deviceName,0 ));
							};
							popupMenu.Append(miUnCheckAll);

							popupMenu.Popup();
							popupMenu.ShowAll();
						}
					}
				};
				
				tvList.Model = ls;
				
				if (!validDevice) tvList.Sensitive = false;
				
				sw.Add(tvList);
				notebook.AppendPage(sw, nl);
			}
		}

		private void RenderOutput(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			bool isError = (bool) model.GetValue (iter, 3);

			if (isError) {
				(cell as Gtk.CellRendererText).Foreground = "Red";
			} else {
				(cell as Gtk.CellRendererText).Foreground = "Black";
			}
		}
		
		private void RenderResolution(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			CombinePublish cp = (CombinePublish) model.GetValue (iter, 2);
			bool type = (bool) model.GetValue (iter, 4);
			
			// Najdem ake je rozlisenie v danej combinacii
			CombineCondition cc = cp.combineRule.Find(x=>x.ConditionId == MainClass.Settings.Resolution.Id);
			
			if(cc == null) return;
			
			Rule rl = MainClass.Settings.Resolution.Rules.Find(x=>x.Id == cc.RuleId);
			
			if(rl == null) {
				return;
			}
			
			if(cc == null) return; /// nema ziadne rozlisenie v combinacii
			Pango.FontDescription fd = new Pango.FontDescription();
			(cell as Gtk.CellRendererText).Text =String.Format("{0}x{1}",rl.Width,rl.Height);
			
			if (!type) {
				fd.Style = Pango.Style.Italic;
				
			} else {
				fd.Style = Pango.Style.Normal;
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;
		}
		
		private void RenderCombine(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			bool type = (bool) model.GetValue (iter, 4);
			
			Pango.FontDescription fd = new Pango.FontDescription();
			
			if (!type) {
				fd.Style = Pango.Style.Italic;
				
			} else {
				fd.Style = Pango.Style.Normal;
			}
			(cell as Gtk.CellRendererText).FontDesc = fd;
			//(cell as Gtk.CellRendererText).Text = type;
		}

		protected virtual void OnChbSignAppToggled (object sender, System.EventArgs e)
		{
			if(chbSignApp.Active){
				if(MainClass.LicencesSystem.CheckFunction("signapp",this)){
					MainClass.Workspace.SignApp = chbSignApp.Active;
				} else {
					chbSignApp.Active = false;
				}
			}		
		}
		
		protected virtual void OnChbOpenOutputDirectoryToggled (object sender, System.EventArgs e)
		{
			MainClass.Settings.OpenOutputAfterPublish = chbOpenOutputDirectory.Active;
		}


		private bool LogginAndVerification(){

			LoggUser vc = new LoggUser();
			
			if((MainClass.User == null)||(string.IsNullOrEmpty(MainClass.User.Token))){
				
				LoginDialog ld = new LoginDialog(this);
				int res = ld.Run();
				
				if (res == (int)Gtk.ResponseType.Cancel){
					ld.Destroy();
					return false;
				} 
				ld.Destroy();
			}
			
			if(!vc.Ping(MainClass.User.Token)){
				MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("invalid_login_f1"), "", Gtk.MessageType.Error,this);
				md.ShowDialog();
				
				LoginDialog ld = new LoginDialog(this);
				int res = ld.Run();
				if (res == (int)Gtk.ResponseType.Cancel){
					ld.Destroy();
					return false;
				}else if(res == (int)Gtk.ResponseType.Ok){
					ld.Destroy();
					return true;
				}
			}
			return true;
		}

		protected void OnBtnNextClicked (object sender, EventArgs e)
		{
			if(notebook1.Page == 0){
				//btnResetMatrix.Visib
				List<CombinePublish> list =project.ProjectUserSetting.CombinePublish.FindAll(x=>x.IsSelected==true);
				
				if(list==null || list.Count<1){
					MessageDialogs md = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, MainClass.Languages.Translate("pleas_select_application"), "", Gtk.MessageType.Error,this);
					md.ShowDialog();
					return;
				}
				int selectTyp =  (int)ddbTypPublish.CurrentItem;

				if((selectTyp != 0) && (MainClass.Workspace.SignApp)){
					if(!LogginAndVerification()){
						return;
					}
				}
				notebook1.Page = 1;
				btnResetMatrix.Sensitive = false;
				btnNext.Sensitive = false;
				btnCancel.Label = "_Close";
				RunPublishTask(list);
			}
		}

		private void RunPublishTask(List<CombinePublish> list){
			
			LoggingInfo log = new LoggingInfo();
			log.LoggWebThread(LoggingInfo.ActionId.IDEPublish,project.ProjectName);

			int selectTyp =  (int)ddbTypPublish.CurrentItem;

			if((selectTyp == 0) || (!MainClass.Workspace.SignApp)){
				
				tlpublish = new TaskList();
				tlpublish.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();

				string selectRemote =  (string)ddbTypRemote.CurrentItem;
				Console.WriteLine("selectRemote-"+selectRemote);
				if(selectRemote != "0"){
					AppFile appF = MainClass.Workspace.ActualProject.AppFile;
					appF.Remote_Console =selectRemote+":"+MainClass.Settings.SocetServerPort;
					appF.Save();
					Console.WriteLine("MainClass.Workspace.ActualProject.AppFile.Remote_Console-"+MainClass.Workspace.ActualProject.AppFile.Remote_Console);
				} else {
					AppFile appF = MainClass.Workspace.ActualProject.AppFile;
					appF.Remote_Console ="";
					appF.Save();
				}

				PublishAsynchronTask pt = new PublishAsynchronTask();
				pt.ParentWindow = this;
				pt.EndTaskWrite+= MainClass.MainWindow.EndTaskWritte;

				pt.EndTaskWrite+= delegate(object sender, string name, string status, List<TaskMessage> errors) {
					runningPublish = false;
					btnCancel.Label = "_Close";
					if(selectRemote != "0"){
						AppFile appF = MainClass.Workspace.ActualProject.AppFile;
						appF.Remote_Console ="";
						appF.Save();
					}
				};

				pt.ErrorWrite+= MainClass.MainWindow.ErrorTaskWritte;
				pt.LogWrite+= MainClass.MainWindow.LogTaskWritte;
				pt.WriteStep+=  delegate(object sender, StepEventArgs e) {
					TreeIter ti =  storeOutput.AppendValues(e.Message1,e.Message2,null,e.IsError);

					if(status!=1)
						status = e.Status;
					while (Gtk.Application.EventsPending ())
						Gtk.Application.RunIteration ();
					//return ti;
				};
				pt.Initialize(list);

				tlpublish.TasksList.Add(pt);

				secondTaskThread = new Thread(new ThreadStart(tlpublish.ExecuteTaskOnlineWrite ));
				secondTaskThread.Name = "Publish Second Task";
				secondTaskThread.IsBackground = true;
				runningPublish = true;
				btnCancel.Label = "_Cancel";
				secondTaskThread.Start();


			} else {
			
				tlpublish = new TaskList();
				tlpublish.TasksList = new System.Collections.Generic.List<Moscrif.IDE.Task.ITask>();
				
				SignPublishAsynchronTask pt = new SignPublishAsynchronTask();
				pt.ParentWindow = this;
				pt.EndTaskWrite+= MainClass.MainWindow.EndTaskWritte;
				pt.EndTaskWrite+= delegate(object sender, string name, string status, List<TaskMessage> errors) {
					runningPublish = false;
					btnCancel.Label = "_Close";
				};
				pt.ErrorWrite+= MainClass.MainWindow.ErrorTaskWritte;
				pt.LogWrite+= MainClass.MainWindow.LogTaskWritte;
				pt.WriteStep+=  delegate(object sender, StepEventArgs e) {
					TreeIter ti =  storeOutput.AppendValues(e.Message1,e.Message2,null,e.IsError);
					if(status!=1)
						status = e.Status;
					while (Gtk.Application.EventsPending ())
						Gtk.Application.RunIteration ();
					//return ti;
				};
				pt.Initialize(list);
				
				tlpublish.TasksList.Add(pt);
				
				secondTaskThread = new Thread(new ThreadStart(tlpublish.ExecuteTaskOnlineWrite ));
				secondTaskThread.Name = "Publish Second Task";
				secondTaskThread.IsBackground = true;
				runningPublish = true;
				btnCancel.Label = "_Cancel";
				secondTaskThread.Start();			
			}
		}



		protected void OnBtnResetMatrixClicked (object sender, EventArgs e)
		{
			project.GeneratePublishCombination();
			int page = notebook.NPages;
			
			for (int i = page ; i>=0 ;i--){
				notebook.RemovePage(i);
			}
			GenerateNotebookPages();
			notebook.ShowAll();
		}

		protected void OnBtnCancelClicked (object sender, EventArgs e)
		{
			if(runningPublish){
				if(tlpublish!= null){
					storeOutput.AppendValues(MainClass.Languages.Translate("waiting_cancel"),"",null,false);
					tlpublish.StopAsynchronTask();
				}
				return;
			}


			if(notebook1.Page == 1){
				if((MainClass.Settings.OpenOutputAfterPublish) && (status==1) ){
					if (!String.IsNullOrEmpty(project.ProjectOutput)){
						MainClass.Tools.OpenFolder(project.OutputMaskToFullPath);
					}
				}
			}
			this.Respond(ResponseType.Close);
		}
	}
}

