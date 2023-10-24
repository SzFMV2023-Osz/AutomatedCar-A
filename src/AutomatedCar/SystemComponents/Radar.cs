namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Radar : Sensor
    {  
        //Would it be better to be implemented in the Sensor class?
        public WorldObject highlightedObject { get; private set; }

        public Points RadarPosition { get; set; }

        // It's not necessary, should be a protected setter be better? 
        public Radar (double ViewAngle, double ViewDistance) : base(ViewAngle, ViewDistance) 
        {
            ViewAngle = 60;
            ViewDistance = 200;
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

        //Is this calculation also needed in the camera class? 
        public void ClosestHighlightedObject(AutomatedCar car)
        {
            this.highlightedObject = currentObjectinView[0];
            for (int i = 0; i < currentObjectinView.Count-1; i++)
            {
                if (CalculateDistance(currentObjectinView[i].X,currentObjectinView[i].Y, car.X, car.Y) <= CalculateDistance(currentObjectinView[i+1].X, currentObjectinView[i + 1].Y, car.X, car.Y))
                {
                    this.highlightedObject = currentObjectinView[i];
                }
            }
        }

        private double CalculateDistance (double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow((xBCoordinate - xACoordinate), 2) + Math.Pow((yBCoordinate - yACoordinate), 2));
            return distance;
        }

        //figure out how the PolylineGeometry works
        public void VisualiseRadarVision()
        {
            //this.sensorTriangle
        }
    }
}
