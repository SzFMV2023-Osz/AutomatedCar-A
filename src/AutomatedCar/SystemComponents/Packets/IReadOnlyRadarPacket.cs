namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface IReadOnlyRadarPacket
    {
        int LimitSpeed { get; }
        List<RelevantObject> RelevantObjects { get; }
    }
}
