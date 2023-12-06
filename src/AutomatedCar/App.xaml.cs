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
    using AutomatedCar.SystemComponents;

    using System.Windows.Markup;
    using AutomatedCar.Models;
    using AutomatedCar.Models.NPC;
    using AutomatedCar.SystemComponents;
    using AutomatedCar.ViewModels;
    using AutomatedCar.Views;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;
    using static AutomatedCar.Models.World;

    public class App : Application
    {
        delegate void LoadSelectedWorldMethod(World world, bool loadOnlyStaticAssets);

        private string TEST_WORLD_KEYWORD = "Test_World";
        private string OVAL_WORLD_KEYWORD = "Oval";

        Dictionary<string, LoadSelectedWorldMethod> worldKeyWorldToActionMap = new Dictionary<string, LoadSelectedWorldMethod>();

        public override void Initialize()
        {
            this.worldKeyWorldToActionMap.Add(TEST_WORLD_KEYWORD, LoadTestWorldAssets);
            this.worldKeyWorldToActionMap.Add(OVAL_WORLD_KEYWORD, LoadOvalWorldAssets);
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var vm = new WorldSelectionViewModel();
                var selectionWindow = new WorldSelectionWindow() { DataContext = vm };
                selectionWindow.Show();

                vm.WorldSelectedEvent += (sender, args) =>
                {

                    var selectedWorld = vm.SelectedWorld;
                    if (selectedWorld == null)
                    {
                        return;
                    }

                    var world = this.CreateWorld(selectedWorld, false);

                    var mainWindow = new MainWindow { DataContext = new MainWindowViewModel(world) };

                    mainWindow.Show();
                    desktop.MainWindow = mainWindow;
                    selectionWindow.Close();
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public World CreateWorld(string selectedWorld, bool loadOnlyStaticAssets)
        {
            var world = World.Instance;

            // this.AddDummyCircleTo(world);
            this.worldKeyWorldToActionMap[selectedWorld].Invoke(world, loadOnlyStaticAssets);
            /*world.PopulateFromJSON($"AutomatedCar.Assets.test_world.json");

            this.AddControlledCarsTo(world);
            this.AddNpcPedestrian(world);
            this.AddNpcCar(world);*/

            return world;
        }

        void LoadTestWorldAssets(World world, bool loadOnlyStaticAssets)
        {
            this.AddDummyCircleTo(world);
            world.SetSelectedWorldTo(WorldType.Test);
            world.PopulateFromJSON($"AutomatedCar.Assets.test_world.json");


            if (!loadOnlyStaticAssets)
            {
                this.AddControlledCarsTo(world);
                this.AddNpcPedestrian(world);
                this.AddNpcCarTestWorld(world);
            }
        }

        void LoadOvalWorldAssets(World world, bool loadOnlyStaticAssets)
        {

            world.PopulateFromJSON($"AutomatedCar.Assets.oval.json");
            world.SetSelectedWorldTo(WorldType.Oval);


            if (!loadOnlyStaticAssets)
            {
                this.AddControlledCarsTo(world);
                this.AddNpcCarOvalWorld(world);
            }
        }

        private PolylineGeometry GetNPCCarBoundaryBox()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
            string json_text = reader.ReadToEnd();
            dynamic stuff = JObject.Parse(json_text);
            var points = new List<Point>();
            foreach (var i in stuff["objects"][5]["polys"][0]["points"])
            {
                points.Add(new Point(i[0].ToObject<int>(), i[1].ToObject<int>()));
            }

            return new PolylineGeometry(points, false);

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

        private PolylineGeometry GetControlledNPCPedestrianBoundaryBox()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
            string json_text = reader.ReadToEnd();
            dynamic stuff = JObject.Parse(json_text);
            var points = new List<Point>();
            foreach (var i in stuff["objects"][31]["polys"][0]["points"])
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
            controlledCar.WorldObjectType = WorldObjectType.Car;

            controlledCar.CreateRadarSensor(); // needs to be after Rotation value assignment
            controlledCar.CreateCameraSensor();


            controlledCar.Start();

            return controlledCar;
        }
        public delegate void CollidedEventArgs(object sender, EventArgs e);
        private void AddControlledCarsTo(World world)
        {

           // var controlledCar = this.CreateControlledCar(480, 1425, 0, "car_1_white.png");
            var controlledCar2 = this.CreateControlledCar(4250, 1420, -90, "car_1_red.png");

           //g world.AddControlledCar(controlledCar);

            world.AddControlledCar(controlledCar2);
        }


        private Pedestrian CreateNpcPedestrian(int x, int y, int rotation, string filename, World world)
        {
            List<NPCPathPoint> list = GetPathPointsFrom("NPC_test_world_path.json", "pedestrian");
            var NpcPedestrian = new Pedestrian(x, y, filename, 1, true, list[0].Rotation, list, world.npcManager);
            NpcPedestrian.Geometry = this.GetControlledNPCPedestrianBoundaryBox();
            NpcPedestrian.RawGeometries.Add(NpcPedestrian.Geometry);
            NpcPedestrian.Geometries.Add(NpcPedestrian.Geometry);


            return NpcPedestrian;
        }

        private NPCCar CreateNpcCarTestWorld(int x, int y, int rotation, string filename, World world)
        {
            var NPCCar = new NPCCar(x, y, filename, 1, true, 0, GetPathPointsFrom("NPC_test_world_path.json", "car"), world.npcManager);
            NPCCar.Geometry = this.GetNPCCarBoundaryBox();
            NPCCar.RawGeometries.Add(NPCCar.Geometry);
            NPCCar.Geometries.Add(NPCCar.Geometry);


            return NPCCar;
        }

        private NPCCar CreateNpcCarOvalWorld(int x, int y, int rotation, string filename, World world)
        {
            var NPCCar = new NPCCar(x, y, filename, 1, true, 0, GetPathPointsFrom("NPC_oval_world_path.json", "car"), world.npcManager);
            NPCCar.Geometry = this.GetNPCCarBoundaryBox();
            NPCCar.RawGeometries.Add(NPCCar.Geometry);
            NPCCar.Geometries.Add(NPCCar.Geometry);


            return NPCCar;
        }

        private void AddNpcPedestrian(World world)
        {

            var Pedestrian = this.CreateNpcPedestrian(1950, 630, 3, "woman.png", world);
            world.AddObject(Pedestrian);
            world.npcManager.Start();

        }

        private void AddNpcCarTestWorld(World world)
        {

            var Car = this.CreateNpcCarTestWorld(3620, 1200, 3, "car_2_blue.png", world);
            world.AddObject(Car);
            world.npcManager.Start();

        }

        private void AddNpcCarOvalWorld(World world)
        {

            var Car = this.CreateNpcCarOvalWorld(3620, 1200, 3, "car_2_blue.png", world);
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