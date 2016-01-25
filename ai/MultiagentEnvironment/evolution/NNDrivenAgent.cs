using Neural;
using System;
using System.Collections.Generic;

namespace MultiagentEnvironment
{
    public class NeuralNetworkDrivenAgent : Agent
    {

        private const double maxSpeed = 4;

        private const double maxDeltaAngle = 1;

        protected const double maxAgentsDistance = 5;

        private const double AGENT = -10;

        private const double EMPTY = 0;

        private const double FOOD = 10;

        private volatile NeuralNetwork brain;

        public NeuralNetworkDrivenAgent(double x, double y, double angle) :
            base(x, y, angle)
        {
        }

        private readonly object ThisLock = new object();

        /**
         * Animating of agents and evolving best brain - might be in different
         * threads <br/>
         * Synchronization prevents from race condition when trying to set new
         * brain, while method "interact" runs <br/>
         * <br/>
         * TODO Maybe consider to use non-blocking technique. But at the moment this
         * simplest solution doesn't cause any overheads
         */
        public void setBrain(NeuralNetwork brain)
        {
            lock (ThisLock) this.brain = brain;
        }

        /**
         * Synchronization prevents from race condition when trying to set new
         * brain, while method "interact" runs <br/>
         * <br/>
         * TODO Maybe consider to use non-blocking technique. But at the moment this
         * simplest solution doesn't cause any overheads
         */
        public void interact(AgentsEnvironment env)
        {
            lock (ThisLock)
            {
                var nnInputs = createNnInputs(env);

                activateNeuralNetwork(nnInputs);

                int neuronsCount = brain.getNeuronsCount();
                double deltaAngle = brain.getAfterActivationSignal(neuronsCount - 2);
                double deltaSpeed = brain.getAfterActivationSignal(neuronsCount - 1);

                deltaSpeed = avoidNaNAndInfinity(deltaSpeed);
                deltaAngle = avoidNaNAndInfinity(deltaAngle);

                double newSpeed = normalizeSpeed(getSpeed() + deltaSpeed);
                double newAngle = getAngle() + normalizeDeltaAngle(deltaAngle);

                setAngle(newAngle);
                setSpeed(newSpeed);

                move();
            }
        }

        private double avoidNaNAndInfinity(double x)
        {
            if ((double.IsNaN(x)) || double.IsInfinity(x))
            {
                x = 0;
            }
            return x;
        }

        private void activateNeuralNetwork(List<Double> nnInputs)
        {
            for (int i = 0; i < nnInputs.Count; i++)
            {
                brain.putSignalToNeuron(i, nnInputs[i]);
            }
            brain.activate();
        }

        protected LinkedList<double> createNnInputs(AgentsEnvironment environment)
        {
            // Find nearest food
            Food nearestFood = null;
            double nearestFoodDist = double.MaxValue;

            foreach (Food currFood in environment.filter<Food>())
            {
                // agent can see only ahead
                if (this.inSight(currFood))
                {
                    double currFoodDist = distanceTo(currFood);
                    if ((nearestFood == null) || (currFoodDist <= nearestFoodDist))
                    {
                        nearestFood = currFood;
                        nearestFoodDist = currFoodDist;
                    }
                }
            }

            // Find nearest agent
            Agent nearestAgent = null;
            double nearestAgentDist = maxAgentsDistance;

            foreach (Agent currAgent in environment.filter<Agent>())
            {
                // agent can see only ahead
                if ((this != currAgent) && (this.inSight(currAgent)))
                {
                    double currAgentDist = this.distanceTo(currAgent);
                    if (currAgentDist <= nearestAgentDist)
                    {
                        nearestAgent = currAgent;
                        nearestAgentDist = currAgentDist;
                    }
                }
            }

            var nnInputs = new LinkedList<double>();

            double rx = getRx();
            double ry = getRy();

            double x = getX();
            double y = getY();

            if (nearestFood != null)
            {
                double foodDirectionVectorX = nearestFood.getX() - x;
                double foodDirectionVectorY = nearestFood.getY() - y;

                // left/right cos
                double foodDirectionCosTeta =
                        Math.Sign(pseudoScalarProduct(rx, ry, foodDirectionVectorX, foodDirectionVectorY))
                                * cosTeta(rx, ry, foodDirectionVectorX, foodDirectionVectorY);

                nnInputs.AddLast(FOOD);
                nnInputs.AddLast(nearestFoodDist);
                nnInputs.AddLast(foodDirectionCosTeta);

            }
            else
            {
                nnInputs.AddLast(EMPTY);
                nnInputs.AddLast(0.0);
                nnInputs.AddLast(0.0);
            }

            if (nearestAgent != null)
            {
                double agentDirectionVectorX = nearestAgent.getX() - x;
                double agentDirectionVectorY = nearestAgent.getY() - y;

                // left/right cos
                double agentDirectionCosTeta =
                        Math.Sign(pseudoScalarProduct(rx, ry, agentDirectionVectorX, agentDirectionVectorY))
                                * cosTeta(rx, ry, agentDirectionVectorX, agentDirectionVectorY);

                nnInputs.AddLast(AGENT);
                nnInputs.AddLast(nearestAgentDist);
                nnInputs.AddLast(agentDirectionCosTeta);

            }
            else
            {
                nnInputs.AddLast(EMPTY);
                nnInputs.AddLast(0.0);
                nnInputs.AddLast(0.0);
            }
            return nnInputs;
        }

