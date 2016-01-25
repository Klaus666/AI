using Neural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiagentEnvironment
{
    public class TournamentEnvironmentFitness : Fitness<OptimizableNeuralNetwork, Double>
    {

        class FitnessObserver : EatenFoodObserver
        {
            private TournamentEnvironmentFitness owner;
            int width;
            int height;

            public FitnessObserver(TournamentEnvironmentFitness Parent, int width, int height)
            {
                owner = Parent;
                this.width = width;
                this.height = height;
            }

            protected void addRandomPieceOfFood(AgentsEnvironment env)
            {
                Food newFood = owner.newPieceOfFood(width, height);
                env.addAgent(newFood);
            }
        }

        private static Random random = new Random();

        public double calculate(OptimizableNeuralNetwork chromosome)
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

                env.addAgent(agent);
            }

            for (int i = 0; i < foodCount; i++)
            {
                Food food = newPieceOfFood(width, height);
                env.addAgent(food);
            }

            EatenFoodObserver tournamentListener = new FitnessObserver(this, width, height);
            env.addListener(tournamentListener);

            for (int i = 0; i < environmentIterations; i++)
            {
                env.timeStep();
            }

            double score = tournamentListener.getScore();
            return 1.0 / score;
        }

        protected Food newPieceOfFood(int width, int height)
        {
            Food food = new Food(random.Next(width), random.Next(height));
            return food;
        }
    }

}
