namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    public class LKAInputPacket : InputDevicePacket, IReadOnlyLKAInputPacket
    {
        private double wheelCorrection;
        public double WheelCorrection
        {
            get => this.wheelCorrection;
            set => this.RaiseAndSetIfChanged(ref this.wheelCorrection, value);
        }

        private bool lKAAvailable;
        private bool lKAOnOff;
        private string message;
        private bool warning;
        private string warningMessage;

        public bool LKAAvailable
        {
            get => this.lKAAvailable;
            set => this.RaiseAndSetIfChanged(ref this.lKAAvailable, value);
        }

        public bool LKAOnOff
        {
            get => this.lKAOnOff;
            set => this.RaiseAndSetIfChanged(ref this.lKAOnOff, value);
        }

        public string Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref this.message, value);
        }

        public bool Warning
        {
            get => this.warning;
            set => this.RaiseAndSetIfChanged(ref this.warning, value);
        }

        public string WarningMessage
        {
            get => this.warningMessage;
            set => this.RaiseAndSetIfChanged(ref this.warningMessage, value);
        }
    }
}
