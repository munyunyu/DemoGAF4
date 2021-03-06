﻿using GAF;
using System;
using System.Collections.Generic;
using System.Linq;
using GAF.Extensions;
using GAF.Operators;

namespace DemoGAF4
{
    class Example1
    {
        static void yMain(string[] args)
        {
            int populationSize = 100;
            double crossoverProbability = 0.65;
            double mutationProbability = 0.02;
            int elitismPercentage = 5;

            var cities = CreateCities().ToList();

            Population population = new Population();

            for (var p = 0; p < populationSize; p++)
            {
                Chromosome chromosome = new Chromosome();
                foreach (City city in cities)
                {
                    chromosome.Genes.Add(new Gene(city));
                }

                var rnd = GAF.Threading.RandomProvider.GetThreadRandom();
                chromosome.Genes.ShuffleFast(rnd);

                population.Solutions.Add(chromosome);
            }

            //Once the population has been defined, the genetic algorithm can be initialised.
            GeneticAlgorithm ga = new GeneticAlgorithm(population, CalculateFitness);

            //To monitor progress of the algorithm, several events are available
            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;

            Crossover crossover = new Crossover(crossoverProbability)
            {
                CrossoverType = CrossoverType.DoublePointOrdered
            };

            Elite elite = new Elite(elitismPercentage);
            SwapMutate mutate = new SwapMutate(mutationProbability);

            ga.Operators.Add(crossover);
            ga.Operators.Add(elite);
            ga.Operators.Add(mutate);
            ga.Run(Terminate);
        }


        /// <summary>
        /// fittnes function evaluation, value between 0-1, 1 with > fittnes
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        public static double CalculateFitness(Chromosome chromosome)
        {
            var distanceToTravel = CalculateDistance(chromosome);
            return 1 - distanceToTravel / 10000;
        }

        //Once the population has been defined, the genetic algorithm can be initialised.
        static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            var distanceToTravel = CalculateDistance(fittest);
            Console.WriteLine("Generation : {0}\t, Fitness: {1}\t, Distance: {2}", e.Generation, fittest.Fitness, distanceToTravel);
        }

        private static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            Chromosome fittest = e.Population.GetTop(1)[0];
            foreach (var gene in fittest.Genes)
            {
                Console.WriteLine(((City)gene.ObjectValue).Name);
            }

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {

            }
        }


        /// <summary>
        /// terminate the execution when iteraration are greater than 40
        /// </summary>
        /// <param name="population"></param>
        /// <param name="currentGeneration"></param>
        /// <param name="currentEvaluation"></param>
        /// <returns></returns>
        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 40;
        }


        /// <summary>
        /// return a list of cities and the coordinates
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<City> CreateCities()
        {
            var cities = new List<City>
            {
                 new City("Birmingham", 52.486125, -1.890507),
                 new City("Bristol", 51.460852, -2.588139),
                 new City("London", 51.515161, -0.116215),
                 new City("Leeds", 53.803895, -1.549931),
                 new City("Manchester", 53.478239, -2.258549),
                 new City("Liverpool", 53.409532, -3.000126),
                 new City("Hull", 53.751959, -0.335941),
                 new City("Newcastle", 54.980766, -1.615849),
                 new City("Carlisle", 54.892406, -2.923222),
                 new City("Edinburgh", 55.958426, -3.186893),
                 new City("Glasgow", 55.862982, -4.263554),
                 new City("Cardiff", 51.488224, -3.186893),
                 new City("Swansea", 51.624837, -3.94495),
                 new City("Exeter", 50.726024, -3.543949),
                 new City("Falmouth", 50.152266, -5.065556),
                 new City("Canterbury", 51.289406, 1.075802)
            };

            return cities;
        }


        #region helperMethods
        private static double CalculateDistance(Chromosome chromosome)
        {
            var distanceToTravel = 0.0;
            City previousCity = null;

            //run through each city in the order specified in the chromosome
            foreach (var gene in chromosome.Genes)
            {
                var currentCity = (City)gene.ObjectValue;
                if (previousCity != null)
                {
                    var distance = previousCity.GetDistanceFromPosition(currentCity.Latitude, currentCity.Longitude);
                    distanceToTravel += distance;
                }

                previousCity = currentCity;
            }

            return distanceToTravel;
        }
        #endregion
    }
}
