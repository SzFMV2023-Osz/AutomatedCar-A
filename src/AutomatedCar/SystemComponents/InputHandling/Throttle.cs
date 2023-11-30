namespace AutomatedCar.SystemComponents.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Throttle : IThrottle
    {
        private int throttle;

        public int GetThrottle()
        {
            return throttle;
        }


        public void SetThrottle(int throttleValue)
        {
            if (throttleValue >= 0
                && throttleValue <= 100)
            {
                throttle = throttleValue;
            }
        }
    }
}
