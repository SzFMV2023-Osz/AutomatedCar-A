namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ConstrainedExecution;
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Controls.Shapes;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;

    internal abstract class Sensor : SystemComponent
    {
        public event EventHandler Collided;
        public List<WorldObject> CurrentObjectsinView { get; protected set; }

        public Polygon SensorTriangle { get; private set; }

        public WorldObject HighlightedObject { get; private set; }
        
        // public List<WorldObject> CurrentObjectsinView { get; protected set; }

        public List<RelevantObject> previousObjectinView { get; protected set; }
        
        public AutomatedCar automatedCarForSensors { get; protected set; }

        public int viewAngle { get; protected set; }

        public int viewDistance { get; protected set; }

        public int distanceFromCarCenter { get; protected set; }
        
        public Sensor(VirtualFunctionBus virtualFunctionBus, AutomatedCar automatedCar)
            : base(virtualFunctionBus)
        {
            this.CurrentObjectsinView = new List<WorldObject>();
            this.previousObjectinView = new List<RelevantObject>();
            //this.previousObjectinView.Add(new RelevantObject(new WorldObject(200, 200, "road_2lane_45right", 0, false, WorldObjectType.Road), (double)200, (double)200));
            this.automatedCarForSensors = automatedCar;
            this.CreateSensorTriangle(automatedCarForSensors, distanceFromCarCenter, viewAngle, viewDistance);
        }

        public void ObjectsinViewUpdate(List<WorldObject> objects)
        {
            foreach (var obj in objects)
            {
                if (IsInTriangle(obj))
                {
                    if (!CurrentObjectsinView.Contains(obj))
                    {
                        CurrentObjectsinView.Add(obj);
                    }
                }
                else
                {
                    CurrentObjectsinView.Remove(obj);
                }
            }
        }

        private bool IsInTriangle(WorldObject obj)
        {
            foreach (var g in obj.Geometries)
            {
                if (SensorTriangle.DefiningGeometry.Bounds.Intersects(new Rect(g.Bounds.X + obj.X, g.Bounds.Y + obj.Y, g.Bounds.Width, g.Bounds.Height)))
                {
                    return true;
                }
            }
            return false;
        }

        public void ClosestHighlightedObject()
        {
            if (CurrentObjectsinView.Count > 0)
            {
                this.HighlightedObject = this.CurrentObjectsinView[0];
            }

            for (int i = 0; i < this.CurrentObjectsinView.Count - 1; i++)
            {
                if (this.CalculateDistance(this.CurrentObjectsinView[i].X, this.CurrentObjectsinView[i].Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y + this.distanceFromCarCenter)
                    <= this.CalculateDistance(this.CurrentObjectsinView[i + 1].X, this.CurrentObjectsinView[i + 1].Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y + this.distanceFromCarCenter) 
                    && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Road) && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.ParkingSpace)
                    && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Other) && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Crosswalk))
                {
                    this.HighlightedObject = this.CurrentObjectsinView[i];
                    //this.automatedCarForSensors.Collideable = true;
                }
                else
                {
                    //this.automatedCarForSensors.Collideable = false;
                }
            }
        }

        public void Collidable(EventArgs e)
        {
            for (int i = 0; i < this.CurrentObjectsinView.Count - 1; i++)
            {
                if (this.CalculateDistance(this.CurrentObjectsinView[i].X, this.CurrentObjectsinView[i].Y, this.automatedCarForSensors.X, this.automatedCarForSensors.Y + this.distanceFromCarCenter) == 0 && !this.CurrentObjectsinView[i].WorldObjectType.Equals(WorldObjectType.Road))
                {
                    this.Collided?.Invoke(this, e);
                }
            }
        }


        protected void CreateSensorTriangle(AutomatedCar automatedCar, int distanceFromCarCenter, int viewAngle, int range)
        {
            // car x : 480
            // car y : 1425
            int alpha = viewAngle / 2;

            // c means c side of a right-triangle
            int cSideLength = (int)(range / Math.Cos(alpha));

            Point point1 = new Point(
                           automatedCar.X + (int)(distanceFromCarCenter * Math.Cos(DegToRad(90 - automatedCar.Rotation))),
                           automatedCar.Y + (int)(distanceFromCarCenter * Math.Sin(DegToRad(90 - automatedCar.Rotation))));

            Point point2 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation + alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation + alpha)))));

            Point point3 = new Point(
                           (int)(point1.X + (cSideLength * Math.Cos(DegToRad(270 + automatedCar.Rotation - alpha)))),
                           (int)(point1.Y + (cSideLength * Math.Sin(DegToRad(270 + automatedCar.Rotation - alpha)))));

            Point point4 = new Point(
                (int)((point1.X+point2.X) / 2),
                (int)((point1.Y + point2.Y) / 2));

            Polygon triangle = new Polygon();
            triangle.Points = new List<Point>
            {
                point1,
                point2,
                point3,
            };

            this.SensorTriangle = triangle;
        }

        private static double DegToRad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        protected double CalculateDistance(double xACoordinate, double yACoordinate, double xBCoordinate, double yBCoordinate)
        {
            double distance = Math.Sqrt(Math.Pow(xBCoordinate - xACoordinate, 2) + Math.Pow(yBCoordinate - yACoordinate, 2));
            return distance;
        }

        protected virtual void CollisionDetection()
        {
            //EventHandler<CollidedEventArgs> handler = Collided;

            //if (handler != null)
            //{
            //    handler(this, e);
            //}

            //List<PolylineGeometry> pg = CurrentObjectsinView[0].Geometries;

            
        }
    }

    internal class CollidedEventArgs : EventArgs
    {
        public bool Collided { get; set; }
    }
}

