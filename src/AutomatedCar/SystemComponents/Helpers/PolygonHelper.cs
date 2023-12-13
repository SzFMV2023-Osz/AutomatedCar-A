namespace AutomatedCar.SystemComponents.Helpers
{
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using Avalonia.Media;
    using Avalonia;

    public static class PolygonHelper
    {
        public static Point GetClosestPointOnPolygon(List<PolylineGeometry> polygon, Point point)
        {
            double minDistanceSquared = double.MaxValue;
            Point closestPoint = new Point();

            foreach (var polyline in polygon)
            {
                var points = polyline.Points;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Point p1 = points[i];
                    Point p2 = points[i + 1];

                    Point closest = GetClosestPointOnLine(p1, p2, point);
                    double distanceSquared = DistanceSquared(closest, point);

                    if (distanceSquared < minDistanceSquared)
                    {
                        minDistanceSquared = distanceSquared;
                        closestPoint = closest;
                    }
                }
            }

            return closestPoint;
        }

        static Point GetClosestPointOnLine(Point lineStart, Point lineEnd, Point point)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;

            if ((dx == 0) && (dy == 0))
            {
                // The line is just a point, return that point.
                return lineStart;
            }

            double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);

            t = Math.Max(0, Math.Min(1, t));

            double closestX = lineStart.X + t * dx;
            double closestY = lineStart.Y + t * dy;

            return new Point(closestX, closestY);
        }

        static double DistanceSquared(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }
    }
}
