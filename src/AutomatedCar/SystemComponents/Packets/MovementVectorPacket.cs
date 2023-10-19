namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    public interface IMovementVectorPacket
    {
        public (int, int) MovementVector { get; }
    }

    public class MovementVectorPacket : ReactiveObject, IMovementVectorPacket
    {
        private (int, int) movementVector;

        public (int, int) MovementVector
        {
            get { return movementVector; }
            set { movementVector = value; }
        }
    }
}
