namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;

    /// <summary>
    /// Interface for a generic input device packet. Every input device should implement this interface for its packet.
    /// </summary>
    public interface IInputDevicePacket
    {
        /// <summary>
        /// Gets or sets the throttle pedal's value.
        ///  Value has to be between 0 (not pressed) and 100 (fully pressed).
        ///  The packet implementing this interface should send null here if it does not affect the throttle pedal (e.g. lane keeping assist)
        ///  or the input device affecting it is inactive.
        /// </summary>
        public int? ThrottlePercentage { get; set; }

        /// <summary>
        /// Gets or sets the brake pedal's value.
        /// Value has to be between 0 (not pressed) and 100 (fully pressed).
        /// The packet implementing this interface should send null here if it does not affect the brake pedal (e.g. lane keeping assist)
        /// or the input device affecting it is inactive.
        /// </summary>
        public int? BrakePercentage { get; set; }

        /// <summary>
        /// Gets or sets the signal of the steering wheel.
        /// Value has to be -100 (all the way to the left, which is -60 degrees) and +100 (all the way to the right, which is +60 degrees). 0 if centered.
        /// The packet implementing this interface should send null here if it does not affect the steering wheel (e.g. autonomous emergency breaking)
        /// or the input device affecting it is inactive.
        /// </summary>
        public double? WheelPercentage { get; set; }

        /// <summary>
        /// Gets or sets the signal of the automatic shifter.
        /// Value has to be -1 (shifting down), 0 (not shifting), +1 (shifting up).
        /// The packet class implementing this interface should send null here if it does not affect the shifter (anything but the keyboard input)
        /// or the input device affecting it is inactive.
        /// </summary>
        public SequentialShiftingDirections? ShiftUpOrDown { get; set; }
    }
}
