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
        private int rotation;
        private int speed;

        public NPCPathPoint(int x, int y, int rotation, int speed)
        {
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.speed = speed;
        }

        public int X { get => this.x; }

        public int Y { get => this.y; }

        public int Rotation { get => this.rotation; }

        public int Speed { get => this.speed; }
    }
}
