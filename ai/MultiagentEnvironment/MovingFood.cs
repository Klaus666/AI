using System;

namespace MultiagentEnvironment
{
    public class MovingFood : Food
    {

        private double angle;

        private double speed;

        public MovingFood(double x, double y, double angle, double speed) : base(x, y) {
            this.speed = speed;
            this.angle = angle;
        }

        public override void Interact(AgentsEnvironment env) {
            move(env);
        }

        protected void move(AgentsEnvironment env) {
            X += -Math.Sin(angle) * speed;
            Y += Math.Cos(angle) * speed;
        }
    }
}
