namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.LaneKeepingAssistant;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
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
            this.viewDistance = 200;
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
            LKAWarnigControll();

            WorldObject HIGHLIGHTR = this.HighlightedObject;

            fortest();
        }

        public void fortest()
        {
            List<WorldObject> helper = this.CurrentObjectsinView;
            var closestroad = helper.Where(x => x.WorldObjectType == WorldObjectType.Road).OrderBy(x => ClosestPointOfObject(SensorTriangle.Points[0], x)).FirstOrDefault();



            List<Geometry> goes = new List<Geometry>();
 
            

            if (closestroad != null) {
                Point UpdatedCenterPoint = this.helo(closestroad.Geometries[1].Points, closestroad);
                Point UpdatedEdgePoint;

                Point Updated0Point = this.helo(closestroad.Geometries[0].Points, closestroad);
                Point Updated2Point = this.helo(closestroad.Geometries[2].Points, closestroad);


                if (CalculateDistance(Updated0Point.X,Updated0Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y)>
                    CalculateDistance(Updated2Point.X, Updated2Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y))
                {
                    UpdatedEdgePoint = Updated2Point;
                }
                else
                {
                    UpdatedEdgePoint = Updated0Point;
                }

            }
        }

        Point helo(Points point, WorldObject wo)
        {
            double distance = double.MaxValue;
            Point result = new Point();
            foreach(var item in point)
            {

                double currentdist = CalculateDistance(GetTransformedPoint(item, wo).X, GetTransformedPoint(item, wo).Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y);
                if (currentdist < distance)
                {
                    distance = currentdist;
                    result = GetTransformedPoint(item, wo);
                }
            }
            return result;
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

        public void LKAWarnigControll()
        {
            if(this.LKAHandlerPacket.Warning)
            {
                this.LKAHandlerPacket.WarningMessage = "WARNING LKA: Steep Turn! LKA will be powered off";
            }
            else
            {
                this.LKAHandlerPacket.WarningMessage = "";
                foreach (WorldObject relobj in this.CurrentObjectsinView)
                {
                    if (relobj.WorldObjectType.Equals(WorldObjectType.Crosswalk))
                    {
                        this.LKAHandlerPacket.Warning = true;
                        this.LKAHandlerPacket.WarningMessage = "WARNING Camera: Crosswalk in range. LKA will be powered off";
                    }
                    else
                    {
                        this.LKAHandlerPacket.Warning = false;
                        this.LKAHandlerPacket.WarningMessage = "";
                    }
                }
            }
        }
    }
}
