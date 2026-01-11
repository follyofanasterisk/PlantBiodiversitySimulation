using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoilFertilitySimulation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilFertilitySimulation.ViewModels
{
    public partial class SimulationControlButtonViewModel : ViewModelBase
    {
        public Species Species { get; }
        private SimulationViewModel Parent;

        public SimulationControlButtonViewModel(Species species, SimulationViewModel parent)
        {
            Species = species;
            this.Parent = parent;
        }

        [RelayCommand]
        private void TellSimulationViewToAddToSpecies()
        {
            Parent.AddPlantOfSpecies(this);
        }
    }
}
