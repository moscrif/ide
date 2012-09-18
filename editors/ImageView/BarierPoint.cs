using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace Moscrif.IDE.Editors.ImageView
{
	//[JsonObject(MemberSerialization.OptIn)]
	public class BarierPoint
	{
		int x, y, z;

		public BarierPoint()
		{
		}

		public BarierPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		//[JsonProperty(PropertyName = "X" ) ]
		public int X
		{
			get { return x; }
			set { x = value; }
		}


		//[JsonProperty(PropertyName = "Y" ) ]
		public int Y
		{
			get { return y; }
			set { y = value; }
		}

		public int Z
		{
			get { return z; }
			set { z = value; }
		}

		public static double Distance(BarierPoint p1, BarierPoint p2)
		{
			double xDist = p1.X - p2.X;
			double yDist = p1.Y - p2.Y;
			return Math.Sqrt(xDist * xDist + yDist * yDist);
		}

		public static BarierPoint ClosestPoint(BarierPoint p, IList<BarierPoint> points)
		{
			double shortestDistance = Double.PositiveInfinity;
			BarierPoint closestPoint = null;
			foreach (var point in points) {
				double distance = Distance(p, point);
				if (distance < shortestDistance) {
					shortestDistance = distance;
					closestPoint = point;
				}
			}
			return closestPoint;
		}

		public static BarierPoint LastPoint(IList<BarierPoint> points)
		{
			if(points == null || points.Count<1) return null;

			BarierPoint lastPoint = points[0];
			foreach (var point in points) {
					if (lastPoint.Z  < point.Z) {
					lastPoint = point;
				}
			}
			return lastPoint;
		}

		public static int ClosestLine(BarierPoint p, IList<BarierPoint> points )//, out BarierPoint a, out BarierPoint b)
		{
			double shortestDistance = Double.PositiveInfinity;
			int indexOfPoint = -1;
			//foreach (var point in points) {
			for (int i = 0; i < points.Count; i++) {

				BarierPoint firstPoint = points[i];
				BarierPoint secondPoint ;

				if(i== points.Count-1)
					secondPoint =points[0];
				else secondPoint = points[i+1];

				double distance =FindDistanceToSegment(p,firstPoint,secondPoint);
				if (distance < shortestDistance) {
					shortestDistance = distance;
					//a = firstPoint;
					//b = secondPoint;
					indexOfPoint = i+1;
				}
			}
			return indexOfPoint;
		}

	        private static double FindDistanceToSegment(BarierPoint pt, BarierPoint p1, BarierPoint p2)//, out PointF closest)
	        {
		    BarierPoint closest = new BarierPoint();
	            float dx = p2.X - p1.X;
	            float dy = p2.Y - p1.Y;
	            if ((dx == 0) && (dy == 0))
	            {
	                // It's a point not a line segment.
	                closest = p1;
	                dx = pt.X - p1.X;
	                dy = pt.Y - p1.Y;
	                return Math.Sqrt(dx * dx + dy * dy);
	            }
	
	            // Calculate the t that minimizes the distance.
	            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);
	
	            // See if this represents one of the segment's
	            // end points or a point in the middle.
	            if (t < 0)
	            {
	                closest = new BarierPoint(p1.X, p1.Y);
	                dx = pt.X - p1.X;
	                dy = pt.Y - p1.Y;
	            }
	            else if (t > 1)
	            {
	                closest = new BarierPoint(p2.X, p2.Y);
	                dx = pt.X - p2.X;
	                dy = pt.Y - p2.Y;
	            }
	            else
	            {
	                closest = new BarierPoint((int)(p1.X + t * dx), (int)(p1.Y + t * dy));
	                dx = pt.X - closest.X;
	                dy = pt.Y - closest.Y;
	            }
	
	            return Math.Sqrt(dx * dx + dy * dy);
	        }

	}
}

