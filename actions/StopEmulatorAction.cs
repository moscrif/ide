using System;
using Gtk;
using System.Reflection;
using System.Diagnostics;

namespace  Moscrif.IDE.Actions
{
	public class StopEmulatorAction : Gtk.Action
	{
		public StopEmulatorAction(): base("stopemulator", MainClass.Languages.Translate("menu_stop_emulator"), MainClass.Languages.Translate("menu_title_stop_emulator"), null)
		{
		}

		protected override void OnActivated ()
		{
			base.OnActivated ();
			
			if(MainClass.Platform.IsMac){
				
				Process []pArry = Process.GetProcesses();
				foreach(Process p in pArry)
				{
					if(p != null){
						try {
							string s = p.ProcessName;
							s = s.ToLower();
							
							if (s.CompareTo("moscrif") ==0){
								Tool.Logger.LogDebugInfo("Kill Emulator Mac",null);
								p.Kill();
								MainClass.MainWindow.RunningEmulator= false;
							}
						} catch {//(Exception ex){
							//Console.WriteLine(ex.Message);
							//Tool.Logger.Error(ex.Message,null);
						}
					}
				}
			} else {
				
				if(MainClass.Platform.IsWindows){
					Process []pArry = Process.GetProcesses();
					foreach(Process p in pArry)
					{
						try{
							string s = p.ProcessName;
							s = s.ToLower();
							
							if (s.CompareTo("moscrif") ==0){
								
								
								Tool.Logger.LogDebugInfo("Kill Emulator win",null);
								p.Kill();
								MainClass.MainWindow.RunningEmulator= false;
							}
						} catch{//(Exception ex){
							//Console.WriteLine(ex.Message);
							//Tool.Logger.Error(ex.Message,null);
						}
					}
				}
			}
		}
	}
}

