namespace AutomatedCar.SystemComponents.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal interface IReadOnlyTempomatPacket
    {

        int userSetSpeed { get; set; }
        int limitSpeed { get; set; }
        int currentSpeed { get; set; }
        bool isEnabled { get; set; }
        int BrakePercentage { get; set; }
        int ThrottlePercentage { get; set; }
    }
}
