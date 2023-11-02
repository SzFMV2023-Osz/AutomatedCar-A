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
            if (virtualFunctionBus.KeyboardHandlerPacket != null)
            {

                int brakePercentage = virtualFunctionBus.KeyboardHandlerPacket.BrakePercentage;
                int wheelPercentage = (int)virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage;
                int throttlePercentage = virtualFunctionBus.KeyboardHandlerPacket.ThrottlePercentage;
                SequentialShiftingDirections shiftUpOrDown = virtualFunctionBus.KeyboardHandlerPacket.ShiftUpOrDown;

                this.Wheel.AngleAsDegree = wheelPercentage;
                this.Throttle.SetThrottle(throttlePercentage);
                this.GearBox.ShiftingGear(shiftUpOrDown);
                this.Brake.SetBrake(brakePercentage);
                this.Engine.CalculateRPM();

                this.GearBox.Velocity = this.GearBox.Velocity - (int)(((double)this.Brake.GetBrake() / 100) * 1.6) * (this.GearBox.Velocity > 0 ? 1 : -1);
                int velocity = this.GearBox.Velocity;

                var asd = MovementCalculator.Calculate(brakePercentage, wheelPercentage, velocity);
                this.PowertrainPacket.MovementVector = asd.MovementVector;
                this.PowertrainPacket.Rotation = asd.Rotation;
                this.PowertrainPacket.RPM = this.Engine.Revolution;
                this.PowertrainPacket.Velocity = this.GearBox.Velocity;
                this.PowertrainPacket.GearStage = this.GearBox.GearStage;
                MovementCalculator.UpdateCarPosition(PowertrainPacket);
            }
        }
    }
}
