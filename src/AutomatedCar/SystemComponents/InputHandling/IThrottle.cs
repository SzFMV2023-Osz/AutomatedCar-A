namespace AutomatedCar.SystemComponents.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IThrottle
    {
        /// <summary>
        /// Returns the state of the throttle pedal.
        /// </summary>
        /// <returns>Throttle pedal state</returns>
        public int GetThrottle();

        /// <summary>
        /// Sets the throttle pedal's value to an int between 0 and 100
        /// </summary>
        /// <param name="throttleValue">Throttle's value between 0 and 100</param>
        public void SetThrottle(int throttleValue);
    }
}
