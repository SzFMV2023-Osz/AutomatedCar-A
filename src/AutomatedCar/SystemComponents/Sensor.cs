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

        
        public void SensorTriangleUpdate()
        {

        }
        

        

    }
}
