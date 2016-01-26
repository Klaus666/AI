namespace MultiagentEnvironment
{
    public class Food : IAbstractAgent
    {

        public double X { get; set; }
        public double Y { get; set; }

        public Food(double x, double y) {
            X = x;
            Y = y;
        }

        public virtual void Interact(AgentsEnvironment env) { }
    }
}
