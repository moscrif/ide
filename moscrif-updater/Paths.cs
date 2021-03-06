using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;

namespace Moscrif.Updater
{
	public class Paths
	{
		private static Platform platform = new Platform();
		
		private string appPath = System.AppDomain.CurrentDomain.BaseDirectory;

		public string AppPath
		{
			get { return appPath; }
		}	
		
		public string ConfingDir
		{
			get { return System.IO.Path.Combine(appPath, "cfg"); }
		}

		public string StylesDir
		{
			get { return System.IO.Path.Combine(ConfingDir, "styles"); }
		}

		public string LanguageDir
		{
			get { return System.IO.Path.Combine(ConfingDir, "languages"); }
		}		
		
		public string ResDir
		{
			get { return System.IO.Path.Combine(appPath, "resources"); }
		}

		public string SampleDir
		{
			get { return System.IO.Path.Combine(appPath, "samples"); }
		}
				
		public string Temp
		{
			get { return System.IO.Path.Combine(appPath, "temp"); }
		}

		private string workDir;
		public string WorkDir
		{
			get { 
				if (String.IsNullOrEmpty(workDir)){
					//if (platform.IsWindows){
					//	workDir = System.IO.Path.Combine(appPath,"Work");
					//} else {
						string userDir =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						workDir = System.IO.Path.Combine(userDir,"MoscrifWorkspace");
					//}
				}
				return workDir;
			}
		}


	/*	private string homeDir;
		public string HomeDir
		{
			get {
				if (String.IsNullOrEmpty(homeDir)){
					//string userDir = Environment.GetEnvironmentVariable("HOME");
					string userDir =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					homeDir = System.IO.Path.Combine(userDir,"Moscrif");

					if (!Directory.Exists(homeDir))
						Directory.CreateDirectory(homeDir);

				}
				//Console.WriteLine("workDir 1-> {0}", workDir);
				return homeDir;
			}
		}*/


		private string settingDir;
		public string SettingDir
		{
			get {
				if (String.IsNullOrEmpty(settingDir)){
					//if (platform.IsWindows){
					//	settingDir = ConfingDir;
					//} else {
						string userDir =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						settingDir = System.IO.Path.Combine(userDir,".Moscrif");

					if (!Directory.Exists(settingDir))
						Directory.CreateDirectory(settingDir);
					//}
				}
				return settingDir;
			}
		}


		private string tempDir;
		public string TempDir
		{
			get {
				if (String.IsNullOrEmpty(tempDir)){

					string session = "Moscrif-"+DateTime.Now.ToString("yyyyMMddHHmmss");
					string tempDirectory ="";

					string userDir =System.IO.Path.GetTempPath(); //Environment.GetFolderPath(Environment.SpecialFolder.);
					tempDirectory = System.IO.Path.Combine(userDir, session);

					try {
						if (!Directory.Exists(tempDirectory))
							Directory.CreateDirectory(tempDirectory);
							tempDir =tempDirectory;
					} catch{
						tempDir = null;
					}
				}
				
				return tempDir;
			}
		}

		public string TempOutputDir
		{
			get { return System.IO.Path.Combine(Temp, "output"); }
		}

		public string TempPrecompileDir
		{
			get { return System.IO.Path.Combine(TempDir, "precompile"); }
		}

		public string TempPublishDir
		{
			get { return System.IO.Path.Combine(TempDir, "publish"); }
		}

		public string TemplateDir {
			get {
				return System.IO.Path.Combine (ConfingDir, "templates");
			}
		}
		
		public string FileTemplateDir {
			get {
				return System.IO.Path.Combine (ConfingDir, "filetemplates");
			}
		}		

		public string DisplayDir
		{
			get { return System.IO.Path.Combine(ConfingDir, "displays"); }
		}

	}
	
	
}

