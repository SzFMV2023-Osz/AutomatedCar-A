namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.ADAS;
    using ReactiveUI;

    public class AEBInputPacket : InputDevicePacket, IReadOnlyInputDevicePacket
    {
        private bool warningOver70kmph;
        private bool warningAvoidableCollision;
        private AEBStatus status;

        public bool WarningOver70kmph
        {
            get { return this.warningOver70kmph; }
            set { this.RaiseAndSetIfChanged(ref this.warningOver70kmph, value); }
        }

        public bool WarningAvoidableCollision
        {
            get { return this.warningAvoidableCollision; }
            set { this.RaiseAndSetIfChanged(ref this.warningAvoidableCollision, value); }
        }

        public AEBStatus Status
        {
            get { return this.status; }
            set { this.RaiseAndSetIfChanged(ref this.status, value); }
        }
    }
}
