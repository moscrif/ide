using System;
using Gtk;
using Gdk;

namespace Moscrif.IDE.Components
{
	public abstract class TooltipWindow : Gtk.Window
	{
		bool nudgeVertical = false;
		bool nudgeHorizontal = false;
		//WindowTransparencyDecorator decorator;
		
		public TooltipWindow () : base(Gtk.WindowType.Popup)
		{
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.Decorated = false;
			this.BorderWidth = 2;
			this.TypeHint = WindowTypeHint.Tooltip;
			this.AllowShrink = false;
			this.AllowGrow = false;
			this.Title = "tooltip"; // fixes the annoying '** Message: ATK_ROLE_TOOLTIP object found, but doesn't look like a tooltip.** Message: ATK_ROLE_TOOLTIP object found, but doesn't look like a tooltip.'
			
			//fake widget name for stupid theme engines
			this.Name = "gtk-tooltip";
		}
		
		public bool NudgeVertical {
			get { return nudgeVertical; }
			set { nudgeVertical = value; }
		}
		
		public bool NudgeHorizontal {
			get { return nudgeHorizontal; }
			set { nudgeHorizontal = value; }
		}
		
		public bool EnableTransparencyControl {
			get { return false; //return decorator != null;
			}
			set {
				/*if (value && decorator == null)
					decorator = WindowTransparencyDecorator.Attach (this);
				else if (!value && decorator != null)
					decorator.Detach ();*/
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			int winWidth, winHeight;
			this.GetSize (out winWidth, out winHeight);
			Gtk.Style.PaintFlatBox (Style, this.GdkWindow, StateType.Normal, ShadowType.Out, evnt.Area, this, "tooltip", 0, 0, winWidth, winHeight);
			foreach (var child in this.Children)
				this.PropagateExpose (child, evnt);
			return false;
		}
		
		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			/*if (decorator != null) {
				decorator.Detach ();
				decorator = null;
			}*/
		}
		
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			if (nudgeHorizontal || nudgeVertical) {
				int x, y;
				this.GetPosition (out x, out y);
				int oldY = y, oldX = x;
				const int edgeGap = 2;
				
//				int w = allocation.Width;
//				
//				if (fitWidthToScreen && (x + w >= screenW - edgeGap)) {
//					int fittedWidth = screenW - x - edgeGap;
//					if (fittedWidth < minFittedWidth) {
//						x -= (minFittedWidth - fittedWidth);
//						fittedWidth = minFittedWidth;
//					}
//					LimitWidth (fittedWidth);
//				}
				
				Gdk.Rectangle geometry = Screen.GetMonitorGeometry (Screen.GetMonitorAtPoint (x, y));
				if (nudgeHorizontal) {
					if (allocation.Width <= geometry.Width && x + allocation.Width >= geometry.Width - edgeGap)
						x = geometry.Left + (geometry.Width - allocation.Height - edgeGap);
					if (x <= geometry.Left)
						x = geometry.Left;
				}
				
				if (nudgeVertical) {
					if (allocation.Height <= geometry.Height && y + allocation.Height >= geometry.Height - edgeGap)
						y = geometry.Top + (geometry.Height - allocation.Height - edgeGap);
					if (y <= geometry.Top)
						y = geometry.Top;
				}
				
				if (y != oldY || x != oldX)
					Move (x, y);
			}
			
			base.OnSizeAllocated (allocation);
		}
		
//		void LimitWidth (int width)
//		{
//			if (Child is MonoDevelop.Components.FixedWidthWrapLabel) 
//				((MonoDevelop.Components.FixedWidthWrapLabel)Child).MaxWidth = width - 2 * (int)this.BorderWidth;
//			
//			int childWidth = Child.SizeRequest ().Width;
//			if (childWidth < width)
//				WidthRequest = childWidth;
//			else
//				WidthRequest = width;
//		}
	}
}
