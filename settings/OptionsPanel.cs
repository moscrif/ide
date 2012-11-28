using System;
using Gtk;

namespace Moscrif.IDE.Option
{
	public abstract class OptionsPanel: IOptionsPanel
	{
		PreferencesDialog dlg;
		object dataObject;

		public virtual void Initialize (PreferencesDialog dialog,object dataObject)
		{
			dlg = dialog;
			this.dataObject = dataObject;
		}

		public abstract Widget CreatePanelWidget ();
		
		public virtual bool IsVisible ()
		{
			return true;
		}

		public virtual bool ValidateChanges ()
		{
			return true;
		}

		public virtual void ShowPanel()
		{
		}


		public abstract void ApplyChanges ();
		
		protected PreferencesDialog ParentDialog {
			get { return dlg; }
		}

		public object DataObject {
			get { return dataObject; }
		}


		public virtual string Label {
			get { return ""; }
		}

		public virtual string Icon {
			get { return "empty.png"; }
		}
	}
}
