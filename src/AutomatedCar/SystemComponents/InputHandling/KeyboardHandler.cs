namespace AutomatedCar.SystemComponents.InputHandling
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.Helpers.Gearbox_helpers;
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
        private SequentialShiftingDirections shiftingDirection;
        private bool brakeSmoothReturnIsActive;
        private bool throttleSmoothReturnIsActive;
        private bool wheelSmoothReturnIsActive;

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

            this.brakeSmoothReturnIsActive = false;
            this.throttleSmoothReturnIsActive = false;
            this.wheelSmoothReturnIsActive = false;

            this.shiftingDirection = SequentialShiftingDirections.Nothing;

            this.brakeTimer.Elapsed += (sender, e) =>
            {
                if (!this.brakeSmoothReturnIsActive)
                {
                    if (this.brakePercentage < 100)
                    {
                        this.brakePercentage++;
                    }
                }
                else
                {
                    if (this.brakePercentage > 0)
                    {
                        this.brakePercentage--;
                    }
                    else
                    {
                        this.brakeTimer.Stop();
                        this.brakeSmoothReturnIsActive = false;
                    }
                }
            };

            this.throttleTimer.Elapsed += (sender, e) =>
            {
                if (!this.throttleSmoothReturnIsActive)
                {
                    if (this.throttlePercentage < 100)
                    {
                        this.throttlePercentage++;
                    }
                }
                else
                {
                    if (this.throttlePercentage > 0)
                    {
                        this.throttlePercentage--;
                    }
                    else
                    {
                        this.throttleTimer.Stop();
                        this.throttleSmoothReturnIsActive = false;
                    }
                }
            };

            this.wheelLeftTimer.Elapsed += (sender, e) =>
            {
                if (!this.wheelSmoothReturnIsActive)
                {
                    // Normal case
                    if (this.wheelPercentage > -100)
                    {
                        this.wheelPercentage--;
                    }
                }
                else
                {
                    // Smooth return
                    if (this.wheelPercentage < 0)
                    {
                        this.wheelPercentage++;
                    }
                    else
                    {
                        this.wheelLeftTimer.Stop();
                        this.wheelSmoothReturnIsActive = false;
                    }
                }
            };

            this.wheelRightTimer.Elapsed += (sender, e) =>
            {
                if (!this.wheelSmoothReturnIsActive)
                {
                    // Normal case
                    if (this.wheelPercentage < 100)
                    {
                        this.wheelPercentage++;
                    }
                }
                else
                {
                    // Smooth return
                    if (this.wheelPercentage > 0)
                    {
                        this.wheelPercentage--;
                    }
                    else
                    {
                        this.wheelRightTimer.Stop();
                        this.wheelSmoothReturnIsActive = false;
                    }
                }
            };
        }

        public void HandleKeyDown_Up()
        {
            this.throttleSmoothReturnIsActive = false;
            this.throttleTimer.Start();
        }

        public void HandleKeyUp_Up()
        {
            this.throttleSmoothReturnIsActive = true;
        }

        public void HandleKeyDown_Down()
        {
            this.brakeSmoothReturnIsActive = false;
            this.brakeTimer.Start();
        }

        public void HandleKeyUp_Down()
        {
            this.brakeSmoothReturnIsActive = true;
        }

        public void HandleKeyDown_Left()
        {
            this.wheelSmoothReturnIsActive = false;
            this.wheelLeftTimer.Start();
        }

        public void HandleKeyUp_Left()
        {
            this.wheelSmoothReturnIsActive = !this.wheelRightTimer.Enabled;
            if (this.wheelRightTimer.Enabled)
            {
                this.wheelLeftTimer.Stop();
            }
        }

        public void HandleKeyDown_Right()
        {
            this.wheelSmoothReturnIsActive = false;
            this.wheelRightTimer.Start();
        }

        public void HandleKeyUp_Right()
        {
            this.wheelSmoothReturnIsActive = !this.wheelLeftTimer.Enabled;
            if (this.wheelLeftTimer.Enabled)
            {
                this.wheelRightTimer.Stop();
            }
        }

        public void HandleKeyDown_Q()
        {
            this.shiftingDirection = SequentialShiftingDirections.Up;
        }

        public void HandleKeyDown_A()
        {
            this.shiftingDirection = SequentialShiftingDirections.Down;
        }

        public override void Process()
        {
            this.KeyboardHandlerPacket.ShiftUpOrDown = this.shiftingDirection;
            this.shiftingDirection = SequentialShiftingDirections.Nothing;
            this.KeyboardHandlerPacket.ThrottlePercentage = this.throttlePercentage;
            this.KeyboardHandlerPacket.BrakePercentage = this.brakePercentage;
            this.KeyboardHandlerPacket.WheelPercentage = this.wheelPercentage;
        }
    }
}
