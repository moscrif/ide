using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Iface;

namespace  Moscrif.IDE.Option
{
	public class Settings
	{
		private static Paths paths = new Paths();
		private List<string> projectMaskDirectory;
		private List<string> workspaceMaskDirectory;

		#region Public Property

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}

		[XmlAttribute("currentWorkspace")]
		public string CurrentWorkspace ;

		[XmlAttribute("precompile")]
		public bool PreCompile = true;

		[XmlAttribute("autoselectProject")]
		public bool AutoSelectProject = true;

		[XmlAttribute("openLastOpenedWorkspace")]
		public bool OpenLastOpenedWorkspace = true;		

		[XmlAttribute("rssUrl")]
		public string RssUrl = "http://moscrif.com/rss/news";//"http://moscrif.com/rss/news.ashx?source=IDE";

		[XmlAttribute("tweetUrl")]
		public string TweetUrl = "http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=mothiva";

		[XmlAttribute("tweetBaseUrl")]
		public string TweetBaseUrl = "http://twitter.com/mothiva/statuses/" ;
		
		[XmlAttribute("maxRssTweetMessageCount")]
		public int MaxRssTweetMessageCount =3;	
		
		[XmlAttribute("stopCompilationOnError")]
		public bool FirstErrorStopCompile = false;

		[XmlAttribute("bringErrorPaneToFront")]
		public bool ShowErrorPane = true;

		//[XmlAttribute("loggAllStep")]
		[XmlIgnore]
		public bool LoggAllStep = true;

		[XmlAttribute("publishDir")]
		public string PublishDirectory = System.IO.Path.Combine(paths.AppPath,"publish");//@"\work\moscrif-publish\";

		[XmlAttribute("frameworkDir")]
		public string LibDirectory = System.IO.Path.Combine(paths.AppPath,"framework");//@"\work\moscrif-lib\";

		[XmlAttribute("emulatorDir")]
		public string EmulatorDirectory = paths.AppPath;//@"\work\moscrif-publish\";

		[XmlAttribute("socetServerPort")]
		public string SocetServerPort = "4321";

		[XmlAttribute("projectCount")]
		public int ProjectCount = 1;

		[XmlAttribute("workspaceCount")]
		public int WorkspaceCount = 1;

		[XmlElement("remoteIpAdress")]
		public string RemoteIpAdress ;

		[XmlElement("samplesBaseUrl")]
		public string SamplesBaseUrl = "http://moscrif.com/samples?source=IDE" ;

		[XmlElement("showcaseBaseUrl")]
		public string ShowcaseBaseUrl = "http://moscrif.com/showcase?source=IDE" ;		

		[XmlElement("apiBaseUrl")]
		public string ApiBaseUrl = "http://moscrif.com/userfiles/pages/developer/api/index.html" ;	

		[XmlElement("videosBaseUrl")]
		public string VideosBaseUrl = "http://moscrif.com/videos?source=IDE" ;

		[XmlElement("tutorialsBaseUrl")]
		public string TutorialsBaseUrl = "http://moscrif.com/tutorials?source=IDE" ;

		[XmlElement("demosBaseUrl")]
		public string DemosBaseUrl = "http://moscrif.com/demos?source=IDE" ;

		[XmlElement("documentsBaseUrl")]
		public string DocumentsBaseUrl = "http://moscrif.com/documents?source=IDE" ;

/*#if DEBUG 
		[XmlIgnore]
		public String signUrl = "http://rc.moscrif.com/ide/signApp.ashx?t={0}&a={1}";

		[XmlIgnore]
		public String checkVersion = "http://rc.moscrif.com/ide/signApp.ashx?v={0}";

		[XmlIgnore]
		public String checkVersionLog = "http://rc.moscrif.com/ide/signApp.ashx?v={0}&t={1}";

		[XmlIgnore]
		public String getVersion = "http://rc.moscrif.com/ide/getVersion.ashx?v={0}";

		[XmlIgnore]
		public String getVersionLog = "http://rc.moscrif.com/ide/getVersion.ashx?v={0}&t={1}";

		[XmlIgnore]
		public String redgisterUrl = "http://rc.moscrif.com/ide/registerLogin.ashx";

		[XmlIgnore]
		public String pingUrl = "http://rc.moscrif.com/ide/ping.ashx";

		[XmlIgnore]
		public String loggUrl = "http://rc.moscrif.com/ide/analytics.ashx";

		[XmlIgnore]
		public String loginUrl = "http://rc.moscrif.com/ide/checkLogin.ashx"; 

		[XmlIgnore]
		public String feedbackUrl = "http://rc.moscrif.com/ide/reportIssue.ashx"; 

		[XmlIgnore]
		public String bannerUrl = "http://rc.moscrif.com/ide/getBanner.ashx"; 

		[XmlIgnore]
		public String licenceUrl = "http://rc.moscrif.com/ide/getLicenses.ashx"; 

*/	
//#else 
		[XmlIgnore]
		public String signUrl = "http://moscrif.com/ide/signApp.ashx?t={0}&a={1}";
		
