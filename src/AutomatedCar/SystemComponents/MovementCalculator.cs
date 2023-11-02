namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Gearbox;
    using System;
    using SystemComponents.InputHandling;
    using AutomatedCar.Models;

    public class MovementCalculator
    {
        const double BRAKING = 50;
        const double DRAG = 20;
        const double ROLLING_RESISTANCE = 12.8;
        const double WHEEL_BASE = 2.6;

        public MovementCalculator()
        {
        }

        public PowertrainPacket Calculate(int brakePercentage, int wheelPercentage, int velocityFromGearbox)
        {
            PowertrainPacket powertrainPacket = new PowertrainPacket();
            int velocityAsKmph = velocityFromGearbox; //(int)(velocityFromGearbox / 3.6);
            if (velocityAsKmph != 0)
            {
                if (wheelPercentage == 0)
                {
                    powertrainPacket.MovementVector = CalculateLongitudinalForce(brakePercentage, velocityAsKmph);
                }
                else
                {
                    var newpowertrainPacket = CalculateTurning(brakePercentage, wheelPercentage, velocityAsKmph);
                    powertrainPacket.MovementVector = newpowertrainPacket.MovementVector;
                    powertrainPacket.Rotation = newpowertrainPacket.Rotation;
                }
            }
            return powertrainPacket;
        }

        public void UpdateCarPosition(PowertrainPacket powertrainPacket)
        {
            Vector2 viewCoordinates = TransformCarCoordinateToViewCoordinate(powertrainPacket.MovementVector, powertrainPacket.Rotation);
            World.Instance.ControlledCar.X += (int)viewCoordinates.X;
            World.Instance.ControlledCar.Y += (int)viewCoordinates.Y;
            World.Instance.ControlledCar.Rotation += powertrainPacket.Rotation;
        }

        private static Vector2 TransformCarCoordinateToViewCoordinate(Vector2 carCoordinate, double rotation)
        {
            double xt = carCoordinate.X * Math.Cos(rotation) - carCoordinate.Y * Math.Sin(rotation);
            double yt = carCoordinate.X * Math.Sin(rotation) + carCoordinate.Y * Math.Cos(rotation);

            // calculate px from m

            return new Vector2(xt / 50, yt / 50);
        }

        private Vector2 CalculateLongitudinalForce(int brakePercentage, int velocity)
        {
            double brakingForce = BRAKING * brakePercentage * (velocity > 0 ? 1 : -1);
            double dragForce = -DRAG * velocity;
            double rollingResistanceForce = -ROLLING_RESISTANCE * velocity * (velocity > 0 ? 1 : -1);
            double longitudinalForce;
            if (dragForce < 0)
            {
                longitudinalForce = dragForce + (brakingForce < dragForce ? brakingForce : dragForce);
            }
            else
            {
                longitudinalForce = dragForce + (brakingForce < dragForce ? dragForce : brakingForce);
            }

            return new Vector2(longitudinalForce, 0);
        }

        private PowertrainPacket CalculateTurning(int brakePercentage, int wheelPercentage, int velocity)
        {
            PowertrainPacket powertrainPacket = new PowertrainPacket();

            double radius = WHEEL_BASE / Math.Sin(Wheel.IntToDegrees(wheelPercentage));
            double angularVelocityAsDegPerSec = RadianPerSecToDegreesPerSec(velocity / radius);

            // ticks per sec = 60
            powertrainPacket.Rotation = angularVelocityAsDegPerSec / 60;
            powertrainPacket.MovementVector = new Vector2(velocity, 0);

            return powertrainPacket;
        }

        private static double RadianPerSecToDegreesPerSec(double radianPerSec)
        {
            return radianPerSec * 57.2958;
        }
    }
}
