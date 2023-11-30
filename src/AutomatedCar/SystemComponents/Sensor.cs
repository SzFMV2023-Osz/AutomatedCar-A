namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ConstrainedExecution;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Controls.Shapes;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;

    public abstract class Sensor : SystemComponent
    {
        public event EventHandler Collided;

        public List<WorldObject> CurrentObjectsinView { get; protected set; }

        public Polygon SensorTriangle { get; private set; }

        public Point SensorPosition { get; set; }

        public WorldObject HighlightedObject { get; protected set; }

        public List<RelevantObject> previousObjectinView { get; protected set; }

        public AutomatedCar automatedCarForSensors { get; protected set; }

        public int viewAngle { get; protected set; }

        public int viewDistance { get; protected set; }

        public int distanceFromCarCenter { get; protected set; }

        public RelevantObjectsHandlerPacket RelevantObjectsPacket { get; set; }

        public Sensor(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus)
        {
            this.CurrentObjectsinView = new List<WorldObject>();
            this.previousObjectinView = new List<RelevantObject>();
            this.automatedCarForSensors = automatedCar;
            this.CreateSensorTriangle(automatedCarForSensors, distanceFromCarCenter, viewAngle, viewDistance);
        }

        public void ObjectsinViewUpdate(List<WorldObject> objects)
        {
            foreach (var obj in objects)
            {
                if (IsInTriangle(obj))
                {
                    if (!CurrentObjectsinView.Contains(obj))
                    {
                        CurrentObjectsinView.Add(obj);
                    }
                }
                else
                {
                    CurrentObjectsinView.Remove(obj);
                }
            }
        }

        private bool IsInTriangle(WorldObject obj)
        {
            foreach (var g in obj.Geometries)
            {
                foreach (var p in g.Points)
                {
                    Point point = GetTransformedPoint(p, obj);
                    if (SensorTriangle.DefiningGeometry.FillContains(point))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void CreateSensorTriangle(AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int range)
        {
            // car x : 480
            // car y : 1425
            int alpha = viewAngle / 2;

            // c means c side of a right-triangle
            int cSideLength = (int)(range / Math.Cos(alpha));

            Point point1 = new Point(
                           automatedCar.X + (int)(distanceFromCarCenter * Math.Cos(DegToRad(270 + automatedCar.Rotation))),
                           automatedCar.Y + (int)(distanceFromCarCenter * Math.Sin(DegToRad(270 + automatedCar.Rotation))));

            Point point2 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation + alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation + alpha)))));

            Point point3 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation - alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation - alpha)))));

            this.SensorPosition = point1;

            Polygon triangle = new Polygon();
            triangle.Points = new List<Point>
            {
                point1,
                point2,
                point3,
            };

            this.SensorTriangle = triangle;
        }

        protected static double DegToRad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        protected double CalculateDistance(double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow(xBCoordinate - xACoordinate, 2) + Math.Pow(yBCoordinate - yACoordinate, 2));
            return distance;
        }

        protected PolylineGeometry ActualizeGeometry(PolylineGeometry oldGeom, WorldObject obj)
        {
            List<Point> updatedPoints = new List<Point>();

            foreach (var item in oldGeom.Points)
            {
                Point updatedPoint = GetTransformedPoint(item, obj);

                updatedPoints.Add(updatedPoint);
            }

            return new PolylineGeometry(updatedPoints, false);
        }

        protected static Point GetTransformedPoint(Point geomPoint, WorldObject obj)
        {
            double angleInRad = DegToRad(obj.Rotation);

            Point transformedPoint;

            if (!obj.RotationPoint.IsEmpty)
            {
                Point offsettedPoint;
                if (obj.RenderTransformOrigin != "0,0" && obj.RenderTransformOrigin != null)
                {
                    // offset with the rotationPoint coordinate
                    offsettedPoint = new Point(geomPoint.X - (obj.RotationPoint.X * 2), geomPoint.Y - (obj.RotationPoint.Y * 2));
                }
                else
                {
                    // offset with the rotationPoint coordinate
                    offsettedPoint = new Point(geomPoint.X - obj.RotationPoint.X, geomPoint.Y - obj.RotationPoint.Y);

                }

                // now apply rotation
                double rotatedX = Math.Round(offsettedPoint.X * Math.Cos(angleInRad)) - (offsettedPoint.Y * Math.Sin(angleInRad));
                double rotatedY = Math.Round(offsettedPoint.X * Math.Sin(angleInRad)) + (offsettedPoint.Y * Math.Cos(angleInRad));

                // offset with the actual coordinate
                transformedPoint = new Point(rotatedX + obj.X, rotatedY + obj.Y);
            }
            else
            {
                // offset with the actual coordinate
                transformedPoint = new Point(geomPoint.X + obj.X, geomPoint.Y + obj.Y);
            }

            return transformedPoint;
        }

        protected double ClosestPointOfObject(Point p, WorldObject obj)
        {
            double minDistance = double.MaxValue;
            foreach (var geom in obj.Geometries)
            {
                PolylineGeometry tgeom = ActualizeGeometry(geom, obj);
                foreach (var point in tgeom.Points)
                {
                    double distance = CalculateDistance(p.X, p.Y, point.X, point.Y);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }

            return minDistance;
        }
    }
}