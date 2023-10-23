namespace AutomatedCar.SystemComponents.Gearbox
{
    using AutomatedCar.Helpers.Gearbox_helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ATGearBox : IGearBox
    {
        private ATGears gear;
        public int Velocity { get; set; }
        public string GearStage { get; private set; }
        public void CalculateGearSpeed(ref int revolution)
        {
            throw new NotImplementedException();
        }
        public void ShiftingGear(GearShift shift)
        {
            if (Enum.IsDefined(typeof(ATGears), gear + ((int)shift)))
            {
                gear += (int)shift;
                GearStage = gear.ToString()[0].ToString();
            }
        }
    }
}
