using CommunityToolkit.Mvvm.ComponentModel;
using SoilFertilitySimulation.Models;
using SoilFertilitySimulation.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;

namespace SoilFertilitySimulation.ViewModels
{
    public partial class SimulationViewModel : ViewModelBase
    {
        public ObservableCollection<SimulationControlButtonViewModel> SpeciesList { get; } = new();
        private PlantSimulation? CurrentSimulation;
        
        public SimulationViewModel()
        {
            SpeciesList.Add(new SimulationControlButtonViewModel(new Species("Example",1,1,1,Colors.Gray), this));
        }

        public void CallForAnUpdate()
        {
            if (CurrentSimulation != null)
            {
                SimulationTimeString = String.Format("Time: {0}", CurrentSimulation.SimulationTime);
                NumberOfPlants = String.Format("Plants: {0}", CurrentSimulation.PlantList.Count);
                Update2DView(CurrentSimulation);
            }
            else
            {
                SimulationTimeString = "Time: N/A";
                NumberOfPlants = "Plants: No Active Simulation";
            }
        }

        //Left Menu Code
        [ObservableProperty]
        private string _newSpeciesName = "New Species";
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddSpeciesToListCommand))]
        private int? _newSpeciesReach;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddSpeciesToListCommand))]
        private int? _newSpeciesStrength;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddSpeciesToListCommand))]
        private int? _newSpeciesConsumption;
        [ObservableProperty]
        private Color? _newSpeciesColor;

        private bool CanCreateSpecies() => NewSpeciesReach != null && NewSpeciesStrength !=null && NewSpeciesConsumption != null;

        [RelayCommand(CanExecute = nameof(CanCreateSpecies))]
        private void AddSpeciesToList()
        {
            if (NewSpeciesColor == null)
            {
                NewSpeciesColor = Colors.Gray;
            }
            if (NewSpeciesReach != null && NewSpeciesStrength != null && NewSpeciesConsumption != null)
            {
                SpeciesList.Add(new SimulationControlButtonViewModel(new Species(NewSpeciesName,NewSpeciesReach.Value,NewSpeciesStrength.Value,NewSpeciesConsumption.Value,NewSpeciesColor.Value), this));
                NewSpeciesName = "New Species";
                NewSpeciesReach = null;
                NewSpeciesStrength = null;
                NewSpeciesConsumption = null;
                NewSpeciesColor = null;
            }
        }

        private Random RandomSeed = new Random();
        private int NextPlantIDNumber = 0;
        [RelayCommand]
        public void AddPlantOfSpecies(SimulationControlButtonViewModel button)
        {
            if (CurrentSimulation != null)
            {
                int gridlocation = RandomSeed.Next(-50, 50) + RandomSeed.Next(-50, 50) * 1000;
                //Currently there is no need for preventing plants from being ontop of other plants
                //The grid can be expanded up to 99 before the viewer stops working and up to 999 before the simulation stops working
                Plant NewPlant = new Plant(CurrentSimulation, button.Species, NextPlantIDNumber,gridlocation);
                CurrentSimulation.PlantList.Add(NextPlantIDNumber,NewPlant);
                NextPlantIDNumber++;
                CallForAnUpdate();
            }
        }

        //Right Menu Code
        [ObservableProperty]
        private string _simulationTimeString = "Time: N/A";

        [ObservableProperty]
        private string _numberOfPlants = "Plants: No Active Simulation";

        [RelayCommand]
        private void StartSimulation()
        {
            if (CurrentSimulation == null)
            {
                CurrentSimulation = new PlantSimulation(this);
            }
            CurrentSimulation.RunSimulation();
        }
        [RelayCommand]
        private void SimulateSingleTimeStep()
        {
            if (CurrentSimulation == null)
            {
                CurrentSimulation = new PlantSimulation(this);
            }
            CurrentSimulation.RunSimulationSingleTimeStep();
        }

        [RelayCommand]
        private void PauseSimulation()
        {
            if (CurrentSimulation != null)
            {
                CurrentSimulation.PauseSimulation();
            }
        }

        [RelayCommand]
        private void ResumeSimulation()
        {
            if (CurrentSimulation != null)
            {
                CurrentSimulation.ResumeSimulation();
            }
        }

        [RelayCommand]
        private void RestartSimulation()
        {
            if (CurrentSimulation != null)
            {
                CurrentSimulation.ResetSimulation();
            }
        }

        //Bottom Menu Code
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(Open3DWindowCommand))]
        private bool _is3DViewInactive = true; //It might be better to use IsRunning in PlantSimulation instead

        [RelayCommand(CanExecute = nameof(Is3DViewInactive))]
        private void Open3DWindow()
        {
            using (_3DViewer _3DView = new _3DViewer(800, 600, "3D Visualizer", this))
            {
                _3DView.Run();
            }
        }

        //Center Panel Code
        [ObservableProperty]
        private int[] _gridKeys = [];
        [ObservableProperty]
        private bool _is2DViewActive = false;

        private void Update2DView(PlantSimulation currentSimulation)
        {
            GridKeys = currentSimulation.Grid.Keys.ToArray();
        }

        [RelayCommand]
        private void Start2DVisualization()
        {
            Is2DViewActive = true;
        }

        [RelayCommand]
        private void Stop2DVisualization()
        {
            Is2DViewActive = false;
        }
    }
}

