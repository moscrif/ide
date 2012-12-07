using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;

namespace Moscrif.IDE.Iface
{
	public class Paths
	{
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

		public string ThemesDir
		{
			get { return System.IO.Path.Combine(ConfingDir, "themes"); }
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


		public string DefaultTheme
		{
			get { 
				string path = System.IO.Path.Combine(ThemesDir, "Moscrif"); 
				path = System.IO.Path.Combine(path, "gtk-2.0"); 
				path = System.IO.Path.Combine(path, "gtkrc"); 
				return path;
			}
		}

		private string workDir;
		public string WorkDir
		{
			get { 
				if (String.IsNullOrEmpty(workDir)){

					string userDir =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					
					if(userDir.Contains(" ")){
						if(MainClass.Platform.IsWindows){
							string root =System.IO.Path.GetPathRoot(userDir);

							userDir	= root+"MoscrifWorkspace";
							//userDir	="c:/MoscrifWorkspace/";
							Tool.Logger.LogDebugInfo("Default Workspace dir contains space. Create alternative in " +userDir);
							workDir = userDir;
							return workDir;
						}
					}

					workDir = System.IO.Path.Combine(userDir,"MoscrifWorkspace");

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
					string userDir =Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					settingDir = System.IO.Path.Combine(userDir,".Moscrif");

					if (!Directory.Exists(settingDir))
						Directory.CreateDirectory(settingDir);

				}
				return settingDir;
			}
		}

		private string bannerCache;
		public string BannerCache
		{
			get {
				if (String.IsNullOrEmpty(bannerCache)){
	
					bannerCache = System.IO.Path.Combine(SettingDir,".Banners");
					if (!Directory.Exists(bannerCache))
						 Directory.CreateDirectory(bannerCache);
	
				}
				return bannerCache;
			}
		}


		private string tempDir;
		public string TempDir
		{
			get {
				if (String.IsNullOrEmpty(tempDir)){

					string session = "Moscrif-"+DateTime.Now.ToString("yyyyMMddHHmmss");
					Tool.Logger.Log("session ->"+session);
					string tempDirectory ="";

					string userDir =System.IO.Path.GetTempPath(); //Environment.GetFolderPath(Environment.SpecialFolder.);
					if(userDir.Contains(" ")){
						if(MainClass.Platform.IsWindows){
							string root =System.IO.Path.GetPathRoot(userDir);

							userDir	=root+"Temp";// "c:/Temp/Moscrif/";
							userDir	= System.IO.Path.Combine(userDir,"Moscrif");
							Tool.Logger.LogDebugInfo("Temp dir contains space. Create alternative temp in " +userDir);
						}
					}


					tempDirectory = System.IO.Path.Combine(userDir, session);

					try {
						if (!Directory.Exists(tempDirectory)){
							Directory.CreateDirectory(tempDirectory);
							Tool.Logger.LogDebugInfo("Create Temp Dir ->"+tempDirectory);
						}
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

