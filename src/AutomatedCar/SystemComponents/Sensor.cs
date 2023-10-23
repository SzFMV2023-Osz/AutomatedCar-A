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
        public double viewAngle { get; set; }
        public double viewDistance { get; set; }
        public List<WorldObject> currentObjectinView { get; set; }
        public PolylineGeometry sensorTriangle { get; set; }

        public Sensor(double ViewAngle, double ViewDistance)
        {
            viewAngle = ViewAngle;
            viewDistance = ViewDistance;
        }
        public void SensorTriangleUpdate()
        {

        }


    }
}
