using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Option
{
	public partial class PreferencesDialog : Gtk.Dialog
	{
		protected TreeStore store;
		HashSet<object> modifiedObjects = new HashSet<object>();
		Dictionary<int, SettingPanel> pages = new Dictionary<int, SettingPanel>();
		List<SettingPanel> list;
		private TypPreferences typPreferences;
		private object inicialize = null;
		 
		public PreferencesDialog(TypPreferences typ,object inicialize,string title)
		{
			this.Build(); 

			this.TransientFor = MainClass.MainWindow;
			this.Title = title;

			this.inicialize = inicialize;
			typPreferences = typ;

			GenerateTree();
		}

		public PreferencesDialog(TypPreferences typ,string title)
		{
			this.Build();

			this.TransientFor = MainClass.MainWindow;
			this.Title = title;

			this.inicialize = null;
			typPreferences = typ;

			GenerateTree();
		}

		private void GenerateTree()
		{
			store = new TreeStore(typeof(SettingPanel), typeof(string), typeof(string), typeof(bool),  typeof(bool), typeof(int));

			tvCategory.Model = store;
			tvCategory.HeadersVisible = false;

			TreeViewColumn col = new TreeViewColumn();
			CellRendererPixbuf crp = new CellRendererPixbuf();
			crp.StockSize = (uint)IconSize.LargeToolbar;
			col.PackStart(crp, false);
			col.AddAttribute(crp, "stock-id", 1);
			col.AddAttribute(crp, "visible", 3);
			col.AddAttribute(crp, "sensitive", 4);
			CellRendererText crt = new CellRendererText();
			col.PackStart(crt, true);
			col.AddAttribute(crt, "markup", 2);
			col.AddAttribute(crt, "sensitive", 4);

			tvCategory.AppendColumn(col);

			tvCategory.Selection.Changed += OnSelectionChanged;
			FillTree();

			TreeIter it;
			if (store.GetIterFirst(out it))
				tvCategory.Selection.SelectIter(it);

		}

		public void FillTree()
		{
			list = new List<SettingPanel>();
			store.Clear();

			switch (typPreferences) {
			case TypPreferences.GlobalSetting:
				{
					list = GetGlobalSettingPages();
					break;
				}

			case TypPreferences.ProjectSetting:
				{
					list = GetProjectSettingPages();
					break;
				}
			case TypPreferences.FileSetting:
				{
					list = GetFileSettingPages();
					break;
				}
			case TypPreferences.DirectorySetting:
				{
					list = GetDirSettingPages();
					break;
				}

			default:
				break;
			}


			int i = 0;
			foreach (SettingPanel section in list) {
				AddSection(section, i);
				i++;
			}
		}

		protected TreeIter AddSection(SettingPanel section, int indx)
		{
			return AddSection(TreeIter.Zero, section, indx);
		}

		protected TreeIter AddSection(TreeIter parentIter, SettingPanel section, int indx)
		{
			TreeIter it;
			string sectionLabel;
			if (section.Active)
				sectionLabel = "<b>" + GLib.Markup.EscapeText(section.Panel.Label) + "</b>";
			else {
				sectionLabel = "<i><b>" + GLib.Markup.EscapeText(section.Panel.Label) + "</b></i>";

			}
			it = store.AppendValues(section, section.Panel.Icon, sectionLabel, true, section.Active, indx);
			pages.Add(indx, section);

			return it;
		}

		void OnSelectionChanged(object s, EventArgs a)
		{
			TreeIter it;
			if (tvCategory.Selection.GetSelected(out it)) {
				SettingPanel section = (SettingPanel)store.GetValue(it, 0);
				int indx = (int)store.GetValue(it, 5);
				//bool active = (bool)store.GetValue(it, 3);
				ShowPage(section, indx);
			}
		}

		public void ShowPage(SettingPanel section, int indx)
		{
			if (section == null)
				return;

			SettingPanel sp;

			if (!pages.TryGetValue(indx, out sp))
				return;

			foreach (Gtk.Widget w in pageFrame.Children) {
				Container cc = w as Gtk.Container;
				if (cc != null)
					foreach (Gtk.Widget cw in cc)
						cw.Hide();
				pageFrame.Remove(w);
			}

			labelTitle.Markup = "<span weight=\"bold\" size=\"x-large\">" + GLib.Markup.EscapeText(section.Panel.Label) + "</span>";
			pageFrame.PackStart(sp.Widget, true, true, 0);
			if(!sp.Active)
				sp.Widget.Sensitive = false;

			sp.Panel.ShowPanel();
			sp.Widget.ShowAll();
		}

		private List<SettingPanel> GetProjectSettingPages()
		{
			List<SettingPanel> list = new List<SettingPanel>();

			SettingPanel projectGlobal = new SettingPanel();
			projectGlobal.Panel = new GeneralPanel();
			if (this.inicialize != null)
				projectGlobal.Panel.Initialize(this, inicialize);
			projectGlobal.Widget = projectGlobal.Panel.CreatePanelWidget();
			list.Add(projectGlobal);


			SettingPanel applicationGlobal = new SettingPanel();
			applicationGlobal.Panel = new ApplicationPanel();
			if (this.inicialize != null)
				applicationGlobal.Panel.Initialize(this, inicialize);
			applicationGlobal.Widget = applicationGlobal.Panel.CreatePanelWidget();
			list.Add(applicationGlobal);

			SettingPanel conditionPanel = new SettingPanel();
			conditionPanel.Panel = new ProjectConditionsPanel();
			if (this.inicialize != null)
				conditionPanel.Panel.Initialize(this, inicialize);
			conditionPanel.Widget = conditionPanel.Panel.CreatePanelWidget();
			list.Add(conditionPanel);

			foreach (Rule rl in MainClass.Settings.Platform.Rules){

				if( (rl.Tag == -1 ) && !MainClass.Settings.ShowUnsupportedDevices) continue;
				if( (rl.Tag == -2 ) && !MainClass.Settings.ShowDebugDevices) continue;
				if (inicialize.GetType() == typeof(Project)) {
					Project project = (Project)inicialize;
					Device dvc = project.DevicesSettings.Find(x => x.TargetPlatformId == rl.Id);
					if (dvc == null) {
						Console.WriteLine("generate device -{0}",rl.Id);
						dvc = new Device();
						dvc.TargetPlatformId = rl.Id;
						project.DevicesSettings.Add(dvc);
					}

					DevicePropertyData dpd = new DevicePropertyData();
					dpd.Project = (Project)inicialize;
					dpd.Device = dvc;

					SettingPanel sp = new SettingPanel();
					sp.Panel = new DevicePanel();
					if (this.inicialize != null)
						sp.Panel.Initialize(this, dpd);
					sp.Widget = sp.Panel.CreatePanelWidget();

					//string dirPublish = MainClass.Tools.GetPublishDirectory(rl.Specific);

					/*if(!Device.CheckDevice(rl.Specific)) {
						sp.Active = false;
					}*/

					list.Add(sp);
				}
			}

			return list;
		}

		private List<SettingPanel> GetFileSettingPages()
		{
			List<SettingPanel> list = new List<SettingPanel>();

			SettingPanel fileGlobal = new SettingPanel();
			fileGlobal.Panel = new FileGeneralPanel();
			if (this.inicialize != null)
				fileGlobal.Panel.Initialize(this, inicialize);
			fileGlobal.Widget = fileGlobal.Panel.CreatePanelWidget();
			list.Add(fileGlobal);

			SettingPanel conditionPanel = new SettingPanel();
			conditionPanel.Panel = new FileConditionsPanel();
			if (this.inicialize != null)
				conditionPanel.Panel.Initialize(this, inicialize);
			conditionPanel.Widget = conditionPanel.Panel.CreatePanelWidget();
			list.Add(conditionPanel);

			return list;
		}

		private List<SettingPanel> GetDirSettingPages()
		{
			List<SettingPanel> list = new List<SettingPanel>();

			SettingPanel dirGlobal = new SettingPanel();
			dirGlobal.Panel = new DirGeneralPanel();
			if (this.inicialize != null)
				dirGlobal.Panel.Initialize(this, inicialize);
			dirGlobal.Widget = dirGlobal.Panel.CreatePanelWidget();
			list.Add(dirGlobal);

			SettingPanel conditionPanel = new SettingPanel();
			conditionPanel.Panel = new DirConditionsPanel();
			if (this.inicialize != null)
				conditionPanel.Panel.Initialize(this, inicialize);
			conditionPanel.Widget = conditionPanel.Panel.CreatePanelWidget();
			list.Add(conditionPanel);

			return list;
		}

		private List<SettingPanel> GetGlobalSettingPages()
		{
			List<SettingPanel> list = new List<SettingPanel>();

			SettingPanel item = new SettingPanel();
			item.Panel = new GlobalOptionsPanel();
			item.Panel.Initialize(this, null);
			item.Widget = item.Panel.CreatePanelWidget();
			list.Add(item);

			SettingPanel compilePanel = new SettingPanel();
			compilePanel.Panel = new CompilePanel();
			compilePanel.Widget = compilePanel.Panel.CreatePanelWidget();
			list.Add(compilePanel);

			SettingPanel resolution = new SettingPanel();
			resolution.Panel = new ResolutionOptionsPanel();
			resolution.Panel.Initialize(this, null);
			resolution.Widget = resolution.Panel.CreatePanelWidget();
			list.Add(resolution);

			SettingPanel emulator = new SettingPanel();
			emulator.Panel = new EmulatorOptionsPanel();
			emulator.Panel.Initialize(this, null);
			emulator.Widget = emulator.Panel.CreatePanelWidget();
			list.Add(emulator);

			SettingPanel textEditor = new SettingPanel();
			textEditor.Panel = new TextEditorPanel();
			textEditor.Widget = textEditor.Panel.CreatePanelWidget();
			list.Add(textEditor);

			SettingPanel imageEditorSetting = new SettingPanel();
			imageEditorSetting.Panel = new ImageEditorPanel();
			imageEditorSetting.Widget = imageEditorSetting.Panel.CreatePanelWidget();
			list.Add(imageEditorSetting);

			SettingPanel keybind = new SettingPanel();
			keybind.Panel = new KeyBindingPanel();
			keybind.Panel.Initialize(this, null);
			keybind.Widget = keybind.Panel.CreatePanelWidget();
			list.Add(keybind);

			SettingPanel filtering = new SettingPanel();
			filtering.Panel = new FilteringPanel();
			filtering.Panel.Initialize(this, null);
			filtering.Widget = filtering.Panel.CreatePanelWidget();
			list.Add(filtering);

			SettingPanel proxyPannel = new SettingPanel();
			proxyPannel.Panel = new ProxyPanel();
			proxyPannel.Panel.Initialize(this, null);
			proxyPannel.Widget = proxyPannel.Panel.CreatePanelWidget();
			list.Add(proxyPannel);

			SettingPanel editorEditor = new SettingPanel();
			editorEditor.Panel = new EditorPanel();
			editorEditor.Widget = editorEditor.Panel.CreatePanelWidget();
			list.Add(editorEditor);

			return list;
		}

		protected virtual void OnButtonOkClicked(object sender, System.EventArgs e)
		{
			if (!ValidateChanges())
				return;

			ApplyChanges();
			this.Respond(ResponseType.Ok);
		}

		protected virtual bool ValidateChanges()
		{
			foreach (SettingPanel sp in pages.Values) {
				if (sp.Panel == null)
					continue;
				if (!sp.Panel.ValidateChanges())
					return false;
				// Not valid
			}
			return true;
		}

		protected virtual void ApplyChanges()
		{
			foreach (SettingPanel sp in pages.Values) {
				if (sp.Panel == null)
					continue;
				sp.Panel.ApplyChanges();
			}
		}
		/*	protected virtual void AddChildSections (TreeIter parentIter, SectionPage section, object dataObject)
		{
			foreach (ExtensionNode nod in section.ChildNodes) {
				if (nod is OptionsDialogSection)
					AddSection (parentIter, (OptionsDialogSection) nod, dataObject);
			}
		}
			*/

		void DetachWidgets()
		{
			foreach (Gtk.Widget w in pageFrame.Children)
				pageFrame.Remove(w);

			foreach (SettingPanel sp in pages.Values)
				if (sp.Widget != null) {
					sp.Widget.Destroy();
					sp.Widget = null;
				}
		}

		protected override void OnResponse(ResponseType resp)
		{
			base.OnResponse(resp);
			DetachWidgets();
		}


		protected void OnResizeChecked (object sender, System.EventArgs e)
		{
			buttonOk.Show();
			buttonCancel.Show();
		}
	}

	public enum TypPreferences
	{
		GlobalSetting,
		WorkspaceSetting,
		ProjectSetting,
		FileSetting,
		DirectorySetting
	}

	public class SettingPanel
	{
		public IOptionsPanel Panel;
		public Gtk.Widget Widget;
		public bool Active =true;
		//public string Label;
	}

}

