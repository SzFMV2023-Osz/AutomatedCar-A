namespace AutomatedCar.SystemComponents.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Brake : IBrake
    {
        private int brake;

        public int GetBrake()
        {
            return brake;
        }


        public void SetBrake(int brakeValue)
        {
            if (brakeValue >= 0
                && brakeValue <= 100)
            {
                brake = brakeValue;
            }
        }
    }
}
