using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiagentEnvironment.evolution
{
    public class EatenFoodObserver : AgentsEnvironmentObserver
    {

        protected const double minEatDistance = 5;

        protected const double maxFishesDistance = 5;

        private Random random = new Random();

        private double score = 0;

        public void notify(AgentsEnvironment env) {
            var eatenFood = this.getEatenFood(env);
            this.score += eatenFood.Count;

            LinkedList<Agent> collidedFishes = this.getCollidedFishes(env);
            this.score -= collidedFishes.Count * 0.5;

            this.removeEatenAndCreateNewFood(env, eatenFood);
        }

        private LinkedList<Agent> getCollidedFishes(AgentsEnvironment env) {
            LinkedList<Agent> collidedFishes = new LinkedList<Agent>();

            List<Agent> allFishes = this.getFishes(env);
            int fishesCount = allFishes.Count;

            for (int i = 0; i < (fishesCount - 1); i++)
            {
                Agent firstFish = allFishes[i];
                for (int j = i + 1; j < fishesCount; j++)
                {
                    Agent secondFish = allFishes[j];
                    double distanceToSecondFish = this.module(firstFish.getX() - secondFish.getX(), firstFish.getY() - secondFish.getY());
                    if (distanceToSecondFish < maxFishesDistance)
                    {
                        collidedFishes.AddLast(secondFish);
                    }
                }
            }
            return collidedFishes;
        }

        private LinkedList<Food> getEatenFood(AgentsEnvironment env) {
            LinkedList<Food> eatenFood = new LinkedList<Food>();

            
            foreach (Food food in this.getFood(env))
            {
                foreach (Agent fish in this.getFishes(env))
                {
                    double distanceToFood = this.module(food.getX() - fish.getX(), food.getY() - fish.getY());
                    if (distanceToFood < minEatDistance)
                    {
                        eatenFood.AddLast(food);
                        break;
                    }
                }
            }
            return eatenFood;
        }

        protected void removeEatenAndCreateNewFood(AgentsEnvironment env, IEnumerable<Food> eatenFood) {
            foreach (Food food in eatenFood)
            {
                env.removeAgent(food);

                this.addRandomPieceOfFood(env);
            }
        }

        protected void addRandomPieceOfFood(AgentsEnvironment env) {
            int x = this.random.Next(env.getWidth());
            int y = this.random.Next(env.getHeight());
            Food newFood = new Food(x, y);
            env.addAgent(newFood);
        }

        private List<Food> getFood(AgentsEnvironment env) {
            List<Food> food = new List<Food>();
            foreach (Food f in env.filter<Food>()) {
                food.Add(f);
            }
            return food;
        }

        private List<Agent> getFishes(AgentsEnvironment env)
        {
            List<Agent> fishes = new List<Agent>();
            foreach (Agent agent in env.filter<Agent>()) {
                fishes.Add(agent);
            }
            return fishes;
        }

        public double getScore()
        {
            if (this.score < 0)
            {
                return 0;
            }
            return this.score;
        }

        protected double module(double vx1, double vy1) => Math.Sqrt((vx1 * vx1) + (vy1 * vy1));

    }

}
