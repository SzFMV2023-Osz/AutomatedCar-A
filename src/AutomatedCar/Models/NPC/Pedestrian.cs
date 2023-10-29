namespace AutomatedCar.Models.NPC{
    using System.Collections.Generic;
    using System;
    public class Pedestrian : WorldObject, INPC
    {
        public List<NPCPathPoint> Points { get; set; }
        public bool Repeating { get; set; }
        public int CurrentPoint { get; set; }
        public int Speed { get; set; }

        private NPCManager nPCManager;

        public Pedestrian(int x, int y, string filename, int speed, bool repeating, int currentPoint, List<NPCPathPoint> points, NPCManager nPCManager) : base(x, y, filename)
        {
            this.MoveLoad(speed, repeating, currentPoint, points);

            this.nPCManager = nPCManager;
            nPCManager.AddNPC(this);
        }


        public void Move()
        {
            if (!(CurrentPoint == Points.Count - 1 && !Repeating))
            {
                int NextPoint = CurrentPoint + 1;
                if (!(NextPoint < Points.Count)) { NextPoint = 0; }
                var difX = Points[NextPoint].X - this.X;
                var difY = Points[NextPoint].Y - this.Y;
                double distance = Math.Sqrt(difX * difX + difY * difY);
                if ((int)Math.Floor(distance) <= this.Speed)
                {
                    this.X = Points[NextPoint].X;
                    this.Y = Points[NextPoint].Y;
                    this.Speed = Points[NextPoint].Speed;
                    this.Rotation = Points[NextPoint].Rotation;
                    CurrentPoint = NextPoint;
                }
                else
                {
                    double distancePerSpeedRatio = distance / this.Speed;
                    this.X += (int)Math.Round(difX / distancePerSpeedRatio);
                    this.Y += (int)Math.Round(difY / distancePerSpeedRatio);
                }
            }
        }

        public void MoveLoad(int speed, bool repeating, int currentPoint, List<NPCPathPoint> points)
        {
            this.Repeating = repeating;
            this.CurrentPoint = currentPoint;
            this.Points = points;
            this.Speed = speed;
        }
    }
}