using System;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using Moscrif.IDE.Controls;
using Moscrif.IDE.Workspace;
using Moscrif.IDE.Devices;
using MessageDialogs = Moscrif.IDE.Controls.MessageDialog;
using Moscrif.IDE.Iface.Entities;
using Moscrif.IDE.Iface;

namespace  Moscrif.IDE.Settings
{
	internal class KeyBindingPanel : OptionsPanel
	{
		KeyBindingWidget widget;
		  
		public override Widget CreatePanelWidget ()
		{
			return widget = new  KeyBindingWidget (ParentDialog);
		}

		public override void ShowPanel()
		{
		}

		public override void ApplyChanges ()
		{
			widget.Store ();
		}

		public override bool ValidateChanges ()
		{
			return true;
		}

		public override void Initialize (PreferencesDialog dialog, object dataObject)
		{
			base.Initialize (dialog, dataObject);
		}

		public override string Label {
			get { return "Key Binding"; }
		}

		public override string Icon {
			get { return "keyboard-shortcuts.png"; }
		}

	}
	public partial class KeyBindingWidget : Gtk.Bin
	{
		const string NOTHING ="Nothing";
		const string WIN ="Visual Studio";
		const string MACOSX ="XCode";
		const string JAVA ="Eclipse";
		const string VisualC ="Visual C++";

		static readonly int nameCol = 0;
		static readonly int descrCol = 1;
		static readonly int keyCol = 2;
		static readonly int boolCol = 3;
		static readonly int keyBindCol = 4;
		static Gdk.Keymap keymap = Gdk.Keymap.Default;
		static Dictionary<Gdk.Key,Gdk.Key> groupZeroMappings = new Dictionary<Gdk.Key,Gdk.Key> ();

		private bool accelComplete;

		List<KeyBindingSection> curentBind ;
		Gtk.TreeStore keybStore = new Gtk.TreeStore( typeof(string),typeof(string),typeof(string),typeof(int),typeof(KeyBinding));

		Gtk.Window parentWindow;

		KeyBindings keyBindFile ;

		public KeyBindingWidget(Gtk.Window parent)
		{
			parentWindow = parent;
			this.Build();

			/*if(!isRequired){
				cbKeyBinding.AppendText(NOTHING);
				cbKeyBinding.Active = 0;
			}*/

			cbKeyBinding.AppendText(WIN);
			cbKeyBinding.AppendText(MACOSX);
			cbKeyBinding.AppendText(JAVA);
			cbKeyBinding.AppendText(VisualC);

			if(MainClass.Platform.IsMac){
				cbKeyBinding.Active = 1;
			} else {
				cbKeyBinding.Active = 0;
			}


			keymap.KeysChanged += delegate {
				groupZeroMappings.Clear ();
			};

			lblMessage.Text = "";
			//string file = System.IO.Path.Combine(MainClass.Paths.ConfingDir, "keybinding");
			keyBindFile = MainClass.KeyBinding;//KeyBindings.OpenKeyBindings(file);
			//entrAccel.InvisibleChar = 'â—Ź';

			//curentBind =keyBindFile.

			TreeViewColumn col = new TreeViewColumn ();
			col.Title = MainClass.Languages.Translate("name");
			col.Spacing = 4;
			CellRendererText crp = new CellRendererText ();
			col.PackStart (crp, false);
			col.AddAttribute (crp, "text", nameCol);
			col.AddAttribute (crp, "weight", boolCol);

			tvKeyBind.AppendColumn(col);
			//tvKeyBind.AppendColumn(MainClass.Languages.Translate("name"), new Gtk.CellRendererText(), "text", 0);
			tvKeyBind.AppendColumn(MainClass.Languages.Translate("description"), new Gtk.CellRendererText(), "text", descrCol);
			tvKeyBind.AppendColumn(MainClass.Languages.Translate("key"), new Gtk.CellRendererText(), "text", keyCol);
			tvKeyBind.Model = keybStore;
			tvKeyBind.EnableSearch = false;

			curentBind = MainClass.Tools.Clone<KeyBindingSection>(keyBindFile.KeyBindingSection);

			foreach(KeyBindingSection kbs in  curentBind){

				TreeIter tiParent = keybStore.AppendValues(kbs.Name,"","",(int) Pango.Weight.Bold,null);
				foreach(KeyBinding kb in kbs.KeyBinding){
					keybStore.AppendValues(tiParent,kb.Name,kb.Description,kb.Key,(int) Pango.Weight.Normal,kb);
				}
			}

			tvKeyBind.ExpandAll();
			tvKeyBind.ColumnsAutosize ();

			entrAccel.Sensitive = true;
			btnAccelAply.Sensitive = true;


			tvKeyBind.Selection.Changed += OnKeysTreeViewSelectionChange;

		}


