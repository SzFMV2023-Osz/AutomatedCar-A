namespace AutomatedCar.SystemComponents.InputHandling
{
    class Wheel : IWheel
    {
        private double angleAsDegree;

        public double AngleAsDegree
        {
            get
            {
                return this.angleAsDegree;
            }

            set
            {
                if (value >= -60 && value <= 60)
                {
                    this.angleAsDegree = value;
                }
            }
        }

        public static double IntToDegrees(int angleInt)
        {
            // +60 deg -> -60 deg
            return angleInt * 0.6;
        }
    }
}