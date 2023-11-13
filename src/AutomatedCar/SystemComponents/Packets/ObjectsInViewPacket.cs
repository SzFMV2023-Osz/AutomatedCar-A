namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ObjectsInViewPacket : ReactiveObject, IReadOnlyObjectsInViewPacket
    {
        private List<string> objectsInView;
        public List<string> ObjectsInView 
        { 
            get => this.objectsInView;
            set => this.RaiseAndSetIfChanged(ref this.objectsInView, value);
        }
    }
}
