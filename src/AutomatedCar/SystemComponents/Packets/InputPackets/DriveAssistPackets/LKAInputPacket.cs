namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    public class LKAInputPacket : InputDevicePacket, IReadOnlyLKAInputPacket
    {
        private bool lKAAvailable;
        private bool lKAOnOff;
        private string onOffMessage;
        private string availableMessage;
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

        public string OnOffMessage
        {
            get => this.onOffMessage;
            set => this.RaiseAndSetIfChanged(ref this.onOffMessage, value);
        }

        public string AvailableMessage
        {
            get => this.availableMessage;
            set => this.RaiseAndSetIfChanged(ref this.availableMessage, value);
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
