namespace AutomatedCar.SystemComponents.ADAS
{
    using System;
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using ColorTextBlock.Avalonia;

    class AEB : SystemComponent
    {
        // Point closestPointOfObject = PolygonHelper.GetClosestPointOnPolygon(relevantWorldObject.Geometries, new Point(radar.X, radar.Y));


        private const double PixelsToMeters = 1.0 / 50.0; // Conversion factor from pixels to meters
        private const double FrameTimeSeconds = 1.0 / 60.0; // Time for one frame in seconds
        private const int MeterToPxRatio = 50;


        /// <summary>
        /// Initializes a new instance of the <see cref="AEB"/> class.
        /// </summary>
        /// <param name="virtualFunctionBus">Virtual function bus.</param>
        public AEB(VirtualFunctionBus virtualFunctionBus)
            : base (virtualFunctionBus)
        {
            this.AEBInputPacket = new AEBInputPacket();
            this.virtualFunctionBus.AEBInputPacket = this.AEBInputPacket;
            this.IsEmergencyBraking = false;
        }

        public AEBInputPacket AEBInputPacket { get; set; }

        public bool IsEmergencyBraking { get; set; }

        public override void Process()
        {
            int egoSpeed = this.virtualFunctionBus.PowertrainPacket.Speed;
            List<RelevantObject> relevantObjects = this.virtualFunctionBus.RelevantObjectsPacket.RelevantObjects;

            // Deactivate AEB after car stops on an emergency breaking.
            if (this.IsEmergencyBraking && egoSpeed == 0)
            {
                    this.IsEmergencyBraking = false;
            }

            if (!this.IsEmergencyBraking)
            {
                this.AEBInputPacket.BrakePercentage = null;
                this.AEBInputPacket.ThrottlePercentage = null;
            }

            if (relevantObjects?.Count > 0 && egoSpeed <= 70 && egoSpeed > 0)
            {
                AEBInputPacket result = new AEBInputPacket();
                foreach (var obj in relevantObjects)
                {
                     result = this.CalculateBrakingForce(
                        obj.CurrentDistance,
                        this.CalculateSpeed(obj.CurrentDistance, obj.PreviousDistance));
                }

                this.AEBInputPacket.WarningAvoidableCollision = result.WarningAvoidableCollision;

                if ((this.AEBInputPacket.BrakePercentage == null && result.BrakePercentage > 0) || this.AEBInputPacket.BrakePercentage < result.BrakePercentage)
                {
                    this.AEBInputPacket.BrakePercentage = result.BrakePercentage;
                    this.AEBInputPacket.ThrottlePercentage = 0;
                }
            }
            else
            {
                this.AEBInputPacket.WarningAvoidableCollision = false;
            }

            this.AEBInputPacket.WarningOver70kmph = egoSpeed > 70;
        }

        /// <summary>
        /// Calculates braking force.
        /// </summary>
        /// <param name="distanceInPixels">Distance from object in question in pixels.</param>
        /// <param name="currentSpeed">Relative speed compared to object in question (m/s).</param>
        /// <returns>AEBInputPacket that the powertrain can use.</returns>
        public AEBInputPacket CalculateBrakingForce(double distanceInPixels, double currentSpeed)
        {
            var distanceInMeters = distanceInPixels * PixelsToMeters;

            AEBInputPacket result = new AEBInputPacket();

            // Calculate the deceleration required to stop before hitting the object
            double calculationErrorInMeters = 2;

            // Apply calculation error only if the distance is bigger than the error itself
            distanceInMeters = distanceInMeters <= calculationErrorInMeters ? distanceInMeters : distanceInMeters - calculationErrorInMeters;

            // Calculate deceleration
            double deceleration = Math.Pow(currentSpeed, 2) / (2 * distanceInMeters);

            // If the deceleration required is greater than the maximum deceleration possible (9 m/s^2), limit it
            if (deceleration > 9)
            {
                deceleration = 9;
            }

            result.WarningAvoidableCollision = true;

            // Activate AEB if object is closer than <closeDistanceThreshold> or if deceleration exceeds <decelerationThreshold>. <farDistanceThreshold> limits how far the radar sees in the context of AEB.
            double decelerationThreshold = 1; // meters per sec
            double farDistanceThreshold = 50; // meters
            double closeDistanceThreshold = 10; // meters // useful at slow speeds
            if (deceleration > decelerationThreshold && distanceInMeters < farDistanceThreshold)
            {
                // Calculate the braking force
                double brakingForce = deceleration * 100 / 9; // Scale to a range of 0-100

                // Update the brake percentage in the result packet
                result.BrakePercentage = (int)brakingForce;

                this.IsEmergencyBraking = true;
            }

            return result;
        }

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
    }
}
