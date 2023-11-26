namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPedalInterface
    {
        //The value could be between 0 and 100
        byte PedalPosition { get; set; }
    }
}