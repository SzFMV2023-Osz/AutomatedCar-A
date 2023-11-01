namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.SystemComponents.Engine;
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.InputHandling;
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Powertrain : SystemComponent
    {
        public IWheel Wheel { get; set; }

        public IThrottle Throttle { get; set; }

        public IEngine Engine { get; set; }

        public IGearBox GearBox { get; set; }

        public MovementCalculator MovementCalculator { get; set; }

        public PowertrainPacket PowertrainPacket { get; set; }

        public Powertrain(VirtualFunctionBus virtualFunctionBus) : base (virtualFunctionBus) { 
            this.Wheel = new Wheel();
            this.Throttle = new Throttle();
            this.GearBox = new ATGearBox();
            this.Engine = new Engine(this.GearBox, this.Throttle);
            this.PowertrainPacket = new PowertrainPacket();

        }

        public override void Process()
        {
            int brakePercentage = virtualFunctionBus.KeyboardHandlerPacket.BrakePercentage;
            int wheelPercentage = virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage;
            int velocity = this.GearBox.Velocity;
            MovementCalculator.Calculate(brakePercentage, wheelPercentage, velocity, this.PowertrainPacket);
            //MovementCalculator.UpdateCarPosition();
        }
    }
}
