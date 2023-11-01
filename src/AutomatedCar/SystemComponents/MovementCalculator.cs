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
        const double DRAG = 0.4257;
        const double ROLLING_RESISTANCE = 12.8;
        const double WHEEL_BASE = 2.6;


        private IGearBox gearBox;

        public MovementCalculator()
        {
        }

        public void Calculate(int brakePercentage, int wheelPercentage, int velocityFromGearbox, PowertrainPacket powertrainPacket)
        {
            if (velocityFromGearbox != 0)
            {
                if (wheelPercentage == 0)
                {
                    powertrainPacket.MovementVector = CalculateLongitudinalForce(brakePercentage);
                }
                else
                {
                    powertrainPacket = CalculateTurning(brakePercentage, wheelPercentage);
                }
            }
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

            return new Vector2(xt, yt);
        }

        private Vector2 CalculateLongitudinalForce(int brakePercentage)
        {
            double velocity = this.gearBox.Velocity;
            double brakingForce = -BRAKING * brakePercentage;
            double dragForce = -DRAG * velocity * velocity;
            double rollingResistanceForce = -ROLLING_RESISTANCE * velocity;

            double longitudinalForce = velocity + brakingForce + dragForce + rollingResistanceForce;

            return new Vector2(longitudinalForce, 0);
        }

        private PowertrainPacket CalculateTurning(int brakePercentage, int wheelPercentage)
        {
            PowertrainPacket powertrainPacket = new PowertrainPacket();

            double velocity = this.gearBox.Velocity;
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
