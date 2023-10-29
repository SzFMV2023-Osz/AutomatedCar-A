namespace AutomatedCar.Models.NPC{
using System.Collections.Generic;
    public interface INPC
    {
        List<NPCPathPoint> Points { get; set; }
        bool Repeating { get; set; }
        int CurrentPoint { get; set; }
        int Speed { get; set; }

        void Move();


    }
}