using System;
using Gtk;
using Moscrif.IDE.Tool;

namespace Moscrif.IDE.Controls
{
	internal class AboutTabPage: VBox
	{
        public AboutTabPage ()
        {
            Label label = new Label();


		string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		string newVersion = MainClass.Tools.VersionConverter(version);

            HBox hBoxVersion = new HBox ();

	    label = null;
            label = new Label ();
            label.Markup = "<b>Copyright</b>\n   (c) 2007-2012 by Mothiva";//"<b>License :</b>\n  ";
           // hBoxLicense.PackEnd (label, false, false, 5);

            hBoxVersion.PackStart (label, false, false, 5);
            this.PackStart (hBoxVersion, false, true, 0);

            label = null;
            label = new Label ();
            label.Markup = "<b>Version</b>\n   "+newVersion;//"<b>License :</b>\n  ";
            HBox hBoxLicense = new HBox ();
            hBoxLicense.PackStart (label, false, false, 5);

            this.PackStart (hBoxLicense, false, true, 5);

            label = null;
            label = new Label ();
            label.Markup = "";
            HBox hBoxCopyright = new HBox ();
            hBoxCopyright.PackStart (label, false, false, 5);
            this.PackStart (hBoxCopyright, false, true, 5);

            this.ShowAll ();
        }
	}
}
