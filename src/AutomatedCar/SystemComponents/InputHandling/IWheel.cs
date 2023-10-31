namespace AutomatedCar.SystemComponents.InputHandling
{
    public interface IWheel
    {
        public double AngleAsDegree { get; set; }

        public double IntToDegrees(int angleInt);
    }
}