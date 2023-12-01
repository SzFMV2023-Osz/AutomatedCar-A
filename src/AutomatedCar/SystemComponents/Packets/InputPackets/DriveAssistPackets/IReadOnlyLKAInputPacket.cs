namespace AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyLKAInputPacket : IReadOnlyInputDevicePacket
    {
        double WheelCorrection { get; }
    }
}
