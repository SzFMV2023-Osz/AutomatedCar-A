namespace AutomatedCar.Models
{
    public class NPC : WorldObject
    {
        public NPC(int x, int y, string filename)
            : base(x, y, filename)
        {
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Rotation { get; set; }

        public void Move(double targetX, double targetY, double speed)
        {
            double deltaX = targetX - X;
            double deltaY = targetY - Y;
            double angleToTarget = Math.Atan2(deltaY, deltaX);

            Rotation = angleToTarget;

            double distanceToTarget = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distanceToTarget > speed)
            {
                X += speed * Math.Cos(angleToTarget);
                Y += speed * Math.Sin(angleToTarget);
            }
            else
            {
                X = targetX;
                Y = targetY;
            }
        }
    }
}
