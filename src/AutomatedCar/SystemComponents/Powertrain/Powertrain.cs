﻿namespace AutomatedCar.SystemComponents.Powertrain
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
            : base (virtualFunctionBus)
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
            int brakePercentage = virtualFunctionBus.KeyboardHandlerPacket.BrakePercentage;
            int wheelPercentage = virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage;
            int throttlePercentage = virtualFunctionBus.KeyboardHandlerPacket.ThrottlePercentage;
            GearShift shiftUpOrDown = virtualFunctionBus.KeyboardHandlerPacket.ShiftUpOrDown;

            this.Wheel.AngleAsDegree = wheelPercentage;
            this.Throttle.SetThrottle(throttlePercentage);
            this.GearBox.ShiftingGear(shiftUpOrDown);
            this.Brake.SetBrake(brakePercentage);

            int velocity = this.GearBox.Velocity;

            MovementCalculator.Calculate(brakePercentage, wheelPercentage, velocity, this.PowertrainPacket);
            MovementCalculator.UpdateCarPosition(PowertrainPacket);

        }
    }
}