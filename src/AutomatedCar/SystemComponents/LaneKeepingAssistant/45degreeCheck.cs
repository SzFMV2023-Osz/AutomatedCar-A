namespace AutomatedCar.SystemComponents.LaneKeepingAssistant
{
    using AutomatedCar.SystemComponents.InputHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class _45degreeCheck : IWheel
    {
        private double angleAsDegree;

        public double AngleAsDegree
        {
            get
            {
                return this.angleAsDegree;
            }

            set
            {
                if (value >= -60 && value <= 60)
                {
                    this.angleAsDegree = value;
                }
            }
        }

        public static double IntToDegrees(int angleInt)
        {
            // +60 deg -> -60 deg
            return angleInt * 0.6;
        }
        public static void CheckToTurnOffLKA(double angleAsDegree)
        {
            if (angleAsDegree>45 || angleAsDegree<-45)
            {

            }
        }
    }
}
