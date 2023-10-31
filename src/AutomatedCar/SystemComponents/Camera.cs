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
        

        public Camera(double ViewAngle, double ViewDistance)
        {
            ViewAngle = 60;
            ViewDistance = 80;
        }
        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

       
    }
}
