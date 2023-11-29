namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LKAHandlerPacket : ReactiveObject, IReadOnlyLKAHandlerPacket
    {
        private bool lKAAvailable;
        private bool lKAOnOff;
        private string message;
        private bool warning;
        private string warningMessage;

        public bool LKAAvailable
        {
            get => this.lKAAvailable;
            set => this.RaiseAndSetIfChanged(ref this.lKAAvailable, value);
        }

        public bool LKAOnOff
        {
            get => this.lKAOnOff;
            set => this.RaiseAndSetIfChanged(ref this.lKAOnOff, value);
        }

        public string Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref this.message, value);
        }

        public bool Warning
        {
            get => this.warning;
            set => this.RaiseAndSetIfChanged(ref this.warning, value);
        }

        public string WarningMessage
        {
            get => this.warningMessage;
            set => this.RaiseAndSetIfChanged(ref this.warningMessage, value);
        }
    }
}
