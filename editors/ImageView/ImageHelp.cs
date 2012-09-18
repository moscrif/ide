using System;
using Cairo;

namespace  Moscrif.IDE.Editors.ImageView
{
	public static class ImageHelp
	{
		public static Rectangle DrawRectangle (this Cairo.Context g, Cairo.Rectangle r, Cairo.Color color, int lineWidth)
		{
			// Put it on a pixel line
			if (lineWidth == 1)
				r = new Rectangle (r.X - 0.5, r.Y - 0.5, r.Width, r.Height);

			g.Save ();

			g.MoveTo (r.X, r.Y);
			g.LineTo (r.X + r.Width, r.Y);
			g.LineTo (r.X + r.Width, r.Y + r.Height);
			g.LineTo (r.X, r.Y + r.Height);
			g.LineTo (r.X, r.Y);

			g.Color = color;
			g.LineWidth = lineWidth;
			g.LineCap = LineCap.Square;

			Rectangle dirty = g.StrokeExtents ();
			g.Stroke ();

			g.Restore ();

			return dirty;
		}

		public static Gdk.Rectangle GetBounds (this ImageSurface surf)
		{
			return new Gdk.Rectangle (0, 0, surf.Width, surf.Height);
		}

		public static Cairo.Color ToCairoColor (this Gdk.Color color)
		{
		        return new Cairo.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue, (double)color.Blue / ushort.MaxValue);
		}

		public static Cairo.Color ToCairoColor (this Gdk.Color color, double alpha )
		{
		        return new Cairo.Color ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue, (double)color.Blue / ushort.MaxValue,(double)alpha / ushort.MaxValue);
		}

		public static Gdk.Color ToGdkColor (this Cairo.Color color)
		{
		        Gdk.Color c = new Gdk.Color ();
		        c.Blue = (ushort)(color.B * ushort.MaxValue);
		        c.Red = (ushort)(color.R * ushort.MaxValue);
		        c.Green = (ushort)(color.G * ushort.MaxValue);
		
		        return c;
		}
		
	}
}

