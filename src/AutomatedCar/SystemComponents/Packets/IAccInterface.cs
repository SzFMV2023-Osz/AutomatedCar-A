namespace AutomatedCar.SystemComponents.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAccInterface
    {
        bool IsActive { get; }
        double SelectedTargetDistance { get; }
        int SelectedSpeed { get; }
        double Distance { get; }
    }
}