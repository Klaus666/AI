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

        public override void interact(AgentsEnvironment env) {
            this.move(env);
        }

        protected void move(AgentsEnvironment env) {
            double rx = -Math.Sin(this.angle);
            double ry = Math.Cos(this.angle);
            this.setX(this.getX() + (rx * this.speed));
            this.setY(this.getY() + (ry * this.speed));
        }
    }
}
