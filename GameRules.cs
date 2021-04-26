using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConnectTheDots
{
    class GameRules
    {
        // Checking if the line is an octilinear line - a horizontal, vertical, or 45° diagonal line and if an END_NODE is not the same as the START_NODE
        public static bool IsOctilinearLine(Point start, Point end)
        {
            return ((start.x == end.x || start.y == end.y || Math.Abs(start.x - end.x) == Math.Abs(start.y - end.y)) && (!(start.x == end.x && start.y == end.y))) ? true : false;
        }

        //Checking for lines intersections and return true as long as new line does not violate game rules: Lines may not intersect
        //except for at the start point of the new line segment they can - it is a valid point of intersection!
        public static bool CheckLinesDoNotIntersect(Line tryNewLine, List<Line> existingLines)
        {
            //Check each existing line segment for intersection with new line segment
            foreach (Line line in existingLines)
            {
                bool linesIntersect = LinesIntersectGeometry.isIntersection(tryNewLine, line);

                //If there is an intersection, check if the intersection is at the start point of the new line segment, because this intersection is OK for Game Rules.
                //Note: the FindIntersection method only works on lines which have a different slope.

                if (linesIntersect)
                {
                    Func<float, float, PointF> p = (x, y) => new PointF(x, y);

                    PointF intersect = LinesIntersectGeometry.FindLinesIntersection(p((float)tryNewLine.start.x, (float)tryNewLine.start.y), p((float)tryNewLine.end.x, (float)tryNewLine.end.y), p((float)line.start.x, (float)line.start.y), p((float)line.end.x, (float)line.end.y));
                    if (intersect.X == (float)tryNewLine.start.x && intersect.Y == (float)tryNewLine.start.y)
                    {
                        continue;
                    }
                    else if (double.IsNaN(intersect.X))
                    {
                        Point intersectPoint = new Point(tryNewLine.start.x, tryNewLine.start.y);
                        Point tryLineEndPoint = new Point(tryNewLine.end.x, tryNewLine.end.y);
                        Point existingLineEndPoint = new Point(0, 0);
                        if (line.start.x == intersectPoint.x && line.start.y == intersectPoint.y)
                        {
                            existingLineEndPoint.x = line.end.x;
                            existingLineEndPoint.y = line.end.y;
                        }
                        else
                        {
                            existingLineEndPoint.x = line.start.x;
                            existingLineEndPoint.y = line.start.y;
                        }

                        bool newLineSameDir = LinesIntersectGeometry.isNotSameDirection(intersectPoint, tryLineEndPoint, existingLineEndPoint);

                        if (newLineSameDir == true)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            //Check each existing line segment for intersection with new line segment
            return true;
        }

        //Returns true if there is no valid node clicks/moves left on the board and board is compete
        public static bool BoardIsComplete(Board board, List<Line> existingLines, List<Point> validStartNodes)
        {
            foreach (Point startNode in validStartNodes)
            {
                foreach (Point point in board.Points)
                {
                    Line tryNewLine = new Line(startNode, point);
                    bool lineIsOctilinear = IsOctilinearLine(startNode, point);
                    bool noInvalidIntersect = CheckLinesDoNotIntersect(tryNewLine, existingLines);
                    //bool noInvalidIntersect = NoInvalidIntersect(tryLine, existingLines);

                    if (lineIsOctilinear && noInvalidIntersect)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

}
