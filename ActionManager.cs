using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;
using Gtk;
using System.IO;
using Moscrif.IDE.Actions;
using Moscrif.IDE.Execution;
using Moscrif.IDE.Components;
using Moscrif.IDE.Editors;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Task;
using Moscrif.IDE.Controls;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using MessageDialogsUrl = Moscrif.IDE.Controls.MessageDialogUrl;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Controls.Wizard;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using System.Diagnostics;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;
using Moscrif.IDE.Extensions;
using System.Net;
using System.Net.Sockets;


namespace Moscrif.IDE
{
	public class ActionManager : IEnumerable
	{
		private UIManager ui = new UIManager();
		//static uint mergeId = 0;
		private ActionGroup main_window_actions = new ActionGroup("MainWindow");

		public ActionManager()
		{
			PopulateActionGroups();
		}

		public void LoadInterface()
		{
			string cPath = MainClass.Paths.ConfingDir;
			//System.Environment.CurrentDirectory ;
			string fullpath = System.IO.Path.Combine(cPath, "MenuStructure.Xml");
			
			if (File.Exists(fullpath)) {
				ui.AddUiFromFile(fullpath);
			} else {
				string ui_info = "<toolbar name=\"toolbarLeft\">\n" + "<toolitem name=\"Run\" action=\"run\" />\n" + "<toolitem name=\"What\" action=\"what\" />\n" + "<toolitem name=\"Save\" action=\"save\" />\n" + "</toolbar>\n" + "<toolbar name=\"toolbarMiddle\">\n" + "</toolbar>\n" + "<toolbar name=\"toolbarRight\">\n" + "<toolitem name=\"About\" action=\"about\" />\n" + "<toolitem name=\"Quit\" action=\"quit\" />\n" + "</toolbar>\n" + "<menubar >\n" + "<menu name=\"Ide\" action=\"IdeAction\">\n" + "<menuitem name=\"About\" action=\"about\" />\n" + "<menuitem name=\"Preferences\" action=\"preferences\" />\n" + "<separator name=\"sep1\" />\n" + "<menuitem name=\"Quit\" action=\"quit\" />\n" + "</menu>\n" + "<menu name=\"File\" action=\"File\">\n" + "<menuitem name=\"NewProject\" action=\"newproject\" />\n" + "<menuitem name=\"LoadProject\" action=\"loadproject\" />\n" + "<menuitem name=\"AppToMsp\" action=\"apptomsp\" />\n" + "<separator name=\"sep2\" />	\n" + "<menuitem name=\"Open\" action=\"open\" />    	\n" + "<menuitem name=\"Save\" action=\"save\" />\n" + "<menuitem name=\"Save All\" action=\"saveall\" />	\n" + "<separator name=\"sep3\" />\n" + "<menu name=\"RecentFile\" action=\"RecentFile\">\n" + "</menu>	\n" + "<menu name=\"RecentProject\" action=\"RecentProject\">	\n" + "</menu>   					  		\n" + "<separator name=\"sep4\" />	    	\n" + "<menuitem name=\"Close\"  action=\"close\" />\n" + "<menuitem name=\"Close All\"  action=\"closeall\" />				\n" + "</menu>\n" + "<menu name=\"Edit\" action=\"EditAction\">\n" + "<menuitem name=\"inserttemplate\" action=\"inserttemplate\" />\n" + "<menuitem name=\"insertcomplete\" action=\"insertcomplete\" />\n" + "<menuitem name=\"Undo\" action=\"undo\" />\n" + "<menuitem name=\"Redo\" action=\"redo\" />\n" + "</menu>	\n" + "<menu name=\"Search\" action=\"SearchAction\">\n" + "<menuitem name=\"Find\" action=\"find\" />\n" + "<menuitem name=\"FindNext\" action=\"findnext\" />\n" + "<menuitem name=\"FindPrevious\" action=\"findprevious\" />\n" + "<separator name=\"sep5\" />\n" + "<menuitem name=\"PreviousBookmark\" action=\"previouBookmark\" />\n" + "<menuitem name=\"NextBookmark\" action=\"nextBookmark\" />		\n" + "<separator name=\"sep5\" />\n" + "<menuitem name=\"ToggleBookmark\" action=\"toggleBookmark\" />\n" + "<menuitem name=\"PreviousBookmark\" action=\"previouBookmark\" />\n" + "<menuitem name=\"NextBookmark\" action=\"nextBookmark\" />				\n" + "<menuitem name=\"ClearBookmarks\" action=\"clearBookmarks\" />\n" + "</menu>					\n" + "<menu name=\"Emulator\" action=\"EmulatorAction\">\n" + "<menuitem name=\"Run\" action=\"run\" />\n" + "<separator name=\"sep6\" />	    	\n" + "<menuitem name=\"Clear\" action=\"clear\" />\n" + "<menuitem name=\"Test\" action=\"test\" />	    	\n" + "</menu>				\n" + "</menubar>\n" + "<popup name='iconMarginPopup'>\n" + "<menuitem name=\"ToggleBookmark\" action=\"toggleBookmark\" />\n" + "<menuitem name=\"ClearBookmarks\" action=\"clearBookmarks\" />\n" + "<separator name=\"sep7\" />\n" + "<menuitem name=\"AddBreakpoint\" action=\"addBreakpoint\" />\n" + "<menuitem name=\"RemoveBreakpoint\" action=\"removeBreakpoint\" />\n" + "<menuitem name=\"ToggleBreakpoint\" action=\"toggleBreakpoint\" />\n" + "<menuitem name=\"EnableDisableBreakpoint\" action=\"enableDisableBreakpoint\" /> \n" + "<menuitem name=\"DisableAllBreakpoints\" action=\"disableAllBreakpoints\" />\n" + "<menuitem name=\"ClearAllBreakpoints\" action=\"clearAllBreakpoints\" />	\n" + "<menuitem name=\"ShowBreakpointProperties\"  action=\"showBreakpointProperties\" />\n" + "</popup>\n" + "<popup name='netbookPagePopup'>\n" + "<menuitem name=\"Close\"  action=\"close\" />\n" + "<menuitem name=\"Close All\"  action=\"closeall\" />\n" + "<menuitem name=\"Close all but this\"  action=\"closeallbutthis\" />\n" + "<separator name=\"sep8\" />\n" + "<menuitem name=\"Save\" action=\"save\" />\n" + "<menuitem name=\"Save All\" action=\"saveall\" />\n" + "</popup>	\n" + "<popup name='workspacePagePopupDirectory'>\n" + "<!--<menu name=\"addgroup\" action=\"addgroup\">\n" + "<menuitem name=\"newfile\"  action=\"newfile\" />\n" + "<separator name=\"sep9\" />\n" + "<menuitem name=\"addfile\"  action=\"addfile\" />\n" + "<menuitem name=\"adddirectory\"  action=\"adddirectory\" />\n" + "</menu>-->	\n" + "<menuitem name=\"unloadproject\"  action=\"unloadproject\" />\n" + "<separator name=\"sep9\" />\n" + "<menuitem name=\"newfile\"  action=\"newfile\" />\n" + "<menuitem name=\"newdirectory\"  action=\"newdirectory\" />\n" + "<separator name=\"sep9\" />\n" + "<menuitem name=\"addfile\"  action=\"addfile\" />\n" + "<menuitem name=\"adddirectory\"  action=\"adddirectory\" />	\n" + "<separator name=\"sep10\" />		\n" + "<menuitem name=\"setasstart\"  action=\"setasstart\" />\n" + "<menuitem name=\"toggleexclude\"  action=\"toggleexclude\" />		\n" + "<menuitem name=\"delete\" action=\"deletefile\" />	    	\n" + "<menuitem name=\"rename\" action=\"renamefile\" />	    	    	\n" + "</popup>\n";
				ui.AddUiFromString(ui_info);
			}
		}

