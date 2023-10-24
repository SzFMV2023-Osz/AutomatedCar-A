namespace AutomatedCar.SystemComponents.Packets.InputHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Brake
    {
        private int brake;

        public int GetBrake()
        {
            return this.brake;
        }


        public void SetBrake(int brakeValue)
        {
            if (brakeValue >= 0
                && brakeValue <= 100)
            {
                this.brake = brakeValue;
            }
        }
    }
}
