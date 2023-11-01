namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using AutomatedCar.Helpers.Gearbox_helpers;

    public class PowertrainPacket : ReactiveObject, IReadOnlyPowertrainPacket
    {
        private Vector2 movementVector;
        private double rotation;
        private int rpm;
        private int velocity;
        private ATGears gearStage;

        public Vector2 MovementVector
        {
            get => this.movementVector;
            set => this.RaiseAndSetIfChanged(ref this.movementVector, value);
        }

        public double Rotation
        {
            get => this.rotation;
            set => this.RaiseAndSetIfChanged(ref this.rotation, value);
        }

        public int RPM
        {
            get => this.rpm;
            set => this.RaiseAndSetIfChanged(ref this.rpm, value);
        }

        public int Velocity
        {
            get => this.velocity;
            set => this.RaiseAndSetIfChanged(ref this.velocity, value);
        }

        public ATGears GearStage
        {
            get => this.gearStage;
            set => this.RaiseAndSetIfChanged(ref this.gearStage, value);
        }
    }
}
