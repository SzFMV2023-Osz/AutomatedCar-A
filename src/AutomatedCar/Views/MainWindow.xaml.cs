namespace AutomatedCar.Views
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents;
    using AutomatedCar.SystemComponents.InputHandling;
    using AutomatedCar.ViewModels;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window
    {
        private KeyboardHandler keyboardHandler;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keyboard.Keys.Add(e.Key);
            base.OnKeyDown(e);

            MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;

            if (Keyboard.IsKeyDown(Key.Up))
            {
                this.keyboardHandler.HandleKeyDown_Up();
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                this.keyboardHandler.HandleKeyDown_Down();
            }

            if (Keyboard.IsKeyDown(Key.Left))
            {
                this.keyboardHandler.HandleKeyDown_Left();
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                this.keyboardHandler.HandleKeyDown_Right();
            }

            if (Keyboard.IsKeyDown(Key.Q))
            {
                this.keyboardHandler.HandleKeyDown_Q();
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                this.keyboardHandler.HandleKeyDown_A();
            }

            if (Keyboard.IsKeyDown(Key.PageUp))
            {
                viewModel.CourseDisplay.PageUp();
            }

            if (Keyboard.IsKeyDown(Key.PageDown))
            {
                viewModel.CourseDisplay.PageDown();
            }

            if (Keyboard.IsKeyDown(Key.D1))
            {
                viewModel.CourseDisplay.ToggleDebug();
            }

            if (Keyboard.IsKeyDown(Key.D2))
            {
                viewModel.CourseDisplay.ToggleCamera();
            }

            if (Keyboard.IsKeyDown(Key.D3))
            {
                viewModel.CourseDisplay.ToggleRadar();
            }

            if (Keyboard.IsKeyDown(Key.D4))
            {
                viewModel.CourseDisplay.ToggleUltrasonic();
            }

            if (Keyboard.IsKeyDown(Key.D5))
            {
                viewModel.CourseDisplay.ToggleRotation();
            }

            if (Keyboard.IsKeyDown(Key.F1))
            {
                new HelpWindow().Show();
                Keyboard.Keys.Remove(Key.F1);
            }

            if (Keyboard.IsKeyDown(Key.F5))
            {
                viewModel.NextControlledCar();
                Keyboard.Keys.Remove(Key.F5);
            }

            if (Keyboard.IsKeyDown(Key.F6))
            {
                viewModel.PrevControlledCar();
                Keyboard.Keys.Remove(Key.F5);
            }

            if (Keyboard.IsKeyDown(Key.L))
            {
                viewModel.LKATurnOnOff();
            }

            var scrollViewer = this.Get<CourseDisplayView>("courseDisplay").Get<ScrollViewer>("scrollViewer");
            viewModel.CourseDisplay.FocusCar(scrollViewer);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Up))
            {
                this.keyboardHandler.HandleKeyUp_Up();
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                this.keyboardHandler.HandleKeyUp_Down();
            }

            if (Keyboard.IsKeyDown(Key.Left))
            {
                this.keyboardHandler.HandleKeyUp_Left();
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                this.keyboardHandler.HandleKeyUp_Right();
            }

            base.OnKeyUp(e);

            Keyboard.Keys.Remove(e.Key);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.keyboardHandler = new KeyboardHandler(World.Instance.ControlledCar.VirtualFunctionBus);
        }
    }
}