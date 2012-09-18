using System;
using System.IO;
using Gtk;
//using Moscrif.Iface;
using Moscrif.Updater.Dialogs;
using MessageDialogs = Moscrif.Updater.Dialogs.MessageDialog;

namespace Moscrif.Updater
{
	class MainClass
	{
		public static void Main(string[] args) {
			Application.Init();

			/*for (int i = 0; i < args.Length; i++){
				Console.WriteLine(args[i]);
			}*/
			
			for (int i = 0; i < args.Length; i++){
				string arg = args[i];
				if (arg.StartsWith("-t:")){
					if (arg == "-t:autom")
						Auto = true;
					else  Auto = false;
				}
				if (arg.StartsWith("-u:")){
					arg = arg.Remove(0,3);
					Token = arg;
				}			
			
			}
			
			MainWindow win = new MainWindow();
			win.Show();
			Application.Run();
		}
			
		static internal bool Auto;
		static internal string Token;
		
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
		
/*		static Moscrif.Iface.Settings settings = null;
		static internal Moscrif.Iface.Settings Settings
		{
			get {
				if (settings != null)
					return settings;
				
				string file = System.IO.Path.Combine(Paths.ConfingDir, "moscrif.mss");
				
				if (File.Exists(file)) {
					try{
						settings = Moscrif.Iface.Settings.OpenSettings(file);
					}catch(Exception ex){
						MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", "Settings file is corrupted!", Gtk.MessageType.Error,null);
						ms.ShowDialog();
						settings = new Moscrif.Iface.Settings();
					}

					settings.FilePath = file;
				}
				if (settings == null)
					settings = new Moscrif.Iface.Settings(file);
				return settings;
			}
		}*/
		
		
	}
}
