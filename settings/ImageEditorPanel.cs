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

namespace Moscrif.IDE.Settings
{
	internal class ImageEditorPanel : OptionsPanel
	{
		ImageEditorWidget widget;

		public override Widget CreatePanelWidget ()
		{
			return widget = new  ImageEditorWidget ();
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
			get { return MainClass.Languages.Translate("image_editor"); }
		}

		public override string Icon {
			get { return "editor-image.png"; }
		}

	}

	public partial class ImageEditorWidget : Gtk.Bin
	{
		public ImageEditorWidget()
		{
			this.Build();
			if(MainClass.Settings.ImageEditors == null){
				MainClass.Settings.ImageEditors = new Settings.ImageEditorSetting();
				MainClass.Settings.ImageEditors.LineWidth = 3;
				MainClass.Settings.ImageEditors.PointWidth = 5;

				MainClass.Settings.ImageEditors.LineColor = new Settings.BackgroundColors(10,10,255,32767);
				MainClass.Settings.ImageEditors.PointColor = new Settings.BackgroundColors(10,10,255,32767);
				MainClass.Settings.ImageEditors.SelectPointColor = new Settings.BackgroundColors(255,10,10,32767);
			}
			sbLine.Value = MainClass.Settings.ImageEditors.LineWidth;
			sbPoint.Value = MainClass.Settings.ImageEditors.PointWidth;

			cbLine.UseAlpha = true;
			cbPoint.UseAlpha = true;
			cbSelectPoint.UseAlpha = true;

			cbLine.Alpha = MainClass.Settings.ImageEditors.LineColor.Alpha;
			cbLine.Color = new Gdk.Color(MainClass.Settings.ImageEditors.LineColor.Red,
				MainClass.Settings.ImageEditors.LineColor.Green,MainClass.Settings.ImageEditors.LineColor.Blue);
			//cbLine.Co Alpha = MainClass.Settings.ImageEditors.LineColor.Alpha;

			cbPoint.Color = new Gdk.Color(MainClass.Settings.ImageEditors.PointColor.Red,
				MainClass.Settings.ImageEditors.PointColor.Green,MainClass.Settings.ImageEditors.PointColor.Blue);
			cbPoint.Alpha =MainClass.Settings.ImageEditors.PointColor.Alpha;

			cbSelectPoint.Color = new Gdk.Color(MainClass.Settings.ImageEditors.SelectPointColor.Red,
				MainClass.Settings.ImageEditors.SelectPointColor.Green,MainClass.Settings.ImageEditors.SelectPointColor.Blue);
			cbSelectPoint.Alpha = MainClass.Settings.ImageEditors.SelectPointColor.Alpha;
		}

		public void Store()
		{
			MainClass.Settings.ImageEditors.LineColor.Red = (byte)cbLine.Color.Red;
			MainClass.Settings.ImageEditors.LineColor.Green= (byte)cbLine.Color.Green;
			MainClass.Settings.ImageEditors.LineColor.Blue= (byte)cbLine.Color.Blue;
			MainClass.Settings.ImageEditors.LineColor.Alpha = cbLine.Alpha;

			MainClass.Settings.ImageEditors.PointColor.Red = (byte)cbPoint.Color.Red;
			MainClass.Settings.ImageEditors.PointColor.Green= (byte)cbPoint.Color.Green;
			MainClass.Settings.ImageEditors.PointColor.Blue= (byte)cbPoint.Color.Blue;
			MainClass.Settings.ImageEditors.PointColor.Alpha = cbPoint.Alpha;

			MainClass.Settings.ImageEditors.SelectPointColor.Red = (byte)cbSelectPoint.Color.Red;
			MainClass.Settings.ImageEditors.SelectPointColor.Green= (byte)cbSelectPoint.Color.Green;
			MainClass.Settings.ImageEditors.SelectPointColor.Blue= (byte)cbSelectPoint.Color.Blue;
			MainClass.Settings.ImageEditors.SelectPointColor.Alpha = cbSelectPoint.Alpha ;

			MainClass.Settings.ImageEditors.LineWidth=(int)sbLine.Value;
			MainClass.Settings.ImageEditors.PointWidth = (int)sbPoint.Value;

			//MainClass.Settings.ShowEolMarker = chbEol.Active;

			/*MainClass.Settings.BackgroundColor.Blue = (byte)sbBlue.Value;
			MainClass.Settings.BackgroundColor.Red = (byte)sbRed.Value;
			MainClass.Settings.BackgroundColor.Green = (byte)sbGreen.Value;

			MainClass.Settings.PreCompile = chbPrecompile.Active;

			MainClass.Settings.EditorFont = fontbutton1.FontName;*/
		}

		
	}
}

