using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MT;
using TaskOptimizer.Model;

namespace TaskOptimizer.View
{
    public partial class GraphView : UserControl
    {
        private readonly CurtainForm m_curtain = new CurtainForm();
        private double m_scalingFactor = 1.0;
        private List<TaskSequence> m_taskSequences = new List<TaskSequence>();

        public GraphView()
        {
            InitializeComponent();
        }

        public void SetTaskSequences(List<TaskSequence> taskSequences)
        {
            m_curtain.Show(this);
            m_taskSequences = taskSequences;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            m_scalingFactor = Width/1020.0;

            drawLines(m_taskSequences, e.Graphics);
            drawTasks(m_taskSequences, e.Graphics);

            m_curtain.Fade();
        }

        private void drawLines(List<TaskSequence> taskSequences, Graphics g)
        {
            foreach (TaskSequence sequence in taskSequences)
            {
                drawLines(sequence, g);
            }
        }

        private void drawTasks(List<TaskSequence> taskSequences, Graphics g)
        {
            foreach (TaskSequence sequence in taskSequences)
            {
                drawTasks(sequence, g);
            }
        }

        private void drawLines(TaskSequence sequence, Graphics g)
        {
            if (sequence == null || sequence.Tasks.Count == 0)
            {
                return;
            }

            var pen = new Pen(Color.FromArgb(80, sequence.Robot.Color), 4);
            pen.DashStyle = DashStyle.Dash;
            pen.DashCap = DashCap.Triangle;

            var points = new Point[sequence.Tasks.Count + 2];

            points[0] = new Point((int) (sequence.StartX*m_scalingFactor), (int) (sequence.StartY*m_scalingFactor));

            int t = 1;
            var tasks = new List<Task>(sequence.Tasks);
            foreach (Task task in tasks)
            {
                points[t] = new Point((int) (task.X*m_scalingFactor) + task.Effort/2,
                                      (int) (task.Y*m_scalingFactor) + task.Effort/2);
                t++;
            }

            points[t] = new Point((int) (sequence.StartX*m_scalingFactor), (int) (sequence.StartY*m_scalingFactor));


            g.DrawLines(pen, points);

            pen.Dispose();
        }

        private void drawTasks(TaskSequence sequence, Graphics g)
        {
            if (sequence == null || sequence.Tasks.Count == 0)
            {
                return;
            }

            var shadowPen = new Pen(Color.FromArgb(50, Color.White), 2);
            shadowPen.EndCap = LineCap.Round;

            var borderPen = new Pen(Color.FromArgb(20, Color.Black), 3);

            var tasks = new List<Task>(sequence.Tasks);
            foreach (Task task in tasks)
            {
                var x = (int) (task.X*m_scalingFactor);
                var y = (int) (task.Y*m_scalingFactor);
                g.FillEllipse(sequence.Robot.Brush, x, y, task.Effort, task.Effort);
                g.DrawEllipse(borderPen, x + 2, y + 2, task.Effort - 4, task.Effort - 4);
                g.DrawArc(shadowPen, x + 2, y + 2, task.Effort - 4, task.Effort - 4, 250, 130);
            }

            borderPen.Dispose();
            shadowPen.Dispose();
        }
    }
}