namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
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

        public LKAInputPacket LKAInputPacket { get; set; }

        double previouserror;

        public Camera(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 10;
            this.viewDistance = 400; // 4000 = 80m
            this.viewAngle = 60;

            this.LKASetup();
        }

        private void LKASetup()
        {
            this.LKAInputPacket = new LKAInputPacket();
            this.virtualFunctionBus.LKAInputPacket = this.LKAInputPacket;

            this.LKAInputPacket.LKAAvailable = true;
            this.LKAInputPacket.LKAOnOff = false;
            this.LKAInputPacket.Message = "LKA OFF";
            this.LKAInputPacket = new LKAInputPacket();
            this.virtualFunctionBus.LKAInputPacket = this.LKAInputPacket;

            this.previouserror = 0;
        }
        
        public override void Process()
        {
            this.CreateSensorTriangle(this.automatedCarForSensors, this.distanceFromCarCenter, this.viewAngle, this.viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            this.RefreshRelevantObjects();
            this.GetClosestHighlightedObject();
            this.LKAOnOffControll();
            this.LKAWarnigControll();
            this.CheckToTurnOffLKA(this.LKAInputPacket.WheelPercentage);

            if (this.LKAInputPacket.LKAOnOff)
            {
                this.PDControl();
            }
            else
            {
                this.LKAInputPacket.WheelPercentage = null;
            }
        }
        
        public void PDControl()
        {
            List<WorldObject> helper = this.CurrentObjectsinView;
            var closestroad = helper.Where(x => x.WorldObjectType == WorldObjectType.Road).OrderBy(x => ClosestPointOfObject(SensorTriangle.Points[0], x)).FirstOrDefault();

            if (closestroad != null) 
            {
                Point UpdatedCenterPoint = this.GeometryPointGetter(closestroad.Geometries[1].Points, closestroad);

                Point UpdatedEdgePoint;

                Point Updated0Point = this.GeometryPointGetter(closestroad.Geometries[0].Points, closestroad);
                Point Updated2Point = this.GeometryPointGetter(closestroad.Geometries[2].Points, closestroad);



                if (CalculateDistance(Updated0Point.X,Updated0Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y) >
                    CalculateDistance(Updated2Point.X, Updated2Point.Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y))
                {
                    UpdatedEdgePoint = Updated2Point;
                }
                else
                {
                    UpdatedEdgePoint = Updated0Point;
                }

                Point LKAPoint = NewLKAPoint();
                
                double LKACenterdistance = CalculateDistance(UpdatedCenterPoint.X, UpdatedCenterPoint.Y, LKAPoint.X, LKAPoint.Y);
                double LKAEdgedistance = CalculateDistance(UpdatedEdgePoint.X, UpdatedEdgePoint.Y, LKAPoint.X, LKAPoint.Y);

                double error = LKACenterdistance - LKAEdgedistance;
                

                double Kp = 0.05;
                double Kd = 0.2;

                double P = Kp * error;
                double D = Kd * (error - this.previouserror);

                double pdOutput = -(P + D);

                this.LKAInputPacket.WheelPercentage = pdOutput;
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
            }
        }

        public void CheckToTurnOffLKA(double? angleAsDegree)
        {

            if (angleAsDegree >= 30 && angleAsDegree <= 45)
            {
                this.LKAInputPacket.Warning = true;
            }
            else if (angleAsDegree <= -30 && angleAsDegree >= -45)
            {
                this.LKAInputPacket.Warning = true;
            }
            else if (angleAsDegree < 30)
            {
                this.LKAInputPacket.Warning = false;
            }

            if(angleAsDegree > 45 || angleAsDegree < -45)
            {
                this.LKATurnOnOffSteering();
            }
        }

        public void LKATurnOnOff()
        {
            this.LKAInputPacket.LKAOnOff = !this.LKAInputPacket.LKAOnOff;
        }

        public void LKATurnOnOffSteering()
        {
            this.LKAInputPacket.LKAOnOff = false;
        }

        public void LKAOnOffControll()
        {
            if (this.LKAInputPacket.LKAOnOff)
            {
                this.LKAInputPacket.Message = "LKA ON";
            }
            else
            {
                this.LKAInputPacket.Message = "LKA OFF";
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

        public void LKAWarnigControll()
        {
            if(this.LKAInputPacket.Warning)
            {
                this.LKAInputPacket.WarningMessage = "WARNING LKA: Steep Turn! LKA will be powered off";
            }
            else
            {
                this.LKAInputPacket.WarningMessage = "";
                foreach (WorldObject relobj in this.CurrentObjectsinView)
                {
                    if (relobj.WorldObjectType.Equals(WorldObjectType.Crosswalk))
                    {
                        this.LKAInputPacket.Warning = true;
                        this.LKAInputPacket.WarningMessage = "WARNING Camera: Crosswalk in range. LKA will be powered off";
                    }
                    else
                    {
                        this.LKAInputPacket.Warning = false;
                        this.LKAInputPacket.WarningMessage = "";
                    }
                }
            }
        }
    }
}
