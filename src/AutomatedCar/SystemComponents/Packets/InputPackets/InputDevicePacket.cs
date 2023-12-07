namespace AutomatedCar.SystemComponents.Packets.InputPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    /// <summary>
    /// Class for a generic input device packet. Every input device packet should be created using this class.
    /// </summary>
    public class InputDevicePacket : ReactiveObject, IReadOnlyInputDevicePacket
    {
        protected int? brakePercentage;
        protected int? throttlePercentage;
        protected double? wheelPercentage;
        protected SequentialShiftingDirections? shiftUpOrDown;

        /// <summary>
        /// Gets or sets the brake pedal's value.
        /// Value has to be between 0 (not pressed) and 100 (fully pressed).
        /// The packet class using this class should send null here if it does not affect the brake pedal (e.g. lane keeping assist)
        /// or the input device affecting it is inactive.
        /// </summary>
        public virtual int? BrakePercentage
        {
            get => brakePercentage;
            set => this.RaiseAndSetIfChanged(ref brakePercentage, value);
        }

        /// <summary>
        /// Gets or sets the throttle pedal's value.
        ///  Value has to be between 0 (not pressed) and 100 (fully pressed).
        ///  The packet class using this class should send null here if it does not affect the throttle pedal (e.g. lane keeping assist)
        ///  or the input device affecting it is inactive.
        /// </summary>
        public virtual int? ThrottlePercentage
        {
            get => throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref throttlePercentage, value);
        }

        /// <summary>
        /// Gets or sets the signal of the steering wheel.
        /// Value has to be -100 (all the way to the left, which is -60 degrees) and +100 (all the way to the right, which is +60 degrees). 0 if centered.
        /// The packet class using this class should send null here if it does not affect the steering wheel (e.g. autonomous emergency breaking)
        /// or the input device affecting it is inactive.
        /// </summary>
        public virtual double? WheelPercentage
        {
            get => wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref wheelPercentage, value);
        }

        /// <summary>
        /// Gets or sets the signal of the automatic shifter.
        /// Value has to be -1 (shifting down), 0 (not shifting), +1 (shifting up).
        /// The packet class using this class should send null here if it does not affect the shifter (anything but the keyboard input)
        /// or the input device affecting it is inactive.
        /// </summary>
        public virtual SequentialShiftingDirections? ShiftUpOrDown
        {
            get => shiftUpOrDown;
            set => this.RaiseAndSetIfChanged(ref shiftUpOrDown, value);
        }
    }
}
