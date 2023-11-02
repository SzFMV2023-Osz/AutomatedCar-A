﻿namespace AutomatedCar.SystemComponents
{
    using System.Numerics;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Gearbox;
    using System;
    using SystemComponents.InputHandling;
    using AutomatedCar.Models;

    public class MovementCalculator
    {
        const float BRAKING = 0.1f;
        const float ROLLING_RESISTANCE = 0.01f;
        const double WHEEL_BASE = 2.6;

        private Vector2 aggregatedVelocity;

        public MovementCalculator()
        {
        }

        public void Process(int brakePercentage, int wheelPercentage, IGearBox gearBox)
        {
            float brakingFroce = brakePercentage * BRAKING;
            float rollingResistance = gearBox.Speed * ROLLING_RESISTANCE;
            float aggregatedForces = brakingFroce + rollingResistance;
            if (gearBox.Speed > 0)
            {
                if (gearBox.Speed - aggregatedForces < 0)
                {
                    gearBox.Speed = 0;
                }
                else
                {
                    gearBox.Speed -= aggregatedForces;
                }
            }
            else if (gearBox.Speed < 0)
            {
                if (gearBox.Speed - aggregatedForces > 0)
                {
                    gearBox.Speed = 0;
        }
                else
        {
                    gearBox.Speed += aggregatedForces;
        }
            }

            AutomatedCar car = World.Instance.ControlledCar;
            double radius = WHEEL_BASE / Math.Sin(Wheel.IntToDegrees(wheelPercentage) * Math.PI/180);
            car.Rotation += gearBox.Speed / radius;

            float rotationInRadian = -(float)(car.Rotation * Math.PI / 180);
            Vector2 directionVector = new Vector2((float)Math.Sin(rotationInRadian), (float)Math.Cos(rotationInRadian));
            Vector2 velocity = directionVector * gearBox.Speed;

            velocity = ConvertVelocity(velocity);

            car.X += (int)velocity.X;
            car.Y += (int)velocity.Y;
        }

        public Vector2 ConvertVelocity(Vector2 velocity)
            {
            Vector2 convertedVelocity = new Vector2();
            this.aggregatedVelocity += velocity;

            if (Math.Abs(this.aggregatedVelocity.X) >= 1)
            {
                convertedVelocity.X = (int)Math.Floor(this.aggregatedVelocity.X);
                this.aggregatedVelocity.X = this.aggregatedVelocity.X % 1;
        }
            if (Math.Abs(this.aggregatedVelocity.Y) >= 1)
        {
                convertedVelocity.Y = (int)Math.Floor(this.aggregatedVelocity.Y);
                this.aggregatedVelocity.Y = this.aggregatedVelocity.Y % 1;
        }

            return -convertedVelocity;
        }
    }
}
