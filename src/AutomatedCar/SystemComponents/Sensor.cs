namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    abstract class Sensor
    {
        public double viewAngle { get; protected set; }
        public double viewDistance { get; protected set; }

        public int sensorPositionX { get; protected set; }
        public int sensorPositionY { get; protected set; }

        public List<WorldObject> currentObjectinView { get; protected set; }
        public List<WorldObject> previousObjectinView { get; protected set; }
        public List<RelevantObject> previousRelevant { get; protected set; }
        public PolylineGeometry sensorTriangle { get;protected set; }

        public WorldObject highlightedObject { get; private set; }
        public void SensorTriangleUpdate()
        {

        }
        public void ClosestHighlightedObject(AutomatedCar car)
        {
            this.highlightedObject = currentObjectinView[0];
            for (int i = 0; i < currentObjectinView.Count - 1; i++)
            {
                if (CalculateDistance(currentObjectinView[i].X, currentObjectinView[i].Y, car.X, car.Y) <= CalculateDistance(currentObjectinView[i + 1].X, currentObjectinView[i + 1].Y, car.X, car.Y))
                {
                    this.highlightedObject = currentObjectinView[i];
                }
            }
        }

        private double CalculateDistance(double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow((xBCoordinate - xACoordinate), 2) + Math.Pow((yBCoordinate - yACoordinate), 2));
            return distance;
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
                        //relevantobj2.modifyPreviousDistance(relevantobj2.CurrentDistance);
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
