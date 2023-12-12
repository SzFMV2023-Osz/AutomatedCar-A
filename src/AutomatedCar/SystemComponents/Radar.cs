namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;
    using DynamicData;
    using DynamicData.Aggregation;
    using AutomatedCar.SystemComponents.Packets;
    using AutomatedCar.SystemComponents.Helpers;

    public class Radar : Sensor
    {
        private List<WorldObject> RelevantSigns = new List<WorldObject>();
        private List<MapSign> Signs = new List<MapSign>();

        private int SpeedLimit;

        private RelevantObjectsHandlerPacket RelevantObjetPacket;
        public Radar(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 115;
            this.viewDistance = 1000; // 10000 = 200m
            this.viewAngle = 60;
            this.SpeedLimit = 0;

            this.RelevantObjectsPacket = new RelevantObjectsHandlerPacket();

            this.virtualFunctionBus.RelevantObjectsPacket = this.RelevantObjectsPacket;


        }

        public override void Process()
        {
            this.ClosestHighlightedObject();
            this.DetectCollision();
            this.CreateSensorTriangle(this.automatedCarForSensors, this.distanceFromCarCenter, this.viewAngle, this.viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            this.RemoveObjectsNotinView();
            this.RefreshDistances();
            this.RefreshPreviousObjects();
            this.PacketUpdate();
            this.UpdateRelevantsigns();
            this.UpdateMaxSpeed();

            List<RelevantObject> gfwer = this.previousObjectinView;
        }
        private void UpdateMaxSpeed()
        {
            var relevants = this.Signs.Where(x => x.MaxSpeed >= 0);
            var max = relevants.Where(x => x.Distance == relevants.Min(y => y.Distance)).FirstOrDefault();
            if (max != null)
            {
                this.SpeedLimit = max.MaxSpeed;
            }
            else
            {
                this.SpeedLimit = 200;
            }


            this.RelevantObjectsPacket.LimitSpeed = SpeedLimit;
        }

        private void UpdateRelevantsigns()
        {
            this.RelevantSigns = new List<WorldObject>();
            this.Signs = new List<MapSign>();
            foreach (WorldObject item in CurrentObjectsinView)
            {
                if (item.WorldObjectType == WorldObjectType.RoadSign)
                {
                    this.RelevantSigns.Add(item);
                    this.Signs.Add(new MapSign(item.Filename, this.automatedCarForSensors.X, this.automatedCarForSensors.Y, item.X, item.Y));
                }
            }
        }

        // Refreshes the distance of elements in previousObjectinView List
        private void RefreshDistances()
        {
            foreach (RelevantObject prevobj in this.previousObjectinView)
            {
                prevobj.modifyPreviousDistance(prevobj.CurrentDistance);
                prevobj.modifyCurrentDistance(this.CalculateDistance(prevobj.RelevantWorldObject.X, prevobj.RelevantWorldObject.Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y));
            }
        }

        // Refreshes previousObjectinView List regarding the elements (excluding roads)
        private void RefreshPreviousObjects()
        {
            foreach (WorldObject WO in this.CurrentObjectsinView)
            {
                if (!WO.WorldObjectType.Equals(WorldObjectType.Road) 
                    && !WO.WorldObjectType.Equals(WorldObjectType.Crosswalk)
                    && !WO.WorldObjectType.Equals(WorldObjectType.ParkingSpace))
                {
                    bool mustadd = true;
                    foreach (RelevantObject prevobj in this.previousObjectinView)
                    {
                        if (prevobj.RelevantWorldObject.Equals(WO))
                        {
                            mustadd = false;
                            break;
                        }
                    }

                    if (mustadd)
                    {
                        double distance = this.CalculateDistance(WO.X, WO.Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y);
                        this.previousObjectinView.Add(new RelevantObject(WO, distance, distance));
                    }
                }
            }
        }

        // Removes objects that are no longer in view from previousObjectinView List
        private void RemoveObjectsNotinView()
        {
            // if other similar methods dont work properly a similar solution should be implemented as here
            List<RelevantObject> helper = new List<RelevantObject>();
            foreach (RelevantObject item in this.previousObjectinView)
            {
                helper.Add(item);
            }

            if (this.previousObjectinView.Count > 0)
            {
                foreach (RelevantObject prevobj in this.previousObjectinView)
                {
                    if (!this.CurrentObjectsinView.Contains(prevobj.RelevantWorldObject))
                    {
                        if (this.previousObjectinView.Contains(prevobj))
                        {
                            helper.Remove(prevobj);
                        }
                    }
                }

                this.previousObjectinView = helper;
            }
        }

        // Returns relevant objects
        public List<RelevantObject> RelevantObjects()
        {
            List<RelevantObject> relevantObjects = new List<RelevantObject>();

            foreach (RelevantObject relobj in this.previousObjectinView)
            {
                if (relobj.CurrentDistance < relobj.PreviousDistance)
                {
                    relevantObjects.Add(relobj);
                }
            }

            return relevantObjects;
        }

        public void ClosestHighlightedObject()
        {
            if (this.CurrentObjectsinView.Count > 0)
            {
                this.HighlightedObject = this.CurrentObjectsinView[0];
            }

            for (int i = 0; i < this.CurrentObjectsinView.Count - 1; i++)
            {
                if (this.CalculateDistance(this.CurrentObjectsinView[i].X, this.CurrentObjectsinView[i].Y, this.SensorPosition.X, this.SensorPosition.Y)
                    <= this.CalculateDistance(this.CurrentObjectsinView[i + 1].X, this.CurrentObjectsinView[i + 1].Y, this.SensorPosition.X, this.SensorPosition.Y)
                    && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Road) && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.ParkingSpace)
                    && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Other) && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Crosswalk))
                {
                    this.HighlightedObject = this.CurrentObjectsinView[i];
                }
            }
        }

        private void PacketUpdate()
        {
            List<RelevantObject> relevantObjects = this.previousObjectinView;


            relevantObjects = OrderByClosestToFurtherest(relevantObjects);
            this.RelevantObjectsPacket.RelevantObjects = relevantObjects;
            this.RelevantObjectsPacket.LimitSpeed = SpeedLimit;
            //this.virtualFunctionBus.RelevantObjectsPacket.RelevantObjects = this.CurrentObjectsinView;
        }

        protected List<RelevantObject> OrderByClosestToFurtherest(List<RelevantObject> list)
        {
            List<RelevantObject> orderedList = new List<RelevantObject>();

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    orderedList.Add(list[i]);
                }
                else
                {
                    for (int j = 0; j < orderedList.Count; j++)
                    {
                        if (this.CalculateDistance(list[i].RelevantWorldObject.X, list[i].RelevantWorldObject.Y, this.SensorPosition.X, this.SensorPosition.Y)
                                                       <= this.CalculateDistance(orderedList[j].RelevantWorldObject.X, orderedList[j].RelevantWorldObject.Y, this.SensorPosition.X, this.SensorPosition.Y))
                        {
                            orderedList.Insert(j, list[i]);
                            break;
                        }
                        else if (j == orderedList.Count - 1)
                        {
                            orderedList.Add(list[i]);
                            break;
                        }
                    }
                }
            }

            return orderedList;
        }

        public void DetectCollision()
        {
            var collidableObjects = World.Instance.WorldObjects.Where(x => x != this.automatedCarForSensors
            && (x.Collideable || x.WorldObjectType == WorldObjectType.Other)).ToList();

            PolylineGeometry newCarGeometry = this.ActualizeGeometry(
                                                                     this.automatedCarForSensors.Geometry,
                                                                     this.automatedCarForSensors);

            foreach (var obj in collidableObjects)
            {

                if (IntersectsWithObject(newCarGeometry, obj))

                {
                    this.automatedCarForSensors.Collideable = true;
                    return;
                }
            }

            this.automatedCarForSensors.Collideable = false;
        }

        private PolylineGeometry ActualizeGeometry(PolylineGeometry oldGeom, WorldObject obj)
        {
            List<Point> updatedPoints = new List<Point>();

            foreach (var item in oldGeom.Points)
            {
                Point updatedPoint = GetTransformedPoint(item, obj);

                updatedPoints.Add(updatedPoint);
            }

            return new PolylineGeometry(updatedPoints, false);
        }

        private static bool IntersectsWithObject(PolylineGeometry updatedGeometry, WorldObject obj)
        {
            foreach (var geom in obj.Geometries)
            {
                foreach (var item in geom.Points)
                {
                    Point updatedPoint = GetTransformedPoint(item, obj);

                    if (updatedGeometry.FillContains(updatedPoint))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Point GetTransformedPoint(Point geomPoint, WorldObject obj)
        {
            double angleInRad = DegToRad(obj.Rotation);

            Point transformedPoint;

            if (!obj.RotationPoint.IsEmpty)
            {
                // offset with the rotationPoint coordinate
                Point offsettedPoint = new Point(geomPoint.X - obj.RotationPoint.X, geomPoint.Y - obj.RotationPoint.Y);

                // now apply rotation
                double rotatedX = (offsettedPoint.X * Math.Cos(angleInRad)) - (offsettedPoint.Y * Math.Sin(angleInRad));
                double rotatedY = (offsettedPoint.X * Math.Sin(angleInRad)) + (offsettedPoint.Y * Math.Cos(angleInRad));

                // offset with the actual coordinate
                transformedPoint = new Point(rotatedX + obj.X, rotatedY + obj.Y);
            }
            else
            {
                // offset with the actual coordinate
                 transformedPoint = new Point(geomPoint.X + obj.X, geomPoint.Y + obj.Y);
            }

            return transformedPoint;
        }
    }
    public class MapSign
    {

        public int MaxSpeed;
        public double Distance;
        public MapSign(string name, int carx, int cary, int sx, int sy)
        {

            this.MaxSpeed = MaxS(name);
            this.Distance = GetDistance(carx, cary, sx, sy);
        }
        private double GetDistance(int carx, int cary, int sx, int sy)
        {
            double dx = sx - carx;
            double dy = sy - cary;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        private int MaxS(string input)
        {
            // Get all the digits from the input string
            string digitsString = new string(input.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrEmpty(digitsString))
            {
                // Convert the string of digits to an integer
                if (int.TryParse(digitsString, out int digits))
                {
                    return digits;
                }
                else
                {
                    throw new Exception("HIBA");
                }
            }
            else
            {
                if (input.Contains("stop"))
                {
                    return 0;
                }
            }
            return -1;
        }
    }
}
