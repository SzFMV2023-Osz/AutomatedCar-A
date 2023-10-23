namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGearBox
    {
        int Velocity { get; set; }
        string GearStage { get; }
        void ShiftingGear(GearShift shift);
        void CalculateGearSpeed(ref int revolution);
    }
}
