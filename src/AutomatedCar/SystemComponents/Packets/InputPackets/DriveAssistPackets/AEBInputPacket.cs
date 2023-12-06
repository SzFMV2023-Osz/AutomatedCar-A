namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    public class AEBInputPacket : InputDevicePacket, IReadOnlyInputDevicePacket
    {
        private bool warningOver70kmph;
        private bool warningAvoidableCollision;

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
    }
}
