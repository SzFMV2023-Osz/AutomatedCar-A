namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Models;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ObjectsinViewPacket : ReactiveObject, IReadOnlyObjectsinViewPacket
    {
        private List<WorldObject> objectsinView;
        public List<WorldObject> ObjectsinView
        {
            get => this.objectsinView;
            set => this.RaiseAndSetIfChanged(ref this.objectsinView, value);
        }
    }
}
