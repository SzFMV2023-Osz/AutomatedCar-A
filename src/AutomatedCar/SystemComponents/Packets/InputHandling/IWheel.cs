namespace AutomatedCar.SystemComponents.Packets.InputHandling
{
    public interface IWheel
    {
        public int Angle { get; set; }

        public double IntToDegrees(int angleInt);
    }
}