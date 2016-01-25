using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Neural;
using MultiagentEnvironment;
using System.Collections.ObjectModel;
using System.Threading;

namespace Render
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Random Rnd = new Random();

        private const int maxWeight = 10;

        private static int GetRandomWeight(Random random) => random.Next(maxWeight) - random.Next(maxWeight);

        protected NeuralNetwork CreateRandomNetwork()
        {
            var Net = new NeuralNetwork(15);
            for (int i = 6; i < 15; i++)
            {
                //   var f = Transfer.Random();
                //   Net.SetNeuronFunction(i, f, Transfer.GetDefaultParams(f));
                Net.SetNeuronFunction(i, ThresholdFunction.Function.Sigma, ThresholdFunction.GetDefaultParams(ThresholdFunction.Function.Sigma));
            }

            for (int i = 0; i < 6; i++) Net.SetNeuronFunction(i, ThresholdFunction.Function.Linear, ThresholdFunction.GetDefaultParams(ThresholdFunction.Function.Linear));

            var input = Net.MarkupLayer(0, 6);
            var hidden = Net.MarkupLayer(6, 9);
            var output = Net.MarkupLayer(13, 2);

            Net.SetOutputLayer(output);
            Net.SetInputLayer(input);

            var firstbus = Net.ConnectLayers(input, hidden, false, (a, b) => GetRandomWeight(Rnd));
            var secondbus = Net.ConnectLayers(hidden, hidden, true, (a, b) => GetRandomWeight(Rnd));

            Net.SetActivationOrder(firstbus, secondbus);
            return Net;
        }

        public MainWindow()
        {
            InitializeComponent();
            Agents = new ObservableCollection<AgentPresenter>();

            Neural = CreateRandomNetwork();

            Scene.OnFoodEaten += (potato) =>
            {
                var agent = Agents.OfType<FoodPresenter>().SingleOrDefault(f => potato == f.Agent);
                if (agent != null) Agents.Remove(agent);
            };

            new Thread(() =>
            {
                while (true)
                    try
                    {
                        var time = DateTime.Now;

                        Dispatcher.Invoke(() =>
                        {
                            Scene.Animate();
                            foreach (var agent in Agents.OfType<ConsumerPresenter>())
                            {
                                agent.X = agent.agent.getX() * 1000.0;
                                agent.Y = agent.agent.getX() * 1000.0;
                                agent.Angle = agent.agent.getAngle() * 180 / Math.PI;
                                if ((agent.agent as Environment.Consumer).IsSick) agent.Foreground = Brushes.LightGray;
                            }
                        });

                        Thread.Sleep(TimeSpan.FromMilliseconds(Math.Max((time + TimeSpan.FromMilliseconds(100) - DateTime.Now).TotalMilliseconds, 0)));
                    }
                    catch { break; }
            }).Start();
        }

        private NeuralNetwork Neural;
        private readonly AgentsEnvironment Scene = new AgentsEnvironment(200, 200);

        public ObservableCollection<AgentPresenter> Agents
        {
            get { return (ObservableCollection<AgentPresenter>)GetValue(AgentsProperty); }
            private set { SetValue(AgentsProperty, value); }
        }
        public static readonly DependencyProperty AgentsProperty = DependencyProperty.Register("Agents", typeof(ObservableCollection<AgentPresenter>), typeof(MainWindow), new PropertyMetadata(null));

        private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(sender as Control);
            var food = new Food(p.X / 1000, p.Y / 1000);
            Agents.Add(new FoodPresenter(food) { X = p.X, Y = p.Y });
            Scene.Add(food);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var rx = Rnd.NextDouble();
            var ry = Rnd.NextDouble();
            var ra = Rnd.NextDouble();

            var agent = new Environment.Consumer(Neural.Duplicate(), rx, ry, ra);
            Scene.Add(agent);
            Agents.Add(new ConsumerPresenter(agent) { X = 1000.0 * rx, Y = 1000.0 * ry, Angle = 360.0 * ra });
        }
    }

    public abstract class AgentPresenter : Control
    {
        public Agent agent { get; }

        public AgentPresenter(Agent Agent)
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
        public FoodPresenter(Agent Agent) : base(Agent) { }

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
