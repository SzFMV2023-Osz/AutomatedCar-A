namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.InputHandling;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        private DashboardViewModel dashboard;
        private CourseDisplayViewModel courseDisplay;
        private KeyboardHandler keyboardHandler;

        public MainWindowViewModel(World world)
        {
            this.CourseDisplay = new CourseDisplayViewModel(world);
            this.Dashboard = new DashboardViewModel(world.ControlledCar);
            this.KeyboardHandler = new KeyboardHandler(world.ControlledCar.VirtualFunctionBus);
        }

        public CourseDisplayViewModel CourseDisplay
        {
            get => this.courseDisplay;
            private set => this.RaiseAndSetIfChanged(ref this.courseDisplay, value);
        }

        public DashboardViewModel Dashboard
        {
            get => this.dashboard;
            private set => this.RaiseAndSetIfChanged(ref this.dashboard, value);
        }
        public KeyboardHandler KeyboardHandler
        {
            get => this.keyboardHandler;
            private set => this.RaiseAndSetIfChanged(ref this.keyboardHandler, value);
        }

        public void NextControlledCar()
        {
            World.Instance.NextControlledCar();
            this.keyboardHandler = new KeyboardHandler(World.Instance.ControlledCar.VirtualFunctionBus);
            this.Dashboard = new DashboardViewModel(World.Instance.ControlledCar);
        }

        public void PrevControlledCar()
        {
            World.Instance.PrevControlledCar();
            this.keyboardHandler = new KeyboardHandler(World.Instance.ControlledCar.VirtualFunctionBus);
            this.Dashboard = new DashboardViewModel(World.Instance.ControlledCar);
        }

        public void LKATurnOnOff()
        {
            dashboard.ControlledCar.Car.CameraSensor.LKATurnOnOff();
        }

        public void LKATurnOnOffSteering()
        {
            dashboard.ControlledCar.Car.CameraSensor.LKATurnOnOffSteering();
        }
    }
}