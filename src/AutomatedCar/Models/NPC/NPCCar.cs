namespace AutomatedCar.Models.NPC{
    public class NPCCar : WorldObject, INPC
    {
        public List<Cordinates> Points { get; set; }
        public bool Repeating { get; set; }
        public Cordinates CurrentPoint { get; set; }
        public int Speed { get; set; }

        public void Move()
        {
            //TODO: Implementation of the Move method for the Car class.
        }
    }
}