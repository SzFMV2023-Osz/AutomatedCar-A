namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.InputPackets;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using System.Collections.Generic;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public List<WorldObject> WorldObjects = new List<WorldObject>();

        public IReadOnlyDummyPacket DummyPacket { get; set; }

        public IReadOnlyKeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

        public IReadOnlyInputDevicePacket AEBInputPacket { get; set; }
        public IReadOnlyInputDevicePacket LCCInputPacket { get; set; }
        public IReadOnlyLKAInputPacket LKAInputPacket { get; set; }

        //public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

        public IReadOnlyLKAHandlerPacket LKAHandlerPacket { get; set; }
        public IReadOnlyRelevantObjects RelevantObjectsPacket { get; set; }

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