using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

namespace VisualIntelligentScissors
{
    /*
     * My HeapPriorityQueue is a high speed priority queue gotten from by https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/
     */
    public class DijkstraScissors : Scissors
    {
        Pen drawer = new Pen(Color.Blue);

        public class Node : PriorityQueueNode
        {
            public Point point { get; set; }
            public int worth { get; set; }
            public Node last { get; set; }
            public Node(Point point, int worth, Node last)
            {
                this.point = point;
                this.worth = worth;
                this.last = last;
            }
        }


        public DijkstraScissors() { }
        /// <summary>
        /// constructor for intelligent scissors. 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="overlay"></param>
        public DijkstraScissors(GrayBitmap image, Bitmap overlay) : base(image, overlay) { }

        // this is the class you need to implement in CS 312

        /// <summary>
        /// this is the class you implement in CS 312. 
        /// </summary>
        /// <param name="points">these are the segmentation points from the pgm file.</param>
        /// <param name="pen">this is a pen you can use to draw on the overlay</param>
        public override void FindSegmentation(IList<Point> points, Pen pen)
        {
            if (Image == null) throw new InvalidOperationException("Set Image property first.");

            HeapPriorityQueue<Node> priorityQueue = new HeapPriorityQueue<Node>(Overlay.Width * Overlay.Height);

            ColorStartingPoints(points);

            for (int i = 0; i < points.Count; i++)
            {
                PathToPoints(priorityQueue, points[i], points[(i + 1) % points.Count]);
            }
        }

        private void PathToPoints(HeapPriorityQueue<Node> priorityQueue, Point start, Point end)
        {
            HashSet<Point> queue = new HashSet<Point>();

            priorityQueue.Enqueue(new Node(start, 0, null), 0);
            queue.Add(start);
            Boolean foundGoal = false;
            HashSet<Point> settledSet = new HashSet<Point>();
            Node finalNode = null;

            while (priorityQueue.Count != 0 && !foundGoal)
            {

                Node priorityNode = priorityQueue.Dequeue();
                queue.Remove(priorityNode.point);

                settledSet.Add(priorityNode.point);

                List<Node> neighs = GetNeighborNodes(priorityNode);

                foreach (Node neigh in neighs)
                {
                    if (neigh.point == end)
                    {
                        finalNode = neigh;
                        foundGoal = true;
                    }
                    else if (!settledSet.Contains(neigh.point) && !queue.Contains(neigh.point))
                    {
                        priorityQueue.Enqueue(neigh, neigh.worth);
                        queue.Add(neigh.point);
                    }
                }
            }

            priorityQueue.Clear();
            DralNodeTrace(finalNode);
        }

        private List<Node> GetNeighborNodes(Node priorityNode)
        {
            List<Node> neighs = new List<Node>();

            Node uNode = new Node(new Point(priorityNode.point.X, priorityNode.point.Y - 1), this.GetPixelWeight(new Point(priorityNode.point.X, priorityNode.point.Y - 1)) + priorityNode.worth, priorityNode);
            Node rNode = new Node(new Point(priorityNode.point.X + 1, priorityNode.point.Y), this.GetPixelWeight(new Point(priorityNode.point.X + 1, priorityNode.point.Y)) + priorityNode.worth, priorityNode);
            Node dNode = new Node(new Point(priorityNode.point.X, priorityNode.point.Y + 1), this.GetPixelWeight(new Point(priorityNode.point.X, priorityNode.point.Y + 1)) + priorityNode.worth, priorityNode);
            Node lNode = new Node(new Point(priorityNode.point.X - 1, priorityNode.point.Y), this.GetPixelWeight(new Point(priorityNode.point.X - 1, priorityNode.point.Y)) + priorityNode.worth, priorityNode);

            if (WithinPicture(uNode.point))
            {
                neighs.Add(uNode);
            }
            if (WithinPicture(rNode.point))
            {
                neighs.Add(rNode);
            }
            if (WithinPicture(dNode.point))
            {
                neighs.Add(dNode);
            }
            if (WithinPicture(lNode.point))
            {
                neighs.Add(lNode);
            }

            return neighs;
        }

        private Boolean WithinPicture(Point point)
        {
            return (point.X < (Overlay.Width - 2) && point.Y < (Overlay.Height - 2) && point.X > 1 && point.Y > 1);
        }

        private void ColorStartingPoints(IList<Point> points)
        {
            using (Graphics g = Graphics.FromImage(Overlay))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Point start = points[i];
                    g.DrawEllipse(drawer, start.X, start.Y, 5, 5);
                }
                Program.MainForm.RefreshImage();
            }
        }

        private void DralNodeTrace(Node finalNode)
        {
            Overlay.SetPixel(finalNode.point.X, finalNode.point.Y, Color.Red);
            if (finalNode.last != null)
            {
                DralNodeTrace(finalNode.last);
            }
        }
    }
}