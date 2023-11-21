namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;
    using System.IO;
    using AutomatedCar.Helpers.Gearbox_helpers;

    public class KeyboardHandlerPacket : ReactiveObject, IReadOnlyKeyboardHandlerPacket
    {

        /// <summary>
        /// Represents how far the brake pedal is being pushed down. 0 meaning not at all, 100 meaning fully pressed.
        /// </summary>
        private int brakePercentage;

        /// <summary>
        /// Represents how far the throttle pedal is being pushed down. 0 meaning not at all, 100 meaning fully pressed.
        /// </summary>
        private int throttlePercentage;

        /// <summary>
        /// Represents steering wheel rotation between values -100 and +100 (left to right). 0 means that the wheel is centered.
        /// </summary>
        private double wheelPercentage;

        /// <summary>
        /// Value can be:
        /// -1 meaning shift down,
        /// 0 meaning don't shift,
        /// +1 meaning shift up.
        /// </summary>
        private SequentialShiftingDirections shiftUpOrDown;

        public int BrakePercentage
        {
            get => brakePercentage;
            set => this.RaiseAndSetIfChanged(ref brakePercentage, value);
        }

        public int ThrottlePercentage
        {
            get => throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref throttlePercentage, value);
        }

        public double WheelPercentage
        {
            get => wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref wheelPercentage, value);
        }

        public SequentialShiftingDirections ShiftUpOrDown
        {
            get => shiftUpOrDown;
            set => this.RaiseAndSetIfChanged(ref shiftUpOrDown, value);
        }
    }
}
