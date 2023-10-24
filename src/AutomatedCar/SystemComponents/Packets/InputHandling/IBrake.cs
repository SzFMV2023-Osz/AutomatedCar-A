namespace AutomatedCar.SystemComponents.Packets.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBrake
    {
        /// <summary>
        /// Returns the state of the brake pedal.
        /// </summary>
        /// <returns>Brake pedal state</returns>
        public int GetBrake();

        /// <summary>
        /// Sets the brake pedal's value to an int between 0 and 100
        /// </summary>
        /// <param name="brakeValue">Brake's value between 0 and 100</param>
        public void SetBrake(int brakeValue);
    }
}
