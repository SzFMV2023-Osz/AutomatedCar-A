namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;

    /// <summary>
    /// This is the Automatic Transmission class.
    /// </summary>
    public class ATGearBox : IGearBox
    {
        private double[] gearRatios = { 0.002, 0.005, 0.01, 0.015, 0.0225, 0.027 };
        private int currentInsideGearStage = 0;
        private int nextLowRevolutionChangeValue = 1000;
        public int Velocity { get; set; } // velocity = revolution * gearRation
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

        /// <summary>
        /// This method calculate the Reverse mode velocity and engine revolution.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns>The engine's new revolution.</returns>
        private int ReverseCalculate(int revolution, int enginespeed)
        {
            revolution = ModifyRevolution(revolution, enginespeed);
            Velocity = -((revolution - 1000) / 200);
            return revolution;
        }

        /// <summary>
        /// This method calculate the Neutral mode engine revolution.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns>The engine's new revolution.</returns>
        private int NeutralCalculate(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }

            return SlowsDownRevolution(revolution);
        }

        /// <summary>
        /// This method calculate the Drive mode velocity and engine revolution.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns>The engine's new revolution.</returns>
        private int DriveCalculate(int revolution, int enginespeed)
        {
            if ((revolution > nextLowRevolutionChangeValue || currentInsideGearStage == 1) && (revolution <= 4000 || currentInsideGearStage == gearRatios.Length - 1))
            {
                revolution = ModifyRevolution(revolution, enginespeed);
            }
            else if (revolution <= nextLowRevolutionChangeValue) 
            {
                // changing to lower gears
                --currentInsideGearStage;
                revolution = (int)(Velocity / gearRatios[currentInsideGearStage]) + 1000;
                if (currentInsideGearStage - 1 > 0)
                {
                    // if not in first gear
                    nextLowRevolutionChangeValue = (int)(4000 * gearRatios[currentInsideGearStage - 1] / gearRatios[currentInsideGearStage]) - 500;
                }
                else
                {
                    // in a first gear
                    nextLowRevolutionChangeValue = 1000;
                }
            }
            else 
            {
                // changing to higher gears
                ++currentInsideGearStage;
                revolution = (int)(Velocity / gearRatios[currentInsideGearStage]) + 1000;
                nextLowRevolutionChangeValue = revolution - 500;
            }

            Velocity = (int)((revolution - 1000) * gearRatios[currentInsideGearStage]);
            return revolution;
        }

        /// <summary>
        /// This method calculate the engine revolution.
        /// Monitors velocity changes (if changed calculate a new revolution based by velocity).
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns>The engine's new revolution.</returns>
        private int ModifyRevolution(int revolution, int enginespeed)
        {
            if (enginespeed > 0 && (revolution < enginespeed + 1000))
            {
                return CalculateRevolution(revolution, enginespeed);
            }
            else
            {
                if (Math.Abs(Velocity) != (int)((revolution - 1000) * gearRatios[currentInsideGearStage])) // aka has break input
                {
                    revolution = (int)(Math.Abs(Velocity) / gearRatios[currentInsideGearStage]) + 1000;
                }

                return SlowsDownRevolution(revolution);
            }
        }

        /// <summary>
        /// Calculate the engine revolution by currently gearstage.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <param name="enginespeed"> The engine torque by gas pedal.</param>
        /// <returns>The engine's new revolution.</returns>
        private int CalculateRevolution(int revolution, int enginespeed)
        {
            double throttenmultiply = (1 - ((double)revolution) / (enginespeed + 1000));
            return (int)(revolution + (enginespeed * throttenmultiply) * gearRatios[gearRatios.Length - 1 - currentInsideGearStage] / 1.5);
        }

        // throttenmultiply -> normally the value is between (0,1) , if the torque is higher then revolution it can be negative number, but this was not used.
        // return value -> is similar for f(x) = -1/x
        // gearRatios[gearRatios.Length - 1 - currentInsideGearStage] / 1.5 --> time controling number, how long takes to convergate to borders, invert gearratio (because it is work fine XD)

        /// <summary>
        /// Slow-down the engine revolution.
        /// </summary>
        /// <param name="revolution">The currently engine revolution.</param>
        /// <returns>The engine's new revolution.</returns>
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
            ATGears nextGearStage = GearStage + ((int)shift);
            if (Enum.IsDefined(typeof(ATGears), GearStage + ((int)shift)))
            {
                if (GearStage == ATGears.R)
                {
                    if (nextGearStage == ATGears.N) // if shifting to the Neutral mode
                    {
                        GearStage = nextGearStage;
                        currentInsideGearStage = 0;
                    }
                    else if (Velocity == 0) // if shifting to the Park mode
                    {
                        GearStage = nextGearStage;
                    }
                    return;
                }
                else if (GearStage == ATGears.N)
                {
                    // for to not change direction in moving immediately while changing from Neutral
                    if ((nextGearStage == ATGears.R && Velocity <= 0) || (nextGearStage == ATGears.D && Velocity >= 0))
                    {
                        GearStage = nextGearStage;
                        currentInsideGearStage = 1;
                    }
                }
                else // Park -> Reverse, Drive -> Neutral
                {
                    GearStage = nextGearStage;
                    if (GearStage == ATGears.N)
                    {
                        currentInsideGearStage = 0;
                    }
                    else
                    {
                        currentInsideGearStage = 1;
                    }
                }
            }
        }
    }
}
