namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ATGearBox : IGearBox
    {
        private ATGears gear;
        public int Velocity { get; set; }
        public ATGears GearStage { get; private set; }
        public int CalculateGearSpeed(int revolution, int enginespeed)
        {
            switch (GearStage)
            {
                case ATGears.R:
                    return ReverseCalculate(revolution, enginespeed);
                case ATGears.N:
                    return NeutralCalculate(revolution, enginespeed);
                case ATGears.D:
                    return DriveCalculate(revolution, enginespeed);
                default:
                    return 1000;
            }
        }

        private int ReverseCalculate(int revolution, object enginespeed)
        {
            throw new NotImplementedException();
        }

        private int NeutralCalculate(int revolution, object enginespeed)
        {
            throw new NotImplementedException();
        }

        private int DriveCalculate(int revolution, object enginespeed)
        {
            throw new NotImplementedException();
        }

        public void ShiftingGear(GearShift shift)
        {
            if (Enum.IsDefined(typeof(ATGears), gear + ((int)shift)))
            {
                gear += (int)shift;
                GearStage = gear.ToString()[0].ToString();
            }
        }
    }
}
