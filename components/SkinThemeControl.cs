using System;
using Gtk;
using System.IO;
using System.Collections.Generic;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Components;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Execution;


namespace  Moscrif.IDE.Components
{
	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SkinThemeControl : Gtk.Bin
	{

		Gtk.ListStore skinListStore = new Gtk.ListStore(typeof(string), typeof(string));
		Gtk.ListStore themeListStore = new Gtk.ListStore(typeof(string), typeof(string));

		//private Project project;
		private Device device;

		public SkinThemeControl()
		{
			this.Build();

			//this.project=null;
			this.device=null;
			InicializeComponents();
		}

		public SkinThemeControl(/*Project project,*/Device device)
		{
			this.Build();

			//this.project=project;
			this.device=device;
			InicializeComponents();
		}

		private void InicializeComponents(){
			Gtk.CellRendererText textRenderer2 = new Gtk.CellRendererText();
			cbTheme.PackStart(textRenderer2, true);
			//cbTheme.AddAttribute(textRenderer2, "text", 0);
			cbTheme.Model = themeListStore;

			Gtk.CellRendererText textRenderer = new Gtk.CellRendererText();
			cbSkin.PackStart(textRenderer, true);
			//cbSkin.AddAttribute(textRenderer, "text", 0);
			cbSkin.Model = skinListStore;

			cbSkin.Changed += new EventHandler(OnComboSkinChanged);

			string skinDir = "";
			if(MainClass.Workspace == null || String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
				skinDir = System.IO.Path.Combine(MainClass.Settings.LibDirectory, MainClass.Settings.SkinDir);
			} else {
				skinDir = System.IO.Path.Combine( MainClass.Workspace.RootDirectory, MainClass.Settings.SkinDir);
			}
			FillSkinComboNew(skinDir);
		}


		public void SetSensitive(bool sensitive){
			cbSkin.Sensitive = sensitive;
			cbTheme.Sensitive = sensitive;
		}

		public void SetLabelWidth(int width){
			label1.WidthRequest =width;
			label2.WidthRequest =width;
		}

		public string GetSkin(){
			Gtk.TreeIter iterSkin;
			string skinName = string.Empty;
			if (cbSkin.GetActiveIter(out iterSkin))
				skinName = (string)cbSkin.Model.GetValue(iterSkin, 0);
			else
				skinName = "";

			return skinName;
		}

		public string GetTheme(){
			Gtk.TreeIter iterTheme;
			string themeName = string.Empty;

			if (cbTheme.GetActiveIter(out iterTheme))
				themeName = (string)cbTheme.Model.GetValue(iterTheme, 0);
			else
				themeName = "";

			return themeName;
		}

		public void SetDevice(Device device){
			this.device = device;
			string skinDir = System.IO.Path.Combine(MainClass.Workspace.RootDirectory,  MainClass.Settings.SkinDir);
			FillSkinComboNew(skinDir);
		}

		public void RefreshSkin(string path){
			skinListStore.Clear();
			FillSkinComboNew(path);
		}

		public void FillSkinComboNew(string path)
		{
			skinListStore.Clear();
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


				if ((device != null)&&(device.Includes.Skin.Name == d.Name)) {
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


				string skinDir = "";
				if(MainClass.Workspace == null || String.IsNullOrEmpty(MainClass.Workspace.FilePath)){
					skinDir = System.IO.Path.Combine(MainClass.Settings.LibDirectory, MainClass.Settings.SkinDir);
				} else {
					skinDir = System.IO.Path.Combine( MainClass.Workspace.RootDirectory, MainClass.Settings.SkinDir);
				}

				stringTheme = System.IO.Path.Combine(skinDir,skin);
				stringTheme = System.IO.Path.Combine(stringTheme, "themes");//".themes");

				if (!Directory.Exists(stringTheme))
					return;

				DirectoryInfo di = new DirectoryInfo(stringTheme);

				int i = 0;
				bool isSelected = false;

				DirectoryInfo[] dirInfos;// = di.GetDirectories(skin + ".*");

				dirInfos = di.GetDirectories();

				foreach (DirectoryInfo d in dirInfos) {

					int indx = -1;
					indx = MainClass.Settings.IgnoresFolders.FindIndex(x => x.Folder == d.Name && x.IsForIde);
	
					//if (!d.Name.StartsWith(".")) {
					if (indx > -1)
						continue;

					// po novom predvybereme default
					if(!isSelected){
						if(d.Name == "default"){
							Gtk.TreeIter iter3 = themeListStore.AppendValues(d.Name, d.FullName);
							cbTheme.SetActiveIter(iter3);
							continue;
						}
					}

					if ((device!=null)&&(device.Includes.Skin.Theme == d.Name)) {

						Gtk.TreeIter iter4 = themeListStore.AppendValues(d.Name, d.FullName);
						cbTheme.SetActiveIter(iter4);
						isSelected = true;
					} else
						themeListStore.AppendValues(d.Name, d.FullName);

					i++;
				}

				if(cbTheme.Active <0) cbTheme.Active=0;


			}
		}

	}
}