		void OnKeysTreeViewSelectionChange (object sender, EventArgs e)
		{
			TreeSelection sel = sender as TreeSelection;
			TreeModel model;
			TreeIter iter;

			if (sel.GetSelected (out model, out iter) && model.GetValue (iter,keyBindCol) != null) {
				entrAccel.Sensitive = true;
				btnAccelAply.Sensitive = true;

				KeyBinding kb = (KeyBinding) model.GetValue (iter, keyBindCol);

				entrAccel.Text =kb.Key;
				entrAccel.GrabFocus ();
			} else {
				entrAccel.Sensitive = false;
				btnAccelAply.Sensitive = false;
			}
		}


		public void Store ()
		{
			KeyBindings kb = MainClass.KeyBinding;//.KeyBindingSection; = curentBind;

			kb.KeyBindingSection= curentBind;

			KeyBindings.SaveKeyBindings(kb);

			MainClass.KeyBinding.KeyBindingSection = curentBind;
			//MainClass.KeyBinding.SaveKeyBindings();

			MainClass.MainWindow.ActionUiManager.ReloadKeyBind();

		}

		protected virtual void OnBtnAccelAplyClicked (object sender, System.EventArgs e)
		{
			TreeSelection sel = tvKeyBind.Selection;
			TreeModel model;
			TreeIter iter;

			if (sel.GetSelected (out model, out iter) && model.GetValue (iter,keyBindCol) != null) {
				entrAccel.Sensitive = true;
				btnAccelAply.Sensitive = true;

				KeyBinding kb = (KeyBinding) model.GetValue (iter, keyBindCol);
				kb.Key = entrAccel.Text;

				model.SetValue(iter,keyCol,kb.Key);
			}
		}

		[GLib.ConnectBefore]
		protected virtual void OnEntrAccelKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{

			string accel = entrAccel.Text;
			if(accel.EndsWith("+"))
				entrAccel.Text = "";
		}

		[GLib.ConnectBefore]
		void OnEntrAccelKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{

			args.RetVal = true;
			//Console.WriteLine(args.Event.Key);
			Gdk.Key key;
			Gdk.ModifierType mt;

			Console.WriteLine("key 1->{0};mt->{1}",args.Event.Key,args.Event.State);

			MapRawKeys (args.Event, out key, out mt);

			Console.WriteLine("key 2->{0};mt->{1}",key,mt);

			if(accelComplete){
				if(key == Gdk.Key.BackSpace){
					entrAccel.Text= "";
					return;
				}
			}

			bool shiftWasConsumed = ((args.Event.State ^ mt) & Gdk.ModifierType.ShiftMask) != 0;
			if(shiftWasConsumed && char.IsUpper((char)Gdk.Keyval.ToUpper((uint)key)))
				mt |= Gdk.ModifierType.ShiftMask;

			bool isModifier = false ;
			string modifier = ModifierToPartialAccel(mt,key,out isModifier);
			accelComplete = !isModifier;

			if (!isModifier) {
				modifier =  modifier + KeyToString (key);
			}

			entrAccel.Text= modifier;

			string conflict =FindKeyBindConflict(modifier);

			if(string.IsNullOrEmpty(conflict)){
				lblMessage.Text = "";
			}
			else{
				lblMessage.Text = String.Format("<b> {0} </b>", MainClass.Languages.Translate("keyBind_conflict",conflict));
				lblMessage.UseMarkup= true;
			}
		}

