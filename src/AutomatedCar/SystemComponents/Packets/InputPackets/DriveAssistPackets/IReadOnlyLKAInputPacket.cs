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
        bool LKAAvailable { get; }
        bool LKAOnOff { get; }
        string Message { get; }
        bool Warning { get; }
        string WarningMessage { get; }
    }
}
