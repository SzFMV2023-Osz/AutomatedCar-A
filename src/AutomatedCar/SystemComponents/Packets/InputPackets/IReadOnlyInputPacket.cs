namespace AutomatedCar.SystemComponents.Packets.InputPackets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyInputPacket
    {
        int? BrakePercentage { get; }

        int? ThrottlePercentage { get; }

        double? WheelPercentage { get; }

        SequentialShiftingDirections? ShiftUpOrDown { get; }
    }

    //public interface IReadOnlyAEBInputPacket : IReadOnlyInputPacket { }

    //public interface IReadOnlyLCCInputPacket : IReadOnlyInputPacket { }

    //public interface IReadOnlyLKAInputPacket : IReadOnlyInputPacket { }
}
