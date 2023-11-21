namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.SystemComponents.Engine;
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.InputHandling;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.Models;
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutomatedCar.SystemComponents.Packets.InputPackets;

    public class Powertrain : SystemComponent
    {
        public IWheel Wheel { get; set; }

        public IThrottle Throttle { get; set; }

        public IEngine Engine { get; set; }

        public IGearBox GearBox { get; set; }

        public IBrake Brake { get; set; }

        public MovementCalculator MovementCalculator { get; set; }

        public PowertrainPacket PowertrainPacket { get; set; }

        InputPacket inputPacket;
        InputPriorityHandler inputPriorityHandler = new InputPriorityHandler();

        public Powertrain(VirtualFunctionBus virtualFunctionBus, AutomatedCar car)
            : base(virtualFunctionBus)
        {
            this.MovementCalculator = new MovementCalculator(car);
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
                if (this.virtualFunctionBus.AEBInputPacket == null) {
                    this.virtualFunctionBus.AEBInputPacket = new AEBInputPacket();
                }

                if (this.virtualFunctionBus.LCCInputPacket == null) {
                    this.virtualFunctionBus.LCCInputPacket = new LCCInputPacket();
                }

                if (this.virtualFunctionBus.LKAInputPacket == null) {
                    this.virtualFunctionBus.LKAInputPacket = new LKAInputPacket();
                }

                this.inputPacket = this.inputPriorityHandler.GetInputs(this.virtualFunctionBus);
                int brakePercentage = (int)this.inputPacket.BrakePercentage;
                int wheelPercentage = (int)this.inputPacket.WheelPercentage;
                int throttlePercentage = (int)this.inputPacket.ThrottlePercentage;
                SequentialShiftingDirections shiftUpOrDown = (SequentialShiftingDirections)this.inputPacket.ShiftUpOrDown;

                this.Wheel.AngleAsDegree = wheelPercentage;
                this.Throttle.SetThrottle(throttlePercentage);
                this.Brake.SetBrake(brakePercentage);
                this.Engine.CalculateRPM();
                this.GearBox.ShiftingGear(shiftUpOrDown);

                this.PowertrainPacket.GearStage = this.GearBox.GearStage;
                this.PowertrainPacket.RPM = this.Engine.Revolution;
                this.PowertrainPacket.Speed = (int)this.GearBox.Speed;

                this.MovementCalculator.Process(brakePercentage, wheelPercentage, this.GearBox);
            }
        }
    }
}