		[XmlIgnore]
		public String checkVersion = "http://moscrif.com/ide/signApp.ashx?v={0}";
		
		[XmlIgnore]
		public String checkVersionLog = "http://moscrif.com/ide/signApp.ashx?v={0}&t={1}";
		
		[XmlIgnore]
		public String getVersion = "http://moscrif.com/ide/getVersion.ashx?v={0}";
		
		[XmlIgnore]
		public String getVersionLog = "http://moscrif.com/ide/getVersion.ashx?v={0}&t={1}";
		
		[XmlIgnore]
		public String redgisterUrl = "http://moscrif.com/ide/registerLogin.ashx";
		
		[XmlIgnore]
		public String pingUrl = "http://moscrif.com/ide/ping.ashx";

		[XmlIgnore]
		public String loggUrl = "http://moscrif.com/ide/analytics.ashx";

		[XmlIgnore]
		public String loginUrl = "http://moscrif.com/ide/checkLogin.ashx"; 

		[XmlIgnore]
		public String feedbackUrl = "http://moscrif.com/ide/reportIssue.ashx"; 

		[XmlIgnore]
		public String bannerUrl = "http://moscrif.com/ide/getBanner.ashx"; 

		[XmlIgnore]
		public String licenceUrl = "http://moscrif.com/ide/getLicenses.ashx"; 
//#endif

		[XmlElement("backgroundColor")]
		public BackgroundColors BackgroundColor = null;

		[XmlElement("imageEditor")]
		public ImageEditorSetting ImageEditors = null;

		[XmlElement("proxy")]
		public ProxySetting Proxy = null;

		[XmlElement ("sourceEditor")]
		public SourceEditorSetting SourceEditorSettings = null;

		[XmlArrayAttribute("ignoresFolders")]
		[XmlArrayItem("folder")]
		public List<IgnoreFolder> IgnoresFolders;
		//public List<string> IgnoreFolders;

		[XmlArrayAttribute("ignoresFiles")]
		[XmlArrayItem("file")]
		public List<IgnoreFolder> IgnoresFiles;

		[XmlArray("logicalViews")]
		[XmlArrayItem("view")]
		public List<LogicalSystem> LogicalSort = null;

		[XmlArrayAttribute("extensionList")]
		[XmlArrayItem("extension")]
		public List<ExtensionSetting> ExtensionList;

		[XmlElement("resolution")]
		public Condition Resolution;

		[XmlElement("account")]
		public Account Account;	

		[XmlElement("emulatorSettings")]
		public EmulatorSetting EmulatorSettings;

		[XmlAttribute("lastOpenedFileDir")]
		public string LastOpenedFileDir ;
		
		[XmlAttribute("lastOpenedWorkspaceDir")]
		public string LastOpenedWorkspaceDir ;
		
		[XmlAttribute("lastOpenedImportDir")]
		public string LastOpenedImportDir ;

		[XmlAttribute("lastOpenedExportDir")]
		public string LastOpenedExportDir ;
		
		[XmlAttribute("clearConsoleBeforeRun")]
		public bool ClearConsoleBeforRuning = true;
		
		[XmlAttribute("openOutputAfterPublish")]
		public bool OpenOutputAfterPublish= true;

		[XmlAttribute("consoleTaskFont")]
		public string ConsoleTaskFont ;

		[XmlAttribute("saveChangesBeforeRun")]
		public bool SaveChangesBeforeRun= true;

		[XmlAttribute("showUnsupportedDevices")]
		public bool ShowUnsupportedDevices= false;

		[XmlAttribute("showDebugDevices")]
		public bool ShowDebugDevices= false;

		[XmlArrayAttribute("displayOrientations")]
		[XmlArrayItem("orientation")]
		public List<SettingValue> DisplayOrientations;

