namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;

    public struct Vector2
    {
        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }
    }

    public interface IReadOnlyPowertrainPacket
    {
        public Vector2 MovementVector { get; }

        public double Rotation { get; }

        public int RPM { get; }

        public int Velocity { get; }

        public ATGears GearStage { get; }
    }
}