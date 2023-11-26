namespace AutomatedCar.ViewModels
{
    using ReactiveUI;
    using System;
    using System.Reactive;

    internal class WorldSelectionViewModel : ViewModelBase
    {
        public event EventHandler WorldSelectedEvent;

        const string testWorld = "Test_World";
        const string oval = "Oval";

        public string TestWorld
        {
            get => testWorld;
        }

        public string Oval
        {
            get => oval;
        }


        string selectedWorld;

        public string SelectedWorld
        {
            get => selectedWorld;
            set => this.RaiseAndSetIfChanged(ref selectedWorld, value);
        }


        public ReactiveCommand<Unit, Unit> TestWorldButton { get; }

        public ReactiveCommand<Unit, Unit> OvalButton { get; }

        public WorldSelectionViewModel()
        {
            this.TestWorldButton = ReactiveCommand.Create(
                () =>
                {
                    this.selectedWorld = this.TestWorld;
                    this.WorldSelectedEvent(this, new EventArgs());

                });

            this.OvalButton = ReactiveCommand.Create(
                () =>
                {
                    this.selectedWorld = this.Oval;
                    this.WorldSelectedEvent(this, new EventArgs());
                });
        }
    }
}