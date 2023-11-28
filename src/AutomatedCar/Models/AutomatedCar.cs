namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using System.ComponentModel;
    using System;
    using System.ComponentModel.DataAnnotations;
    using SystemComponents;
    using ReactiveUI;
    using SystemComponents.Powertrain;

    public class AutomatedCar : Car
    {
        private VirtualFunctionBus virtualFunctionBus;
        private Powertrain powertrain;

        private Radar radarSensor;

        private Camera cameraSensor;


        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.powertrain = new Powertrain(this.virtualFunctionBus, this);
            this.ZIndex = 10;
        }

        public VirtualFunctionBus VirtualFunctionBus { get => this.virtualFunctionBus; }

        public Powertrain Powertrain { get => this.powertrain; }

        public Camera CameraSensor { get => this.cameraSensor; }

        public Radar RadarSensor { get => this.radarSensor; }

        public int Revolution { get; set; }

        public int Velocity { get; set; }

        public PolylineGeometry Geometry { get; set; }

        public void CreateRadarSensor()
        {
            this.radarSensor = new Radar(this.virtualFunctionBus, this);
        }

        public void CreateCameraSensor()
        {
            this.cameraSensor = new Camera(this.virtualFunctionBus, this);
        }

        /// <summary>Starts the automated cor by starting the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Start()
        {
            this.virtualFunctionBus.Start();
        }

        /// <summary>Stops the automated cor by stopping the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Stop()
        {
            this.virtualFunctionBus.Stop();
        }
    }
}