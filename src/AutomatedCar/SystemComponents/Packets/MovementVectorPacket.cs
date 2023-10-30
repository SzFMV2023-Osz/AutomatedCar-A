namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;

    public class MovementVectorPacket : ReactiveObject, IMovementVectorPacket
    {
        private (double, double) movementVector;

        public (double, double) MovementVector
        {
            get { return movementVector; }
            set { movementVector = value; }
        }
    }
}
