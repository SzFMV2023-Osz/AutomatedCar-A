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
       

        public int RadarPositionX { get; set; }
        public int RadarPositionY { get; set; }
        

        // It's not necessary, should be a protected setter be better? 
        public Radar (double ViewAngle, double ViewDistance) 
        {
            ViewAngle = 60;
            ViewDistance = 200;
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

        //Is this calculation also needed in the camera class? 
        

        //figure out how the PolylineGeometry works
        public void VisualiseRadarVision()
        {
            //this.sensorTriangle
        }
        
    }
}
