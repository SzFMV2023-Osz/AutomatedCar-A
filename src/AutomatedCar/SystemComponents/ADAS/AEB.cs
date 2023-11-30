namespace AutomatedCar.SystemComponents.ADAS
{
    using System;
    using System.Collections.Generic;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using AutomatedCar.Models;

    class AEB : SystemComponent
    {
        public AEBInputPacket AEBPacket { get; set; }

        public AEB(VirtualFunctionBus virtualFunctionBus)
            : base (virtualFunctionBus)
        {
            this.AEBPacket = new AEBInputPacket();
            this.virtualFunctionBus.AEBInputPacket = this.AEBPacket;
        }

        public override void Process()
        {
            List<RelevantObject> relevantObjects = this.virtualFunctionBus.RadarPacket.RelevantObjects;
        }
    }
}
