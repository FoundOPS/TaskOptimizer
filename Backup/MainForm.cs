using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using TaskOptimizer.Model;
using TaskOptimizer.View;

namespace TaskOptimizer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            

            m_fitnessLevels.CostMultiplier = 10;
            m_fitnessLevels.TimeMultiplier = 10;

            this.costLevel.Minimum = 0;
            this.costLevel.Maximum = 20;


            this.MinimumSize = this.Size;
            this.costLevel.Value = this.costLevel.Maximum / 2;

            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Interval = 500;

            newOptimization(100, false, 5, true);
            
        }

        private void newOptimization(int nbTasks, bool randomTaskSizes, int nbRobots, bool restart)
        {
            int startX = 500;
            int startY = 300;            

            if (!restart)
            {
                m_lastRandomSeed = (int)DateTime.Now.Ticks;
            }

            PercentProgressForm form = new PercentProgressForm();
            TaskGenerator generator = new TaskGenerator(m_lastRandomSeed);
            List<Task> tasks = generator.generateTasks(nbTasks, randomTaskSizes, startX, startY);
            List<Robot> robots = generator.generateRobots(nbRobots);
            Optimizer.Configuration config = new Optimizer.Configuration();
            config.robots = robots;
            config.tasks = tasks;
            config.startX = startX;
            config.startY = startY;
            config.fitnessLevels = m_fitnessLevels;
            config.nbDistributors = Environment.ProcessorCount * 3 - 1;
            config.randomSeed = (int)DateTime.Now.Ticks;
            config.progress = form;
                        
            m_optimizer = new Optimizer(config);
            form.ShowDialog(this);

            if (form.WorkCancelled)
            {
                this.graphView.Visible = false;
            }
            else
            {
                this.graphView.Visible = true;
                m_timer.Start();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            stopOptimization();
            base.OnFormClosing(e);
        }        

        void m_timer_Tick(object sender, EventArgs e)
        {
            m_optimizer.compute();

            if (m_optimizer.NbUsedRobots != m_currentNbRobots ||
                m_optimizer.TotalTime != m_currentTotalTime ||
                m_optimizer.TotalCost != m_currentTotalCost)
            {
                this.graphView.setTaskSequences(m_optimizer.MinSequences);
            }

            m_currentNbRobots = m_optimizer.NbUsedRobots;
            m_currentTotalTime = m_optimizer.TotalTime;
            m_currentTotalCost = m_optimizer.TotalCost;

            this.robotLabel.Text = m_optimizer.NbUsedRobots.ToString();
            this.timeLabel.Text = m_optimizer.TotalTime.ToString();
            this.costLabel.Text = m_optimizer.TotalCost.ToString();
            this.iterationLabel.Text = m_optimizer.CurrentIteration.ToString();
        }

        private int m_currentTotalTime = 0, m_currentTotalCost, m_currentNbRobots;


        private void fitnessLevels_Scroll(object sender, EventArgs e)
        {
            m_fitnessLevels.CostMultiplier = this.costLevel.Value;
            m_fitnessLevels.TimeMultiplier = this.costLevel.Maximum - this.costLevel.Value;

            m_optimizer.recomputeFitness();
        }


        private void restartBtn_Click(object sender, EventArgs e)
        {
            NewForm form = new NewForm();
            stopOptimization();
            newOptimization(form.NbTasks, form.RandomTaskSizes, form.NbRobots, true);
        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            NewForm form = new NewForm();
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

        private int m_lastRandomSeed = 11;
        
        private System.Windows.Forms.Timer m_timer = new System.Windows.Forms.Timer();
        private FitnessLevels m_fitnessLevels = new FitnessLevels();
        private Optimizer m_optimizer;


      

        
    }
}