		private void PopulateActionGroups()
	{
	    main_window_actions.Add(new ActionEntry[] { new ActionEntry("IdeAction",null,"Ide",	null,null,null ),
				new ActionEntry("FileAction", null, "File", null, null, null),
				new ActionEntry("Workspace", null, "Workspace", null, null, null),
				new ActionEntry("Project", null, "Project", null, null, null),
				new ActionEntry("File", null, "File", null, null, null),
				new ActionEntry("newProject", null, "Open Project", null, null, null),
				new ActionEntry("RecentAll", null, "Recent", null, null, null),
				new ActionEntry("HelpAction", null, "Help", null, null, null),

			new ActionEntry("RecentFile", null, "Recent File", null, null, null),new ActionEntry("RecentProject", null, "Recent Project", null, null, null),
			new ActionEntry("RecentWorkspace", null, "Recent Workspace", null, null, null),
			new ActionEntry("EditAction", null, "Edit", null, null, null),
			new ActionEntry("SearchAction", null, "Search", null, null, null),
			new ActionEntry("EmulatorAction", null, "Emulator", null, null, null),
			new ActionEntry("ViewAction", null, "View", null, null, null),

			new ActionEntry("FileGroup", null, "Files", null, null, null),
			new ActionEntry("DirGroup", null, "Directories", null, null, null),
			new ActionEntry("addFileSubmenu",null,"New",null,null,null ),
			//new ActionEntry("copy", "copy.png", "_Copy", "<control>C", "Copy the selected text to the clipboard", new EventHandler(OnActivate)),
			//new ActionEntry("paste", "paste.png", "_Paste", "<control>V", "Paste the text from the clipboard", new EventHandler(OnActivate)),

			new ActionEntry("removeBreakpoint", "mobil.png", "Remove Breakpoint", null, "Remove Breakpoint", new EventHandler(OnActivate)),
			new ActionEntry("toggleBreakpoint", "mobil.png", "Toggle Breakpoint", null, "Toggle Breakpoint", new EventHandler(OnActivate)),
			new ActionEntry("enableDisableBreakpoint", "mobil.png", "Enable/Disable Breakpoint", null, "Enable/Disable Breakpoint", new EventHandler(OnActivate)),
			new ActionEntry("disableAllBreakpoints", "mobil.png", "Disable All Breakpoints", null, "Disable All Breakpoints", new EventHandler(OnActivate)),
			new ActionEntry("clearAllBreakpoints", "mobil.png", "Clear All Breakpoints", null, "Clear All Breakpoints", new EventHandler(OnActivate)),
			new ActionEntry("showBreakpointProperties", "mobil.png", "Show Breakpoint Properties", null, "Show Breakpoint Properties", new EventHandler(OnActivate)),

			new ActionEntry("ToolsAction", null, "Tools", null, null, null),
			new ActionEntry("test", "mobil.png", "Test", null, "test", new EventHandler(OnTest))
			});

			//main_window_actions.Add(new Gtk.Action("test", "mobil.png", "Test",null), keyBind.FindAccelerator("test"));

			main_window_actions.Add(new AddBreakpointAction(), null);
			main_window_actions.Add(new AboutAction(), null);
			main_window_actions.Add(new QuitAction(),"<control>Q");
			main_window_actions.Add(new NewAction(), null);
			main_window_actions.Add(new NewProjectAction(), null);
			main_window_actions.Add(new NewProjectWizzardAction(), null);
			main_window_actions.Add(new ProjectPreferencesAction(),null);

			main_window_actions.Add(new OpenAction(), "<control>O");
			main_window_actions.Add(new OpenProjectAction(), "<control><shift>O");
			main_window_actions.Add(new SavePageAction(), "<control>S");
			main_window_actions.Add(new SaveAsPageAction(), "<control><shift>S");
			main_window_actions.Add(new SaveAllPageAction(), "<control><shift>A");
			
			main_window_actions.Add(new UndoAction(), "<control>Z");
			main_window_actions.Add(new RedoAction(), "<control><shift>Z");
			
			main_window_actions.Add(new ToggleBookmarkAction(), "<control>F2");
			main_window_actions.Add(new NextBookmarkAction(), "F2");
			main_window_actions.Add(new PreviousBookmarkAction(), "<shift>F2");
			main_window_actions.Add(new ClearBookmarksAction(), null);
			
			main_window_actions.Add(new ClosePageAction(), "<control>F4");
			main_window_actions.Add(new CloseAllPageAction(), "<control><shift>F4");
			main_window_actions.Add(new CloseAllButThisAction(), null);
			
			main_window_actions.Add(new FindAction(), "<control>F");
			main_window_actions.Add(new FindNextAction(), "F3");
			main_window_actions.Add(new FindPreviousAction(), "<control>F3");
			main_window_actions.Add(new DeleteFileAction(), null);
			main_window_actions.Add(new RenameFileAction(), null);
			main_window_actions.Add(new AddFileAction(), null);
			main_window_actions.Add(new AddDirectoryAction(), null);
			main_window_actions.Add(new SetAsStartAction(), null);
			main_window_actions.Add(new ToggleExcludeAction(), null);
			main_window_actions.Add(new NewDirectoryAction(), null);
			main_window_actions.Add(new UnloadProjectAction(), null);
			main_window_actions.Add(new DeleteProjectAction(), null);
			main_window_actions.Add(new ShowDirectoryAction(), null);

			main_window_actions.Add(new RenameProjectAction(), null);			
			main_window_actions.Add(new InsertTemplateAction(), "<control>" + Gdk.Key.T);
			main_window_actions.Add(new InsertAutoCompletAction(), "<control>" + Gdk.Key.space);
			main_window_actions.Add(new ClearConsoleAction(), null);
			main_window_actions.Add(new NewWorkspaceAction(), null);
			main_window_actions.Add(new OpenWorkspace(), null);
			main_window_actions.Add(new IdePreferencesAction(), null);

			main_window_actions.Add(new UniqueSequenceAction(), null);
			main_window_actions.Add(new OpenStartPageAction(), null);
			main_window_actions.Add(new CheckVersionAction(), null);

			main_window_actions.Add(new CommentUncommentAction(), "<control><alt>c");

			main_window_actions.Add(new GoToLineAction(), "<control>G");

			main_window_actions.Add(new RunEmulatorAction(), "<control>F5");
			main_window_actions.Add(new RunEmulatorNoWindAction(), "<shift>F5");
			main_window_actions.Add(new RunEmulatorDebugAction(), null);
			main_window_actions.Add(new StopEmulatorAction(), null);
			main_window_actions.Add(new CompileProjectAction(), "F9");
			main_window_actions.Add(new PublishAction(), "<control><shift>F9");
			main_window_actions.Add(new ImportZipProjectAction(), "");
			main_window_actions.Add(new ImportFolderProjectAction(), "");
			main_window_actions.Add(new ExportZipProjectAction(), "");
			main_window_actions.Add(new ArchiveProjectAction(), "");

			main_window_actions.Add(new PropertyAllAction(), "");
			main_window_actions.Add(new FilePropertyAction(), "");

			main_window_actions.Add(new OpenOutputAction(), "");
			main_window_actions.Add(new OpenOutputPopUpAction(), "");
			main_window_actions.Add(new ShowProject(), "");

			main_window_actions.Add(new SendFeedbackAction(), "");
			main_window_actions.Add(new OpenApiReferenceAction(), "");
			main_window_actions.Add(new OpenOnlineApiReferenceAction(), "");
			main_window_actions.Add(new OpenOnlineDocument(), "");
			main_window_actions.Add(new OpenSubmitSupport(), "");
			main_window_actions.Add(new OpenOnlineDemos(), "");
			main_window_actions.Add(new OpenOnlineSamples(), "");

			main_window_actions.Add(new PasteTextAction(), "");
			main_window_actions.Add(new CopyTextAction(), "");
			main_window_actions.Add(new CutTextAction(), "");
			main_window_actions.Add(new GoToDefinitionAction(), "");

			main_window_actions.Add(new ViewProjectAppAction(), null);
			main_window_actions.Add(new ViewFileAction(), null);
			main_window_actions.Add(new AddThemeAction(), null);

			main_window_actions.Add(new FilePreferencesPopUpAction(), null);
			main_window_actions.Add(new DirectoryPreferencesPopUpAction(), null);
			main_window_actions.Add(new ProjectPreferencesPopUpAction(), null);
			main_window_actions.Add(new RefreshProjectAction(), null);

			main_window_actions.Add(new GenerateAutoCompleteWords(), null);
			main_window_actions.Add(new SettingPlatformAction(), null);

			main_window_actions.Add(new ShowLeftPaneAction(), null);
			main_window_actions.Add(new ShowBottomPaneAction(), null);
			main_window_actions.Add(new ShowRightPaneAction(), null);


			ui.InsertActionGroup(main_window_actions, 0);
			ui.EnsureUpdate();

			ReloadKeyBind();

		}

