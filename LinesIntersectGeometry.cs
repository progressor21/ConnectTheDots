using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConnectTheDots
{
    //How to check if two line segments intersect or are drawn not in the same direction from the same point - line geometry theory and algorithm(s):
    //https://algorithmtutor.com/Computational-Geometry/Check-if-two-line-segment-intersect/
    //https://www.tutorialspoint.com/Check-if-two-line-segments-intersect
    //https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/
    //Where do two line segments intersect? FindLinesIntersection() method:
    //https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/
    //https://rosettacode.org/wiki/Find_the_intersection_of_two_lines
    public class LinesIntersectGeometry
    {
        //Check if point/node p lies on a line segment L1 between the start and end nodes of this segment
        static Boolean onLine(Line l1, Point p)
        {   //Check whether p is on the line segment l1 or not

            //if (p.x <= Math.Max(l1.start.x, l1.end.x) && p.x <= Math.Min(l1.start.x, l1.end.x) &&
            //    p.y <= Math.Max(l1.start.y, l1.end.y) && p.y <= Math.Min(l1.start.y, l1.end.y))
            //    return true;

            if (p.x <= Math.Max(l1.start.x, l1.end.x) && p.x >= Math.Min(l1.start.x, l1.end.x) &&
                p.y <= Math.Max(l1.start.y, l1.end.y) && p.y >= Math.Min(l1.start.y, l1.end.y))
                return true;

            return false;
        }

        static int lineDirection(Point a, Point b, Point c)
        {
            int val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);

            //if (val == 0) 
            //    return 0;     //Collinear
            //else if (val < 0)
            //    return 2;    //Counter-clockwise direction
            //return 1;    //Clockwise direction

            return (val == 0) ? 0 : (val > 0 ? 1 : 2); // 0 - Collinear : 1 - Clockwise : 2 - Counter-clockwise 
        }

        public static Boolean isIntersection(Line l1, Line l2)
        {
            //Four direction for two lines and points of other line
            int dir1 = lineDirection(l1.start, l1.end, l2.start);
            int dir2 = lineDirection(l1.start, l1.end, l2.end);
            int dir3 = lineDirection(l2.start, l2.end, l1.start);
            int dir4 = lineDirection(l2.start, l2.end, l1.end);

            if (dir1 != dir2 && dir3 != dir4)
                return true; //They are intersecting

            if (dir1 == 0 && onLine(l1, l2.start)) //When p2 of line2 are on the line1
                return true;

            if (dir2 == 0 && onLine(l1, l2.end)) //When p1 of line2 are on the line1
                return true;

            if (dir3 == 0 && onLine(l2, l1.start)) //When p2 of line1 are on the line2
                return true;

            if (dir4 == 0 && onLine(l2, l1.end)) //When p1 of line1 are on the line2
                return true;

            return false;
        }

        public static PointF FindLinesIntersection(PointF s1, PointF e1, PointF s2, PointF e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;

            float delta = a1 * b2 - a2 * b1;

            //If lines are parallel, the result will be (NaN, NaN)

            return delta == 0 ? new PointF(float.NaN, float.NaN) : new PointF((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        //Check if 2 line segments of Line1 and Line2 started from the same start point/node are not drawn in the same direction
        //but rather in the different/opposite directions
        //Used this article to find a slope of a line: https://www.geeksforgeeks.org/program-find-slope-line/
        public static bool isNotSameDirection(Point startPoint, Point endLine1, Point endLine2)
        {
            if (startPoint.x == endLine1.x)
            {
                bool dirL1 = (endLine1.y - startPoint.y) > 0;
                bool dirL2 = (endLine2.y - startPoint.y) > 0;

                return (dirL1 != dirL2) ? true : false;

            }
            else if (startPoint.y == endLine1.y)
            {
                bool dirL1 = (endLine1.x - startPoint.x) > 0;
                bool dirL2 = (endLine2.x - startPoint.x) > 0;

                return (dirL1 != dirL2) ? true : false;
            }
            else
            {
                bool dirL1X = (endLine1.x - startPoint.x) > 0;
                bool dirL2X = (endLine2.x - startPoint.x) > 0;
                bool dirL1Y = (endLine1.y - startPoint.y) > 0;
                bool dirL2Y = (endLine2.y - startPoint.y) > 0;

                return (dirL1X != dirL2X && dirL1Y != dirL2Y) ? true : false;
            }
        }
    }

}
