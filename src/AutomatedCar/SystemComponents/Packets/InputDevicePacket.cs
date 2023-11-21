namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    /// <summary>
    /// Abstract class for a generic input device packet. Every input device packet should be created using this class.
    /// </summary>
    public abstract class InputDevicePacket : ReactiveObject, IInputDevicePacket
    {
        private int? brakePercentage;
        private int? throttlePercentage;
        private double? wheelPercentage;
        private SequentialShiftingDirections? shiftUpOrDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevicePacket"/> class.
        /// </summary>
        public InputDevicePacket()
        {
            this.brakePercentage = 0;
            this.throttlePercentage = 0;
            this.wheelPercentage = 0;
            this.shiftUpOrDown = 0;
        }

        /// <summary>
        /// Gets or sets the brake pedal's value.
        /// Value has to be between 0 (not pressed) and 100 (fully pressed).
        /// The packet class using this class should send null here if it does not affect the brake pedal (e.g. lane keeping assist)
        /// or the input device affecting it is inactive.
        /// </summary>
        public int? BrakePercentage
        {
            get => this.brakePercentage;
            set => this.RaiseAndSetIfChanged(ref this.brakePercentage, value);
        }

        /// <summary>
        /// Gets or sets the throttle pedal's value.
        ///  Value has to be between 0 (not pressed) and 100 (fully pressed).
        ///  The packet class using this class should send null here if it does not affect the throttle pedal (e.g. lane keeping assist)
        ///  or the input device affecting it is inactive.
        /// </summary>
        public int? ThrottlePercentage
        {
            get => this.throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref this.throttlePercentage, value);
        }

        /// <summary>
        /// Gets or sets the signal of the steering wheel.
        /// Value has to be -100 (all the way to the left, which is -60 degrees) and +100 (all the way to the right, which is +60 degrees). 0 if centered.
        /// The packet class using this class should send null here if it does not affect the steering wheel (e.g. autonomous emergency breaking)
        /// or the input device affecting it is inactive.
        /// </summary>
        public double? WheelPercentage
        {
            get => this.wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref this.wheelPercentage, value);
        }

        /// <summary>
        /// Gets or sets the signal of the automatic shifter.
        /// Value has to be -1 (shifting down), 0 (not shifting), +1 (shifting up).
        /// The packet class using this class should send null here if it does not affect the shifter (anything but the keyboard input)
        /// or the input device affecting it is inactive.
        /// </summary>
        public SequentialShiftingDirections? ShiftUpOrDown
        {
            get => this.shiftUpOrDown;
            set => this.RaiseAndSetIfChanged(ref this.shiftUpOrDown, value);
        }
    }
}
