namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.ADAS;
    using ReactiveUI;

    public class AEBInputPacket : InputDevicePacket, IReadOnlyAEBInputPacket
    {
        private bool warningOver70kmph;
        private bool warningAvoidableCollision;

        public bool WarningOver70kmph
        {
            get => this.warningOver70kmph;
            set => this.RaiseAndSetIfChanged(ref this.warningOver70kmph, value);
        }

        public bool WarningAvoidableCollision
        {
            get => this.warningAvoidableCollision;
            set => this.RaiseAndSetIfChanged(ref this.warningAvoidableCollision, value);
        }
    }
}
