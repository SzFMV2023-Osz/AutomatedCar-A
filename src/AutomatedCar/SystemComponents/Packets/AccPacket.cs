namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class AccPacket : ReactiveObject, IAccInterface
    {
        bool isActive;
        bool isAccelerating;
        double selectedTargetDistance;
        int selectedSpeed;
        double distance;

        public bool IsActive { get => this.isActive; set => this.RaiseAndSetIfChanged(ref this.isActive, value); }

        public bool IsAccelerating { get => this.isAccelerating; set => this.RaiseAndSetIfChanged(ref this.isAccelerating, value); }

        public double SelectedTargetDistance { get => this.selectedTargetDistance; set => this.RaiseAndSetIfChanged(ref this.selectedTargetDistance, value); }

        public int SelectedSpeed { get => this.selectedSpeed; set => this.RaiseAndSetIfChanged(ref this.selectedSpeed, value); }

        public double Distance { get => this.distance; set => this.RaiseAndSetIfChanged(ref this.distance, value); }

    }
}