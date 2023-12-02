namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Models;
    using ReactiveUI;
    using System.Collections.Generic;

    class RadarPacket : ReactiveObject, IReadOnlyRadarPacket
    {
        private List<RelevantObject> relevantObjects;

        public List<RelevantObject> RelevantObjects
        {
            get => this.relevantObjects;
            set => this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
        }
    }
}
