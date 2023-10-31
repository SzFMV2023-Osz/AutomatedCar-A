namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Controls.Shapes;
    using Avalonia.Media;

    internal abstract class Sensor : SystemComponent
    {
        public List<WorldObject> CurrentObjectsinView { get; protected set; }

        public Polygon SensorTriangle { get; private set; }

        public WorldObject HighlightedObject { get; private set; }
        
        public List<WorldObject> currentObjectinView { get; protected set; }
        
        public List<WorldObject> previousObjectinView { get; protected set; }
        
        public List<RelevantObject> previousRelevant { get; protected set; }

        public Sensor(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int viewDistance)
            : base(virtualFunctionBus)
        {
            this.CreateSensorTriangle(automatedCar, distanceFromCarCenter, viewAngle, viewDistance);
            this.CurrentObjectsinView = new List<WorldObject>();
        }

        public void ObjectsinViewUpdate(List<WorldObject> objects)
        {
            foreach (var obj in objects)
            {
                Points points = new Points();
                Console.WriteLine(obj.Filename);
                foreach (var geom in obj.Geometries)
                {
                    points.AddRange(geom.Points);
                }

                if (IsInTriangle(points))
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

        private bool IsInTriangle(Points points)
        {
            var triPoints = this.SensorTriangle.Points;
            foreach (var point in points)
            {
                double A = area(triPoints[0].X, triPoints[0].Y, triPoints[1].X, triPoints[1].Y, triPoints[2].X, triPoints[2].Y);

                double A1 = area((double)point.X, (double)point.Y, triPoints[1].X, triPoints[1].Y, triPoints[2].X, triPoints[2].Y);

                double A2 = area(triPoints[0].X, triPoints[0].Y, (double)point.X, (double)point.Y, triPoints[2].X, triPoints[2].Y);

                double A3 = area(triPoints[0].X, triPoints[0].Y, triPoints[1].X, triPoints[1].Y, (double)point.X, (double)point.Y);
                if (A == A1 + A2 + A3)
                {
                    return true;
                }
            }

            return false;
        }

        private double area(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
        }

        public void ClosestHighlightedObject(AutomatedCar car)
        {
            this.HighlightedObject = this.CurrentObjectsinView[0];
            for (int i = 0; i < this.CurrentObjectsinView.Count - 1; i++)
            {
                if (this.CalculateDistance(this.CurrentObjectsinView[i].X, this.CurrentObjectsinView[i].Y, car.X, car.Y)
                    <= this.CalculateDistance(this.CurrentObjectsinView[i + 1].X, this.CurrentObjectsinView[i + 1].Y, car.X, car.Y))
                {
                    this.HighlightedObject = this.CurrentObjectsinView[i];
                }
            }
        }

        private void CreateSensorTriangle(AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int range)
        {
            // car x : 480
            // car y : 1425
            int alpha = viewAngle / 2;

            // c means c side of a right-triangle
            int cSideLength = (int)(range / Math.Cos(alpha));

            Point point1 = new Point(
                           automatedCar.X + (int)(distanceFromCarCenter * Math.Cos(DegToRad(90 - automatedCar.Rotation))),
                           automatedCar.Y + (int)(distanceFromCarCenter * Math.Sin(DegToRad(90 - automatedCar.Rotation))));

            Point point2 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation + alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation + alpha)))));

            Point point3 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation - alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation - alpha)))));

            Polygon triangle = new Polygon();
            triangle.Points = new List<Point>
            {
                point1,
                point2,
                point3,
            };

            this.SensorTriangle = triangle;
        }

        private static double DegToRad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double CalculateDistance(double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow(xBCoordinate - xACoordinate, 2) + Math.Pow(yBCoordinate - yACoordinate, 2));
            return distance;
        }

        // Returns relevant objects
        public List<RelevantObject> RelevantObjects()
        {
            List<RelevantObject> ReturnRelevants = new List<RelevantObject>();
            List<WorldObject> currentlyRelevant = new List<WorldObject>();

            foreach (WorldObject relevantobj in this.currentObjectinView)
            {
                if (previousObjectinView.Contains(relevantobj))
                {
                    currentlyRelevant.Add(relevantobj);
                }
            }

            foreach (RelevantObject relevantobj2 in previousRelevant)
            {
                foreach (WorldObject WO in currentlyRelevant)
                {
                    if (relevantobj2.RelevantWorldObject.Equals(WO) && relevantobj2.CurrentDistance > this.examDistance(WO))
                    {
                        //relevantobj2.modifyPreviousDistance(relevantobj2.CurrentDistance);
                        relevantobj2.modifyCurrentDistance(this.examDistance(WO));
                        ReturnRelevants.Add(relevantobj2);
                    }
                    else
                    {
                        currentlyRelevant.Remove(WO);
                        this.previousRelevant.Remove(relevantobj2);
                    }
                }
            }

            return ReturnRelevants;
        }

        // Returns a RelevantObjects distance
        public double examDistance(WorldObject WO)
        {
            foreach (RelevantObject rev in this.previousRelevant)
            {
                if (rev.RelevantWorldObject.Equals(WO))
                {
                    return rev.CurrentDistance;
                }
            }

            return 999; // akkorát adunk visza, hogy mindenképp nagyobb legyen
        }
    }
}
