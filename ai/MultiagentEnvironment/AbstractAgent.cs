namespace MultiagentEnvironment
{
    public interface IAbstractAgent
    {

        void Interact(AgentsEnvironment env);

        double X { get; set; }
        double Y { get; set; }
    }
}
