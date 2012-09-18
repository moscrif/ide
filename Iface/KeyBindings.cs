using System;
using Moscrif.IDE.Iface.Entities;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Moscrif.IDE.Iface
{
	public class KeyBindings
	{

		[XmlIgnore]
		public string FilePath
		{
			get;
			set;
		}


		public enum TypeKeyBinding {
			Nothing,
			VisualStudio,
			XCode,
			Eclipse,
			VisualC
		}

		[XmlAttribute("typeBinding")]
		public TypeKeyBinding TypeBinding;

		[XmlArrayAttribute("sections")]
		[XmlArrayItem("section")]
		public List<KeyBindingSection> KeyBindingSection;


		public KeyBindings()
		{
		}


		public string FindAccelerator(string action){
			if(KeyBindingSection == null) return null;

			foreach(KeyBindingSection kbs in KeyBindingSection){
				KeyBinding kb = kbs.KeyBinding.Find(x=> x.Action == action);
				if(kb != null){
					return KeyToAccel(kb.Key);
				}
			}
			return null;
		}

		public string KeyToAccel(string key){
			/*while (i < accel.Length) {
				for (j = i + 1; j < accel.Length; j++) {
					if (accel[j] == '+' || accel[j] == '|')
						break;
				}

				str = accel.Substring (i, j - i);
				if ((mod = ModifierMask (str)) == Gdk.ModifierType.None) {
					if (str == "Space")
						k = (uint) Gdk.Key.space;
					else if (str.Length > 1)
						k = Gdk.Keyval.FromName (str);
					else
						k = (uint) str[0];

					break;
				}

				mask |= mod;
				i = j + 1;
			}*/

			key = key.ToUpper();

			key=key.Replace("<",Gdk.Key.less.ToString());
			key=key.Replace(">",Gdk.Key.equal.ToString());

			key=key.Replace("CONTROL","<control>");

			if(MainClass.Platform.IsMac){
				key=key.Replace("ALT","<alt>");//Gdk.Key.Alt_L.ToString());//Gdk.Key.Meta_L.ToString());//"<alt>");
			} else {
				key=key.Replace("ALT","<alt>");//Gdk.Key.Alt_L.ToString());//"<alt>");
			}
			key=key.Replace("SHIFT","<shift>");
			key=key.Replace("COMMAND","<meta>");
			key=key.Replace("SUPER","<super>");
			key=key.Replace("SPACE",Gdk.Key.space.ToString());
			key=key.Replace("RETURN",Gdk.Key.Return.ToString());
			key=key.Replace("TAB",Gdk.Key.Tab.ToString());
			key=key.Replace("HOME",Gdk.Key.Home.ToString());
			key=key.Replace("END",Gdk.Key.End.ToString());
			key=key.Replace(",",Gdk.Key.comma.ToString());
			key=key.Replace(".",Gdk.Key.period.ToString());
			key=key.Replace(";",Gdk.Key.semicolon.ToString());
			key=key.Replace("`",Gdk.Key.grave.ToString());
			key=key.Replace("/",Gdk.Key.slash.ToString());

			key=key.Replace("UP",Gdk.Key.Up.ToString());
			key=key.Replace("DOWN",Gdk.Key.Down.ToString());
			key=key.Replace("LEFT",Gdk.Key.Left.ToString());
			key=key.Replace("RIGHT",Gdk.Key.Right.ToString());

			key=key.Replace("+","");

			//Console.WriteLine("key->{0}",key);
			return key;
		}


		public void ConnectAccelerator(Gtk.Action action, string keyBind, Gtk.AccelGroup ag)
		{
			string path = "<Actions>/MainWindow/" + action.Name;
			Gdk.ModifierType mods;
			uint key;
			/*Console.WriteLine(action.AccelPath);

			if(action.AccelPath == "<Actions>/MainWindow/idepreferences"){
				Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
			}*/

			Gtk.Accelerator.Parse(keyBind,out key, out mods);
			/*
			Console.WriteLine("keyBind->{0}",keyBind);
			Console.WriteLine("key->{0}",key);
			Console.WriteLine("mods->{0}",mods);
			Console.WriteLine("action->{0}",action.Name);
			 */

			if((keyBind.Contains("<alt>") && MainClass.Platform.IsMac)){
				//mods |= Gdk.ModifierType.Mod2Mask;
				mods ^= Gdk.ModifierType.Mod1Mask;
				mods |= Gdk.ModifierType.Mod5Mask;
				//mods |= Gdk.ModifierType.Mod5Mask;
			}
			//Console.WriteLine("mods 2->{0}",mods);

			Gtk.AccelMap.ChangeEntry(path, key, mods,true);

			action.AccelGroup = ag;
			action.AccelPath  = path;

			//Console.WriteLine("action->{0}",action.AccelPath);
			//Console.WriteLine("action->{0}",action.AccelGroup);
		}


		public KeyBindings(string filePath)
		{
			FilePath = filePath;
		}

		public void SaveKeyBindings()
		{
			using (FileStream fs = new FileStream(FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(KeyBindings));
				serializer.Serialize(fs, this);
			}
		}

		static public void SaveKeyBindings(KeyBindings kb){

			using (FileStream fs = new FileStream(kb.FilePath, FileMode.Create)) {
				XmlSerializer serializer = new XmlSerializer(typeof(KeyBindings));
				serializer.Serialize(fs, kb);
			}

		}

		static public void CreateKeyBindingsJava(string filePath){
			string keyBindFileOld;
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream ("keybinding.java"))
				using (var sr = new StreamReader (stream))
					keyBindFileOld = sr.ReadToEnd ();

				try {
					if(File.Exists(filePath)){
						File.Delete(filePath);
					}

					using (StreamWriter file = new StreamWriter(filePath)) {
						file.Write (keyBindFileOld);
						file.Close();
						file.Dispose();
					}
					//return s;

				} catch (Exception ex) {
					Tool.Logger.Error("Cannot Create keybinding"+ex.Message);
				}
		}

		static public void CreateKeyBindingsVisualC(string filePath){
			string keyBindFileOld;
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream ("keybinding.visualC"))
				using (var sr = new StreamReader (stream))
					keyBindFileOld = sr.ReadToEnd ();

				try {
					if(File.Exists(filePath)){
						File.Delete(filePath);
					}

					using (StreamWriter file = new StreamWriter(filePath)) {
						file.Write (keyBindFileOld);
						file.Close();
						file.Dispose();
					}
					//return s;

				} catch (Exception ex) {
					Tool.Logger.Error("Cannot Create keybinding"+ex.Message);
				}
		}

		static public void CreateKeyBindingsMac(string filePath){
			string keyBindFileOld;
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream ("keybinding.mac"))
				using (var sr = new StreamReader (stream))
					keyBindFileOld = sr.ReadToEnd ();

				try {
					if(File.Exists(filePath)){
						File.Delete(filePath);
					}

					using (StreamWriter file = new StreamWriter(filePath)) {
						file.Write (keyBindFileOld);
						file.Close();
						file.Dispose();
					}
					//return s;

				} catch (Exception ex) {
					Tool.Logger.Error("Cannot Create keybinding"+ex.Message);
				}
		}

		static public void CreateKeyBindingsWin(string filePath){
			string keyBindFileOld;
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream ("keybinding.win"))
				using (var sr = new StreamReader (stream))
					keyBindFileOld = sr.ReadToEnd ();

				try {
					if(File.Exists(filePath)){
						File.Delete(filePath);
					}

					using (StreamWriter file = new StreamWriter(filePath)) {
						file.Write (keyBindFileOld);
						file.Close();
						file.Dispose();
					}
					//return s;

				} catch (Exception ex) {
					Tool.Logger.Error("Cannot Create keybinding"+ex.Message);
				}
		}

		static public KeyBindings OpenKeyBindings(string filePath)
		{
			if (System.IO.File.Exists(filePath)) {
				
				try {
					using (FileStream fs = File.OpenRead(filePath)) {
						XmlSerializer serializer = new XmlSerializer(typeof(KeyBindings));
						KeyBindings s = (KeyBindings)serializer.Deserialize(fs);
						s.FilePath= filePath;
						return s;
					}
				} catch (Exception ex) {
					
					throw ex;
					/*MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, \"Error\", \"Settings file is corrupted!\", Gtk.MessageType.Error);
					ms.ShowDialog();
					return new Settings();*/
				}
			} else {
				KeyBindings kb= new KeyBindings(filePath);
				kb.KeyBindingSection = new List<KeyBindingSection>();

				kb.KeyBindingSection.Add(new KeyBindingSection("section_name"));
				kb.KeyBindingSection[0].KeyBinding.Add(new KeyBinding("name","description","action","key"));
				//kb.KeyBinding
				return kb;
				/*MessageDialogs ms = new MessageDialogs(MessageDialogs.DialogButtonType.Ok, "Error", "Settings file does not exist!", Gtk.MessageType.Error);
				ms.ShowDialog();
				return new Settings();*/
			}
		}


	}
}

