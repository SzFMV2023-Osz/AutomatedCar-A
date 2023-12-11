namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Packets.InputPackets.DriveAssistPackets;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    using Metsys.Bson;
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
            this.LKAInputPacket.OnOffMessage = "LKA OFF";
            this.LKAInputPacket.AvailableMessage = "LKA Available";
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
            this.LKAOnOffAvailableControll();
            this.LKAWarnigControll();
            this.CheckToTurnOffLKA(this.LKAInputPacket.WheelPercentage);

           
            this.LKAHandler();
        }

        // calls everything which is needed for the LKA
        void LKAHandler()
        {
            List<WorldObject> roads = this.CurrentObjectsinView.Where(x => x.WorldObjectType.Equals(WorldObjectType.Road)).ToList();
            // getting the closest road to the car based on its closest point in the middle lane
            WorldObject closestroad = this.GettingClosestRoad(roads);
            
            if (closestroad == null || !this.CanLKA(closestroad))
            {
                this.LKAInputPacket.LKAAvailable = false;
            }
            else
            {
                this.LKAInputPacket.LKAAvailable = true;
            }
            
            if (this.LKAInputPacket.LKAAvailable && this.LKAInputPacket.LKAOnOff)
            {
                this.PDControl(closestroad);
            }
            else
            {
                this.LKAInputPacket.WheelPercentage = null;
            }
        }
        
        public void PDControl(WorldObject closestroad)
        {
            Point UpdatedCenterPoint = this.GeometryPointGetter(closestroad.Geometries[1].Points, closestroad);

            Point UpdatedEdgePoint;
            Point rightmeasurepoint = this.RightMeasurePoint();

            Point Updated0Point = this.GeometryPointGetter(closestroad.Geometries[0].Points, closestroad);
            Point Updated2Point = this.GeometryPointGetter(closestroad.Geometries[2].Points, closestroad);

            if (CalculateDistance(Updated0Point.X, Updated0Point.Y, rightmeasurepoint.X, rightmeasurepoint.Y) >
                   CalculateDistance(Updated2Point.X, Updated2Point.Y, rightmeasurepoint.X, rightmeasurepoint.Y))
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


            double Kp = 0.1;
            double Kd = 0.05;

            double P = Kp * error;
            double D = Kd * (error - this.previouserror);

            double pdOutput = -(P + D);

            this.LKAInputPacket.WheelPercentage = pdOutput;
            
            if (error > 2)
            {
                this.previouserror = 3;
            }
            else if (previouserror < -2)
            {
                this.previouserror = -3;
            }
            else
            {
                this.previouserror = error;
            }

            /*
            List<WorldObject> helper = this.CurrentObjectsinView;
            var closestroad = helper.Where(x => x.WorldObjectType == WorldObjectType.Road).
                OrderBy(x => ClosestPointOfObject(SensorTriangle.Points[0], x)).FirstOrDefault();

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
            */
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

        // method that checks if the LKA should be switched to unavailable due to the angle of the steeringwheel
        // !!! beacuse of the priority handler when the LKA is off we can't observe the WheelPercentage if the LKA Packet since it's null !!!        
        /*bool WheelPercentageCheck()
        {
            if (this.LKAInputPacket.WheelPercentage > 45 || this.LKAInputPacket.WheelPercentage < -45)
            {
                return false;
            }

            return true;
        }
        */

        // method that switches the LKA OnOFff state if the LKA is available, else it just turns it off
        public void LKATurnOnOff()
        {
            if (this.LKAInputPacket.LKAAvailable)
            {
                this.LKAInputPacket.LKAOnOff = !this.LKAInputPacket.LKAOnOff;
            }
            else
            {
                this.LKAInputPacket.LKAOnOff = false;
            }
        }

        // method that turns off the LKA if it's beacuse of steering
        public void LKATurnOnOffSteering()
        {
            this.LKAInputPacket.LKAOnOff = false;
        }

        // adjusts the messages during every Tick according to the Packet values
        public void LKAOnOffAvailableControll()
        {
            if (this.LKAInputPacket.LKAOnOff)
            {
                this.LKAInputPacket.OnOffMessage = "LKA ON";
            }
            else
            {
                this.LKAInputPacket.OnOffMessage = "LKA OFF";
            }

            if (this.LKAInputPacket.LKAAvailable)
            {
                this.LKAInputPacket.AvailableMessage = "LKA Available";
            }
            else
            {
                this.LKAInputPacket.AvailableMessage = "LKA Unavailable";
                this.LKAInputPacket.LKAOnOff = false;
            }
        }

        // returns the LKAPoint, its suposed to be a point of a vector at the other side of the edge points compared to the car
        Point NewLKAPoint()
        {
            double doubleX = SensorPosition.X - automatedCarForSensors.X;
            double doubleY = SensorPosition.Y - automatedCarForSensors.Y;

            return new Point(doubleX * 10 + automatedCarForSensors.X, doubleY * 10 + automatedCarForSensors.Y);
        }

        Point RightMeasurePoint()
        {
            double doubleX = automatedCarForSensors.Geometries[0].Bounds.TopRight.X - automatedCarForSensors.X;
            double doubleY = automatedCarForSensors.Geometries[0].Bounds.TopRight.Y - automatedCarForSensors.Y;

            return new Point(doubleX * 100 + automatedCarForSensors.X, doubleY * 100 + automatedCarForSensors.Y );
        }

        // returns if the LKA can handle the closest road object to the car
        bool CanLKA(WorldObject WO)
        {
            if(WO == null ||
               (!WO.Filename.Contains("straight") &&
               !WO.Filename.Contains("45") &&
               !WO.Filename.Contains("6")))
            {
                return false;
            }

            return true;
        }

        // returns the closest Road type Worldobject to the car
        WorldObject GettingClosestRoad(List<WorldObject> roads)
        {
            WorldObject closestroad = null;
            Point crhelper = new Point(100000, 100000);

            foreach (WorldObject WO in roads)
            {
                foreach (Point observedpoint in WO.Geometries[1].Points)
                {
                    if (CalculateDistance(observedpoint.X, observedpoint.Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y)
                        < CalculateDistance(crhelper.X, crhelper.Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y))
                    {
                        crhelper = observedpoint;
                        closestroad = WO;
                    }
                }
            }

            return closestroad;
        }

        Point GeometryPointGetter(Points point, WorldObject wo)
        {
            double distance = 0; //double.MaxValue;
            Point result = new Point();
            foreach(var item in point)
            {

                double currentdist = CalculateDistance(GetTransformedPoint(item, wo).X, GetTransformedPoint(item, wo).Y, SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y);
                if (currentdist > distance)
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

        // checks if the LKA can handle the second closest road object from the currentobjectinview list
        bool CheckSecondClosestRoad()
        {
            // list of roads from the currentobjectinview list (it's neccesary to filter since we can see the roads signs too)
            List<WorldObject> roads = this.CurrentObjectsinView.Where(x => x.WorldObjectType.Equals(WorldObjectType.Road)).ToList();

            // getting the closest road to the car based on its closest point in the middle lane
            WorldObject closestroad = this.GettingClosestRoad(roads);

            // removing the closest road from the roads, so we can get the second closest
            roads.Remove(closestroad);

            //getting the second closest road
            WorldObject secondclosestroad = roads.OrderBy(x => this.ClosestPointOfObject(this.SensorTriangle.Points[0], x)).FirstOrDefault();

            // true if the LKA can handle the next roadobject, false if the LKA can't handle the next roadobject
            return this.CanLKA(secondclosestroad);
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
