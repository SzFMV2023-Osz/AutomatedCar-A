namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Threading.Tasks;

    internal class Tempomat
    {
        int userSetSpeed;
        int limitSpeed;
        int currentSpeed;
        private const int minimumSpeed = 160;
        private const int maximumSpeed = 30;
        private const int speedChangeInterval = 10;

        int GoalSpeed
        {
            get { return getGoalSpeed(); }
        }

        Tempomat()
        {
            userSetSpeed = ReturnSpeedValid(currentSpeed);
        }

        public void ActiveTempomatProcess() 
        { 
            if (currentSpeed < GoalSpeed)
            {
                Accelerate();
            }
            else if (currentSpeed > GoalSpeed)
            {
                Decelerate();
            }
        }
        public void IncreaseGoalSpeed() {
            if (IsSpeedValid(userSetSpeed + speedChangeInterval))
            {
                userSetSpeed += speedChangeInterval;
            }
            else
            {
                userSetSpeed = ReturnSpeedValid(userSetSpeed);
            }
        }
        public void DecreaseGoalSpeed() {
            if (IsSpeedValid(userSetSpeed - speedChangeInterval))
            {
                userSetSpeed += speedChangeInterval;
            }
            else
            {
                userSetSpeed = ReturnSpeedValid(userSetSpeed);
            }

        }
        private void Accelerate()
        {
            // todo  
        }
        private void Decelerate()
        {
            // todo
        }
        private bool IsSpeedValid (int speed)
        {
                if (speed > maximumSpeed) { return false; }
                else if (speed < minimumSpeed) { return false; }
                return true;
        }
        private int ReturnSpeedValid(int speed)
        {
            return Math.Min(Math.Max(speed, minimumSpeed), maximumSpeed);
        }
        private int getGoalSpeed()
        {
            return ReturnSpeedValid(Math.Min(userSetSpeed,limitSpeed));
        }
    }
}
