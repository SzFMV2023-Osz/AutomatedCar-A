namespace AutomatedCar.SystemComponents.Powertrain
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.Packets.InputPackets;
    using Packets;

    public class InputPriorityHandler
    {
        private InputDevicePacket inputPacket = new InputDevicePacket();

        public InputDevicePacket GetInputs(VirtualFunctionBus vfb)
        {
            this.inputPacket.BrakePercentage = PriorityHandler<int?>(
                vfb.AEBInputPacket.BrakePercentage,
                vfb.LCCInputPacket.BrakePercentage,
                vfb.LKAInputPacket.BrakePercentage,
                vfb.KeyboardHandlerPacket.BrakePercentage);

            this.inputPacket.WheelPercentage = PriorityHandler<double?>(
                vfb.AEBInputPacket.WheelPercentage,
                vfb.LCCInputPacket.WheelPercentage,
                vfb.LKAInputPacket.WheelPercentage,
                vfb.KeyboardHandlerPacket.WheelPercentage);

            this.inputPacket.ThrottlePercentage = PriorityHandler<int?>(
                vfb.AEBInputPacket.ThrottlePercentage,
                vfb.LCCInputPacket.ThrottlePercentage,
                vfb.LKAInputPacket.ThrottlePercentage,
                vfb.KeyboardHandlerPacket.ThrottlePercentage);

            this.inputPacket.ShiftUpOrDown = PriorityHandler<SequentialShiftingDirections?>(
                vfb.AEBInputPacket.ShiftUpOrDown,
                vfb.LCCInputPacket.ShiftUpOrDown,
                vfb.LKAInputPacket.ShiftUpOrDown,
                vfb.KeyboardHandlerPacket.ShiftUpOrDown);

            return this.inputPacket;
        }

        private static T PriorityHandler<T>(T prio1, T prio2, T prio3, T prio4)
        {
            if (prio1 != null) { return prio1; }
            if (prio2 != null) { return prio2; }
            if (prio3 != null) { return prio3; }

            return prio4;
        }
    }
}
