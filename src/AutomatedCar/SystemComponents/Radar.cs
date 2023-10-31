﻿namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Linq;
    using AutomatedCar.Models;

    internal class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus, automatedCar, 10, 60, 200)
        {
        }

        public void ObjectInRange (WorldObject worldObject)
        {
            this.CurrentObjectsinView.Add(worldObject);
        }

        public override void Process()
        {
            // this.CollisionDetection();
            this.CreateSensorTriangle(automatedCarForSensors, distanceFromCarCenter, viewAngle, viewDistance);
            this.ObjectsinViewUpdate(World.Instance.WorldObjects);
            
            this.RemoveObjectsNotinView();
            this.RefreshDistances(); 
            this.RefreshPreviousObjects();
        }

        //figure out how the PolylineGeometry works
        public void VisualiseRadarVision()
        {
            //this.sensorTriangle
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
            if (this.previousObjectinView.Count > 0)
            {
                foreach (RelevantObject prevobj in this.previousObjectinView)
                {
                    if (!this.CurrentObjectsinView.Contains(prevobj.RelevantWorldObject))
                    {
                        this.previousObjectinView.Remove(prevobj);
                    }
                }
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
    }
}
