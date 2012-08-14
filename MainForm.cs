using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TaskOptimizer.Model;
using TaskOptimizer.View;

namespace TaskOptimizer
{
    public partial class MainForm : Form
    {
        private readonly FitnessLevels m_fitnessLevels = new FitnessLevels();
        private readonly Timer m_timer = new Timer();
        private int m_currentNbRobots;
        private int m_currentTotalCost;
        private int m_currentTotalTime;
        private int m_lastRandomSeed = 11;
        private Optimizer m_optimizer;

        public MainForm()
        {
            InitializeComponent();


            m_fitnessLevels.CostMultiplier = 10;
            m_fitnessLevels.TimeMultiplier = 10;

            costLevel.Minimum = 0;
            costLevel.Maximum = 20;


            MinimumSize = Size;
            costLevel.Value = costLevel.Maximum/2;

            m_timer.Tick += m_timer_Tick;
            m_timer.Interval = 500;

            newOptimization(100, false, 5, true);
        }

        private void newOptimization(int nbTasks, bool randomTaskSizes, int nbRobots, bool restart)
        {
            int startX = 500;
            int startY = 300;

            if (!restart)
            {
                m_lastRandomSeed = (int) DateTime.Now.Ticks;
            }

            var form = new PercentProgressForm();
            var generator = new TaskGenerator(m_lastRandomSeed);
            List<Task> tasks = generator.generateTasks(nbTasks, randomTaskSizes, startX, startY);
            List<Robot> robots = generator.generateRobots(nbRobots);
            var config = new Optimizer.Configuration();
            config.robots = robots;
            config.tasks = tasks;
            config.startX = startX;
            config.startY = startY;
            config.fitnessLevels = m_fitnessLevels;
            config.nbDistributors = Environment.ProcessorCount*3 - 1;
            config.randomSeed = (int) DateTime.Now.Ticks;

            m_optimizer = new Optimizer(config);
            form.ShowDialog(this);

            if (form.WorkCancelled)
            {
                graphView.Visible = false;
            }
            else
            {
                graphView.Visible = true;
                m_timer.Start();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            stopOptimization();
            base.OnFormClosing(e);
        }

        private void m_timer_Tick(object sender, EventArgs e)
        {
            m_optimizer.compute();

            if (m_optimizer.NbUsedRobots != m_currentNbRobots ||
                m_optimizer.TotalTime != m_currentTotalTime ||
                m_optimizer.TotalCost != m_currentTotalCost)
            {
                graphView.SetTaskSequences(m_optimizer.MinSequences);
            }

            m_currentNbRobots = m_optimizer.NbUsedRobots;
            m_currentTotalTime = m_optimizer.TotalTime;
            m_currentTotalCost = m_optimizer.TotalCost;

            robotLabel.Text = m_optimizer.NbUsedRobots.ToString();
            timeLabel.Text = m_optimizer.TotalTime.ToString();
            costLabel.Text = m_optimizer.TotalCost.ToString();
            iterationLabel.Text = m_optimizer.CurrentIteration.ToString();
        }


        private void fitnessLevels_Scroll(object sender, EventArgs e)
        {
            m_fitnessLevels.CostMultiplier = costLevel.Value;
            m_fitnessLevels.TimeMultiplier = costLevel.Maximum - costLevel.Value;

            m_optimizer.recomputeFitness();
        }


        private void restartBtn_Click(object sender, EventArgs e)
        {
            var form = new NewForm();
            stopOptimization();
            newOptimization(form.NbTasks, form.RandomTaskSizes, form.NbRobots, true);
        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            var form = new NewForm();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            stopOptimization();
            newOptimization(form.NbTasks, form.RandomTaskSizes, form.NbRobots, false);
        }

        private void stopOptimization()
        {
            m_timer.Stop();
            m_optimizer.stop();
        }
    }
}