		[XmlArrayAttribute("applicationType")]
		[XmlArrayItem("type")]
		public List<SettingValue> ApplicationType;
		//GenerateApplicationType

		[XmlArrayAttribute("InstallLocations")]
		[XmlArrayItem("location")]
		public List<SettingValue> InstallLocations;

		[XmlArrayAttribute("osSupportedDevices")]
		[XmlArrayItem("supportedDevices")]
		public List<SettingValue> OSSupportedDevices;

		[XmlArrayAttribute("androidSupportedDevices")]
		[XmlArrayItem("supportedDevices")]
		public List<SettingValue> AndroidSupportedDevices;

		[XmlArray("platformResolutions")]
		[XmlArrayItem("platform")]
		public List<PlatformResolution> PlatformResolutions = null;

		[XmlAttribute("javaCommand")]
		public string JavaCommand = "java";

		/*[XmlAttribute("signAllow")]
		public bool SignAllow = false;*/

		[XmlAttribute("javaArgument")]
		public string JavaArgument = "-version";

		[XmlAttribute("versionSetting")]
		public int VersionSetting ;

		[XmlAttribute("logPublish")]
		public bool LogPublish = false;

		[XmlIgnore]
		public List<string> WorkspaceMaskDirectory
		{
			get{
				if(workspaceMaskDirectory == null){
					workspaceMaskDirectory = new List<string>();
					workspaceMaskDirectory.Add("$(workspace_dir)");
					//workspaceMaskDirectory.Add("$(workspace_work)");
				}
				return workspaceMaskDirectory;
			}
		}
		
		[XmlIgnore]
		public List<string> ProjectMaskDirectory
		{
			get{
				if(projectMaskDirectory == null){
					projectMaskDirectory = new List<string>();
					projectMaskDirectory.Add("$(publish_dir)");
					projectMaskDirectory.Add("$(project_dir)");
				}
				return projectMaskDirectory;
			}
		}		
		
		[XmlIgnore]
		public string ThemeDir = ".themes";

		[XmlIgnore]
		public string SkinDir = "uix-skin";


		[XmlIgnore]
		public Condition Platform;

		//private List<string> libsDefine;
		//[XmlArrayAttribute("libs")]
		//[XmlArrayItem("lib")]
		[XmlIgnore]
		public List<string> LibsDefine{
			get{
				return GenerateLibs();
			}
		}
		
		[XmlElement("symbian-unprotectedRange")]
		public Range UnprotectedRange;
		
		[XmlElement("symbian-protectedRange")]
		public Range ProtectedRange;

		public List<string> GetUnprotectedRange()
		{
			if (UnprotectedRange == null){
				UnprotectedRange = new Range();
				UnprotectedRange.Minimal = "0xA89FDE0E";
				UnprotectedRange.Maximal = "0xA89FDE21";
			}
			
			uint min =0xA89FDE0E;
			uint max =0xA89FDE21;
			
			 if (!UInt32.TryParse(UnprotectedRange.Minimal, out min))
				min = 0xA89FDE0E;
			 if (!UInt32.TryParse(UnprotectedRange.Maximal, out max))
				max = 0xA89FDE21;			
			
			if(min> max){
				min = 0xA89FDE0E;
				max = 0xA89FDE21;
			}
			
			List<string> unprRange = new List<string>();
			for (uint i =min ; i <= max  ;i++ ){				
				unprRange.Add(string.Format( "0x"+"{0:X}", i));
			}
			return unprRange;
		}
	 	 
		public List<string> GetProtectedRange()
		{
			if (ProtectedRange == null){
				ProtectedRange = new Range();
				ProtectedRange.Minimal = "0x20041437";
				ProtectedRange.Maximal = "0x2004144A";
			}
			
			uint min =0xA89FDE0E;
			uint max =0xA89FDE21;
			
			 if (!UInt32.TryParse(ProtectedRange.Minimal, out min))
				min = 0x20041437;
			 if (!UInt32.TryParse(ProtectedRange.Maximal, out max))
				max = 0x2004144A;			
			if(min> max){
				min = 0x20041437;
				max = 0x2004144A;
			}
			
			List<string> unprRange = new List<string>();
			for (uint i = min ; i <=max  ;i++ ){				
				unprRange.Add(string.Format( "0x"+"{0:X}", i));
			}
			return unprRange;
		}				
		
		#endregion

		#region RecentFiles

