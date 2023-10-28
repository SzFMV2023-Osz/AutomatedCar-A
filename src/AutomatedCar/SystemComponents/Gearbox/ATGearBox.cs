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

        private int ReverseCalculate(int revolution, int enginespeed)
        {
            throw new NotImplementedException();
        }

        private int NeutralCalculate(int revolution, int enginespeed)
        {
            throw new NotImplementedException();
        }

        private int DriveCalculate(int revolution, int enginespeed)
        {
            throw new NotImplementedException();
        }

        public void ShiftingGear(GearShift shift)
        {
            if (Enum.IsDefined(typeof(ATGears), GearStage + ((int)shift)))
            {
                GearStage += (int)shift;
            }
        }
    }
}
