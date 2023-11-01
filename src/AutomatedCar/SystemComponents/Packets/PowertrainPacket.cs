namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;

    public class PowertrainPacket : ReactiveObject, IPowertrainPacket
    {
        private Vector2 movementVector;
        private double rotation;

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
    }
}