		[XmlIgnore]
		public RecentFiles RecentFiles
		{
			get { return recentFiles ?? (recentFiles = CreateRecentFilesProvider()); }
		}		
		
		protected virtual RecentFiles CreateRecentFilesProvider()
		{

			return new FdoRecentFiles(System.IO.Path.Combine(paths.SettingDir, ".recently-used"));
		}

		RecentFiles recentFiles;
		#endregion

		#region FavoriteFile

		[XmlIgnore]
		public RecentFiles FavoriteFiles
		{
			get { return favoriteFiles ?? (favoriteFiles = CreateFavoriteFilesProvider()); }
		}		
		
		protected virtual RecentFiles CreateFavoriteFilesProvider()
		{

			return new FdoRecentFiles(System.IO.Path.Combine(paths.SettingDir, ".favorite-files"));
		}

		RecentFiles favoriteFiles;
		#endregion
	
		
		public Settings()
		{
			FilePath = System.IO.Path.Combine(paths.SettingDir, "moscrif.mss");
			if (Platform == null) GeneratePlatform();
			if (Resolution == null) GenerateResolution();
		}

		public Settings(string filePath)
		{
			FilePath = filePath;
			if (Platform == null) GeneratePlatform();
			if (Resolution == null) GenerateResolution();
			if(Resolution.Rules[0].Width<0){
				GenerateResolution();
			}

			if (IgnoresFolders == null) GenerateIgnoreFolder();
			if (IgnoresFiles == null) GenerateIgnoreFiles();
			if (ExtensionList == null) GenerateExtensionList();
			if (DisplayOrientations == null) GenerateOrientations();
			if (InstallLocations == null) GenerateInstallLocation();
			if (OSSupportedDevices == null) GenerateOSSupportedDevices();
			if (PlatformResolutions == null) GeneratePlatformResolutions();
			if (ApplicationType == null) GenerateApplicationType();
			if (AndroidSupportedDevices == null) GenerateAndroidSupportedDevices();
			if ((EmulatorSettings == null)) EmulatorSettings = new EmulatorSetting();
			EmulatorSettings.UsDefault = true;
			//if(LibsDefine == null)) GenerateLibs();

			//VersionSetting = 111202;

		}


