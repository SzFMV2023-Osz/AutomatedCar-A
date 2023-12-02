namespace AutomatedCar.Models.NPC{
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class NPCCar : WorldObject, INPC
    {
        public List<NPCPathPoint> Points { get; set; }
        public bool Repeating { get; set; }
        public int CurrentPoint { get; set; }
        public int Speed { get; set; }

        private NPCManager nPCManager;
        public PolylineGeometry Geometry { get; set; }

        public NPCCar(int x, int y, string filename, int speed, bool repeating, int currentPoint, List<NPCPathPoint> points, NPCManager nPCManager) : base(x, y, filename, collideable: true, worldObjectType: WorldObjectType.Car)
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
                    //this.Rotation = Points[NextPoint].Rotation;
                    this.CurrentPoint = NextPoint;
                }
                else
                {
                    this.Speed = Points[NextPoint].Speed;
                    double distancePerSpeedRatio = distance / this.Speed;
                    this.X += (int)Math.Round(difX / distancePerSpeedRatio);
                    this.Y += (int)Math.Round(difY / distancePerSpeedRatio);
                    this.RotateNPC();
                }
            }
        }

        private void RotateNPC()
        {
            if (this.CurrentPoint + 1 >= this.Points.Count) return;
            var nextPoint = this.Points[this.CurrentPoint + 1];
            double nextRotation = nextPoint.Rotation;
            if (nextRotation == 0 && this.Rotation >= 270)
            {
                nextRotation = 359;
            }
            double rotationDifference = nextRotation - this.Rotation;

            if (rotationDifference < 0)
            {
                rotationDifference = 1;
            }
            double rotationPerTick = Math.Abs(rotationDifference * this.Speed / 60);

            this.Rotation += (rotationDifference < 0) ? -rotationPerTick : rotationPerTick;
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