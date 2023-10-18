namespace AutomatedCar.SystemComponents.Packets
{
    public interface IMovementVectorPacket
    {
        public (int, int) MovementVector { get; }
    }

    public class MovementVectorPacket : IMovementVectorPacket
    {
        private (int, int) movementVector;

        public (int, int) MovementVector
        {
            get { return movementVector; }
            set { movementVector = value; }
        }
    }
}
