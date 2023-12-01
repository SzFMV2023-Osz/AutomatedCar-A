namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.LaneKeepingAssistant;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
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

        public LKAInputPacket LKAInputPacket { get; set; }

        double previouserror;

        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 10;
            this.viewDistance = 200;
            this.viewAngle = 60;

            this.LKAHandlerPacket = new LKAHandlerPacket();
            this.virtualFunctionBus.LKAHandlerPacket = this.LKAHandlerPacket;

            this.LKAHandlerPacket.LKAAvailable = true;
            this.LKAHandlerPacket.LKAOnOff = false;
            this.LKAHandlerPacket.Message = "LKA OFF";
            this.LKA45degreeCheck = new _45degreeCheck(virtualFunctionBus);
            this.LKAInputPacket = new LKAInputPacket();
            this.virtualFunctionBus.LKAInputPacket = this.LKAInputPacket;

            this.previouserror = 0;
        }

        private void LKASetup()
        {

        }
        
        public override void Process()
        {
            this.CreateSensorTriangle(this.automatedCarForSensors, this.distanceFromCarCenter, this.viewAngle, this.viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            this.RefreshRelevantObjects();
            this.GetClosestHighlightedObject();
            LKAOnOffControll();
            LKAWarnigControll();

            //WorldObject HIGHLIGHTR = this.HighlightedObject;

            PDControl();
        }

        public void PDControl()
        {
            List<WorldObject> helper = this.CurrentObjectsinView;
            var closestroad = helper.Where(x => x.WorldObjectType == WorldObjectType.Road).OrderBy(x => ClosestPointOfObject(SensorTriangle.Points[0], x)).FirstOrDefault();
            // closestroad = the road we are on
            if (closestroad != null) 
            {
                // int the next few lanes we determine the points we need for the steering
                // center point on the road, the left edge point of the lane
                Point UpdatedCenterPoint = this.GeometryPointGetter(closestroad.Geometries[1].Points, closestroad);

                // right edge point of the lane
                Point UpdatedEdgePoint;

                Point Updated0Point = this.GeometryPointGetter(closestroad.Geometries[0].Points, closestroad);
                Point Updated2Point = this.GeometryPointGetter(closestroad.Geometries[2].Points, closestroad);



                if (CalculateDistance(Updated0Point.X,Updated0Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y) >
                    CalculateDistance(Updated2Point.X, Updated2Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y)
                    /*CalculateDistance(Updated0Point.X, Updated0Point.Y, UpdatedCenterPoint.X, UpdatedCenterPoint.Y) >
                    CalculateDistance(Updated2Point.X, Updated2Point.Y, UpdatedCenterPoint.X, UpdatedCenterPoint.Y)*/)
                {
                    UpdatedEdgePoint = Updated2Point;
                }
                else
                {
                    UpdatedEdgePoint = Updated0Point;
                }
                ;

                //double updatedcenterdistance = CalculateDistance(UpdatedCenterPoint.X, UpdatedCenterPoint.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y);
                //double updatededgedistance = CalculateDistance(UpdatedEdgePoint.X, UpdatedEdgePoint.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y);

                // the point we will use to determine the direction of the steering
                Point LKAPoint = NewLKAPoint();
                ;
                // the distances of the edge points from the LKAPoint
                double LKACenterdistance = CalculateDistance(UpdatedCenterPoint.X, UpdatedCenterPoint.Y, LKAPoint.X, LKAPoint.Y);
                double LKAEdgedistance = CalculateDistance(UpdatedEdgePoint.X, UpdatedEdgePoint.Y, LKAPoint.X, LKAPoint.Y);

                double error = LKACenterdistance - LKAEdgedistance;
                

                double Kp = 0.05;
                double Kd = 0.2;

                double P = Kp * error;
                double D = Kd * (error - this.previouserror);

                double pdOutput = -(P + D);

                this.LKAInputPacket.WheelCorrection = pdOutput;
                ;
                if (error > 2)
                {
                    this.previouserror = 2;
                }
                else if (previouserror < -2)
                {
                    this.previouserror = -2;
                }
                else
                {
                    this.previouserror = error;
                }


                ;
            }
        }

        // method to get the LKAPoint, its suposed to be a point of a vector at the other sides of the edge points compared to the car
        Point NewLKAPoint()
        {
            double doubleX = SensorPosition.X - automatedCarForSensors.X;
            double doubleY = SensorPosition.Y - automatedCarForSensors.Y;

            return new Point(doubleX * 100 + automatedCarForSensors.X, doubleY * 100 + automatedCarForSensors.Y);
        }

        Point GeometryPointGetter(Points point, WorldObject wo)
        {
            double distance=  double.MaxValue;
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
