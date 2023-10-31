namespace AutomatedCar.SystemComponents.Engine
{
    /// <summary>
    /// The engine interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Gets or sets the engine revolution (RPM).
        /// </summary>
        int Revolution { get; set; }

        /// <summary>
        /// This method calculate the engine torque by gas pedal.
        /// </summary>
        /// <returns>The engine torque value (0-7000).</returns>
        int CalculateRPM();
    }
}