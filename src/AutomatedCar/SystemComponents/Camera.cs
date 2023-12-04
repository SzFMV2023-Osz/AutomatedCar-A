namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.LaneKeepingAssistant;
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    public class Camera : Sensor
    {
        public List<WorldObject> RelevantObjects { get; set; }

        public LKAHandlerPacket LKAHandlerPacket { get; set; }

        public _45degreeCheck LKA45degreeCheck { get; set; }

        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 10;
            this.viewDistance = 80;
            this.viewAngle = 60;

            this.LKAHandlerPacket = new LKAHandlerPacket();
            this.virtualFunctionBus.LKAHandlerPacket = this.LKAHandlerPacket;

            this.LKAHandlerPacket.LKAAvailable = true;
            this.LKAHandlerPacket.LKAOnOff = true;
            this.LKAHandlerPacket.Message = "LKA ON";
            this.LKA45degreeCheck = new _45degreeCheck(virtualFunctionBus);

        }
        
        public override void Process()
        {
            this.CreateSensorTriangle(this.automatedCarForSensors, this.distanceFromCarCenter, this.viewAngle, this.viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            this.RefreshRelevantObjects();
            this.GetClosestHighlightedObject();
            LKAOnOffControll();
        }

        // Returns relevant objects (Roads)
        public void RefreshRelevantObjects()
        {
            List<WorldObject> relevantObjects = new List<WorldObject>();

            foreach (WorldObject relobj in World.Instance.WorldObjects)
            {
                if (relobj.WorldObjectType.Equals(WorldObjectType.Road) || relobj.WorldObjectType.Equals(WorldObjectType.Crosswalk))
                {
                    relevantObjects.Add(relobj);
                }
            }

            this.RelevantObjects = relevantObjects;
        }

        public void GetClosestHighlightedObject()
        {
            for (int i = 0; i < this.RelevantObjects.Count - 1; i++)
            {
                if (this.CalculateDistance(this.RelevantObjects[i].X, this.RelevantObjects[i].Y, this.SensorPosition.X, this.SensorPosition.Y)
                    <= this.CalculateDistance(this.RelevantObjects[i + 1].X, this.RelevantObjects[i + 1].Y, this.SensorPosition.X, this.SensorPosition.Y))
                {
                    this.HighlightedObject = this.RelevantObjects[i];
                }
            }
        }

        public void LKATurnOnOff()
        {
            this.LKAHandlerPacket.LKAOnOff = !this.LKAHandlerPacket.LKAOnOff;
            
            
        }
        public void LKAOnOffControll()
        {
            if (this.LKAHandlerPacket.LKAOnOff)
            {
                this.LKAHandlerPacket.Message = "LKA ON";
            }
            else
            {
                this.LKAHandlerPacket.Message = "LKA OFF";
            }
        }
    }
}
