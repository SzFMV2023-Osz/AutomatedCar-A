namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using ReactiveUI;

    public class PowertrainPacket : ReactiveObject, IReadOnlyPowertrainPacket
    {
        private Vector2 movementVector;
        private double rotation;
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

        public ATGears GearStage
        {
            get => this.gearStage;
            set => this.RaiseAndSetIfChanged(ref this.gearStage, value);
        }
    }
}
