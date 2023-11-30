namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Packets;
    using System.Collections.Generic;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();

        public IReadOnlyDummyPacket DummyPacket { get; set; }

<<<<<<< HEAD
        public ICharacteristicsInterface CharacteristicsPacket { get; set; }
        public IPedalInterface BrakePedalPacket { get; set; }
        public IPedalInterface GasPedalPacket { get; set; }

        public IAccInterface AccPacket { get; set; }

        public IReadOnlyKeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public IReadOnlyPowertrainPacket PowertrainPacket { get; set; }

=======
>>>>>>> parent of c42ccb5 (Merge branch 'develop' into master (#67))
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