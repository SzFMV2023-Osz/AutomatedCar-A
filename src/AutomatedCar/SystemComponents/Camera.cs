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
        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int viewDistance) : base(virtualFunctionBus, automatedCar, distanceFromCarCenter, viewAngle, viewDistance)
        {
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }
    }
}
