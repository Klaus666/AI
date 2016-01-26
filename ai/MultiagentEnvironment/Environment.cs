using System.Collections.Generic;

namespace MultiagentEnvironment
{
    public class AgentsEnvironment
    {

        public int Width { get; private set; }

        public int Height { get; private set; }

        private List<IAbstractAgent> agents = new List<IAbstractAgent>();

        private List<AgentsEnvironmentObserver> listeners = new List<AgentsEnvironmentObserver>();

        public AgentsEnvironment(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void addListener(AgentsEnvironmentObserver listener)
        {
            listeners.Add(listener);
        }

        private readonly object this_lock = new object();

        public void timeStep()
        {
            lock (this_lock)
            {
                foreach (IAbstractAgent agent in getAgents())
                {
                    agent.Interact(this);
                    avoidMovingOutsideOfBounds(agent);
                }
                foreach (AgentsEnvironmentObserver l in this.listeners)
                {
                    l.notify(this);
                }
            }
        }

        private void avoidMovingOutsideOfBounds(IAbstractAgent agent)
        {
            double newX = agent.X;
            double newY = agent.Y;
            if (newX < 0) newX = Width - 1;
            if (newY < 0) newY = Height - 1;
            if (newX > Width) newX = 1;
            if (newY > Height) newY = 1;

            agent.X = newX;
            agent.Y = newY;
        }

        public LinkedList<IAbstractAgent> getAgents()
        {
#warning linked?
            // to avoid concurrent modification exception
            return new LinkedList<IAbstractAgent>(agents);
        }

        public void Add(IAbstractAgent agent)
        {
            lock (this_lock) agents.Add(agent);
        }

        public void Remove(IAbstractAgent agent)
        {
            lock (this_lock) agents.Remove(agent);
        }


        public IEnumerable<T> filter<T>() where T : IAbstractAgent
        {
            LinkedList<T> filtered = new LinkedList<T>();
            foreach (IAbstractAgent agent in this.getAgents())
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
