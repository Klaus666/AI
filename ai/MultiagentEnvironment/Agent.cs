using System;

namespace MultiagentEnvironment
{
    public abstract class Agent : IAbstractAgent
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Angle { get; protected set; }

        protected double Speed { get; set; }

        public Agent(double x, double y, double angle) {
            X = x;
            Y = y;
            Speed = 0;
            Angle = angle;
        }

        public void Move() {
            X += Rx * Speed;
            Y += Ry * Speed;
        }

        public double Rx => -Math.Sin(Angle);

        public double Ry => Math.Cos(Angle);

        public abstract void Interact(AgentsEnvironment env);
    }

}
