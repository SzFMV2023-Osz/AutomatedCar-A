namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal interface IReadOnlyAEBInputPacket : IReadOnlyInputDevicePacket
    {
        bool WarningOver70kmph { get; set; }

        bool WarningAvoidableCollision { get; set; }
    }
}
