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

    }
}
