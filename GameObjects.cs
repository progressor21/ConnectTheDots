using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectTheDots
{
    public class Point
    {
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Line
    {
        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }
        public Point start { get; set; }
        public Point end { get; set; }
    }

    public class Board
    {
        public Board()
        {
            // build 4 X 4 board
            const int size = 4;
            Points = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Point point = new Point(i, j);
                    Points.Add(point);
                }
            }
        }
        public List<Point> Points { get; set; }
    }

}
