using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;

namespace VisualIntelligentScissors
{
	public class SimpleScissors : Scissors
	{

        Pen yellowpen = new Pen(Color.Blue);

		public SimpleScissors() { }

        /// <summary>
        /// constructor for SimpleScissors. 
        /// </summary>
        /// <param name="image">the image you are going to segment including methods for getting gradients.</param>
        /// <param name="overlay">a bitmap on which you can draw stuff.</param>
		public SimpleScissors(GrayBitmap image, Bitmap overlay) : base(image, overlay) { }

        // this is a class you need to implement in CS 312. 

        /// <summary>
        ///  this is the class to implement for CS 312. 
        /// </summary>
        /// <param name="points">the list of segmentation points parsed from the pgm file</param>
        /// <param name="pen">a pen for writing on the overlay if you want to use it.</param>
		public override void FindSegmentation(IList<Point> points, Pen pen)
		{
            // this is the entry point for this class when the button is clicked for 
            // segmenting the image using the simple greedy algorithm. 
            // the points
            
			if (Image == null) throw new InvalidOperationException("Set Image property first.");


            ColorStartingPoints(points);

            for(int x = 0; x < points.Count; x++)
            {
                PathToPoints(points[x], points[(x + 1) % points.Count]);
            }

		}

        private void ColorStartingPoints(IList<Point> points)
        {
            using (Graphics g = Graphics.FromImage(Overlay))
            {
                for (int l = 0; l < points.Count; l++)
                {
                    Point starting = points[l];
                    g.DrawEllipse(yellowpen, starting.X, starting.Y, 5, 5);
                }
                Program.MainForm.RefreshImage();
            }
        }

        private void PathToPoints(Point start, Point end)
        {
            HashSet<Point> visited = new HashSet<Point>();
            Point currPoint = start;

            while(currPoint != end)
            {
                Overlay.SetPixel(currPoint.X, currPoint.Y, Color.Red);
                visited.Add(currPoint);

                int smallestPoint = int.MaxValue;

                Point uPoint = new Point(currPoint.X, currPoint.Y - 1);
                Point rPoint = new Point(currPoint.X + 1, currPoint.Y);
                Point dPoint = new Point(currPoint.X, currPoint.Y + 1);
                Point lPoint = new Point(currPoint.X - 1, currPoint.Y);

                int uWeight = this.GetPixelWeight(uPoint);
                int rWeight = this.GetPixelWeight(rPoint);
                int dWeight = this.GetPixelWeight(dPoint);
                int lWeight = this.GetPixelWeight(lPoint);

                if (WithinPicture(uPoint) && !visited.Contains(uPoint) && uWeight < smallestPoint)
                {
                    currPoint = uPoint;
                    smallestPoint = uWeight;
                }

                if (WithinPicture(rPoint) && !visited.Contains(rPoint) && rWeight < smallestPoint)
                {
                    currPoint = rPoint;
                    smallestPoint = rWeight;
                }

                if (WithinPicture(dPoint) && !visited.Contains(dPoint) && dWeight < smallestPoint)
                {
                    currPoint = dPoint;
                    smallestPoint = dWeight;
                }

                if (WithinPicture(lPoint) && !visited.Contains(lPoint) && lWeight < smallestPoint)
                {
                    currPoint = lPoint;
                    smallestPoint = lWeight;
                }

                if (smallestPoint == int.MaxValue)
                {
                    break;
                }
            }
        }
        private Boolean WithinPicture(Point p)
        {
            return (p.X < (Overlay.Width - 2) && p.Y < (Overlay.Height - 2) && p.X > 1 && p.Y > 1);
        }
	}
}
