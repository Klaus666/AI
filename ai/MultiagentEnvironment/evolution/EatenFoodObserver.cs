using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiagentEnvironment
{
    public class EatenFoodObserver
    {
        protected const double minEatDistance = 5;

        protected const double maxFishesDistance = 5;

        private Random random = new Random();
        
        private double score = 0;

        public virtual void notify(AgentsEnvironment env) {
            var eatenFood = getEatenFood(env);
            score += eatenFood.Count;

            LinkedList<Agent> collidedFishes = getCollidedFishes(env);
            score -= collidedFishes.Count * 0.5;

            removeEatenAndCreateNewFood(env, eatenFood);
        }

        private LinkedList<Agent> getCollidedFishes(AgentsEnvironment env) {
            LinkedList<Agent> collidedFishes = new LinkedList<Agent>();

            List<Agent> allFishes = getFishes(env);
            int fishesCount = allFishes.Count;

            for (int i = 0; i < (fishesCount - 1); i++)
            {
                Agent firstFish = allFishes[i];
                for (int j = i + 1; j < fishesCount; j++)
                {
                    Agent secondFish = allFishes[j];
                    double distanceToSecondFish = module(firstFish.X - secondFish.X, firstFish.Y - secondFish.Y);
                    if (distanceToSecondFish < maxFishesDistance)
                    {
                        collidedFishes.AddLast(secondFish);
                    }
                }
            }
            return collidedFishes;
        }

        protected LinkedList<Food> getEatenFood(AgentsEnvironment env) {
            LinkedList<Food> eatenFood = new LinkedList<Food>();

            
            foreach (Food food in getFood(env))
            {
                foreach (Agent fish in getFishes(env))
                {
                    double distanceToFood = module(food.X - fish.X, food.Y - fish.Y);
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
                env.Remove(food);

                addRandomPieceOfFood(env);
            }
        }

        protected virtual void addRandomPieceOfFood(AgentsEnvironment env) {
            int x = random.Next(env.Width);
            int y = random.Next(env.Height);
            Food newFood = new Food(x, y);
            env.Add(newFood);
        }

        private List<Food> getFood(AgentsEnvironment env) {
            List<Food> food = new List<Food>();
            foreach (Food f in env.getAgents().OfType<Food>()) {
                food.Add(f);
            }
            return food;
        }

        private List<Agent> getFishes(AgentsEnvironment env)
        {
            List<Agent> fishes = new List<Agent>();
            foreach (Agent agent in env.getAgents().OfType<Agent>()) {
                fishes.Add(agent);
            }
            return fishes;
        }

        public double getScore()
        {
            if (score < 0)
            {
                return 0;
            }
            return score;
        }

        protected double module(double vx1, double vy1) => Math.Sqrt((vx1 * vx1) + (vy1 * vy1));
    }
}
