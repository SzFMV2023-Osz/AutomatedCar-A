﻿namespace AutomatedCar.SystemComponents.Packets.InputPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;

    /// <summary>
    /// Interface for a generic input device packet.
    /// </summary>
    public interface IReadOnlyInputDevicePacket
    {
        /// <summary>
        /// Gets the brake pedal's value.
        /// Value has to be between 0 (not pressed) and 100 (fully pressed).
        /// The packet class implementing this interface should send null here if it does not affect the brake pedal (e.g. lane keeping assist)
        /// or the input device affecting it is inactive.
        /// </summary>
        public int? BrakePercentage { get; }

        /// <summary>
        /// Gets the throttle pedal's value.
        ///  Value has to be between 0 (not pressed) and 100 (fully pressed).
        ///  The packet class implementing this interface should send null here if it does not affect the throttle pedal (e.g. lane keeping assist)
        ///  or the input device affecting it is inactive.
        /// </summary>
        public int? ThrottlePercentage { get; }

        /// <summary>
        /// Gets the signal of the steering wheel.
        /// Value has to be -100 (all the way to the left, which is -60 degrees) and +100 (all the way to the right, which is +60 degrees). 0 if centered.
        /// The packet class implementing this interface should send null here if it does not affect the steering wheel (e.g. autonomous emergency breaking)
        /// or the input device affecting it is inactive.
        /// </summary>
        public double? WheelPercentage { get; }

        /// <summary>
        /// Gets the signal of the automatic shifter.
        /// Value has to be -1 (shifting down), 0 (not shifting), +1 (shifting up).
        /// The packet class implementing this interface should send null here if it does not affect the shifter (anything but the keyboard input)
        /// or the input device affecting it is inactive.
        /// </summary>
        public SequentialShiftingDirections? ShiftUpOrDown { get; }
    }
}