        protected bool inSight(AbstractAgent agent)
        {
            double crossProduct = this.cosTeta(this.getRx(), this.getRy(), agent.getX() - this.getX(), agent.getY() - this.getY());
            return (crossProduct > 0);
        }

        protected double distanceTo(AbstractAgent agent)
        {
            return this.module(agent.getX() - this.getX(), agent.getY() - this.getY());
        }

        protected double cosTeta(double vx1, double vy1, double vx2, double vy2)
        {
            double v1 = module(vx1, vy1);
            double v2 = module(vx2, vy2);
            if (v1 == 0)
            {
                v1 = 1E-5;
            }
            if (v2 == 0)
            {
                v2 = 1E-5;
            }
            double ret = ((vx1 * vx2) + (vy1 * vy2)) / (v1 * v2);
            return ret;
        }

        protected double module(double vx1, double vy1)
        {
            return Math.Sqrt((vx1 * vx1) + (vy1 * vy1));
        }

        protected double pseudoScalarProduct(double vx1, double vy1, double vx2, double vy2)
        {
            return (vx1 * vy2) - (vy1 * vx2);
        }

        private double normalizeSpeed(double speed)
        {
            double abs = Math.Abs(speed);
            if (abs > maxSpeed)
            {
                double sign = Math.Sign(speed);
                speed = sign * (abs - (Math.Floor(abs / maxSpeed) * maxSpeed));
            }
            return speed;
        }

        private double normalizeDeltaAngle(double angle)
        {
            double abs = Math.Abs(angle);
            if (abs > maxDeltaAngle)
            {
                double sign = Math.Sign(angle);
                angle = sign * (abs - (Math.Floor(abs / maxDeltaAngle) * maxDeltaAngle));
            }
            return angle;
        }

        public static OptimizableNeuralNetwork randomNeuralNetworkBrain()
        {
            OptimizableNeuralNetwork nn = new OptimizableNeuralNetwork(15);
            for (int i = 0; i < 15; i++)
            {
                var f = ThresholdFunction.Random();
                nn.setNeuronFunction(i, f, ThresholdFunction.GetDefaultParams(f));
            }
            for (int i = 0; i < 6; i++)
            {
                nn.setNeuronFunction(i, ThresholdFunction.Function.Linear, ThresholdFunction.GetDefaultParams(ThresholdFunction.Function.Linear));
            }
            for (int i = 0; i < 6; i++)
            {
                for (int j = 6; j < 15; j++)
                {
                    nn.addLink(i, j, Math.random());
                }
            }
            for (int i = 6; i < 15; i++)
            {
                for (int j = 6; j < 15; j++)
                {
                    if (i < j)
                    {
                        nn.addLink(i, j, Math.random());
                    }
                }
            }
            return nn;
        }
    }
}
