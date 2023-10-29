namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct RelevantObject
    {
        public RelevantObject(WorldObject relevantobject, double currentdistance, double previousdistance)
        {
            this.RelevantWorldObject = relevantobject;
            this.CurrentDistance = currentdistance;
            this.PreviousDistance = previousdistance;
        }

        // kell korábbi távolság is?
        public double PreviousDistance { get; set; }

        public WorldObject RelevantWorldObject { get; set; }

        public double CurrentDistance { get; set; }

        //privátra rakás
        public void modifyCurrentDistance(double distance)
        {
            this.CurrentDistance = distance;
        }

        public void modifyPreviousDistance(double distance)
        {
            this.PreviousDistance = distance;
        }
    }
}