		public void SaveSettings()
		{
			try{
				using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
					XmlSerializer serializer = new XmlSerializer(typeof(Settings));
					serializer.Serialize(fs, this);
				}
			} catch(Exception ex){
				Moscrif.IDE.Tool.Logger.Error(ex.Message);
			}
		}


		static public Settings OpenSettings(string filePath)
		{
			if (System.IO.File.Exists(filePath)) {
				
				try {
					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(Settings));
						Settings s = (Settings)serializer.Deserialize(fs);


						if ((s.EmulatorSettings == null) || ( String.IsNullOrEmpty(s.EmulatorSettings.Exec) ) ) {
							s.EmulatorSettings = new EmulatorSetting();
							s.EmulatorSettings.UsDefault = true;
						}

						if ((s.LogicalSort == null) || ((s.LogicalSort.Count <1 )) ) {
							s.LogicalSort = LogicalSystem.GetDefaultLogicalSystem();
						}

						if ((s.Resolution == null) || ((s.Resolution.Rules.Count <1 )) ){
							s.GenerateResolution();
						} else {
							if(s.Resolution.Rules[1].Width<1){
								s.GenerateResolution();
							}
						}
						
						if ((s.IgnoresFolders == null) || (s.IgnoresFolders.Count<1)){
							s.GenerateIgnoreFolder();
						}
						if((s.ExtensionList == null)||(s.ExtensionList.Count<1)){
							s.GenerateExtensionList();
						}

						if ((s.IgnoresFiles == null) || (s.IgnoresFiles.Count<1)){
							s.GenerateIgnoreFiles();
						}

						if ((s.Platform == null) || (s.Platform.Rules.Count < 1)){
							s.GeneratePlatform();
						}

						if ((s.DisplayOrientations == null) || ((s.DisplayOrientations.Count <1 )) ){
							s.GenerateOrientations();
						}

						if ((s.InstallLocations == null) || ((s.InstallLocations.Count <1 )) ){
							s.GenerateInstallLocation();
						}

						if ((s.OSSupportedDevices == null) || ((s.OSSupportedDevices.Count <1 )) ){
							s.GenerateOSSupportedDevices();
						}

						if ((s.PlatformResolutions == null) || ((s.PlatformResolutions.Count <1 )) ){
							s.GeneratePlatformResolutions();
						}

						if ((s.ApplicationType == null) || ((s.ApplicationType.Count <1 )) ){
							s.GenerateApplicationType();
						}

						if ((s.AndroidSupportedDevices == null) || ((s.AndroidSupportedDevices.Count <1 )) ){
							s.GenerateAndroidSupportedDevices();
						}

						if (s.VersionSetting < 111202){ //year, month, day
							s.GenerateIgnoreFolder();
							s.VersionSetting = 111202;
						}
						if (s.VersionSetting < 120314){ //year, month, day
							s.GenerateResolution();
							s.GeneratePlatformResolutions();
							s.VersionSetting = 120314;

						}
						if (s.VersionSetting < 120828){ //year, month, day
							s.TweetUrl = "http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=moscrif";
							s.TweetBaseUrl = "http://twitter.com/moscrif/statuses/" ;
							s.VersionSetting = 120828;
						}
						if (s.VersionSetting < 120903){ //year, month, day
							s.GeneratePlatformResolutions();
							s.VersionSetting = 120903;
						}
						if (s.VersionSetting < 121001){ //year, month, day
							s.ShowUnsupportedDevices = false;
							s.VersionSetting = 121001;
						}

						if (s.VersionSetting < 121017){ //year, month, day
							s.GenerateResolution();
							s.GeneratePlatformResolutions();
							s.VersionSetting = 121017;
						}
						if (s.VersionSetting < 121106){ //year, month, day
							s.MaxRssTweetMessageCount = 3;
							s.VersionSetting = 121106;
						}
						return s;
					}
				} catch (Exception ex) {
					
					throw ex;
				}
			} else {
				throw new Exception("Settings file does not exist!");
			}
		}

		public class EmulatorSetting
		{
			public EmulatorSetting(){
			}
			
			[XmlAttribute("usDefault")]
			public bool UsDefault ;

			[XmlAttribute("params")]
			public string Params = "" ;
			
			[XmlAttribute("exec")]
			public string Exec = "" ;
		}

		public class BackgroundColors
		{
			public BackgroundColors(){}

			public BackgroundColors(byte red,byte green,byte blue){
				Red= red;
				Green=green;
				Blue=blue;
			}

			public BackgroundColors(byte red,byte green,byte blue,ushort alpha){
				Red= red;
				Green=green;
				Blue=blue;
				Alpha = alpha;
			}

			[XmlAttribute("red")]
			public byte Red;
			[XmlAttribute("green")]
			public byte Green;
			[XmlAttribute("blue")]
			public byte Blue;

			[XmlAttribute("alpha")]
			public ushort Alpha;
		}

		public class SourceEditorSetting
		{
			public SourceEditorSetting(){

				if( MainClass.Platform.IsMac){
					EditorFont= "Monaco 12";//"Monospace 15";
				}
			}

			[XmlAttribute("sourceEditorFont")]
			public string EditorFont = "Monospace 10";

			[XmlAttribute("sourceEditorTabSpace")]
			public int TabSpace = 4;

			[XmlAttribute("sourceEditorTabsToSpaces")]
			public bool TabsToSpaces = true;

			[XmlAttribute("sourceEditorShowEolMarker")]
			public bool ShowEolMarker = false;

			[XmlAttribute("sourceEditorShowTab")]
			public bool ShowTab = false;

			[XmlAttribute("sourceEditorSpaces")]
			public bool ShowSpaces = false;

			[XmlAttribute("sourceEditorRulerColumn")]
			public int RulerColumn = 85;

			[XmlAttribute("sourceEditorShowRuler")]
			public bool ShowRuler = false;

			[XmlAttribute("sourceEditorShowLineNumber")]
			public bool ShowLineNumber = true;

			[XmlAttribute("sourceEditorEnableAnimations")]
			public bool EnableAnimations = true;

			[XmlAttribute("sourceEditorAggressivelyCompletion")]
			public bool AggressivelyTriggerCL = true;
		}

		public class ImageEditorSetting
		{
			[XmlAttribute("line")]
			public int LineWidth;

			[XmlAttribute("point")]
			public int PointWidth;

			[XmlElement("lineColor")]
			public BackgroundColors LineColor;

			[XmlElement("pointColor")]
			public BackgroundColors PointColor;

			[XmlElement("selectPointColor")]
			public BackgroundColors SelectPointColor;
		}

		public class Range
		{
			[XmlAttribute("minimal")]
			public string Minimal;
			[XmlAttribute("maximal")]
			public string Maximal;
		}		

		private void GeneratePlatform(){
			Platform = new Condition();
			Platform.Id = -1;
			Platform.Name = "Platform";
			Platform.System = true;

			Platform.Rules = new List<Rule> ();

			Platform.Rules.Add(new Rule((int)DeviceType.Android_1_6,"Android 1.6 +","android_1_6",0));
			Platform.Rules.Add(new Rule((int)DeviceType.Android_2_2,"Android 2.2 +","android_2_2",0));
			Platform.Rules.Add(new Rule((int)DeviceType.iOS_5_0,"iOS","ios_5_0",0));
			Platform.Rules.Add(new Rule((int)DeviceType.Bada_1_0,"Bada 1.0","bada_1_0",0));
			Platform.Rules.Add(new Rule((int)DeviceType.Bada_1_1,"Bada 1.1","bada_1_1",0));
			Platform.Rules.Add(new Rule((int)DeviceType.Symbian_9_4,"Symbian","symbian_9_4",-1));
			Platform.Rules.Add(new Rule((int)DeviceType.WindowsMobile_6,"Windows Mobile","wm_6",-1));

			Platform.Rules.Add(new Rule((int)DeviceType.Bada_2_0,"Bada 2.0","bada_2_0",-2));
			Platform.Rules.Add(new Rule((int)DeviceType.Windows,"Windows","windows",-2));
			Platform.Rules.Add(new Rule((int)DeviceType.MacOs,"MacOs","mac",-2));

			//Platform.Rules.Add(new Rule((int)DeviceType.webOs,"webOS","webOS"));
			//Platform.Rules.Add(new Rule((int)DeviceType.MeeGo,"MeeGo","MeeGo"));
		}

		public void GeneratePlatformResolutions(){
			PlatformResolutions = new List<PlatformResolution>();

			PlatformResolution prAndr16 = new PlatformResolution((int)DeviceType.Android_1_6);
			prAndr16.AllowResolution = new List<int>();
			prAndr16.AllowResolution.Add(-1);
			//prAndr16.AllowResolution.Add(-2);
			prAndr16.AllowResolution.Add(-3);
			//prAndr16.AllowResolution.Add(-4);
			prAndr16.AllowResolution.Add(-5);
			prAndr16.AllowResolution.Add(-6);
			prAndr16.AllowResolution.Add(-7);
			prAndr16.AllowResolution.Add(-8);
			prAndr16.AllowResolution.Add(-9);
			prAndr16.AllowResolution.Add(-10);
			//prAndr16.AllowResolution.Add(-11);
			prAndr16.AllowResolution.Add(-12);
			prAndr16.AllowResolution.Add(-13);
			prAndr16.AllowResolution.Add(-14);
			prAndr16.AllowResolution.Add(-15);
			prAndr16.AllowResolution.Add(-16);
			PlatformResolutions.Add(prAndr16);

			PlatformResolution prAndr22 = new PlatformResolution((int)DeviceType.Android_2_2);
			prAndr22.AllowResolution = new List<int>();
			prAndr22.AllowResolution.Add(-1);
			//prAndr22.AllowResolution.Add(-2);
			prAndr22.AllowResolution.Add(-3);
			//prAndr22.AllowResolution.Add(-4);
			prAndr22.AllowResolution.Add(-5);
			prAndr22.AllowResolution.Add(-6);
			prAndr22.AllowResolution.Add(-7);
			prAndr22.AllowResolution.Add(-8);
			prAndr22.AllowResolution.Add(-9);
			prAndr22.AllowResolution.Add(-10);
			//prAndr22.AllowResolution.Add(-11);
			prAndr22.AllowResolution.Add(-12);
			prAndr22.AllowResolution.Add(-13);
			prAndr22.AllowResolution.Add(-14);
			prAndr22.AllowResolution.Add(-15);
			prAndr22.AllowResolution.Add(-16);
			PlatformResolutions.Add(prAndr22);


			PlatformResolution prIos = new PlatformResolution((int)DeviceType.iOS_5_0);
			prIos.AllowResolution = new List<int>();
			prIos.AllowResolution.Add(-1);
			prIos.AllowResolution.Add(-3);
			prIos.AllowResolution.Add(-7);
			prIos.AllowResolution.Add(-8);
			prIos.AllowResolution.Add(-15);
			prIos.AllowResolution.Add(-17);
			PlatformResolutions.Add(prIos);

			PlatformResolution prBada10 = new PlatformResolution((int)DeviceType.Bada_1_0);
			prBada10.AllowResolution = new List<int>();
			prBada10.AllowResolution.Add(-1);
			//prBada10.AllowResolution.Add(-3);
			prBada10.AllowResolution.Add(-6);
			//prBada10.AllowResolution.Add(-11);
			PlatformResolutions.Add(prBada10);

			PlatformResolution prBada11 = new PlatformResolution((int)DeviceType.Bada_1_1);
			prBada11.AllowResolution = new List<int>();
			prBada11.AllowResolution.Add(-1);
			prBada11.AllowResolution.Add(-3);
			//prBada11.AllowResolution.Add(-6);
			//prBada11.AllowResolution.Add(-11);
			PlatformResolutions.Add(prBada11);

			/*PlatformResolution prBada2 = new PlatformResolution((int)DeviceType.Bada_1_2);
			prBada2.AllowResolution = new List<int>();
			prBada2.AllowResolution.Add(-1);
			prBada2.AllowResolution.Add(-6);
			PlatformResolutions.Add(prBada2);*/

			PlatformResolution prSymbian94 = new PlatformResolution((int)DeviceType.Symbian_9_4);
			prSymbian94.AllowResolution = new List<int>();
			prSymbian94.AllowResolution.Add(-1);
			//prSymbian94.AllowResolution.Add(-4);
			prSymbian94.AllowResolution.Add(-6);
			PlatformResolutions.Add(prSymbian94);

			/*PlatformResolution prSymbian95 = new PlatformResolution((int)DeviceType.Symbian_9_5);
			prSymbian95.AllowResolution = new List<int>();
			prSymbian95.AllowResolution.Add(-1);
			prSymbian95.AllowResolution.Add(-4);
			prSymbian95.AllowResolution.Add(-6);
			prSymbian95.AllowResolution.Add(-11);
			PlatformResolutions.Add(prSymbian95);*/

			/*PlatformResolution prWindowsMobile5 = new PlatformResolution((int)DeviceType.WindowsMobile_5);
			prWindowsMobile5.AllowResolution = new List<int>();
			prWindowsMobile6.AllowResolution.Add(-1);
			prWindowsMobile5.AllowResolution.Add(-3);
			prWindowsMobile5.AllowResolution.Add(-2);
			prWindowsMobile5.AllowResolution.Add(-5);
			prWindowsMobile5.AllowResolution.Add(-6);
			PlatformResolutions.Add(prWindowsMobile5);*/

			PlatformResolution prWindowsMobile6 = new PlatformResolution((int)DeviceType.WindowsMobile_6);
			prWindowsMobile6.AllowResolution = new List<int>();
			prWindowsMobile6.AllowResolution.Add(-1);
			prWindowsMobile6.AllowResolution.Add(-3);
			//prWindowsMobile6.AllowResolution.Add(-2);
			prWindowsMobile6.AllowResolution.Add(-5);
			prWindowsMobile6.AllowResolution.Add(-6);
			PlatformResolutions.Add(prWindowsMobile6);
		}

		public void GenerateResolution(){
			Resolution = new Condition();
			Resolution.Id = -2;
			Resolution.Name = "Resolution";
			Resolution.System = true;

			Resolution.Rules = new List<Rule> ();
			Resolution.Rules.Add(new Rule(-1,"Universal","uni",0,0));
			Resolution.Rules.Add(new Rule(-2,"QVGA","qvga",240,320));
			Resolution.Rules.Add(new Rule(-3,"HVGA","hvga",320,480));
			Resolution.Rules.Add(new Rule(-4,"nHD","nhd",360,640));
			Resolution.Rules.Add(new Rule(-5,"VGA","vga",480,640));
			Resolution.Rules.Add(new Rule(-6,"WVGA","wvga",480,800));
			Resolution.Rules.Add(new Rule(-7,"DVGA","dvga",640,960));
			Resolution.Rules.Add(new Rule(-8,"XGA","xga",768,1024));
			Resolution.Rules.Add(new Rule(-9,"SVGA","svga",600,800));
			Resolution.Rules.Add(new Rule(-10,"qHD","qhd",540,960));
			Resolution.Rules.Add(new Rule(-11,"WQVGA","wqvga",240,400));
			Resolution.Rules.Add(new Rule(-12,"VXGA","vxga",800,1280));
			Resolution.Rules.Add(new Rule(-13,"FWVGA","fwvga",480,854));
			Resolution.Rules.Add(new Rule(-14,"WSVGA","wsvga",600,1024));
			Resolution.Rules.Add(new Rule(-15,"QXGA","qxga",1536,2048));
			Resolution.Rules.Add(new Rule(-16,"HD720","hd720",720,1280));
			Resolution.Rules.Add(new Rule(-17,"WDVGA","wdvga",640,1136));
		}

		public void GenerateOrientations(){
			
			DisplayOrientations  = new List<SettingValue>();
			DisplayOrientations.Add(new SettingValue("portrait","Portrait"));
			DisplayOrientations.Add(new SettingValue("landscape left","Landscape Left"));
			DisplayOrientations.Add(new SettingValue("landscape right","Landscape Right"));
		}

		public void GenerateApplicationType(){

			ApplicationType  = new List<SettingValue>();
			ApplicationType.Add(new SettingValue("application","Application"));
			ApplicationType.Add(new SettingValue("game","Game"));
		}

		public void GenerateInstallLocation(){
			//"internalOnly", "auto", "preferExternal"
			InstallLocations  = new List<SettingValue>();
			InstallLocations.Add(new SettingValue("internalOnly","Internal Only"));
			InstallLocations.Add(new SettingValue("auto","Auto"));
			InstallLocations.Add(new SettingValue("preferExternal","Prefer External"));
		}


		public void GenerateOSSupportedDevices(){
			//"internalOnly", "auto", "preferExternal"
			OSSupportedDevices  = new List<SettingValue>();
			OSSupportedDevices.Add(new SettingValue("universal","Universal"));
			OSSupportedDevices.Add(new SettingValue("iPhone","iPhone"));
			OSSupportedDevices.Add(new SettingValue("iPad","iPad"));
		}

		public void GenerateAndroidSupportedDevices(){
			//"internalOnly", "auto", "preferExternal"
			AndroidSupportedDevices  = new List<SettingValue>();
			AndroidSupportedDevices.Add(new SettingValue("universal","Universal"));
			AndroidSupportedDevices.Add(new SettingValue("armv7a","ARMv7a only"));
		}


		public List<string> GenerateLibs(){
			List<string> libsDefine = new List<string>();
			DirectoryInfo dirWorkspace = new DirectoryInfo(LibDirectory);
			DirectoryInfo[] listDirectory = dirWorkspace.GetDirectories("*",SearchOption.TopDirectoryOnly);

			foreach(DirectoryInfo di in listDirectory){
			 	int ignore = IgnoresFolders.FindIndex( x=> x.Folder == di.Name && x.IsForIde);
				if(ignore <0){
					libsDefine.Add( di.Name);
				}
			}
			return libsDefine;
		}


		public void GenerateIgnoreFiles(){
			IgnoresFiles = new List<IgnoreFolder>();
			IgnoresFiles.Add(new IgnoreFolder(".DS_Store",true,true));
			IgnoresFiles.Add(new IgnoreFolder("Thumbs.db",true,true));
		}


		public void GenerateExtensionList(){
			ExtensionList = new List<ExtensionSetting>();
			ExtensionList.Add(new ExtensionSetting(".xml,.ms,.mso,.txt,.tab,.app,.js",ExtensionSetting.OpenTyp.TEXT ));
			ExtensionList.Add(new ExtensionSetting(".png,.jpg,.bmp",ExtensionSetting.OpenTyp.IMAGE));
			ExtensionList.Add(new ExtensionSetting(".db",ExtensionSetting.OpenTyp.DATABASE));
		}

		public void GenerateIgnoreFolder(){
			IgnoresFolders = new List<IgnoreFolder>();
			IgnoresFolders.Add(new IgnoreFolder(".svn",true,true));
			IgnoresFolders.Add(new IgnoreFolder(".git",true,true));
			IgnoresFolders.Add(new IgnoreFolder("platform",false,true));
			IgnoresFolders.Add(new IgnoreFolder("marketing",false,true));
			IgnoresFolders.Add(new IgnoreFolder("output",false,true));
			//IgnoresFolders.Add(new IgnoreFolder("temp",true,true));
		}
		
	}
}

