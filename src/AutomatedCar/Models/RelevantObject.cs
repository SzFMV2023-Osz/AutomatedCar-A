namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct RelevantObject
    {
        public RelevantObject(WorldObject objectt, double objectDistance)
        {
            this.Object = objectt;
            this.ObjectDistance = objectDistance;
        }

        public WorldObject Object { get; set; }
        public double ObjectDistance { get; set; }

        public void modifyDistance(double distance)
        {
            this.ObjectDistance = distance;
        }
    }
}
