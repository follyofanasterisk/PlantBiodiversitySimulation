namespace SoilFertilitySimulation.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public SimulationViewModel SimulationViewModel { get; } = new SimulationViewModel();

        public MainWindowViewModel()
        {

        }
    }
}
