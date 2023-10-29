namespace AutomatedCar.Models.NPC{
    using System.Collections.Generic;
    public class Pedestrian : WorldObject, INPC
    {
        public List<Cordinates> Points { get; set; }
        public bool Repeating { get; set; }
        public Cordinates CurrentPoint { get; set; }
        public int Speed { get; set; }

        public Pedestrian(int x,int y,string filename) : base(x,y,filename)
        {

        }


        public void Move()
        {
            //TODO: Implementation of the Move method for the Pedestrian class.
        }
    }
}