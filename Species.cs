using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace SoilFertilitySimulation.Models
{
    public partial class Species : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private int _reach;
        [ObservableProperty]
        private int _strength;
        [ObservableProperty]
        private int _consumption;
        [ObservableProperty]
        private Color _speciesColor;

        public Species(string Name, int Reach, int Strength, int Consumption, Color SpeciesColor)
        {
            this._name = Name;
            this._reach = Reach;
            this._strength = Strength;
            this._consumption = Consumption;
            this._speciesColor = SpeciesColor;
        }

    }
}
