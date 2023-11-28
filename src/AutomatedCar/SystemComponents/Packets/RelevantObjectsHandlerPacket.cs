namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RelevantObjectsHandlerPacket : IReadOnlyRelevantObjects
    {
        private List<WorldObject> relevantObjects { get; set; } = new List<WorldObject>();

        public List<WorldObject> RelevantObjects
        {
            get => this.relevantObjects;
            set => this.relevantObjects = value; //fix this for RaiseAndSetIfChanged
        }
    }
}
