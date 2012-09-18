using System;
using System.IO;
using Gtk;
using Gdk;
using Moscrif.IDE.Iface;
using System.Collections;
using System.Collections.Generic;

namespace Moscrif.IDE.Components
{
	public class WarningControl : Gtk.Table
	{
		Label lbl = new Label();
		public WarningControl() : base(2,2,true)
		{
			Pango.FontDescription customFont =  Pango.FontDescription.FromString(MainClass.Settings.ConsoleTaskFont);
			customFont.Weight = Pango.Weight.Bold;
			lbl.ModifyFont(customFont);


			this.Attach(lbl,0,1,1,2,AttachOptions.Expand,AttachOptions.Fill,0,0);
		}
	}
}

