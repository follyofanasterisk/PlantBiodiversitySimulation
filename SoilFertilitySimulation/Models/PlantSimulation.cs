using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using SoilFertilitySimulation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilFertilitySimulation.Models
{
    public class PlantSimulation
    {
        public Dictionary<int, Plant> PlantList = new Dictionary<int, Plant>();
        public Dictionary<int, int> Grid = new Dictionary<int, int>();
        public Dictionary<int, int> SoilHealth = new Dictionary<int, int>();
        private bool IsSimulationRunning = true;
        private readonly SimulationViewModel Parent;
        //Separate PlantList into a single list for each species

        public int SimulationTime = 0;

        public PlantSimulation(SimulationViewModel parent)
        {
            Parent = parent;
            Parent.CallForAnUpdate();
            /* For Debugging
             * 
             * ObservableCollection<SimulationControlButtonViewModel> specieslist = parent.SpeciesList;
            Species ExampleSpecies;
            try
            {
                ExampleSpecies = specieslist[0].Species;

            }
            catch
            {
                ExampleSpecies = new Species("Example",1,1,1, Color.Gray);
                throw new IndexOutOfRangeException();
            }

            
            PlantList.Add(-10, new Plant(this, ExampleSpecies, -10, 0));
            PlantList.Add(-9, new Plant(this, ExampleSpecies, -9, 1));
            PlantList.Add(-8, new Plant(this, ExampleSpecies, -8, -1));
            PlantList.Add(-7, new Plant(this, ExampleSpecies, -7, 1000));
            PlantList.Add(-6, new Plant(this, ExampleSpecies, -6, -1000));
            PlantList.Add(-5, new Plant(this, ExampleSpecies, -5, 1001));
            PlantList.Add(-4, new Plant(this, ExampleSpecies, -4, 999));
            PlantList.Add(-3, new Plant(this, ExampleSpecies, -3, -999));
            PlantList.Add(-2, new Plant(this, ExampleSpecies, -2, 2));
            PlantList.Add(-1, new Plant(this, ExampleSpecies, -1, 5));*/
        }

        public void RunSimulationSingleTimeStep()
        {
            CallPlantsToEat();
            CheckIfPlantsLive();
            ResetGrid();
            SimulationTime++;
            Parent.CallForAnUpdate();
        }

        public async void RunSimulation()
        {
            IsSimulationRunning = true;
            do
            {
                CallPlantsToEat();
                CheckIfPlantsLive();
                ResetGrid();
                SimulationTime++;
                Parent.CallForAnUpdate();
                await Task.Delay(1000);
            } while (IsSimulationRunning);
        }

        public event EventHandler? PlantsToEat;
        public event EventHandler? PlantsToLiveOrNot;
        public event EventHandler? PlantsToSelfDestruct;

        protected virtual void CallPlantsToEat()
        {
            PlantsToEat?.Invoke(this, new EventArgs()); //If needed, change EventArgs to SimulatePlantsEventArgs class below
        }

        protected virtual void CheckIfPlantsLive()
        {
           PlantsToLiveOrNot?.Invoke(this, new EventArgs());
        }

        protected virtual void CallPlantsToSelfDestruct()
        {
            PlantsToSelfDestruct?.Invoke(this, new EventArgs());
        }

        public void ResetGrid()
        {
            foreach (int key in Grid.Keys)
            {
                Grid[key] = 0;
            }
        }

        public void PauseSimulation()
        {
            IsSimulationRunning = false;
        }

        public void ResumeSimulation()
        {
            RunSimulation();
        }

        public void ResetSimulation()
        {
            IsSimulationRunning = false;
            CallPlantsToSelfDestruct();
            PlantList.Clear();
            Grid.Clear();
            SimulationTime = 0;
            Parent.CallForAnUpdate();
        }
        
        public void PrepareItemsFor2DView()
        {
            //Get Grid
        }
    }



    //Only use if an event arg is needed
    /*public class SimulatePlantsEventArgs : EventArgs
    {

    }*/
}
