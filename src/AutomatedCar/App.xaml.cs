namespace AutomatedCar
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using System.Runtime.Remoting;
    using AutomatedCar.Models;
    using AutomatedCar.Models.NPC;

    using AutomatedCar.ViewModels;
    using AutomatedCar.Views;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var world = this.CreateWorld();
                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel(world) };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public World CreateWorld()
        {
            var world = World.Instance;

            // this.AddDummyCircleTo(world);

            world.PopulateFromJSON($"AutomatedCar.Assets.test_world.json");

            this.AddControlledCarsTo(world);
            this.AddNpcPedestrian(world);
            this.AddNpcCar(world);

            return world;
        }

        private PolylineGeometry GetControlledCarBoundaryBox()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
            string json_text = reader.ReadToEnd();
            dynamic stuff = JObject.Parse(json_text);
            var points = new List<Point>();
            foreach (var i in stuff["objects"][0]["polys"][0]["points"])
            {
                points.Add(new Point(i[0].ToObject<int>(), i[1].ToObject<int>()));
            }

            return new PolylineGeometry(points, false);
        }

        private void AddDummyCircleTo(World world)
        {
            var circle = new Circle(200, 200, "circle.png", 20);
            
            circle.Width = 40;
            circle.Height = 40;
            circle.ZIndex = 20;
            circle.Rotation = 45;

            world.AddObject(circle);
        }

        private AutomatedCar CreateControlledCar(int x, int y, int rotation, string filename)
        {
            var controlledCar = new Models.AutomatedCar(x, y, filename);

            controlledCar.Geometry = this.GetControlledCarBoundaryBox();
            controlledCar.RawGeometries.Add(controlledCar.Geometry);
            controlledCar.Geometries.Add(controlledCar.Geometry);
            controlledCar.RotationPoint = new System.Drawing.Point(54, 120);
            controlledCar.Rotation = rotation;

            controlledCar.CreateRadarSensor(); // needs to be after Rotation value assignment
            controlledCar.CreateCameraSensor();


            controlledCar.Start();

            return controlledCar;
        }
public delegate void CollidedEventArgs(object sender, EventArgs e);
        private void AddControlledCarsTo(World world)
        {
            var controlledCar = this.CreateControlledCar(480, 1425, 0, "car_1_white.png");
            var controlledCar2 = this.CreateControlledCar(4250, 1420, -90, "car_1_red.png");

            world.AddControlledCar(controlledCar);
            world.AddControlledCar(controlledCar2);
        }


        private Pedestrian CreateNpcPedestrian(int x, int y, int rotation, string filename,World world)
        {
            List<NPCPathPoint> list = GetPathPointsFrom("NPC_test_world_path.json", "pedestrian");
            var NpcPedestrian = new Pedestrian(x, y, filename, 1, true, list[0].Rotation, list, world.npcManager);
            //NpcPedestrian.Geometry = this.GetControlledCarBoundaryBox();
            //NpcPedestrian.RawGeometries.Add(NpcPedestrian.Geometry);
            //NpcPedestrian.Geometries.Add(NpcPedestrian.Geometry);
            //NpcPedestrian.RotationPoint = new System.Drawing.Point(54, 120);
            //NpcPedestrian.Rotation = rotation;
            //NpcPedestrian.Start();
            

            return NpcPedestrian;
        }

        private NPCCar CreateNpcCar(int x, int y, int rotation, string filename, World world)
        {
            var NPCCar = new NPCCar(x, y, filename, 1, true, 0, GetPathPointsFrom("NPC_test_world_path.json", "car"), world.npcManager);
            //NpcPedestrian.Geometry = this.GetControlledCarBoundaryBox();
            //NpcPedestrian.RawGeometries.Add(NpcPedestrian.Geometry);
            //NpcPedestrian.Geometries.Add(NpcPedestrian.Geometry);
            //NpcPedestrian.RotationPoint = new System.Drawing.Point(54, 120);
            //NpcPedestrian.Rotation = rotation;
            //NpcPedestrian.Start();


            return NPCCar;
        }

        private void AddNpcPedestrian(World world)
        {

            var Pedestrian = this.CreateNpcPedestrian(1950, 630,3, "woman.png",world);
            world.AddObject(Pedestrian);
            world.npcManager.Start();

        }

        private void AddNpcCar(World world)
        {

            var Car = this.CreateNpcCar(3620, 1200, 3, "car_2_blue.png", world);
            world.AddObject(Car);
            world.npcManager.Start();

        }

        private List<NPCPathPoint> GetPathPointsFrom(string filePath, string type)
        {




            List<NPCPathPoint> pathPoints = new List<NPCPathPoint>();



            string fullPath = $"AutomatedCar.Assets.NPC_paths." + filePath;
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(fullPath));
            string json_text = reader.ReadToEnd();
            dynamic pathPointList = JObject.Parse(json_text);



            foreach (var point in pathPointList[type])
            {
                pathPoints.Add(new NPCPathPoint(
                point["x"].ToObject<int>(),
                point["y"].ToObject<int>(),
                point["rotation"].ToObject<int>(),
                point["speed"].ToObject<int>()));
            }



            return pathPoints;
        }

    }



}