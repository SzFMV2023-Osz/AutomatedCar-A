namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.Helpers.Gearbox_helpers;

    /// <summary>
    /// Interface for the transmission types.
    /// </summary>
    public interface IGearBox
    {
        /// <summary>
        /// Gets or sets the transmission Speed.
        /// </summary>
        int Velocity { get; set; }

        /// <summary>
        /// Gets current Gear Stage.
        /// </summary>
        ATGears GearStage { get; }

        /// <summary>
        /// This method can use to shift gears in transmission.
        /// </summary>
        /// <param name="shift"> The direction to shift.</param>
        void ShiftingGear(GearShift shift);

        /// <summary>
        /// This method use for calculate engine speed and Velocity.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns> The engine's new revolution. Calculated by the transmission stage.</returns>
        int CalculateGearSpeed(int revolution, int enginespeed);
    }
}