	public void CreateMacMenu(MenuBar mainMenu)
	{

	    //enable the global key handler for keyboard shortcuts
	    IgeMacMenu.GlobalKeyHandlerEnabled = false;
	
	    //Tell the IGE library to use your GTK menu as the Mac main menu
	    IgeMacMenu.MenuBar = mainMenu;
	
	    //tell IGE which menu item should be used for the app menu's quit item
	    MenuItem miQ = (MenuItem)this.GetWidget("/menubar/FileAction/Quit");
	    IgeMacMenu.QuitMenuItem = miQ;

	    //add a new group to the app menu, and add some items to it
	    var appGroup = IgeMacMenu.AddAppMenuGroup();

	    MenuItem miA = (MenuItem)this.GetWidget("/menubar/Tools/About");
	    appGroup.AddMenuItem(
		miA,
		MainClass.Languages.Translate("menu_about").Replace("_", "")
	    );

	    MenuItem miP = (MenuItem)this.GetWidget("/menubar/Tools/idepreferences");
	    appGroup.AddMenuItem(
		miP,
		MainClass.Languages.Translate("menu_preferences").Replace("_", "")
	    );

	    appGroup.AddMenuItem(
		miQ,
		MainClass.Languages.Translate("menu_quit").Replace("_", "")
	    );	
	    //hide the menu bar so it no longer displays within the window
		mainMenu.Hide ();
	}

