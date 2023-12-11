namespace AutomatedCar.SystemComponents.ADAS
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using System.ComponentModel;
    using System.Linq;
    using System;

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

        int meterToPxRatio = 50;

        public override void Process()
        {
            //List<WorldObject> relevantObjects = this.virtualFunctionBus.RelevantObjectsPacket.RelevantObjects?.Where(x => x.Collideable).ToList();

            //We give up...

            int egoSpeed = this.virtualFunctionBus.PowertrainPacket.Speed;
            List<RelevantObject> relevantObjects = this.virtualFunctionBus.RelevantObjectsPacket.RelevantObjects;
            if (relevantObjects?.Count > 0)
            {
                foreach (var obj in relevantObjects)
                {
                    if (this.status != AEBStatus.Off)
                    {
                        break;
                    }

                    //this.AEBPacket = this.CalculateBrakingForce(
                    //    obj.CurrentDistance,
                    //    this.CalculateSpeed(obj.CurrentDistance, obj.PreviousDistance / meterToPxRatio));

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
            }

            this.AEBPacket.WarningOver70kmph = egoSpeed > 70 ? true : false;
        }

        public AEBInputPacket CalculateBrakingForce(double distanceInPixels, double currentSpeed)
        {
            var distance = distanceInPixels * PixelsToMeters;

            AEBInputPacket result = new AEBInputPacket();

            // Convert speed from km/h to m/s
            double speedMetersPerSecond = currentSpeed * 1000 / 3600;

            // Check if current speed exceeds 70 km/h
            if (currentSpeed <= 70)
            {
                // Calculate the deceleration required to stop before hitting the object
                double deceleration = Math.Pow(speedMetersPerSecond, 2) / (2 * distance);

                // If the deceleration required is greater than the maximum deceleration possible (9 m/s^2), limit it
                if (deceleration > 9)
                {
                    deceleration = 9;
                    result.WarningAvoidableCollision = true;
                }

                // Calculate the braking force
                double brakingForce = deceleration * 100 / 9; // Scale to a range of 0-100

                // Update the brake percentage in the result packet
                result.BrakePercentage = (int)brakingForce;
            }
            else
            {
                result.WarningOver70kmph = true;
            }

            return result;
        }

        private const double PixelsToMeters = 1.0 / 50.0; // Conversion factor from pixels to meters
        private const double FrameTimeSeconds = 1.0 / 60.0; // Time for one frame in seconds

        public double CalculateSpeed(double currentDistance, double previousDistance)
        {
            // Calculate distance change in pixels
            double distanceChangePixels = Math.Abs(currentDistance - previousDistance);

            // Convert distance change from pixels to meters
            double distanceChangeMeters = distanceChangePixels * PixelsToMeters;

            // Calculate speed in meters per second based on fixed frame time
            double speed = distanceChangeMeters / FrameTimeSeconds;

            return speed;
        }

        public double CalculateDistance(int x1, int y1, int x2, int y2)
        {
            // Calculate distance using Euclidean distance formula
            double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return distance;
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
