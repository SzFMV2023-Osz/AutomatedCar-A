namespace AutomatedCar.Models.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NPCPathPoint
    {
        private int x;
        private int y;
        private double rotation;
        private int speed;

        public NPCPathPoint(int x, int y, double rotation, int speed)
        {
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.speed = speed;
        }

        public int X { get => this.x; }

        public int Y { get => this.y; }

        public double Rotation { get => this.rotation; }

        public int Speed { get => this.speed; }
    }
}
