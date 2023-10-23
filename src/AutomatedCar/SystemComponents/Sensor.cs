namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    interface Sensor
    {
        public double viewAngle { get; set; }
        public double viewDistance { get; set; }
        public List<WorldObject> currentObjectinView { get; set; }
        public PolylineGeometry sensorTriangle { get; set; }

    }
}
