namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.ADAS;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using System;
    using System.Reflection.Metadata;
    using System.Text.RegularExpressions;

    public class Tempomat : SystemComponent
    {
        VirtualFunctionBus virtualFunctionBus;
        public int userSetSpeed { get; set; }
        public int limitSpeed { get; set; }

        private TempomatPacket tempomatPacket;

        public int currentSpeed { get; set; }
        public bool isEnabled { get; set; }
        public int BrakePercentage { get; set; }
        public int ThrottlePercentage { get; set; }
        private const int minimumSpeed = 30;
        private const int maximumSpeed = 160;
        private const int speedChangeInterval = 10;

        int GoalSpeed
        {
            get { return GetGoalSpeed(); }
        }

        public Tempomat(VirtualFunctionBus VirtualFunctionBus) : base(VirtualFunctionBus)
        {
            userSetSpeed = ReturnSpeedValid(currentSpeed);
            virtualFunctionBus = VirtualFunctionBus;
            limitSpeed = maximumSpeed;
            this.tempomatPacket = new TempomatPacket();
            this.virtualFunctionBus.TempomatPacket = this.tempomatPacket;
        }

        public override void Process()
        {
            if (this.virtualFunctionBus.AEBInputPacket.BrakePercentage != 0)
            {
                tempomatPacket.isEnabled = false;
                return;
            }
            this.limitSpeed = this.virtualFunctionBus.RelevantObjectsPacket.LimitSpeed;
            if (this.virtualFunctionBus.TempomatPacket.isEnabled)
            {
                ActiveTempomatProcess();
            }
        }

        public void ActiveTempomatProcess()
        {
            currentSpeed = this.virtualFunctionBus.PowertrainPacket.Speed;
            if (currentSpeed < GoalSpeed/* || IsNeededAdaptiveCorrection()*/)
            {
                Accelerate();
            }
            else if (currentSpeed > GoalSpeed /*|| IsNeededAdaptiveCorrection()*/)
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
                userSetSpeed -= speedChangeInterval;
            }
            else
            {
                userSetSpeed = ReturnSpeedValid(userSetSpeed);
            }
        }

        public void ToggleACC(string key)
        {
            if (key == "OFF")
            {
                tempomatPacket.isEnabled = false;
            }
            else
            {
                isEnabled = !isEnabled;
                tempomatPacket.isEnabled = !tempomatPacket.isEnabled;
                if (isEnabled)
                {
                    userSetSpeed = ReturnSpeedValid(currentSpeed);
                }
            }
        }

        private void Accelerate()
        {

            int result = (int)Math.Floor(Convert.ToDouble(currentSpeed + GetGoalSpeed() / 100 + currentSpeed / 2 + 50));
            if (result > 100)
            {
                ThrottlePercentage = 100;
            }
            else
            {
                ThrottlePercentage = result;
            }
            tempomatPacket.BrakePercentage = 0;
            tempomatPacket.ThrottlePercentage = ThrottlePercentage;

        }

        private void Decelerate()
        {
            int result = (int)Math.Floor(Convert.ToDouble(currentSpeed + GetGoalSpeed() / 100 + 50));
            if (result > 100)
            {
                BrakePercentage = 33;
            }
            else
            {
                BrakePercentage = (int)Math.Floor(Convert.ToDouble(result / 3));
            }
            tempomatPacket.ThrottlePercentage = 0;
            tempomatPacket.BrakePercentage = BrakePercentage;
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
            return Math.Min(ReturnSpeedValid(userSetSpeed),limitSpeed);
        }
    }
}