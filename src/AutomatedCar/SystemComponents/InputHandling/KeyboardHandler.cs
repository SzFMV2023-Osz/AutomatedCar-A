﻿namespace AutomatedCar.SystemComponents.InputHandling
{
    using System.Timers;
    using AutomatedCar.Helpers.Gearbox_helpers;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;

    public class KeyboardHandler : SystemComponent, IInputHandler
    {
        private Timer brakeTimer;
        private Timer throttleTimer;
        private Timer wheelReturnTimer;
        private int brakePercentage;
        private int throttlePercentage;
        private int wheelPercentage;
        private SequentialShiftingDirections shiftingDirection;
        private bool brakeSmoothReturnIsActive;
        private bool throttleSmoothReturnIsActive;
        private bool wheelIsTurningLeft;
        private bool wheelIsTurningRight;

        public KeyboardHandlerPacket KeyboardHandlerPacket { get; set; }

        public KeyboardHandler(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.KeyboardHandlerPacket = new KeyboardHandlerPacket();
            this.virtualFunctionBus.KeyboardHandlerPacket = this.KeyboardHandlerPacket;

            this.brakeTimer = new Timer(10);
            this.throttleTimer = new Timer(10);
            this.wheelReturnTimer = new Timer(10);

            this.brakeSmoothReturnIsActive = false;
            this.throttleSmoothReturnIsActive = false;


            this.shiftingDirection = SequentialShiftingDirections.Nothing;

            this.shiftingDirection = SequentialShiftingDirections.Nothing;

            this.brakeTimer.Elapsed += (sender, e) =>
            {
                if (!this.brakeSmoothReturnIsActive)
                {
                    this.throttlePercentage = 0;
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
                    this.brakePercentage = 0;
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

            this.wheelReturnTimer.Elapsed += (sender, e) =>
            {
                if (this.wheelPercentage < 0)
                {
                    this.wheelPercentage++;
                }
                else if (this.wheelPercentage > 0)
                {
                    this.wheelPercentage--;
                }
                else
                {
                    this.wheelReturnTimer.Stop();
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
            this.wheelReturnTimer.Stop();
            this.wheelIsTurningLeft = true;
        }

        public void HandleKeyUp_Left()
        {
            this.wheelIsTurningLeft = false;
            if (!this.wheelIsTurningRight)
            {
                this.wheelReturnTimer.Start();
            }
        }

        public void HandleKeyDown_Right()
        {
            this.wheelReturnTimer.Stop();
            this.wheelIsTurningRight = true;
        }

        public void HandleKeyUp_Right()
        {
            this.wheelIsTurningRight = false;
            if (!this.wheelIsTurningLeft)
            {
                this.wheelReturnTimer.Start();
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

        public void ResetAllValues()
        {
            this.throttlePercentage = 0;
            this.brakePercentage = 0;
            this.wheelPercentage = 0;
            this.shiftingDirection = SequentialShiftingDirections.Nothing;
        }

        public override void Process()
        {
            this.KeyboardHandlerPacket.ShiftUpOrDown = this.shiftingDirection;
            this.shiftingDirection = SequentialShiftingDirections.Nothing;
            this.KeyboardHandlerPacket.ThrottlePercentage = this.throttlePercentage;
            this.KeyboardHandlerPacket.BrakePercentage = this.brakePercentage;

            if (this.wheelIsTurningLeft && this.wheelPercentage > -100)
            {
                this.wheelPercentage -= 100 / 30;
            }

            if (this.wheelIsTurningRight && this.wheelPercentage < 100)
            {
                this.wheelPercentage += 100 / 30;
            }

            // ez azért 
            //if (this.virtualFunctionBus.LKAInputPacket.LKAOnOff)
            //{
            //    // this.KeyboardHandlerPacket.WheelPercentage = this.virtualFunctionBus.LKAInputPacket.WheelPercentage;
            //}
            //else
            //{
                this.KeyboardHandlerPacket.WheelPercentage = this.wheelPercentage;
            //}
        }
    }
}