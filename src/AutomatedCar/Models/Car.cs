namespace AutomatedCar.Models
{
    public class Car : WorldObject
    {
        public Car(double x, double y, string filename)
            : base((int)x, (int)y, filename)
        {
        }

        /// <summary>Gets or sets Speed in px/s.</summary>
        public int Speed { get; set; }
    }
}