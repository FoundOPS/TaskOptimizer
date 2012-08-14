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
    
            
        }


        
        
        private int m_minValue = 0, m_maxValue = 100;
        private Point[] m_points = null;
        private int m_valueSpan = 100;
        private Brush m_backBrush = null;
        private Pen m_borderPen, m_neutralPen;
    }
}
