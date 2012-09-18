using System;
using Gtk;

namespace Moscrif.IDE.Settings
{
	public interface IOptionsPanel
	{
		void Initialize (PreferencesDialog dialog, object dataObject);

		string Label {get;}
		string Icon {get;}

		Widget CreatePanelWidget ();
		
		bool IsVisible ();
		bool ValidateChanges ();
		void ApplyChanges ();
		void ShowPanel();
	}
}
