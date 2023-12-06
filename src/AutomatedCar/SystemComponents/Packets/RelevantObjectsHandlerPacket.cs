namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Models;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RelevantObjectsHandlerPacket : ReactiveObject, IReadOnlyRelevantObjects
    {
        private List<WorldObject> relevantObjects;

        /// <summary>
        /// Gives back the list of relevant objects based on what is in the radar view, ordered by distance closest to furthest.
        /// </summary>
        public List<WorldObject> RelevantObjects
        {
            get => this.relevantObjects;
            set => this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
        }
    }
}
