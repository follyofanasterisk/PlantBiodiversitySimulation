using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoilFertilitySimulation.Models
{
    public class Plant
    {
        private Species SpeciesOfPlant;
        private readonly PlantSimulation ReferenceOfSimulation;

        private int PlantID;
        private int[]? PlantReachGrid;

        //Make IDisposable?

        public Plant(PlantSimulation currentSimulation, Species species, int PlantID, int GridLocation)
        {
            ReferenceOfSimulation = currentSimulation;
            SpeciesOfPlant = species;
            this.PlantID = PlantID;
            CreatePlantEatGrid(GridLocation);

            ReferenceOfSimulation.PlantsToEat += Eat;
            ReferenceOfSimulation.PlantsToLiveOrNot += CheckConsumption;
            ReferenceOfSimulation.PlantsToSelfDestruct += Die;
        }

        private void CreatePlantEatGrid(int GridLocation)
        {
            PlantReachGrid = new int[2 * SpeciesOfPlant.Reach * SpeciesOfPlant.Reach + 2 * SpeciesOfPlant.Reach + 1];
            int LoopCounter = 0;
            for (int i = 0; i < SpeciesOfPlant.Reach; i++)
            {
                for (int j = 0; j < SpeciesOfPlant.Reach - i; j++)
                {
                    int IterationAdditiveR = (SpeciesOfPlant.Reach - i) * 1000 - 1000 * j - j;
                    int IterationAdditiveL = (SpeciesOfPlant.Reach - i) + 1000 * j - j;
                    if (!ReferenceOfSimulation.Grid.ContainsKey(GridLocation + IterationAdditiveR))
                    {
                        ReferenceOfSimulation.Grid[GridLocation + IterationAdditiveR] = 0;
                    }
                    PlantReachGrid[LoopCounter] = GridLocation + IterationAdditiveR;
                    LoopCounter++;
                    if (!ReferenceOfSimulation.Grid.ContainsKey(GridLocation - IterationAdditiveR))
                    {
                        ReferenceOfSimulation.Grid[GridLocation - IterationAdditiveR] = 0;
                    }
                    PlantReachGrid[LoopCounter] = GridLocation - IterationAdditiveR;
                    LoopCounter++;
                    if (!ReferenceOfSimulation.Grid.ContainsKey(GridLocation + IterationAdditiveL))
                    {
                        ReferenceOfSimulation.Grid[GridLocation + IterationAdditiveL] = 0;
                    }
                    PlantReachGrid[LoopCounter] = GridLocation + IterationAdditiveL;
                    LoopCounter++;
                    if (!ReferenceOfSimulation.Grid.ContainsKey(GridLocation - IterationAdditiveL))
                    {
                        ReferenceOfSimulation.Grid[GridLocation - IterationAdditiveL] = 0;
                    }
                    PlantReachGrid[LoopCounter] = GridLocation - IterationAdditiveL;
                    LoopCounter++;
                }
            }
            if (!ReferenceOfSimulation.Grid.ContainsKey(GridLocation))
            {
                ReferenceOfSimulation.Grid[GridLocation] = 0;
            }
            PlantReachGrid[LoopCounter] = GridLocation;
        }

        private void Eat(object? sender, EventArgs e)
        {
            for (int i = 0; i < PlantReachGrid?.Length; i++)
            {
                ReferenceOfSimulation.Grid[PlantReachGrid[i]] += SpeciesOfPlant.Strength;
            }
        }

        private void CheckConsumption(object? sender, EventArgs e)
        {
            float FoodCollected = 0;
            for (int i = 0; i < PlantReachGrid?.Length; i++)
            {
                if (!ReferenceOfSimulation.SoilHealth.ContainsKey(PlantReachGrid[i]))
                {
                    FoodCollected += 1 * (float)SpeciesOfPlant.Strength / (float)ReferenceOfSimulation.Grid[PlantReachGrid[i]];
                }
                else
                {
                    FoodCollected += (float)ReferenceOfSimulation.SoilHealth[PlantReachGrid[i]] * (float)SpeciesOfPlant.Strength / (float)ReferenceOfSimulation.Grid[PlantReachGrid[i]];
                }
            }
            //Debug.WriteLine($"Plant {GridLocation} collects {FoodCollected}");
            if (FoodCollected >= (float)SpeciesOfPlant.Consumption)
            {
                //Debug.WriteLine($"Plant at {GridLocation} lives!");
                //Reproduction
            }
            else
            {
                //Debug.WriteLine($"Plant at {GridLocation} has died");
                ReferenceOfSimulation.PlantsToEat -= Eat;
                ReferenceOfSimulation.PlantsToLiveOrNot -= CheckConsumption;
                ReferenceOfSimulation.PlantsToSelfDestruct -= Die;
                ReferenceOfSimulation.PlantList.Remove(PlantID);
            }
        }

        private void Die(object? sender, EventArgs e)
        {
            ReferenceOfSimulation.PlantsToEat -= Eat;
            ReferenceOfSimulation.PlantsToLiveOrNot -= CheckConsumption;
            ReferenceOfSimulation.PlantsToSelfDestruct -= Die;
            ReferenceOfSimulation.PlantList.Remove(PlantID);
        }
    }
}
