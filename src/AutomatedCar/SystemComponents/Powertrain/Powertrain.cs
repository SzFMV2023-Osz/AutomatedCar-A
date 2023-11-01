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

        // public MovementVector MovementVector { get; set; }

        public IMovementVectorPacket MovementVectorPacket { get; set; }

        public Powertrain(VirtualFunctionBus virtualFunctionBus) : base (virtualFunctionBus) { 
            this.Wheel = new Wheel();
            this.Throttle = new Throttle();
            this.GearBox = new ATGearBox();
            this.Engine = new Engine(this.GearBox, this.Throttle);
            this.MovementVectorPacket = new MovementVectorPacket();

        }

        public override void Process()
        {
            (this.MovementVectorPacket as MovementVectorPacket).MovementVector = (0, 0);
        }
    }
}
