using System.Drawing;

namespace TaskOptimizer.Model
{
    public class Robot
    {
        private Brush m_brush;
        private Color m_color;

        public Robot()
        {
            DistanceCost = 1;
            WorkCost = 1;
            DistanceTime = 1;
            WorkTime = 1;
        }

        public int DistanceCost { get; set; }

        public int WorkCost { get; set; }

        public int DistanceTime { get; set; }

        public int WorkTime { get; set; }

        public Color Color
        {
            get { return m_color; }

            set
            {
                m_color = value;
                m_brush = new SolidBrush(m_color);
            }
        }

        public Brush Brush
        {
            get { return m_brush; }
        }
    }
}