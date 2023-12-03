namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Reflection.Metadata;
    using System.Text.RegularExpressions;

    internal class Tempomat : SystemComponent
    {
        VirtualFunctionBus virtualFunctionBus;
        public int userSetSpeed { get; set; }
        public int limitSpeed { get; set; }

        private TempomatPacket tempomatPacket;

        public int currentSpeed { get; set; }
        public bool isEnabled { get; set; }
        public int BrakePercentage { get; set; }
        public int ThrottlePercentage { get; set; }
        Car car;
        Radar localRadar;
        private const int minimumSpeed = 30;
        private const int maximumSpeed = 160;
        private const int speedChangeInterval = 10;

        int GoalSpeed
        {
            get { return GetGoalSpeed(); }
        }

        Tempomat(VirtualFunctionBus VirtualFunctionBus, Radar radar, Car Car) : base(VirtualFunctionBus)
        {
            userSetSpeed = ReturnSpeedValid(currentSpeed);
            localRadar = radar;
            car = Car;
            virtualFunctionBus = VirtualFunctionBus;
            limitSpeed = maximumSpeed;
            this.tempomatPacket = new TempomatPacket();
            this.virtualFunctionBus.TempomatPacket = this.tempomatPacket;
        }

        public override void Process()
        {
            limitSpeed = localRadar.viewAngle; //TODO
            currentSpeed = 0;

            if (this.virtualFunctionBus.TempomatPacket.isEnabled)
            {
                ActiveTempomatProcess();
            }
        }

        public void ActiveTempomatProcess()
        {
            if (currentSpeed < GoalSpeed || IsNeededAdaptiveCorrection())
            {
                Accelerate();
            }
            else if (currentSpeed > GoalSpeed || IsNeededAdaptiveCorrection())
            {
                Decelerate();
            }
        }

        private bool IsNeededAdaptiveCorrection()
        {
            throw new NotImplementedException();
        }
        private void AdaptiveCorrection()
        {

        }

        public void IncreaseGoalSpeed()
        {
            if (IsSpeedValid(userSetSpeed + speedChangeInterval))
            {
                userSetSpeed += speedChangeInterval;
            }
            else
            {
                userSetSpeed = ReturnSpeedValid(userSetSpeed);
            }
        }

        public void DecreaseGoalSpeed()
        {
            if (IsSpeedValid(userSetSpeed - speedChangeInterval))
            {
                userSetSpeed += speedChangeInterval;
            }
            else
            {
                userSetSpeed = ReturnSpeedValid(userSetSpeed);
            }
        }

        public void Enable()
        {
            isEnabled = true;
            tempomatPacket.isEnabled = true;
            userSetSpeed = ReturnSpeedValid(currentSpeed);
        }

        public void Disable()
        {
            isEnabled = false;
        }

        private void Accelerate()
        {

            var temp = 50;
            if (temp > 1)
            {
                ThrottlePercentage = 1;
            }
            else
            {
               ThrottlePercentage = temp;
            }
        }

        private void Decelerate()
        {
           BrakePercentage = 50;
        }

        private bool IsSpeedValid(int speed)
        {
            if (speed > maximumSpeed) { return false; }
            else if (speed < minimumSpeed) { return false; }
            return true;
        }

        private int ReturnSpeedValid(int speed)
        {
            return Math.Min(Math.Max(speed, minimumSpeed), maximumSpeed);
        }

        private int GetGoalSpeed()
        {
            var temp = ReturnSpeedValid(Math.Min(userSetSpeed, limitSpeed));
            if (temp == null)
            {
                return maximumSpeed;
            }
            return ReturnSpeedValid(Math.Min(userSetSpeed, limitSpeed));
        }
    }
}
