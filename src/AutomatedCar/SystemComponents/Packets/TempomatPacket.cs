namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TempomatPacket: ReactiveObject, IReadOnlyTempomatPacket
    {
        public int userSetSpeed { get; set; }
        public int limitSpeed { get; set; }
        public int currentSpeed { get; set; }
        public bool isEnabled { get; set; }
        public int BrakePercentage { get; set; }
        public int ThrottlePercentage { get; set; }
    }
}
