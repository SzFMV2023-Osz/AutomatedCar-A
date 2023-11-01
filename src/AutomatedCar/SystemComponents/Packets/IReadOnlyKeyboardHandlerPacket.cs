namespace AutomatedCar.SystemComponents.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutomatedCar.Helpers.Gearbox_helpers;

    public interface IReadOnlyKeyboardHandlerPacket
    {
        int BrakePercentage { get; }

        int ThrottlePercentage { get; }

        double WheelPercentage { get; }

        GearShift ShiftUpOrDown { get; }
    }
}