		public static void MapRawKeys (Gdk.EventKey evt, out Gdk.Key key, out Gdk.ModifierType mod)
		{
			mod = evt.State;
			key = evt.Key;
			uint keyval;
			int effectiveGroup, level;
			Gdk.ModifierType consumedModifiers;
			keymap.TranslateKeyboardState (evt.HardwareKeycode, evt.State, evt.Group, out keyval, out effectiveGroup,
			                               out level, out consumedModifiers);
			
			key = (Gdk.Key)keyval;
			mod = evt.State & ~consumedModifiers;

			if (MainClass.Platform.IsX11) {
				//this is a workaround for a common X mapping issue
				//where the alt key is mapped to the meta key when the shift modifier is active
				if (key.Equals (Gdk.Key.Meta_L) || key.Equals (Gdk.Key.Meta_R))
					key = Gdk.Key.Alt_L;
			}
			
			//HACK: the MAC GTK+ port currently does some horrible, un-GTK-ish key mappings
			// so we work around them by playing some tricks to remap and decompose modifiers.
			// We also decompose keys to the root physical key so that the Mac command 
			// combinations appear as expected, e.g. shift-{ is treated as shift-[.
			if (MainClass.Platform.IsMac && !MainClass.Platform.IsX11) {
				// Mac GTK+ maps the command key to the Mod1 modifier, which usually means alt/
				// We map this instead to meta, because the Mac GTK+ has mapped the cmd key
				// to the meta key (yay inconsistency!). IMO super would have been saner.
				if ((mod & Gdk.ModifierType.Mod1Mask) != 0) {
					mod ^= Gdk.ModifierType.Mod1Mask;
					mod |= Gdk.ModifierType.MetaMask;
				}
				
				// If Mod5 is active it *might* mean that opt/alt is active,
				// so we can unset this and map it back to the normal modifier.
				if ((mod & Gdk.ModifierType.Mod5Mask) != 0) {
					mod ^= Gdk.ModifierType.Mod5Mask;
					mod |= Gdk.ModifierType.Mod1Mask;
				}

				// When opt modifier is active, we need to decompose this to make the command appear correct for Mac.
				// In addition, we can only inspect whether the opt/alt key is pressed by examining
				// the key's "group", because the Mac GTK+ treats opt as a group modifier and does
				// not expose it as an actual GDK modifier.
				if (evt.Group == (byte) 1) {
					mod |= Gdk.ModifierType.Mod1Mask;
					key = GetGroupZeroKey (key, evt);
				}
			}
			
			//fix shift-tab weirdness. There isn't a nice name for untab, so make it shift-tab
			if (key == Gdk.Key.ISO_Left_Tab) {
				key = Gdk.Key.Tab;
				mod |= Gdk.ModifierType.ShiftMask;
			}
		}

		static Gdk.Key GetGroupZeroKey (Gdk.Key mappedKey, Gdk.EventKey evt)
		{
			Gdk.Key ret;
			if (groupZeroMappings.TryGetValue (mappedKey, out ret))
				return ret;
			
			//LookupKey isn't implemented on Mac, so we have to use this workaround
			uint[] keyvals;
			Gdk.KeymapKey [] keys;
			keymap.GetEntriesForKeycode (evt.HardwareKeycode, out keys, out keyvals);
			
			//find the key that has the same level (so we preserve shift) but with group 0
			for (uint i = 0; i < keyvals.Length; i++)
				if (keyvals[i] == (uint)mappedKey)
					for (uint j = 0; j < keys.Length; j++)
						if (keys[j].Group == 0 && keys[j].Level == keys[i].Level)
							return groupZeroMappings[mappedKey] = ret = (Gdk.Key)keyvals[j];
			
			//failed, but avoid looking it up again
			return groupZeroMappings[mappedKey] = mappedKey;
		}

		private string FindKeyBindConflict(string key){

			string conflicted ="";

			foreach(KeyBindingSection kbs in curentBind){
				KeyBinding kb =kbs.KeyBinding.Find(x=>x.Key== key);
				if(kb != null)
					conflicted = conflicted+kb.Name+", ";
			}
			if(!string.IsNullOrEmpty(conflicted)){
				int i =conflicted.Length-3;
				conflicted = conflicted.Remove(i);
			}

			return conflicted;
		}


