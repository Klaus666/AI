using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Neural;
using MultiagentEnvironment;
using System.Collections.ObjectModel;
using System.Threading;
using Genetic;

namespace Render
{
    public partial class MainWindow : Window
    {
        private static readonly Random Rnd = new Random();

        private static GeneticAlgorithm<OptimizableNeuralNetwork, double> ga;

        private static AgentsEnvironment environment;

        private static int populationNumber = 0;

        private static volatile bool play = true;

        private static volatile bool staticFood = true;

        private static volatile bool regenerateFood = true;


        public class RenderObserver : EatenFoodObserver
        {
            private MainWindow owner;

            public RenderObserver(MainWindow parent)
            {
                owner = parent;
            }

            public override void notify(AgentsEnvironment env)
            {
                var e = getEatenFood(env);

                owner.Dispatcher.Invoke(() =>
                {
                    foreach (var a in owner.Agents.ToArray())
                        if (e.Any(f => a.agent == f))
                            owner.Agents.Remove(a);
                });

                base.notify(env);
            }

            protected override void addRandomPieceOfFood(AgentsEnvironment env)
            {
                if (regenerateFood)
                {
                    Food food = createRandomFood(env.getWidth(), env.getHeight());
                    env.addAgent(food);
                    owner.Dispatcher.Invoke(() =>
                    {
                        owner.Agents.Add(new FoodPresenter(food));
                    });
                }
            }
        }

        private static Food createRandomFood(int width, int height)
        {
            int x = Rnd.Next(width);
            int y = Rnd.Next(height);

            Food food = null;
            if (staticFood)
            {
                food = new Food(x, y);
            }
            else
            {
                double speed = Rnd.Next() * 2;
                double direction = Rnd.Next() * 2 * Math.PI;

                food = new MovingFood(x, y, direction, speed);
            }
            return food;
        }


        private void Evolve(int iterCount)
        {
            new Thread(() => {

                var EvolveStartTime = DateTime.Now;

                ga.Evolve(iterCount);
                populationNumber += iterCount;

                NeuralNetwork newBrain = ga.GetBest();
                setAgentBrains(newBrain);

                var EvolveEndTime = DateTime.Now;

                Dispatcher.Invoke(() => { MessageBox.Show($"Evolved within {(EvolveEndTime - EvolveStartTime).TotalSeconds} s"); });

            }).Start();
        }


        private static void initializeGeneticAlgorithm(
            int populationSize,
            int parentalChromosomesSurviveCount,
            OptimizableNeuralNetwork baseNeuralNetwork)
        {
            Population<OptimizableNeuralNetwork> brains = new Population<OptimizableNeuralNetwork>();

            for (int i = 0; i < (populationSize - 1); i++)
            {
                if (baseNeuralNetwork == null)
                {
                    brains.addChromosome(NeuralNetworkDrivenAgent.randomNeuralNetworkBrain());
                }
                else
                {
                    brains.addChromosome(baseNeuralNetwork.Mutate());
                }
            }
            if (baseNeuralNetwork != null)
            {
                brains.addChromosome(baseNeuralNetwork);
            }
            else
            {
                brains.addChromosome(NeuralNetworkDrivenAgent.randomNeuralNetworkBrain());
            }

            ga = new GeneticAlgorithm<OptimizableNeuralNetwork, double>(brains, TournamentEnvironmentFitness.Calculate);

            ga.ParentChromosomesSurviveCount = parentalChromosomesSurviveCount;
        }

        private static void setAgentBrains(NeuralNetwork newBrain)
        {
            foreach (NeuralNetworkDrivenAgent agent in environment.filter<NeuralNetworkDrivenAgent>())
            {
                agent.setBrain(newBrain.Clone() as NeuralNetwork);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Agents = new ObservableCollection<AgentPresenter>();

            initializeGeneticAlgorithm(5, 1, null);

            const int w = 600;
            const int h = 400; 
            environment = new AgentsEnvironment(w, h);
            environment.addListener(new RenderObserver(this));

            NeuralNetwork brain = ga.GetBest();

            for (int i = 0; i < 15; i++)
            {
                int x = Rnd.Next(w);
                int y = Rnd.Next(h);
                double direction = Rnd.NextDouble() * 2 * Math.PI;

                NeuralNetworkDrivenAgent agent = new NeuralNetworkDrivenAgent(x, y, direction);
                agent.setBrain(brain);

                Agents.Add(new ConsumerPresenter(agent));

                environment.addAgent(agent);
            }

            for (int i = 0; i < 10; i++)
            {
                Food food = createRandomFood(w, h);
                environment.addAgent(food);
                Agents.Add(new FoodPresenter(food));
            }


            new Thread(() =>
            {
                while (true)
                    try
                    {
                        Thread.Sleep(50);
                        if (play)
                        {
                            environment.timeStep();

                            Dispatcher.Invoke(() =>
                            {
                                foreach (var pres in Agents)
                                {
                                    pres.X = pres.agent.getX() * 1000 / w;
                                    pres.Y = pres.agent.getY() * 1000 / h;
                                    var cons = pres as ConsumerPresenter;
                                    if (cons != null) cons.Angle = (cons.agent as NeuralNetworkDrivenAgent).getAngle() * 180 / Math.PI;
                                }
                            });
                        }
                    }
                    catch { break; }
            }).Start();
        }

        public ObservableCollection<AgentPresenter> Agents
        {
            get { return (ObservableCollection<AgentPresenter>)GetValue(AgentsProperty); }
            private set { SetValue(AgentsProperty, value); }
        }
        public static readonly DependencyProperty AgentsProperty = DependencyProperty.Register("Agents", typeof(ObservableCollection<AgentPresenter>), typeof(MainWindow), new PropertyMetadata(null));

        private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
         /*   Point p = e.GetPosition(sender as Control);
            var food = new Food(p.X / 1000, p.Y / 1000);
            Agents.Add(new FoodPresenter(food) { X = p.X, Y = p.Y });
            Scene.Add(food);*/
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var rx = Rnd.NextDouble();
            var ry = Rnd.NextDouble();
            var ra = Rnd.NextDouble();

            var agent = new NeuralNetworkDrivenAgent(rx, ry, ra);
            environment.addAgent(agent);
            Agents.Add(new ConsumerPresenter(agent) { X = 1000.0 * rx, Y = 1000.0 * ry, Angle = 360.0 * ra });
        }

        private void MenuItem10_Click(object sender, RoutedEventArgs e)
        {
            Evolve(10);
        }
    }

    public abstract class AgentPresenter : Control
    {
        public AbstractAgent agent { get; }

        public AgentPresenter(AbstractAgent Agent)
        {
            agent = Agent;
        }

        static AgentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AgentPresenter), new FrameworkPropertyMetadata(typeof(AgentPresenter)));
        }

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(AgentPresenter), new PropertyMetadata(.0));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(AgentPresenter), new PropertyMetadata(.0));
    }

    public class FoodPresenter : AgentPresenter
    {
        public FoodPresenter(AbstractAgent Agent) : base(Agent) { }

        static FoodPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FoodPresenter), new FrameworkPropertyMetadata(typeof(FoodPresenter)));
        }
    }

    public class ConsumerPresenter : AgentPresenter
    {
        public ConsumerPresenter(Agent Agent) : base(Agent) { Foreground = Brushes.Red; }

        static ConsumerPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConsumerPresenter), new FrameworkPropertyMetadata(typeof(ConsumerPresenter)));
        }

        /// <summary>
        /// DEGREES
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(ConsumerPresenter), new PropertyMetadata(.0));
    }
}
