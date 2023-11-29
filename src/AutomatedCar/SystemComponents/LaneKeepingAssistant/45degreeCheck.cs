namespace AutomatedCar.SystemComponents.LaneKeepingAssistant
{
    using AutomatedCar.SystemComponents.InputHandling;
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class _45degreeCheck : SystemComponent, IWheel
    {
        private LKAHandlerPacket lKAHandlerPacket;
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

        public _45degreeCheck(VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            this.lKAHandlerPacket = (LKAHandlerPacket)virtualFunctionBus.LKAHandlerPacket;
        }

        public static double IntToDegrees(int angleInt)
        {
            // +60 deg -> -60 deg
            return angleInt * 0.6;
        }

        public void CheckToTurnOffLKA(double? angleAsDegree)
        {
            if (angleAsDegree > 45 || angleAsDegree < -45)
            {
                this.lKAHandlerPacket.LKAOnOff = false;
                this.lKAHandlerPacket.Warning = false;
            }
            else if ((angleAsDegree <= 45 && angleAsDegree >= 30) || (angleAsDegree >= -45 && angleAsDegree <= -30))
            {
                this.lKAHandlerPacket.Warning = true;
            }
        }

        public override void Process()
        {
            this.CheckToTurnOffLKA(virtualFunctionBus.KeyboardHandlerPacket.WheelPercentage);
        }
    }
}
