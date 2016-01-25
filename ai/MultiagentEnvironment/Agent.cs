using System;

namespace MultiagentEnvironment
{
    public abstract class Agent : AbstractAgent
    {

        private double x;

        private double y;

        private double angle;

        private double speed;

        public Agent(double x, double y, double angle) {
            this.x = x;
            this.y = y;
            this.speed = 0;
            this.angle = angle;
        }

        public void move() {
            double rx = -Math.Sin(this.angle);
            double ry = Math.Cos(this.angle);
            this.x += rx * this.speed;
            this.y += ry * this.speed;
        }

        
        public double getX() {
            return this.x;
        }

        public double getY() {
            return this.y;
        }

        public void setX(double x) {
            this.x = x;
        }

        public void setY(double y) {
            this.y = y;
        }

        public double getSpeed() {
            return this.speed;
        }

        public void setSpeed(double v) {
            this.speed = v;
        }

        public double getAngle() {
            return this.angle;
        }

        public void setAngle(double angle) {
            this.angle = angle;
        }

        public double getRx() {
            double rx = -Math.Sin(this.angle);
            return rx;
        }

        public double getRy() {
            double ry = Math.Cos(this.angle);
            return ry;
        }

        public abstract void interact(AgentsEnvironment env);

    }

}
