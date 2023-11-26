namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets;
    using Newtonsoft.Json.Linq;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;


    public class AdaptiveCruiseControl : SystemComponent
    {
        private const int SPEED_CHANGE_RATE = 10;

        double[] availableTargetDistances = new double[4] { 0.8, 1.0, 1.2, 1.4 };

        private int pedalPosition;
        double targetSpeed;
        double currentSpeed;

        private int selectedTargetDistanceIndex = 0;

        private AccPacket accPacket;
        private ICharacteristicsInterface carCharacteristics;

        public AdaptiveCruiseControl(VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            accPacket = new AccPacket();
            accPacket.IsActive = false;
            virtualFunctionBus.AccPacket = accPacket;
            carCharacteristics = virtualFunctionBus.CharacteristicsPacket;
        }

        public override void Process()
        {
            if (accPacket.IsActive)
            {
                this.pedalPosition = CalculatePedalPosition(accPacket.SelectedSpeed, carCharacteristics.Speed);
                if (this.pedalPosition < 0)
                {
                    virtualFunctionBus.BrakePedalPacket.PedalPosition = (byte)Math.Abs(pedalPosition);

                }
                else
                {
                    virtualFunctionBus.GasPedalPacket.PedalPosition = (byte)pedalPosition;
                }

            }
            carCharacteristics = virtualFunctionBus.CharacteristicsPacket;
            virtualFunctionBus.AccPacket = this.accPacket;
        }

        public static int CalculatePedalPosition(double targetSpeed, double currentSpeed)
        {
            double speedDifference = targetSpeed - currentSpeed;
            double maxPedalPosition = 100.0;
            double pedalPosition = speedDifference / maxPedalPosition;

            double threshold = 40.0; // Speed difference threshold, if the difference is big enough this gives more umpf for the pedals
            if (pedalPosition > 0 && speedDifference > threshold)
            {
                pedalPosition += (speedDifference - threshold) / maxPedalPosition;
            }
            else if (pedalPosition < 0 && speedDifference < -threshold)
            {
                pedalPosition += (speedDifference - threshold) / maxPedalPosition;
            }

            pedalPosition = Math.Clamp(pedalPosition, -1.0, 1.0);
            int pedalPositionInt = (int)(pedalPosition * 100.0);

            return pedalPositionInt;
        }

        internal void ToggleCruiseControl()
        {
            accPacket.IsActive = !accPacket.IsActive;

            var selectedSpeed = (int)carCharacteristics.Speed;

            if (selectedSpeed < 30)
            {
                selectedSpeed = 30;
            }
            else if (selectedSpeed > 160)
            {
                selectedSpeed = 160;
            }

            if (!accPacket.IsActive)
            {

                ResetAcc();
            }
            else
            {
                accPacket.SelectedTargetDistance = availableTargetDistances[selectedTargetDistanceIndex];
                accPacket.SelectedSpeed = selectedSpeed;
            }

            virtualFunctionBus.AccPacket = accPacket;
        }

        private void ResetAcc()
        {
            accPacket.SelectedTargetDistance = 0;
            accPacket.SelectedSpeed = 0;
            selectedTargetDistanceIndex = 0;

        }

        internal void DecreaseTargetSpeed()
        {
            if (accPacket.SelectedSpeed != 0) 
            {
                if (accPacket.SelectedSpeed == 30)
                    return;

                accPacket.SelectedSpeed = (int)(Math.Floor((double)accPacket.SelectedSpeed / 10) * 10) - SPEED_CHANGE_RATE;
            }
        }

        internal void IncreaseTargetSpeed()
        {
            if (accPacket.SelectedSpeed != 0)
            {

                if (accPacket.SelectedSpeed == 160)
                    return;

                accPacket.SelectedSpeed = (int)(Math.Floor((double)accPacket.SelectedSpeed / 10) * 10) + SPEED_CHANGE_RATE;
            }
        }

        internal void ChangeTargetDistance()
        {
            selectedTargetDistanceIndex++;
            accPacket.SelectedTargetDistance = availableTargetDistances[selectedTargetDistanceIndex % availableTargetDistances.Length];
        }

        public void SetIsActiveFalse()
        { 
            accPacket.IsActive = false;
            ResetAcc();
        }
    }
}