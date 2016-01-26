using Genetic;
using Neural;
using System;

namespace MultiagentEnvironment
{
    public static class TournamentEnvironmentFitness
    {

        private class FitnessObserver : EatenFoodObserver
        {
            int width;
            int height;

            public FitnessObserver(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            protected void addRandomPieceOfFood(AgentsEnvironment env)
            {
                Food newFood = newPieceOfFood(width, height);
                env.Add(newFood);
            }
        }

        private static Random random = new Random();

        public static double Calculate(OptimizableNeuralNetwork chromosome)
        {
            // TODO maybe, its better to initialize these parameters in constructor
            const int width = 200;
            const int height = 200;
            int agentsCount = 10;
            int foodCount = 5;
            int environmentIterations = 50;

            AgentsEnvironment env = new AgentsEnvironment(width, height);

            for (int i = 0; i < agentsCount; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                double direction = 2 * Math.PI * random.NextDouble();

                NeuralNetworkDrivenAgent agent = new NeuralNetworkDrivenAgent(x, y, direction);
                agent.setBrain(chromosome.Clone() as NeuralNetwork);

                env.Add(agent);
            }

            for (int i = 0; i < foodCount; i++)
            {
                Food food = newPieceOfFood(width, height);
                env.Add(food);
            }

            EatenFoodObserver tournamentListener = new FitnessObserver(width, height);
            env.AgentEvent += tournamentListener.notify;

            for (int i = 0; i < environmentIterations; i++)
            {
                env.timeStep();
            }

            double score = tournamentListener.getScore();
            return 1.0 / score;
        }

        private static Food newPieceOfFood(int width, int height)
        {
            Food food = new Food(random.Next(width), random.Next(height));
            return food;
        }
    }

}
