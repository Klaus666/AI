namespace MultiagentEnvironment
{
    public class Food : AbstractAgent
    {

        private double x;

        private double y;

        public Food(double x, double y) {
            this.x = x;
            this.y = y;
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

        public virtual void interact(AgentsEnvironment env) {
            // Stub
        }
    }
}