		static string ModifierToPartialAccel (Gdk.ModifierType mod, Gdk.Key key, out bool keyIsModifier)
		{
			//Console.WriteLine("PRESSED MOD: {0} ; KEY {1}",mod,key);

			string labelMod = String.Empty;
			string labelKey = String.Empty;

			if ((mod & Gdk.ModifierType.ControlMask) != 0)
				labelMod += "Control+";
			if ((mod & Gdk.ModifierType.Mod1Mask) != 0)
				labelMod += "Alt+";
			if ((mod & Gdk.ModifierType.ShiftMask) != 0)
				labelMod += "Shift+";
			if ((mod & Gdk.ModifierType.MetaMask) != 0)
				labelMod += "Command+";//labelMod += "Meta+";
			if ((mod & Gdk.ModifierType.SuperMask) != 0)
				labelMod += "Super+";

			//Console.WriteLine("labelMod-> {0}",labelMod);

			keyIsModifier = true;
			if (key.Equals (Gdk.Key.Control_L) || key.Equals (Gdk.Key.Control_R))
				labelKey += "Control+";
			else if (key.Equals (Gdk.Key.Alt_L) || key.Equals (Gdk.Key.Alt_R))
				labelKey += "Alt+";
			else if (key.Equals (Gdk.Key.Shift_L) || key.Equals (Gdk.Key.Shift_R))
				labelKey += "Shift+";
			else if (key.Equals (Gdk.Key.Meta_L) || key.Equals (Gdk.Key.Meta_R))
				labelKey += "Command+";//labelKey += "Meta+";
			else if (key.Equals (Gdk.Key.Super_L) || key.Equals (Gdk.Key.Super_L))
				labelKey += "Super+";
			else
				keyIsModifier = false;


			//Console.WriteLine("labelKey-> {0}",labelKey);

			if(labelMod.Contains(labelKey)){

				return labelMod;
			} else return labelMod+labelKey;
		}

		static string KeyToString (Gdk.Key key)
		{
			char c = (char) Gdk.Keyval.ToUnicode ((uint) key);
			
			if (c != 0) {
				if (c == ' ')
					return "Space";
				
				return Char.ToUpper (c).ToString ();
			}

			//HACK: Page_Down and Next are synonyms for the same enum value, but alphabetically, Next gets returned 
			// first by enum ToString(). Similarly, Page_Up and Prior are synonyms, but Page_Up is returned. Hence the 
			// default pairing is Next and Page_Up, which is confusingly inconsistent, so we fix this here.
			//
			//The same problem applies to some other common keys, so we ensure certain values are mapped
			// to the most common name.
			switch (key) {
			case Gdk.Key.Next:
				return "Page_Down";
			case Gdk.Key.L1:
				return "F11";
			case Gdk.Key.L2:
				return "F12";
			}
			
			return key.ToString ();
		}

		protected virtual void OnEntrAccelChanged (object sender, System.EventArgs e)
		{
		}

		protected void OnBtnAccelAply1Clicked (object sender, System.EventArgs e)
		{

			string active = cbKeyBinding.ActiveText;
			string file = System.IO.Path.Combine(MainClass.Paths.SettingDir, "keybinding");

			switch (active) {
			case WIN:{
				KeyBindings.CreateKeyBindingsWin(file);
				break;
			}
			case MACOSX:{
				KeyBindings.CreateKeyBindingsMac(file);
				break;
			}
			case JAVA:{
				KeyBindings.CreateKeyBindingsJava(file);
				break;
			}
			case VisualC:{
				KeyBindings.CreateKeyBindingsVisualC(file);
				break;
			}
			default:
				break;
			}
			keyBindFile = MainClass.KeyBinding;

			curentBind = MainClass.Tools.Clone<KeyBindingSection>(keyBindFile.KeyBindingSection);
			keybStore.Clear();
			foreach(KeyBindingSection kbs in  curentBind){

				TreeIter tiParent = keybStore.AppendValues(kbs.Name,"","",(int) Pango.Weight.Bold,null);
				foreach(KeyBinding kb in kbs.KeyBinding){
					keybStore.AppendValues(tiParent,kb.Name,kb.Description,kb.Key,(int) Pango.Weight.Normal,kb);
				}
			}

			tvKeyBind.ExpandAll();
			tvKeyBind.ColumnsAutosize ();
			//MainClass.Settings.SaveSettings();
		}
	}
}

