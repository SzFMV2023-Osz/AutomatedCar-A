namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Camera : Sensor
    {
        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)

        {
            this.distanceFromCarCenter = 10;
            this.viewDistance = 80;
            this.viewAngle = 60;
        }

        public override void Process()
        {
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.CurrentObjectsinView.Add(worldObject);
        }

        // Returns relevant objects (Roads)
        public List<WorldObject> RelevantObjects()
        {
            List<WorldObject> relevantObjects = new List<WorldObject>();

            foreach (WorldObject relobj in this.CurrentObjectsinView)
            {
                if (relobj.WorldObjectType.Equals(WorldObjectType.Road) || relobj.WorldObjectType.Equals(WorldObjectType.Crosswalk))
                {
                    relevantObjects.Add(relobj);
                }
            }

            return relevantObjects;
        }
    }
}
