using GAF;
using System;
using System.Collections.Generic;
using System.Linq;
using GAF.Extensions;
using GAF.Operators;
using System.IO;

namespace DemoGAF4
{
    internal class Program
    {
        private static void Main(string[] args)
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
                foreach (Products city in cities)
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

        public static double CalculateFitness(Chromosome chromosome)
        {
            return 1;
        }

        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 1000;
        }

        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
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
                Console.WriteLine(((Products)gene.ObjectValue).Sugar);
            }

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {

            }
        }

        private static string CalculateDistance(Chromosome chromosome)
        {
            string distanceToTravel = null;
            
            Products previousCity = null;

            //run through each city in the order specified in the chromosome
            //compare and get associations
            foreach (var gene in chromosome.Genes)
            {
                var currentCity = (Products)gene.ObjectValue;
                if (previousCity != null)
                {
                    var distance = previousCity.GetAssociationsFromProducts(currentCity.Sugar, currentCity.Salt);
                    distanceToTravel = distance;
                }

                previousCity = currentCity;
            }

            return distanceToTravel;
        }

        private static IEnumerable<Products> CreateCities()
        {
            var cities = new List<Products>
            {
                 new Products("yes","no"),
                 new Products("no","no"),
                 new Products("yes","yes"),
                 new Products("no","yes"),
                 new Products("yes","no"),
                 new Products("yes","no"),
                 new Products("yes","no"),
                 new Products("yes","yes"),
                 new Products("yes","no"),
                 new Products("yes","no"),
                 new Products("yes","yes"),
                 new Products("yes","yes")

                
            };

            return cities;
        }

    }
}