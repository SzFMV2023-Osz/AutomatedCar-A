namespace AutomatedCar.Models.NPC{
    using System.Collections.Generic;
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
            //TODO: Implementation of the Move method for the Pedestrian class.
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