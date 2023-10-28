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
        public int sensorPositionX { get; protected set; }
        public int sensorPositionY { get; protected set; }

        public Radar (int radarPositionX, int radarPositionY)
        {
            this.viewAngle = 60;
            this.viewDistance = 200;
            this.sensorPositionX = radarPositionX;
            this.sensorPositionY = radarPositionY;
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

        //figure out how the PolylineGeometry works
        public void VisualiseRadarVision()
        {
            //this.sensorTriangle
        }
    }
}
