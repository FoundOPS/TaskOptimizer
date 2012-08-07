using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TaskOptimizer.Model
{
    public class Robot
    {
        public int DistanceCost
        {
            get
            {
                return m_distanceCost;
            }

            set
            {
                m_distanceCost = value;
            }
        }

        public int WorkCost
        {
            get
            {
                return m_workCost;
            }

            set
            {
                m_workCost = value;
            }
        }

        public int DistanceTime
        {
            get
            {
                return m_distanceTime;
            }

            set
            {
                m_distanceTime = value;
            }
        }

        public int WorkTime
        {
            get
            {
                return m_workTime;
            }

            set
            {
                m_workTime = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_color;
            }

            set
            {
                m_color = value;
                m_brush = new SolidBrush(m_color);
            }
        }

        public Brush Brush
        {
            get
            {
                return m_brush;
            }
        }

        private int m_workCost, m_distanceCost, m_distanceTime, m_workTime;
        private Color m_color;
        private Brush m_brush;
    }

}
