namespace AutomatedCar.SystemComponents.Packets.InputHandling
{
    public interface IWheel
    {
        public double AngleAsDegree { get; set; }

        public double IntToDegrees(int angleInt);
    }
}