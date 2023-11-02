namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.SystemComponents.Engine;
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.InputHandling;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.Helpers.Gearbox_helpers;
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

        public IBrake Brake { get; set; }

        public MovementCalculator MovementCalculator { get; set; }

        public PowertrainPacket PowertrainPacket { get; set; }

        public Powertrain(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.MovementCalculator = new MovementCalculator();
            this.Wheel = new Wheel();
            this.Throttle = new Throttle();
            this.GearBox = new ATGearBox();
            this.Brake = new Brake();
            this.Engine = new Engine(this.GearBox, this.Throttle);
            this.PowertrainPacket = new PowertrainPacket();
            this.virtualFunctionBus.PowertrainPacket = this.PowertrainPacket;
        }

        public override void Process()
        {
            if (this.virtualFunctionBus.KeyboardHandlerPacket != null)
            {
                int brakePercentage = this.virtualFunctionBus.KeyboardHandlerPacket.BrakePercentage;
                int wheelPercentage = (int)this.virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage;
                int throttlePercentage = virtualFunctionBus.KeyboardHandlerPacket.ThrottlePercentage;
                SequentialShiftingDirections shiftUpOrDown = virtualFunctionBus.KeyboardHandlerPacket.ShiftUpOrDown;

                this.Wheel.AngleAsDegree = wheelPercentage;
                this.Throttle.SetThrottle(throttlePercentage);
                this.Brake.SetBrake(brakePercentage);
                this.Engine.CalculateRPM();
                this.GearBox.ShiftingGear(this.virtualFunctionBus.KeyboardHandlerPacket.ShiftUpOrDown);

                this.PowertrainPacket.GearStage = this.GearBox.GearStage;
                this.PowertrainPacket.RPM = this.Engine.Revolution;
                this.PowertrainPacket.Speed = (int)this.GearBox.Speed;

                this.MovementCalculator.Process(brakePercentage, wheelPercentage, this.GearBox);
            }
        }
    }
}
