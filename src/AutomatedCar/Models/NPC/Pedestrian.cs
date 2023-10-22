namespace AutomatedCar.Models.NPC{
    public class Pedestrian : WorldObject, INPC
    {
        public List<Coordinates> Points { get; set; }
        public bool Repeating { get; set; }
        public Coordinates CurrentPoint { get; set; }
        public int Speed { get; set; }

        public void Move()
        {
            //TODO: Implementation of the Move method for the Pedestrian class.
        }
    }
}