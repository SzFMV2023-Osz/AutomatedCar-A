namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.InputPackets;
    using System.Collections.Generic;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public IReadOnlyDummyPacket DummyPacket { get; set; }

        public IReadOnlyKeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

        public IReadOnlyInputPacket AEBInputPacket { get; set; }
        public IReadOnlyInputPacket LCCInputPacket { get; set; }
        public IReadOnlyInputPacket LKAInputPacket { get; set; }

        public void RegisterComponent(SystemComponent component)
        {
            this.components.Add(component);
        }

        protected override void Tick()
        {
            foreach (SystemComponent component in this.components)
            {
                component.Process();
            }
        }
    }
}