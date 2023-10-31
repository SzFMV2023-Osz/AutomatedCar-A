namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Camera : Sensor
    {
        public int SensorPositionX {get;set;}
        public int CameraPositionY { get; set; }
        public WorldObject highlightedObjectCamera { get; set; }

        public Camera(double ViewAngle, double ViewDistance)
        {
            ViewAngle = 60;
            ViewDistance = 80;
        }
        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

        public void ClosestHighlightedObject(AutomatedCar car)
        {
            this.highlightedObjectCamera = currentObjectinView[0];
            for (int i = 0; i < currentObjectinView.Count - 1; i++)
            {
                if (CalculateDistance(currentObjectinView[i].X, currentObjectinView[i].Y, car.X, car.Y) <= CalculateDistance(currentObjectinView[i + 1].X, currentObjectinView[i + 1].Y, car.X, car.Y))
                {
                    this.highlightedObjectCamera = currentObjectinView[i];
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
