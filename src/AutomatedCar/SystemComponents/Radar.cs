namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Linq;
    using AutomatedCar.Models;

    internal class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 10;
            this.viewDistance = 200;
            this.viewAngle = 60;
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.CurrentObjectsinView.Add(worldObject);
        }

        public override void Process()
        {
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
        }

        //figure out how the PolylineGeometry works
        public void VisualiseRadarVision()
        {
            //this.sensorTriangle
        }
    }
}
