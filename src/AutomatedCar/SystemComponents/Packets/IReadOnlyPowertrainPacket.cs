namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System.Numerics;

    public interface IReadOnlyPowertrainPacket
    {
        public Vector2 MovementVector { get; }

        public double Rotation { get; }

        public int RPM { get; }

        public int Speed { get; }

        public ATGears GearStage { get; }
    }
}