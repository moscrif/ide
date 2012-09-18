using System;
using Gtk;
using System.IO;
using System.Timers;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Tool;
using Moscrif.IDE.Task;
using Moscrif.IDE.Execution;
using GLib;
using Moscrif.IDE.Settings;
using Moscrif.IDE.Iface;
using Moscrif.IDE.Iface.Entities;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Mono.Unix;
using System.Threading;

namespace Moscrif.IDE
{
	class MainClass
	{

		public static void Main(string[] args)
		{
			//foreach(string str in args)
			//	Logger.Log("arg ->{0}",str);

			Logger.Log(Languages.Translate("start_app"));
			Application.Init();
			//Logger.Log("1");
			ExceptionManager.UnhandledException += delegate(UnhandledExceptionArgs argsum)
			{
				Exception ex = (Exception)argsum.ExceptionObject;
				
				Logger.Error(ex.Message);
				Logger.Error(ex.StackTrace);
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				
				if (ex.InnerException != null) {
					Logger.Error(ex.InnerException.Message);
					Logger.Error(ex.InnerException.StackTrace);
					Logger.Error(ex.InnerException.Source);
					Console.WriteLine(ex.InnerException.Message);
					Console.WriteLine(ex.InnerException.StackTrace);
					Console.WriteLine(ex.InnerException.Source);
				}
				argsum.ExitApplication = true;
			};
			Gdk.Global.InitCheck(ref args);

			//SplashScreenForm splash = new SplashScreenForm();
			//splash.Show();
			mainWindow = new MainWindow(args);

			MainWindow.Show();

			//MainWindow win = MainWindow;
			//if ((MainClass.Settings == null) ||(MainClass.Settings.BackgroundColor == null)){
			/*if(!File.Exists(MainClass.Settings.FilePath)){
				SettingPlatformDialog spd = new SettingPlatformDialog(true);
				 spd.Run();
				spd.Destroy();
			}*/
			//splash.Destroy();

			//win.Show();

			//splash.Hide();
			if (!String.IsNullOrEmpty(Paths.TempDir))
			{
				Application.Run();
			}

			Logger.Log("END Application");
		}

		static ProcessService processService = null;
		static internal ProcessService ProcessService
		{
			get {
				if (processService != null)
					return processService;
				processService = new ProcessService();
				return processService;
			}
		}


		static TaskServices taskServices = null;
		static internal TaskServices TaskServices
		{
			get {
				if (taskServices != null)
					return taskServices;
				taskServices = new TaskServices();
				return taskServices;
			}
		}


		/*	private static void LogImpl(Exception e)
		{
			//if (disabled) return text;
			string fileFullFileName = System.IO.Path.Combine(Tools.AppPath, "moscrift.ide.log");
			try {
				using (StreamWriter stream = File.AppendText(fileFullFileName)) {
					stream.WriteLine("+=======================================================================================================================");
					stream.WriteLine(DateTime.Now.ToString("dd.MM.yyyy hh:mm.ss"));
					stream.WriteLine("+-----------------------------------------------------------------------------------------------------------------------");
					stream.WriteLine("Message:");
					stream.WriteLine(e.Message);
					stream.WriteLine("Source:");
					stream.WriteLine(e.Source);
					stream.WriteLine("InnerException:");
					stream.WriteLine(e.InnerException);
					stream.WriteLine("StackTrace:");
					stream.WriteLine(e.StackTrace);
					//string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
					//foreach (string line in lines)
					//    stream.WriteLine(String.Format("| {0}", line));
					stream.WriteLine("+=======================================================================================================================");
					stream.WriteLine();
					stream.Flush();
					stream.Close();
					stream.Dispose();
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
		 */
		//static MainWindow mainWindow = null;
		static MainWindow mainWindow = null;
		static internal MainWindow MainWindow
		{
			get {
				/*if (mainWindow != null)
					return mainWindow;
				mainWindow = new MainWindow();*/
				return mainWindow;
			}
		}

