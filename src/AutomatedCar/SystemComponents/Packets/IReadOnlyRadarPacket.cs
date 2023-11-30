namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface IReadOnlyRadarPacket
    {
        List<RelevantObject> RelevantObjects { get; }
    }
}