	public void ReloadKeyBind(){

		KeyBindings keyBindFile = MainClass.KeyBinding;

		foreach (ActionGroup @group in ui.ActionGroups){
			foreach (Gtk.Action action in @group.ListActions()){

				string accel = keyBindFile.FindAccelerator(action.Name);
				if(!String.IsNullOrEmpty(accel)){
					keyBindFile.ConnectAccelerator(action,accel,ui.AccelGroup);
					//ConnectAccelerator(action,accel);
				}

			}
		}
	}


		#region RecentFileALL
	uint mergeIdAll = 1;
	ActionGroup dynGroupAll = new ActionGroup("RecentAll");
	public void RecentAll(IList<RecentFile> lRecentFile,IList<RecentFile> lRecentProject,IList<RecentFile> lRecentWorkspace)
	{
		ui.InsertActionGroup(dynGroupAll, 0);
		mergeIdAll = ui.NewMergeId();

		int i = 0;
		
		foreach (RecentFile rf in lRecentFile) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenFile(fileName,false);
			};
			dynGroupAll.Add(action);
			ui.AddUi(mergeIdAll, "/menubar/FileAction/RecentAll", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
		if(i>0){
			ui.AddUi(mergeIdAll,"/menubar/FileAction/RecentAll","separatorAll11","",UIManagerItemType.Separator, false);
		}
		int j = i;
			/*
		foreach (RecentFile rf in lRecentProject) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenFile(fileName,true);
			};
			dynGroupAll.Add(action);
			ui.AddUi(mergeIdAll, "/menubar/FileAction/RecentAll", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
		ui.AddUi(mergeIdAll,"/menubar/FileAction/RecentAll","separatorAll12","",UIManagerItemType.Separator, false);
			i++;*/

		foreach (RecentFile rf in lRecentWorkspace) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fileName);
				if (workspace != null)
						MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
			};
			dynGroupAll.Add(action);
			ui.AddUi(mergeIdAll, "/menubar/FileAction/RecentAll", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
		if(i>j){
			ui.AddUi(mergeIdAll,"/menubar/FileAction/RecentAll","separatorAll13","",UIManagerItemType.Separator, false);
		}

		string nameCF = "ClearFile";
		string labelCF = String.Format("Clear Menu");
		Gtk.Action actionCF = new Gtk.Action(nameCF, labelCF);
		actionCF.Activated += delegate(object sender, EventArgs e) { 
				MainClass.Settings.RecentFiles.ClearFiles();
				MainClass.Settings.RecentFiles.ClearProjects();
				MainClass.Settings.RecentFiles.ClearWorkspace();
				//ClearRecentAll();
				RefreshRecentAll(MainClass.Settings.RecentFiles.GetFiles(),
				                 MainClass.Settings.RecentFiles.GetProjects(),
				                 MainClass.Settings.RecentFiles.GetWorkspace());
			};
		dynGroupAll.Add(actionCF);
		ui.AddUi(mergeIdAll, "/menubar/FileAction/RecentAll", nameCF, nameCF, UIManagerItemType.Menuitem, false);
		 
	}

