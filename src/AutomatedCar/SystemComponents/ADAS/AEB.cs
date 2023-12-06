namespace AutomatedCar.SystemComponents.ADAS
{
    using System;
    using System.Collections.Generic;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using AutomatedCar.Models;
    using System.ComponentModel;
    using System.Linq;

    class AEB : SystemComponent
    {
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
            List<WorldObject> relevantObjects = this.virtualFunctionBus.RelevantObjectsPacket.RelevantObjects?.Where(x => x.Collideable).ToList();


            //We give up...
            //if (relevantObjects?.Count > 0)
            //{
            //    virtualFunctionBus.AEBInputPacket = CalculateBrakingForce(CalculateDistance(relevantObjects[0].X, relevantObjects[0].Y),
            //        CalculateSpeed(relevantObjects[0].CurrentDistance, relevantObjects[0].PreviousDistance / meterToPxRatio));
            //}
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
    }
}
