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
        private double[] gearRatios = { 0.002, 0.005, 0.01, 0.015, 0.0225, 0.027 };
        private int currentInsideGearStage = 0;
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
            revolution = ModifyRevolution(revolution, enginespeed);
            Velocity = -((revolution) / 200);
            return revolution;
        }

        private int NeutralCalculate(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }

            return SlowsDownRevolution(revolution);
        }

        private int DriveCalculate(int revolution, int enginespeed)
        {
            throw new NotImplementedException();
        }

        private int ModifyRevolution(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }
            else
            {
                if (Math.Abs(Velocity) != (int)((revolution) * gearRatios[currentInsideGearStage]))
                {
                    revolution = (int)(Math.Abs(Velocity) / gearRatios[currentInsideGearStage]);
                }

                return SlowsDownRevolution(revolution);
            }
        }

        private int CalculateRevolution(int revolution, int enginespeed)
        {
            double throttenmultiply = (1 - ((double)revolution) / (enginespeed));
            return (int)(revolution + (enginespeed * throttenmultiply) * gearRatios[gearRatios.Length - 1 - currentInsideGearStage] / 1.5);
        }

        private int SlowsDownRevolution(int revolution)
        {
            int newRevolution = revolution - revolution / (GearStage == ATGears.N ? 30 : 600);
            if (newRevolution < 1000)
            {
                return 1000;
            }
            else
            {
                return newRevolution;
            }
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
