namespace AutomatedCar.SystemComponents.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReadOnlyLKAHandlerPacket
    {
        bool LKAAvailable { get; }
        bool LKAOnOff { get; }
        string Message { get; }
    }
}
