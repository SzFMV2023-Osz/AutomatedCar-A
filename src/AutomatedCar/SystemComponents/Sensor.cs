namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Sensor
    {
        public double viewAngle { get; set; }

        public double viewDistance { get; set; }

        public List<WorldObject> currentObjectinView { get; set; }

        // kell az összehasonlításhoz:
        public List<WorldObject> previousObjectinView { get; set; }

        public List<RelevantObject> previousRelevant { get; set; }

        public PolylineGeometry sensorTriangle { get; set; }

        public Sensor(double ViewAngle, double ViewDistance)
        {
            viewAngle = ViewAngle;
            viewDistance = ViewDistance;
        }
        public void SensorTriangleUpdate()
        {
            //frissíteni kell a previousObjectinView listát
        }

        // Returns relevant objects
        public List<RelevantObject> RelevantObjects()
        {
            List<RelevantObject> ReturnRelevants = new List<RelevantObject>();
            List<WorldObject> currentlyRelevant = new List<WorldObject>();

            foreach (WorldObject relevantobj in this.currentObjectinView)
            {
                if (previousObjectinView.Contains(relevantobj))
                {
                    currentlyRelevant.Add(relevantobj);
                }
            }

            foreach (RelevantObject relevantobj2 in previousRelevant)
            {
                foreach (WorldObject WO in currentlyRelevant)
                {
                    if (relevantobj2.RelevantWorldObject.Equals(WO) && relevantobj2.CurrentDistance > this.examDistance(WO))
                    {
                        relevantobj2.modifyCurrentDistance(this.examDistance(WO));
                        ReturnRelevants.Add(relevantobj2);
                    }
                    else
                    {
                        currentlyRelevant.Remove(WO);
                        this.previousRelevant.Remove(relevantobj2);
                    }
                }
            }

            return ReturnRelevants;
        }

        // Returns a RelevantObjects distance
        public double examDistance(WorldObject WO)
        {
            foreach (RelevantObject rev in this.previousRelevant)
            {
                if (rev.RelevantWorldObject.Equals(WO))
                {
                    return rev.CurrentDistance;
                }
            }

            return 999; // akkorát adunk visza, hogy mindenképp nagyobb legyen
        }


    }
}
