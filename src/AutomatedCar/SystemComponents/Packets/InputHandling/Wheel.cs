namespace AutomatedCar.SystemComponents.Packets.InputHandling
{
    class Wheel : IWheel
    {
        private int angle;

        public int Angle
        {
            get
            {
                return this.angle;
            }

            set
            {
                if (value >= -100 && value <= 100)
                {
                    this.angle = value;
                }
            }
        }

        public double IntToDegrees(int angleInt)
        {
            // +60° -> -60°
            return angleInt * 0.6;
        }
    }
}