namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
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
                prevobj.modifyCurrentDistance(this.CalculateDistance(prevobj.RelevantWorldObject.X, prevobj.RelevantWorldObject.Y, 0, 0));
            }
        }

        // Refreshes previousObjectinView List regarding the elements (excluding roads)
        private void RefreshPreviousObjects()
        {
            // végig megyünk a mostani objecteken, és ha nem találunk egyet
            // a korábbi objectek listájában, hozzáadjuk
            foreach (WorldObject WO in this.currentObjectinView)
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
                        double distance = this.CalculateDistance(WO.X, WO.Y, 0, 0);
                        this.previousObjectinView.Add(new RelevantObject(WO, distance, distance));
                    }
                }
            }
        }

        // Removes objects that are no longer in view from previousObjectinView List
        private void RemoveObjectsNotinView()
        {
            // ha előbb láttuk de most nem, töröljük
            foreach (RelevantObject prevobj in this.previousObjectinView)
            {
                if (!this.currentObjectinView.Contains(prevobj.RelevantWorldObject))
                {
                    this.previousObjectinView.Remove(prevobj);
                }
            }
        }

        // Returns relevant objects
        public List<RelevantObject> RelevantObjects()
        {
            /*
            // visszaadandó relevantobject lista
            List<RelevantObject> returnRelevants = new List<RelevantObject>();
            // segédlista
            List<WorldObject> helpList = new List<WorldObject>();

            // ha egy obj most is és előbb is látva volt, a segédlistába rakjuk
            foreach (WorldObject relobj in this.currentObjectinView)
            {
                if (this.previousObjectinView.Contains(relobj))
                {
                    helpList.Add(relobj);
                }
            }

            // végig megyünk a korábban releváns objektumok listáján
            foreach (RelevantObject relobj2 in this.previousRelevant)
            {
                // végig megyünk a segédlistán
                foreach (WorldObject WO in helpList)
                {
                    // ha a segédlista eleme megegyezik a korábbi relevánssal és a távolsága is kisebb lett:
                    if (relobj2.RelevantWorldObject.Equals(WO) && 
                        this.CalculateDistance(relobj2.RelevantWorldObject.X, relobj2.RelevantWorldObject.Y, 0, 0) <= relobj2.PreviousDistance)
                    {
                        // frissítjük a távolságait, hozzáadjuk a visszaadandó listába
                        // éshogy ne iteráljon rajta végig fölöslegesen megnt, kitöröljük a segédlistából
                        relobj2.modifyPreviousDistance(relobj2.CurrentDistance);
                        relobj2.modifyCurrentDistance(this.CalculateDistance(relobj2.RelevantWorldObject.X, relobj2.RelevantWorldObject.Y, 0, 0));
                        returnRelevants.Add(relobj2);
                        helpList.Remove(WO);
                    }
                    // ha a segédlista eleme megegyezik a korábbi relevánssal, de nem csökkent a távolsága:
                    else if (relobj2.RelevantWorldObject.Equals(WO) &&
                             this.CalculateDistance(relobj2.RelevantWorldObject.X, relobj2.RelevantWorldObject.Y, 0, 0) <= relobj2.PreviousDistance)
                    {
                        // eltávolítjuk a segédlistából, hogy ne iteráljon végig rajta mégegyszer
                        // és a korábbi relevánsok közül is
                        helpList.Remove(WO);
                        this.previousRelevant.Remove(relobj2);
                    }
                }
            }

            return returnRelevants;
            */

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
