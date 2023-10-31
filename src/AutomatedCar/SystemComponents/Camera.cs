namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Camera : Sensor


    {
        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar, 10, 60, 80)

        {
        }
        public override void Process()
        {
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
        }
        public void ObjectInRange (WorldObject worldObject)
        {
            this.currentObjectinView.Add(worldObject);
        }

       
    }
}
