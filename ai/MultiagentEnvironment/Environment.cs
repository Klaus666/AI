using System.Collections.Generic;

namespace MultiagentEnvironment
{
    public delegate void OnAgentsEvent(AgentsEnvironment env);

    public class AgentsEnvironment
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        private List<IAbstractAgent> agents = new List<IAbstractAgent>();

        public event OnAgentsEvent AgentEvent;

        public AgentsEnvironment(int width, int height)
        {
            Width = width;
            Height = height;
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
                if (AgentEvent != null) AgentEvent(this);
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
    }

}