		static Tools tools = null;
		static internal Tools Tools
		{
			get {
				if (tools != null)
					return tools;
				tools = new Tools();
				return tools;
			}
		}

		static Paths paths = null;
		static internal Paths Paths
		{
			get {
				if (paths != null)
					return paths;
				paths = new Paths();
				return paths;
			}
		}

		static Platform platform = null;
		static internal Platform Platform
		{
			get {
				if (platform != null)
					return platform;
				platform = new Platform();
				return platform;
			}
		}

		static Languages languages = null;
		static internal Languages Languages
		{
			get {
				if (languages != null)
					return languages;
				
				string file = System.IO.Path.Combine(Paths.LanguageDir, "en.lng");
				languages = new Languages(file);

				return languages;
			}
		}

		static Moscrif.IDE.Settings.Settings settings = null;
		static internal Moscrif.IDE.Settings.Settings Settings
		{
			get {
				if (settings != null)
					return settings;
				
				string file = System.IO.Path.Combine(Paths.SettingDir, "moscrif.mss");
				
				if (File.Exists(file)) {
					try{
						settings = Moscrif.IDE.Settings.Settings.OpenSettings(file);
					}catch{//(Exception ex){
						MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", Languages.Translate("setting_file_corrupted"), Gtk.MessageType.Error,null);
						ms.ShowDialog();

						settings = new Moscrif.IDE.Settings.Settings(file);
					}

					settings.FilePath = file;
				}
				if (settings == null)
					settings = new Moscrif.IDE.Settings.Settings(file);
				return settings;
			}
		}

		static Workspace.Workspace workspace = null;
		static internal Workspace.Workspace Workspace
		{
			get {
				if (workspace != null)
					return workspace;
				Logger.Log("Workspace.Workspace ->"+Settings.CurrentWorkspace);
				workspace = new Workspace.Workspace();
				
				//string file =  System.IO.Path.Combine(Tools.ConfingDir, "workspace.msw");
				if (String.IsNullOrEmpty(Settings.CurrentWorkspace))
					return workspace;
				//null;//new Workspace.Workspace();
				string file = Settings.CurrentWorkspace;
				
				if (File.Exists(file)) {
					workspace = Moscrif.IDE.Workspace.Workspace.OpenWorkspace(file);
					if (workspace == null){
						workspace  = new Workspace.Workspace();
						return workspace;
					}
					
					workspace.FilePath = file;
				}
				//null;
				//workspace = new Workspace.Workspace();
				return workspace;
			}
			set {
				workspace = value;
				if (workspace != null)
					Settings.CurrentWorkspace = workspace.FilePath;
				else
					Settings.CurrentWorkspace = String.Empty;
			}
		}

		//static KeyBindings keyBinding = null;
		static internal KeyBindings KeyBinding{
			get{
				KeyBindings keyBinding = null;
				if(keyBinding == null){
					string file = System.IO.Path.Combine(MainClass.Paths.SettingDir, "keybinding");
					try{
						if(!File.Exists(file)){
							KeyBindings.CreateKeyBindingsWin(file);
						}

						keyBinding = KeyBindings.OpenKeyBindings(file);
					} catch(Exception ex){
						Logger.Error(ex.Message);
						return new KeyBindings(file);
					}
				} return keyBinding;
			}
		}

		static CompletedCache completedCache = null;
		static internal CompletedCache CompletedCache{
			get{
				if(completedCache == null){
					string file = System.IO.Path.Combine(MainClass.Paths.ConfingDir, ".completedcache");
					try{
						completedCache = CompletedCache.OpenCompletedCache(file);
						completedCache.GetCompletedData();
					} catch(Exception ex){
						Logger.Error(ex.Message);
						return new CompletedCache();
					}
				}
				return completedCache;
			}
		}

		static Account account = null;
		static internal Account User{
			get{
				return account;
			}
			set{
				account = value;

				if ((account == null) || (account.Remember)){
					MainClass.Settings.Account = account;
				}
			}
		}
	}
}

