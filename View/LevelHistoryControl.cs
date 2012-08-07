using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TaskOptimizer.Model;

namespace TaskOptimizer.View
{
    public partial class LevelHistoryControl : UserControl
    {
        public LevelHistoryControl()
        {
            
            InitializeComponent();
            m_backBrush = new SolidBrush(this.BackColor);
            m_borderPen = new Pen(Color.DarkGray, 1);
            m_neutralPen = new Pen(Color.Blue, 1);
            this.DoubleBuffered = true;
            
            setMinMax(0, 100);
        }

      

        public void setLevelHistory(ILevelHistory levelHistory)
        {
            m_levelHistory = levelHistory;               
        }

        public void setMinMax(int min, int max)
        {
            m_minValue = min;
            m_maxValue = max;
            m_valueSpan = max - min;
        }

        

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            if (m_backBrush != null)
            {
                m_backBrush.Dispose();
            }
            m_backBrush = new SolidBrush(this.BackColor);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_levelHistory == null)
            {
                return;
            }

            setMinMax((int)m_levelHistory.Minimum, (int)m_levelHistory.Maximum);

            double[] history = m_levelHistory.History;
            int nbPoints = Math.Min(this.Width, history.Length);

            if (m_points == null || m_points.Length < nbPoints)
            {
                m_points = new Point[nbPoints];
                for (int t = 0; t < nbPoints; t++)
                {
                    m_points[t] = new Point(t, 0);
                }
            }
           
            e.Graphics.FillRectangle(m_backBrush, 0, 0, this.Width, this.Height);
             

            int firstIndex = m_levelHistory.HistoryFirstIndex + history.Length - 1 - this.Width;
            

            int count = 0;
            int displayHeight = Height - 2;

            for (int t = firstIndex; count < nbPoints; t++, count++)
            {
                t = t % history.Length;
                double value = history[t];
                if (value < m_minValue)
                {                    
                    value = m_minValue;
                }
                else if (value > m_maxValue)
                {
                    value = m_maxValue;
                }
                

                int height = (int)((value - m_minValue) / (double)m_valueSpan * (displayHeight));
                m_points[count].Y = displayHeight - height;
                
                
            }

            //e.Graphics.DrawLine(m_neutralPen, 0.0f, Height / 2.0f, (float)nbPoints, Height / 2.0f);
            e.Graphics.DrawLines(m_borderPen, m_points);
         
            
        }


        
        ILevelHistory m_levelHistory = null;
        
        private int m_minValue = 0, m_maxValue = 100;
        private Point[] m_points = null;
        private int m_valueSpan = 100;
        private Brush m_backBrush = null;
        private Pen m_borderPen, m_neutralPen;
    }
}
