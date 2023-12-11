namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyLKAInputPacket : IReadOnlyInputDevicePacket
    {
        bool LKAAvailable { get; }
        bool LKAOnOff { get; }
        string OnOffMessage { get; }
        string AvailableMessage { get; }
        bool Warning { get; }
        string WarningMessage { get; }
    }
}
