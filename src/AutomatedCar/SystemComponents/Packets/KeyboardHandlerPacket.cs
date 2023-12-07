namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.Packets.InputPackets;
    using ReactiveUI;

    public class KeyboardHandlerPacket : InputDevicePacket, IReadOnlyKeyboardHandlerPacket
    {

        public KeyboardHandlerPacket()
        {
            this.brakePercentage = 0;
            this.throttlePercentage = 0;
            this.wheelPercentage = 0;
            this.shiftUpOrDown = 0;
        }
        public override int? BrakePercentage
        {
            get => this.brakePercentage;
            set {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.brakePercentage, value);
            }
        }

        public override int? ThrottlePercentage
        {
            get => this.throttlePercentage;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.throttlePercentage, value);
            }
        }

        public override double? WheelPercentage
        {
            get => this.wheelPercentage;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.wheelPercentage, value);
            }
        }

        public override SequentialShiftingDirections? ShiftUpOrDown
        {
            get => this.shiftUpOrDown;
            set
            {
                if (value != null)
                    this.RaiseAndSetIfChanged(ref this.shiftUpOrDown, value);
            }
        }
    }
}
