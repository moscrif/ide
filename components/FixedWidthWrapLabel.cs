using System;
using Gtk;

namespace Moscrif.IDE.Components
{
	[System.ComponentModel.Category("Moscfift.Ide.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class FixedWidthWrapLabel : Widget
	{
		string text;
		bool use_markup = false;
		Pango.Layout layout;
		int indent;
		int width = int.MaxValue;
		
		bool breakOnPunctuation;
		bool breakOnCamelCasing;
		string brokentext;
		
		Pango.WrapMode wrapMode = Pango.WrapMode.Word;
		
		Pango.FontDescription fontDescription;
		public Pango.FontDescription FontDescription { 
			get {
				return fontDescription;
			}
			set {
				fontDescription = value;
			}
			
		}
		public FixedWidthWrapLabel ()
		{
			WidgetFlags |= WidgetFlags.NoWindow;
		}
		
		public FixedWidthWrapLabel (string text)
			: this ()
		{
			this.text = text;
		}
		
		public FixedWidthWrapLabel (string text, int width)
			: this (text)
		{
			this.width = width;
		}
		
		void CreateLayout ()
		{
			if (layout != null) {
				layout.Dispose ();
			}
			
			layout = new Pango.Layout (PangoContext);
			if (FontDescription != null)
				layout.FontDescription = FontDescription;
			if (use_markup) {
				layout.SetMarkup (brokentext != null? brokentext : (text ?? string.Empty));
			} else {
				layout.SetText (brokentext != null? brokentext : (text ?? string.Empty));
			}
			layout.Indent = (int) (indent * Pango.Scale.PangoScale);
			layout.Wrap = wrapMode;
			if (width >= 0)
				layout.Width = (int)(width * Pango.Scale.PangoScale);
			else
				layout.Width = int.MaxValue;
			QueueResize ();
		}
		
		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			if (layout != null) {
				layout.Dispose ();
				layout = null;
			}
		}
		
		void UpdateLayout ()
		{
			if (layout == null) {
				CreateLayout ();
			}
		}
		
		public int MaxWidth {
			get { return width; }
			set {
				width = value;
				if (layout != null) {
					if (width >= 0)
						layout.Width = (int)(width * Pango.Scale.PangoScale);
					else
						layout.Width = int.MaxValue;
					QueueResize ();
				}
			}
		}
		
		public int RealWidth {
			get {
				UpdateLayout ();
				int lw, lh;
				layout.GetPixelSize (out lw, out lh);
				return lw;
			}
		}
		
		protected override void OnStyleSet (Style previous_style)
		{
			CreateLayout ();
			base.OnStyleSet (previous_style);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			UpdateLayout ();
			int lw, lh;
			layout.GetPixelSize (out lw, out lh);
			requisition.Height = lh;
			requisition.Width = lw;
		}
		
	
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			UpdateLayout ();
			if (evnt.Window != GdkWindow) {
				return base.OnExposeEvent (evnt);
			}
            
			Gtk.Style.PaintLayout (Style, GdkWindow, State, false, evnt.Area, 
			    this, null, Allocation.X, Allocation.Y, layout);
			
			return true;
		}
        
		public string Markup {
			get { return text; }
			set {
				use_markup = true;
				text = value;
				breakText ();
			}
		}
        
		public string Text {
			get { return text; }
			set {
				use_markup = false;
				text = value;
				breakText ();
			}
		}  
		
		public int Indent {
			get { return indent; }
			set {
				indent = value;
				if (layout != null) {
					layout.Indent = (int) (indent * Pango.Scale.PangoScale);
					QueueResize ();
				}
			}
		}
		
		public bool BreakOnPunctuation {
			get { return breakOnPunctuation; }
			set {
				breakOnPunctuation = value;
				breakText ();
			}
		}
		
		public bool BreakOnCamelCasing {
			get { return breakOnCamelCasing; }
			set {
				breakOnCamelCasing = value;
				breakText ();
			}
		}
		
		public Pango.WrapMode Wrap {
			get { return wrapMode; }
			set {
				wrapMode = value;
				if (layout != null) {
					layout.Wrap = wrapMode;
					QueueResize ();
				}
			}
		}
		
		void breakText ()
		{
			brokentext = null;
			if ((!breakOnCamelCasing && !breakOnPunctuation) || string.IsNullOrEmpty (text)) {
				QueueResize ();
				return;
			}
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder (text.Length);
			
			bool prevIsLower = false;
			bool inMarkup = false;
			bool inEntity = false;
			
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				
				//ignore markup
				if (use_markup) {
					switch (c) {
					case '<':
						inMarkup = true;
						sb.Append (c);
						continue;
					case '>':
						inMarkup = false;
						sb.Append (c);
						continue;
					case '&':
						inEntity = true;
						sb.Append (c);
						continue;
					case ';':
						if (inEntity) {
							inEntity = false;
							sb.Append (c);
							continue;
						}
						break;
					}
				}
				if (inMarkup || inEntity) {
					sb.Append (c);
					continue;
				}
					
				//insert breaks using zero-width space unicode char
				if ((breakOnPunctuation && char.IsPunctuation (c))
				    || (breakOnCamelCasing && prevIsLower && char.IsUpper (c)))
					sb.Append ('\u200b');
				
				sb.Append (c);
				
				if (breakOnCamelCasing)
					prevIsLower = char.IsLower (c);
			}
			brokentext = sb.ToString ();
			
			if (layout != null) {
				if (use_markup) {
					layout.SetMarkup (brokentext != null? brokentext : (text ?? string.Empty));
				} else {
					layout.SetText (brokentext != null? brokentext : (text ?? string.Empty));
				}
			}
			QueueResize ();
		}
	}
}
