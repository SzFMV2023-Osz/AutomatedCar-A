namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Threading.Tasks;

    internal class Tempomat : SystemComponent
    {
        int userSetSpeed;
        int limitSpeed;
        int currentSpeed;
        bool isEnabled;
        private const int minimumSpeed = 160;
        private const int maximumSpeed = 30;
        private const int speedChangeInterval = 10;

        int GoalSpeed
        {
            get { return getGoalSpeed(); }
        }
        Tempomat(VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            userSetSpeed = ReturnSpeedValid(currentSpeed);
            isEnabled = false;
        }

        public override void Process()
        {
            limitSpeed = 0;
            currentSpeed = 0;
            if(isEnabled)
            {
                ActiveTempomatProcess();
            }
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
        public void Enable()
        {
            isEnabled = true;
            userSetSpeed = ReturnSpeedValid(currentSpeed);
        }
        public void Disable()
        {
            isEnabled = false;
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