	public void RefreshRecentAll(IList<RecentFile> lRecentFile,IList<RecentFile> lRecentProject,IList<RecentFile> lRecentWorkspace)
	{
		//ui.RemoveUi(mergeIdAll);
		//ui.RemoveActionGroup(dynGroupAll);
		Menu subMenu = new Menu();


		MenuItem miR = (MenuItem)this.GetWidget("/menubar/FileAction/RecentAll");
		if(miR!= null)
				miR.Submenu = subMenu;


		//dynGroupAll = new ActionGroup("RecentAll");
		//ui.InsertActionGroup(dynGroupAll, 0);

		int i = 0;
		
		foreach (RecentFile rf in lRecentFile) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));


			MenuItem mi = new MenuItem(label);
			mi.Name = name;
			string fileName = rf.FileName;
			mi.Activated+= delegate(object sender, EventArgs e) {
					MainClass.MainWindow.OpenFile(fileName,true);
			};
			i++;
			subMenu.Add(mi);
		}
		if(i>0){
			subMenu.Add(new SeparatorMenuItem());
		}
		int j = i;

		/*foreach (RecentFile rf in lRecentProject) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenFile(fileName,true);
			};
			dynGroupAll.Add(action);
			ui.AddUi(mergeIdAll, "/menubar/FileAction/RecentAll", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
		ui.AddUi(mergeIdAll,"/menubar/FileAction/RecentAll","separatorAll12","",UIManagerItemType.Separator, false);
			i++;*/

		foreach (RecentFile rf in lRecentWorkspace) {
			string name = "RecentAll" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			
			MenuItem mi = new MenuItem(label);
			mi.Name = name;
			string fileName = rf.FileName;
			mi.Activated+= delegate(object sender, EventArgs e) {
				Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fileName);
				if (workspace != null)
					MainClass.MainWindow.ReloadWorkspace(workspace,true,true);
			};
			i++;
			subMenu.Add(mi);
		}

		if(i>j){
			subMenu.Add(new SeparatorMenuItem());		
		}

		string nameCF = "ClearFile";
		string labelCF = String.Format("Clear Menu");

		MenuItem miCF = new MenuItem(labelCF);
		miCF.Name = nameCF;
		miCF.Activated+= delegate(object sender, EventArgs e) {
			MainClass.Settings.RecentFiles.ClearFiles();
			MainClass.Settings.RecentFiles.ClearProjects();
			MainClass.Settings.RecentFiles.ClearWorkspace();
			RefreshRecentAll(MainClass.Settings.RecentFiles.GetFiles(),
			                 MainClass.Settings.RecentFiles.GetProjects(),
			                 MainClass.Settings.RecentFiles.GetWorkspace());
		};
		i++;
		subMenu.Add(miCF);

		miR.ShowAll();
	}
	#endregion


		/*
	uint mergeIdWorkspace = 1;
	ActionGroup dynGroupWorkspace = new ActionGroup("RecentWorkspace");
	
		//int workspaceCount ;
	public void RecentWorkspace(IList<RecentFile> lRecentProjects)
	{
	    ui.InsertActionGroup(dynGroupWorkspace, 0);
	    mergeIdWorkspace = ui.NewMergeId();
	    int workspaceCount = 0;

	    foreach (RecentFile rf in lRecentProjects) {

		string name = "RecentWorkspace" + workspaceCount;
		string label = String.Format(
		    "_{0} {1}",
		    workspaceCount,
		    rf.DisplayName.Replace("_", "__")
		);
		Gtk.Action action = new Gtk.Action(name, label);

		string fileName = rf.FileName;

		action.Activated += delegate(object sender, EventArgs e)
		{
		    Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fileName);
		    if (workspace != null)
			MainClass.MainWindow.ReloadWorkspace(workspace, true, true);

		};
		dynGroupWorkspace.Add(action);
		///menubar/IdeAction/RecentWorkspace

		ui.AddUi(
		    mergeIdWorkspace,"/menubar/FileAction/RecentWorkspace", name, name, UIManagerItemType.Menuitem, false);
		workspaceCount++;
		}
	}

	public void RefreshRecentWorkspace(IList<RecentFile> lRecentProjects)
	{
		ui.RemoveUi(mergeIdWorkspace);
		ui.RemoveActionGroup(dynGroupWorkspace);

		dynGroupWorkspace = new ActionGroup("RecentWorkspace");
		ui.InsertActionGroup(dynGroupWorkspace, 0);
		int workspaceCount = 0;

		foreach (RecentFile rf in lRecentProjects) {

			string name = "RecentWorkspace" + workspaceCount;
			string label = String.Format("_{0} {1}", workspaceCount, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;

			action.Activated += delegate(object sender, EventArgs e) {
			Workspace.Workspace workspace = Workspace.Workspace.OpenWorkspace(fileName);
				if (workspace != null)
					MainClass.MainWindow.ReloadWorkspace(workspace,true,true);

			};
			dynGroupWorkspace.Add(action);
			ui.AddUi(mergeIdWorkspace, "/menubar/FileAction/RecentWorkspace", name, name, UIManagerItemType.Menuitem, false);
			workspaceCount++;
		}
		ui.EnsureUpdate();
	}

	uint mergeIdProject = 1;
	ActionGroup dynGroupProject = new ActionGroup("RecentProject");

	public void RecentProjects(IList<RecentFile> lRecentProjects)
	{
		//uint mergeId = 1;
		;
		ui.InsertActionGroup(dynGroupProject, 0);
		mergeIdProject = ui.NewMergeId();
		
		int i = 0;
		
		foreach (RecentFile rf in lRecentProjects) {
			string name = "RecentProject" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;

			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenProject(fileName, true);
			};
			dynGroupProject.Add(action);
			ui.AddUi(mergeIdProject, "/menubar/FileAction/RecentProject", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
	}

	public void RefreshRecentProjects(IList<RecentFile> lRecentProjects)
	{
		ui.RemoveUi(mergeIdProject);
		ui.RemoveActionGroup(dynGroupProject);

		dynGroupProject = new ActionGroup("RecentProject");
		ui.InsertActionGroup(dynGroupProject, 0);

		int i = 0;
		
		foreach (RecentFile rf in lRecentProjects) {
			string name = "RecentProject" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;

			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenProject(fileName, true);
			};
			dynGroupProject.Add(action);
			ui.AddUi(mergeIdProject, "/menubar/FileAction/RecentProject", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
	}


	uint mergeIdFiles = 1;
	ActionGroup dynGroupFiles = new ActionGroup("RecentFile");
	public void RecentFiles(IList<RecentFile> lRecentFiles)
	{
		ui.InsertActionGroup(dynGroupFiles, 0);
		mergeIdFiles = ui.NewMergeId();

		int i = 0;
		
		foreach (RecentFile rf in lRecentFiles) {
			string name = "RecentFile" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				MainClass.MainWindow.OpenFile(fileName,true);
			};
			dynGroupFiles.Add(action);
			ui.AddUi(mergeIdFiles, "/menubar/FileAction/RecentFile", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
	}

	public void RefreshRecentFiles(IList<RecentFile> lRecentFiles)
	{
		ui.RemoveUi(mergeIdFiles);
		ui.RemoveActionGroup(dynGroupFiles);

		dynGroupFiles = new ActionGroup("RecentFile");
		ui.InsertActionGroup(dynGroupFiles, 0);

		int i = 0;
		
		foreach (RecentFile rf in lRecentFiles) {
			string name = "RecentFile" + i;
			string label = String.Format("_{0} {1}", i, rf.DisplayName.Replace("_","__"));
			Gtk.Action action = new Gtk.Action(name, label);

			string fileName = rf.FileName;
			action.Activated += delegate(object sender, EventArgs e) {
				//en.Open(fileName);
				MainClass.MainWindow.OpenFile(fileName,true);
			};
			dynGroupFiles.Add(action);
			ui.AddUi(mergeIdFiles, "/menubar/FileAction/RecentFile", name, name, UIManagerItemType.Menuitem, false);
			i++;
		}
	}
		*/
		/*
		string name = "ClearFile";
		string label = String.Format("Clear Recent File");
		Gtk.Action action = new Gtk.Action(name, label);
		action.Activated += delegate(object sender, EventArgs e) {  };
		dynGroup.Add(action);
		ui.AddUi(mergeId, "/menubar/File/RecentFile", name, name, UIManagerItemType.Menuitem, false);
		 */			
	
	public void SetDefaultMenu(bool state){
		//ActionUiManager.SetSensitive("propertyall",state);
		SetSensitive("FileGroup",state);
		SetSensitive("DirGroup",state);
		
		SetSensitive("adddirectory",state);		
		SetSensitive("newdirectory",state);
		//SetSensitive("addtheme",state);
		SetSensitive("addfile",state);
		SetSensitive("newfile",state);
	}

	public void SetEditMenu(bool state){
		SetSensitive("EditAction",state);

		SetSensitive("cutText",state);
		SetSensitive("copyText",state);
		SetSensitive("pasteText",state);
		SetSensitive("inserttemplate",state);
		SetSensitive("insertcomplete",state);
		SetSensitive("undo",state);
		SetSensitive("redo",state);
		SetSensitive("gotoline",state);

		/*MenuItem wgt = (MenuItem)ActionUiManager.GetWidget("/menubar/Edit");
		if(wgt != null){
			foreach ( Widget mi in wgt.Children){
				if(mi.GetType() == typeof(MenuItem)){
					mi.Visible = state;
				}
			}
		}*/
	}

	public Gtk.Action FindActionByName(string action_name)
	{
		foreach (ActionGroup @group in ui.ActionGroups)
			foreach (Gtk.Action action in @group.ListActions())
				if (action.Name == action_name)
					return action;
		
		return null;
	}

	public Gtk.Action this[string widget_path_or_action_name]
	{
		get {
			Gtk.Action action = FindActionByName(widget_path_or_action_name);
			if (action == null)
				return ui.GetAction(widget_path_or_action_name);
			
			return action;
		}
	}

	public Widget GetWidget(string widget_path)
	{
		return ui.GetWidget(widget_path);
	}

	public void SetActionLabel(string action_name, string label)
	{
		this[action_name].Label = label;
	}

	public void SetActionIcon(string action_name, string icon)
	{
		this[action_name].StockId = icon;
	}

	public void UpdateAction(string action_name, string label, string icon)
	{
		Gtk.Action action = this[action_name];
		action.Label = label;
		action.StockId = icon;
	}

	public void UpdateAccel(string action_name, string accel)
	{
		Gtk.Action action = this[action_name];
		if(action!= null){
			action.AccelPath = accel;
			Console.WriteLine(action.AccelPath);
		}
	}

	public void SetSensitive(string action_name, bool sensitive)
	{
		if( this.FindActionByName(action_name)!= null)
			this[action_name].Sensitive = sensitive;
	}

	public void SetVisible(string action_name, bool sensitive)
	{
		if( this.FindActionByName(action_name)!= null)
			this[action_name].Visible = sensitive;
	}

	public IEnumerator GetEnumerator()
	{
		foreach (ActionGroup @group in ui.ActionGroups)
			foreach (Gtk.Action action in @group.ListActions())
				yield return action;
	}

	public UIManager UI
	{
		get { return ui; }
	}

	public ActionGroup MainWindowActions
	{
		get { return main_window_actions; }
	}

	#region Default action
	static void OnActivate(object obj, EventArgs args)
	{

	}

	void OnTest(object obj, EventArgs args)
	{
			LicensesSystem ls = new LicensesSystem();

			foreach(License lc in ls.Licenses.Items){
				Console.WriteLine(lc.Name);
			}

			LicenceDialog ld = new LicenceDialog();
			if(ld.Run() == (int)ResponseType.Ok){
				
			}
			ld.Destroy();

		Console.WriteLine("MainClass.MainWindow.hpRight.Position SW 1 -:"+MainClass.MainWindow.hpRight.Position );
			//	throw new NotImplementedException();
		return;
		/*PublishDialogWizzard npw = new PublishDialogWizzard();
		int result = npw.Run();
		if (result == (int)ResponseType.Ok) {

		}
		npw.Destroy();*/
		PublishDialog pd = new PublishDialog();
		if(pd.Run() == (int)ResponseType.Ok){
			
		}
		pd.Destroy();
					
	}
	#endregion	
	}
	
}

