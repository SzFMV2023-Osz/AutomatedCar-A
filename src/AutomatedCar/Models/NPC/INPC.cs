using System.Collections.Generic;
public interface INPC
{
    List<Cordinates> Points { get; set; }
    bool Repeating { get; set; }
    Cordinates CurrentPoint { get; set; }
    int Speed { get; set; }

    void Move();
}