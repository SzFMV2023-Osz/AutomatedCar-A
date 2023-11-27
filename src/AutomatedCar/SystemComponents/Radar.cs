namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    using DynamicData;
    using ReactiveUI;

    internal class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar)
        {
            this.distanceFromCarCenter = 115;
            this.viewDistance = 200;
            this.viewAngle = 60;
        }

        private void SelectLane(List<WorldObject> roads)
        {
            
            List<Point> CenterLine = new List<Point>();
            List<Point> ALine = new List<Point>();
            List<Point> BLine = new List<Point>();
            List<Point> cp = new List<Point>();
            cp.AddRange(ActualizeGeometry(roads[0].Geometries[1], roads[0]).Points);
            cp.AddRange(ActualizeGeometry(roads[0].Geometries[0], roads[0]).Points);
            cp.AddRange(ActualizeGeometry(roads[0].Geometries[2], roads[0]).Points);
            CenterLine.AddRange(ActualizeGeometry(roads[0].Geometries[1], roads[0]).Points);
            ALine.AddRange(ActualizeGeometry(roads[0].Geometries[0], roads[0]).Points);
            BLine.AddRange(ActualizeGeometry(roads[0].Geometries[2], roads[0]).Points);

            for (int i = 1; i < roads.Count; i++)
            {
                
                    cp.AddRange(ActualizeGeometry(roads[i].Geometries[1], roads[i]).Points);
                    cp.AddRange(ActualizeGeometry(roads[i].Geometries[0], roads[i]).Points);
                    cp.AddRange(ActualizeGeometry(roads[i].Geometries[2], roads[i]).Points);
                
                foreach (var g in roads[i].Geometries)
                {
                    PolylineGeometry tg=ActualizeGeometry(g, roads[i]); 
                    if (tg.Points.Any(x => ALine.Contains(x)))
                    {
                        ALine.AddRange(tg.Points);
                    }
                    else if (tg.Points.Any(x => BLine.Contains(x)))
                    {
                        BLine.AddRange(tg.Points);
                    }
                }
                CenterLine.AddRange(ActualizeGeometry(roads[i].Geometries[1], roads[i]).Points);
            }
            PolylineGeometry center = new PolylineGeometry();
            center.Points.Add(CenterLine);
            PolylineGeometry a = new PolylineGeometry();
            a.Points.Add(ALine);

        }

        public WorldObject ClosestObjInLane()
        {
            var roadsInView = CurrentObjectsinView
                .Where(x => x.WorldObjectType == WorldObjectType.Road)
                .OrderBy(x => ClosestPointOfObject(SensorTriangle.Points[0], x))
                .ToList();
            SelectLane(roadsInView);

            if (roadsInView.Count > 0)
            {
                PolylineGeometry aLane = MakeLane(roadsInView, 0);
                PolylineGeometry bLane = MakeLane(roadsInView, 2);
                if (aLane.FillContains(this.SensorTriangle.Points[0]))
                {
                    var closestobj = this.CurrentObjectsinView
                        .Where(x => IntersectsWithObject(aLane, x))
                        .OrderBy(x => CalculateDistance(SensorTriangle.Points[0].X, SensorTriangle.Points[0].Y, x.X, x.Y))
                        .FirstOrDefault();
                    return closestobj;
                }
                else if (bLane.FillContains(this.SensorTriangle.Points[0]))
                {
                    return this.CurrentObjectsinView.Where(x => x.Geometries.All(y => y.Points.All(p => bLane.FillContains(GetTransformedPoint(p, x))))
                         && x.WorldObjectType != WorldObjectType.Road && x != this.automatedCarForSensors)
                        .OrderBy(x => CalculateDistance(SensorTriangle.Points[0].X, SensorTriangle.Points[0].X, x.X, x.Y)).ToList().FirstOrDefault();

                    //return CurrentObjectsinView.Where(x => bLane
                    //    .FillContains(new Point(x.X, x.Y)) && x.WorldObjectType != WorldObjectType.Road && x != this.automatedCarForSensors)
                    //    .OrderBy(x => CalculateDistance(SensorTriangle.Points[0].X, SensorTriangle.Points[0].X, x.X, x.Y)).ToList().FirstOrDefault();
                }
            }

            return null;
        }

        private PolylineGeometry MakeLane(List<WorldObject> roads, int lane)
        {
            
            PolylineGeometry pl = new PolylineGeometry();
            foreach (var road in roads)
            {
                pl.Points.AddRange(this.ActualizeGeometry(road.Geometries[lane], road).Points);
            }

            for (int i = roads.Count-1; i >= 0; i--)
            {
                for (int j = roads[i].Geometries[1].Points.Count-1; j >= 0; j--)
                {
                    pl.Points.Add(this.ActualizeGeometry(roads[i].Geometries[1], roads[i]).Points[j]);
                }
            }

            pl.Points.Add(pl.Points.First());
            return pl;
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
            try
            {
                this.ClosestObjInLane();
            }

            catch (Exception a)
            {
                Console.WriteLine(a.Message);
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


    }
}
