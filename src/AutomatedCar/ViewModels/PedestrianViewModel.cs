namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;
    using AutomatedCar.Models.NPC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PedestrianViewModel : WorldObjectViewModel
    {
        public Pedestrian NPC { get; set; }
        public PedestrianViewModel(Pedestrian NPC) : base(NPC)
        {
            this.NPC = NPC;
        }
    }
}
