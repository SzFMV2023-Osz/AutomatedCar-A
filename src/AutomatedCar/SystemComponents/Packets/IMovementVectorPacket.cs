namespace AutomatedCar.SystemComponents.Packets
{
    public interface IMovementVectorPacket
    {
        public (double, double) MovementVector { get; }
    }
}