namespace AutomatedCar.SystemComponents.ADAS
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;

    public enum AEBStatus
    {
        Off,
        PartialBrake1,
        PartialBrake2,
        FullBrake
    }

    class AEB : SystemComponent
    {
        private bool activateFCW;

        private AEBStatus status;



        /// <summary>
        /// First-stage partial braking in m/s2.
        /// </summary>
        private const double PB1_DECELERATION = 4;

        /// <summary>
        /// Second-stage partial braking in m/s2.
        /// </summary>
        private const double PB2_DECELERATION = 6;

        /// <summary>
        /// Full braking in m/s2.
        /// </summary>
        private const double FB_DECELERATION = 9;

        public AEBInputPacket AEBPacket { get; set; }

        public AEB(VirtualFunctionBus virtualFunctionBus)
            : base (virtualFunctionBus)
        {
            this.AEBPacket = new AEBInputPacket();
            this.virtualFunctionBus.AEBInputPacket = this.AEBPacket;
        }

        public override void Process()
        {
            int egoSpeed = this.virtualFunctionBus.PowertrainPacket.Speed;
            List<RelevantObject> relevantObjects = this.virtualFunctionBus.RadarPacket.RelevantObjects;
            foreach (var obj in relevantObjects)
            {
                if (this.status != AEBStatus.Off)
                {
                    break;
                }

                // calculate speed from previos and current distance
                int relativeSpeed = this.CalculateSpeedForRelevantObject(obj);
                double driverDeceleration = 0; // to calculate ?
                int driverReactionTime = 5;
                this.AEBLogic(obj.CurrentDistance, relativeSpeed, egoSpeed, driverDeceleration, driverReactionTime);

                this.AEBPacket.WarningAvoidableCollision = this.activateFCW;
                this.AEBPacket.Status = this.status;

                switch (this.status)
                {
                    case AEBStatus.PartialBrake1:
                        // calculate and set AEBPacket.BrakePercentage according to PB1_DECELERATION
                        break;
                    case AEBStatus.PartialBrake2:
                        // calculate and set AEBPacket.BrakePercentage according to PB2_DECELERATION
                        break;
                    case AEBStatus.FullBrake:
                        // calculate and set AEBPacket.BrakePercentage according to FB_DECELERATION
                        break;
                    default:
                        break;
                }
            }

            this.AEBPacket.WarningOver70kmph = egoSpeed > 70 ? true : false;
        }

        private int CalculateSpeedForRelevantObject(RelevantObject relevantObject)
        {
            return (int)((relevantObject.PreviousDistance - relevantObject.CurrentDistance) / (1 / 60));
        }

        /// <summary>
        /// Calculates Time-To-Collision.
        /// MIO: Most Important Object.
        /// </summary>
        /// <param name="mioDistance">Distance from MIO in meters.</param>
        /// <param name="mioSpeed">Relative speed of MIO in m/s.</param>
        /// <returns>Time-To-Collision in seconds.</returns>
        private int CalculateTTC(double mioDistance, int mioSpeed)
        {
            return (int)(mioDistance / mioSpeed);
        }

        internal struct StoppingTime
        {
            internal int FCWStoppingTime;
            internal int PB1StoppingTime;
            internal int PB2StoppingTime;
            internal int FBStoppingTime;
        }

        private StoppingTime CalculateStoppingTimes(int egoSpeed, double driverDeceleration, int driverReactionTime)
        {
            StoppingTime stoppingTimes = new StoppingTime();

            stoppingTimes.FCWStoppingTime = (int)(driverReactionTime + (egoSpeed / driverDeceleration));
            stoppingTimes.PB1StoppingTime = (int)(egoSpeed / PB1_DECELERATION);
            stoppingTimes.PB2StoppingTime = (int)(egoSpeed / PB2_DECELERATION);
            stoppingTimes.FBStoppingTime = (int)(egoSpeed / FB_DECELERATION);

            return stoppingTimes;
        }

        private void AEBLogic(double mioDistance, int mioSpeed, int egoSpeed, double driverDeceleration, int driverReactionTime)
        {
            int TTC = this.CalculateTTC(mioDistance, mioSpeed);
            StoppingTime stoppingTimes = this.CalculateStoppingTimes(egoSpeed, driverDeceleration, driverReactionTime);

            if (TTC < stoppingTimes.PB2StoppingTime)
            {
                this.status = AEBStatus.FullBrake;
            }
            else if (TTC < stoppingTimes.PB1StoppingTime)
            {
                this.status = AEBStatus.PartialBrake2;
            }
            else if (TTC < stoppingTimes.FCWStoppingTime - driverReactionTime)
            {
                this.status = AEBStatus.PartialBrake1;
            }
            else if (TTC < stoppingTimes.FCWStoppingTime)
            {
                this.activateFCW = true;
            }
        }
    }
}
