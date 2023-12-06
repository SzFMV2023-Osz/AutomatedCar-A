namespace AutomatedCar.Views
{
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public partial class WorldSelectionWindow : Window
    {
        public WorldSelectionWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
