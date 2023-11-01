namespace AutomatedCar.SystemComponents.Packets
{
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

    public interface IPowertrainPacket
    {
        public Vector2 MovementVector { get; }

        public double Rotation { get; }
    }
}