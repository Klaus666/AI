using System.Collections.Generic;

namespace MultiagentEnvironment
{
    public class AgentsEnvironment
    {

        private int width;

        private int height;

        private List<AbstractAgent> agents = new List<AbstractAgent>();

        private List<AgentsEnvironmentObserver> listeners = new List<AgentsEnvironmentObserver>();

        public AgentsEnvironment(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void addListener(AgentsEnvironmentObserver listener)
        {
            this.listeners.Add(listener);
        }

        public int getWidth()
        {
            return this.width;
        }

        public int getHeight()
        {
            return this.height;
        }

        private readonly object this_lock = new object();

        public void timeStep()
        {
            lock (this_lock)
            {
                foreach (AbstractAgent agent in this.getAgents())
                {
                    agent.interact(this);
                    this.avoidMovingOutsideOfBounds(agent);
                }
                foreach (AgentsEnvironmentObserver l in this.listeners)
                {
                    l.notify(this);
                }
            }
        }

        /**
         * avoid moving outside of environment
         */
        private void avoidMovingOutsideOfBounds(AbstractAgent agent)
        {
            double newX = agent.getX();
            double newY = agent.getY();
            if (newX < 0)
            {
                newX = this.width - 1;
            }
            if (newY < 0)
            {
                newY = this.height - 1;
            }
            if (newX > this.width)
            {
                newX = 1;
            }
            if (newY > this.height)
            {
                newY = 1;
            }

            agent.setX(newX);
            agent.setY(newY);
        }

        public LinkedList<AbstractAgent> getAgents()
        {
            // to avoid concurrent modification exception
            return new LinkedList<AbstractAgent>(this.agents);
        }

        public void addAgent(AbstractAgent agent)
        {
            lock (this_lock)
            {
                this.agents.Add(agent);
            }
        }

        public void removeAgent(AbstractAgent agent)
        {
            lock (this_lock)
                this.agents.Remove(agent);
        }


        public IEnumerable<T> filter<T>() where T : AbstractAgent
        {
            LinkedList<T> filtered = new LinkedList<T>();
            foreach (AbstractAgent agent in this.getAgents())
            {
                if (agent is T)
                {
                    filtered.AddLast((T)agent);
                }
            }
            return filtered;
        }
    }

}
