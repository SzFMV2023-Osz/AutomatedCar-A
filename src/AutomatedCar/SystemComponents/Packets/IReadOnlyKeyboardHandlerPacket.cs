namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.Helpers.Gearbox_helpers;

    public interface IReadOnlyKeyboardHandlerPacket : IInputDevicePacket
    {
        int BrakePercentage { get; }

        int ThrottlePercentage { get; }

        double WheelPercentage { get; }

        SequentialShiftingDirections ShiftUpOrDown { get; }
    }
}
