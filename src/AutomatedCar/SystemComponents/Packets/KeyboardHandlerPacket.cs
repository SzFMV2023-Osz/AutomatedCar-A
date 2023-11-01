namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System.IO;

    public class KeyboardHandlerPacket : ReactiveObject, IReadOnlyKeyboardHandlerPacket
    {

        /// <summary>
        /// Represents how far the brake pedal is being pushed down. 0 meaning not at all, 100 meaning fully pressed.
        /// </summary>
        private int brakePercentage;

        /// <summary>
        /// Represents how far the throttle pedal is being pushed down. 0 meaning not at all, 100 meaning fully pressed.
        /// </summary>
        private int throttlePercentage;

        /// <summary>
        /// Represents steering wheel rotation between values -100 and +100 (left to right). 0 means that the wheel is centered.
        /// </summary>
        private int wheelPercentage;

        /// <summary>
        /// Value can be:
        /// -1 meaning shift down,
        /// 0 meaning don't shift,
        /// +1 meaning shift up.
        /// </summary>
        private int shiftUpOrDown;

        public int BrakePercentage
        {
            get => this.brakePercentage;
            set => this.RaiseAndSetIfChanged(ref this.brakePercentage, value);
        }

        public int ThrottlePercentage
        {
            get => this.throttlePercentage;
            set => this.RaiseAndSetIfChanged(ref this.throttlePercentage, value);
        }

        public int WheelPercentage
        {
            get => this.wheelPercentage;
            set => this.RaiseAndSetIfChanged(ref this.wheelPercentage, value);
        }

        public int ShiftUpOrDown
        {
            get => this.shiftUpOrDown;
            set
            {
                if (value < -1 || value > 1)
                {
                    throw new InvalidDataException("Inappropriate gear shift value. The value has to be between -1 and +1.");
                }

                this.RaiseAndSetIfChanged(ref this.shiftUpOrDown, value);
            }
        }
    }
}
