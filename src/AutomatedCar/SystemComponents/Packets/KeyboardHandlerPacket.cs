namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;

    public class KeyboardHandlerPacket : ReactiveObject, IReadOnlyKeyboardHandlerPacket
    {
        private int brakePercentage;
        private int throttlePercentage;
        private int wheelPercentage;

        public int BrakePercentage
        {
            get => this.brakePercentage;
            set => this.RaiseAndSetIfChanged(ref this.brakePercentage, value);
        }

        public int ThrottlePercentage
        {
            get => this.throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref this.throttlePercentage, value);
        }

        public int WheelPercentage
        {
            get => this.wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref this.wheelPercentage, value);
        }
    }
}
