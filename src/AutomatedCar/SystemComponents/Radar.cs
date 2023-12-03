namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using Avalonia;
    using DynamicData;
    using DynamicData.Aggregation;

    internal class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 115;
            this.viewDistance = 200;
            this.viewAngle = 60;
        }

        // releváns road signokat tároljuk
        private List<WorldObject> RelevantSigns = new List<WorldObject>();
        private List<MapSign> Signs = new List<MapSign>();

        public int SpeedLimit = new int();

        public override void Process()
        {
            this.ClosestHighlightedObject();
            this.DetectCollision();
            this.CreateSensorTriangle(this.automatedCarForSensors, this.distanceFromCarCenter, this.viewAngle, this.viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            this.RemoveObjectsNotinView();
            this.RefreshDistances();
            this.RefreshPreviousObjects();
            this.UpdateRelevantsigns();
            this.UpdateMaxSpeed();
        }

        private void UpdateMaxSpeed()
        {
            var relevants = this.Signs.Where(x => x.MaxSpeed >= 0);
            var max = relevants.Where(x => x.Distance == relevants.Min(y=>y.Distance)).FirstOrDefault();
            if (max != null)
            {
                this.SpeedLimit = max.MaxSpeed;
            }
        }
        private void UpdateRelevantsigns()
        {
            this.RelevantSigns = new List<WorldObject>();
            this.Signs = new List<MapSign>();
            foreach (WorldObject item in CurrentObjectsinView)
            {
                if(item.WorldObjectType== WorldObjectType.RoadSign)
                {
                    this.RelevantSigns.Add(item);
                    this.Signs.Add(new MapSign(item.Filename,this.automatedCarForSensors.X,this.automatedCarForSensors.Y,item.X,item.Y));
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
                if (!WO.WorldObjectType.Equals(WorldObjectType.Road))
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

        public void DetectCollision()
        {
            var data = World.Instance.WorldObjects.Where(x => x.WorldObjectType != WorldObjectType.Crosswalk
                     && x.WorldObjectType != WorldObjectType.Road
                     && x.WorldObjectType != WorldObjectType.Other
                     && x.WorldObjectType != WorldObjectType.ParkingSpace);

            foreach (var obj in data)
            {
                if (this.IsInCar(obj))
                {
                    this.automatedCarForSensors.Collideable = true;
                    return;
                }
            }

            this.automatedCarForSensors.Collideable = false;
        }

        private bool IsInCar(WorldObject obj)
        {
            foreach (var g in obj.Geometries)
            {
                Rect old = this.automatedCarForSensors.Geometries[0].Bounds;
                AutomatedCar car = this.automatedCarForSensors;
                Rect actualPos = new Rect(old.X + car.X - (old.Width / 2), old.Y + car.Y - (old.Height / 2), old.Width, old.Height);

                if (actualPos.Intersects(new Rect(g.Bounds.X + obj.X, g.Bounds.Y + obj.Y, g.Bounds.Width, g.Bounds.Height)))
                {
                    return true;
                }
            }

            return false;
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
    }

    public class MapSign
    {

        public int MaxSpeed;
        public double Distance;
        public MapSign(string name,int carx, int cary, int sx,int sy) 
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
                if (digitsString.Contains("stop"))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
