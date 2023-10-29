namespace AutomatedCar.Models.NPC{
using System.Collections.Generic;
    public interface INPC
    {
        List<Cordinates> Points { get; set; }
        bool Repeating { get; set; }
        Cordinates CurrentPoint { get; set; }
        int Speed { get; set; }

        void Move();

        void MoveLoad(int speed,bool repeating,Cordinates currentPoint,List<Cordinates> points)
        {
            this.Repeating = repeating;
            this.CurrentPoint = currentPoint;
            this.Points = points;
            this.Speed = speed;
        }
    }
}