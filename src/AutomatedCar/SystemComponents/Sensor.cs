namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    abstract class Sensor
    {
        public double viewAngle { get; protected set; }
        public double viewDistance { get; protected set; }
        public List<WorldObject> currentObjectinView { get; protected set; }
        public PolylineGeometry sensorTriangle { get;protected set; }
        public WorldObject highlightedObject { get; set; }

        public void SensorTriangleUpdate()
        {

        }
        public void ClosestHighlightedObject(AutomatedCar car)
        {
            this.highlightedObject = currentObjectinView[0];
            for (int i = 0; i < currentObjectinView.Count - 1; i++)
            {
                if (CalculateDistance(currentObjectinView[i].X, currentObjectinView[i].Y, car.X, car.Y) <= CalculateDistance(currentObjectinView[i + 1].X, currentObjectinView[i + 1].Y, car.X, car.Y))
                {
                    this.highlightedObject = currentObjectinView[i];
                }
            }
        }
        private double CalculateDistance(double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow((xBCoordinate - xACoordinate), 2) + Math.Pow((yBCoordinate - yACoordinate), 2));
            return distance;
        }



    }
}
