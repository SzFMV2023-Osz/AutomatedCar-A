namespace AutomatedCar.SystemComponents.InputHandling
{
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;

    public class KeyboardHandler : SystemComponent, IInputHandler
    {
        private Timer brakeTimer;
        private Timer throttleTimer;
        private Timer wheelLeftTimer;
        private Timer wheelRightTimer;
        private int brakePercentage;
        private int throttlePercentage;
        private int wheelPercentage;

        public KeyboardHandlerPacket KeyboardHandlerPacket { get; set; }


        public KeyboardHandler(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.KeyboardHandlerPacket = new KeyboardHandlerPacket();
            this.virtualFunctionBus.KeyboardHandlerPacket = this.KeyboardHandlerPacket;

            this.brakeTimer = new Timer(10);
            this.throttleTimer = new Timer(10);
            this.wheelLeftTimer = new Timer(10);
            this.wheelRightTimer = new Timer(10);

            this.brakeTimer.Elapsed += (sender, e) =>
            {
                if (this.brakePercentage < 100)
                {
                    this.brakePercentage++;
                }
            };

            this.throttleTimer.Elapsed += (sender, e) =>
            {
                if (this.throttlePercentage < 100)
                {
                    this.throttlePercentage++;
                }
            };

            this.wheelLeftTimer.Elapsed += (sender, e) =>
            {
                if (this.wheelPercentage > -100)
                {
                    this.wheelPercentage--;
                }
            };

            this.wheelRightTimer.Elapsed += (sender, e) =>
            {
                if (this.wheelPercentage < 100)
                {
                    this.wheelPercentage++;
                }
            };
        }

        public void HandleKeyDown_Up()
        {
            this.throttleTimer.Start();
        }

        public void HandleKeyUp_Up()
        {
            this.throttleTimer.Stop();
            this.throttlePercentage = 0;
        }

        public void HandleKeyDown_Down()
        {
            this.brakeTimer.Start();
        }

        public void HandleKeyUp_Down()
        {
            this.brakeTimer.Stop();
            this.brakePercentage = 0;
        }

        public void HandleKeyDown_Left()
        {
            this.wheelLeftTimer.Start();
        }

        public void HandleKeyUp_Left()
        {
            this.wheelLeftTimer.Stop();
            this.wheelPercentage = 0;
        }

        public void HandleKeyDown_Right()
        {
            this.wheelRightTimer.Start();
        }

        public void HandleKeyUp_Right()
        {
            this.wheelRightTimer.Stop();
            this.wheelPercentage = 0;
        }

        public override void Process()
        {
            this.KeyboardHandlerPacket.ThrottlePercentage = this.throttlePercentage;
            this.KeyboardHandlerPacket.BrakePercentage = this.brakePercentage;
            this.KeyboardHandlerPacket.WheelPercentage = this.wheelPercentage;

        }
    }
}
