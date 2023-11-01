namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Gearbox;
    using AutomatedCar.SystemComponents.Engine;
    using System;
    using SystemComponents.InputHandling;

    public class MovementCalculator
    {
        const double BRAKING = 10;
        const double DRAG = 0.4257;
        const double ROLLING_RESISTANCE = 12.8;
        const double WHEEL_BASE = 2.6;

        PowertrainPacket powertrainPacket;

        private IGearBox gearBox;

        public MovementCalculator(VirtualFunctionBus virtualFunctionBus)
        {
            this.powertrainPacket = new PowertrainPacket();
            virtualFunctionBus.MovementVectorPacket = this.powertrainPacket;
            this.gearBox = new ATGearBox();
        }

        public void Calculate(int brakePercentage, int wheelPercentage, int velocityFromGearbox)
        {
            if (velocityFromGearbox != 0)
            {
                if (wheelPercentage == 0)
                {
                    this.powertrainPacket.MovementVector = CalculateLongitudinalForce(brakePercentage);
                }
                else
                {
                    this.powertrainPacket = CalculateTurning(brakePercentage, wheelPercentage);
                }
            }
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
            PowertrainPacket movementVectorPacket = new PowertrainPacket();

            double velocity = this.gearBox.Velocity;
            double radius = WHEEL_BASE / Math.Sin(Wheel.IntToDegrees(wheelPercentage));
            double angularVelocityAsDegPerSec = RadianPerSecToDegreesPerSec(velocity / radius);

            // ticks per sec = 60
            movementVectorPacket.Rotation = angularVelocityAsDegPerSec / 60;
            movementVectorPacket.MovementVector = new Vector2(velocity, 0);

            return movementVectorPacket;
        }

        private static double RadianPerSecToDegreesPerSec(double radianPerSec)
        {
            return radianPerSec * 57.2958;
        }
    }
}
