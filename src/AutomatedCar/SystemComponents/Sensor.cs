namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Controls.Shapes;

    internal abstract class Sensor : SystemComponent
    {
        public List<WorldObject> CurrentObjectsinView { get; protected set; }

        public Polygon SensorTriangle { get; set; }

        public WorldObject HighlightedObject { get; private set; }

        public Sensor(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int viewDistance)
            : base(virtualFunctionBus)
        {
            this.SensorTriangle = this.CreateSensorTriangle(automatedCar, distanceFromCarCenter, viewAngle, viewDistance);
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

        private Polygon CreateSensorTriangle(AutomatedCar automatedCar ,int distanceFromCarCenter, int viewAngle, int range)
        {
            // car x : 480
            // car y : 1425
            int alpha = viewAngle / 2;

            // c means c side of a right-triangle
            int cSideLength = (int)(range / Math.Cos(alpha));

            Point point1 = new Point(
                           automatedCar.X + (distanceFromCarCenter * (int)Math.Cos(DegToRad(90 + automatedCar.Rotation))),
                           automatedCar.Y + (distanceFromCarCenter * (int)Math.Sin(DegToRad(90 + automatedCar.Rotation))));

            Point point2 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(90 + automatedCar.Rotation + alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(90 + automatedCar.Rotation + alpha)))));

            Point point3 = new Point(
                          (int)(point1.X + (cSideLength * Math.Cos(DegToRad(90 + automatedCar.Rotation - alpha)))),
                          (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(90 + automatedCar.Rotation - alpha)))));

            Polygon triangle = new Polygon();
            triangle.Points = new List<Point>
            {
                point1,
                point2,
                point3,
            };

            return triangle;
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

    }
}
