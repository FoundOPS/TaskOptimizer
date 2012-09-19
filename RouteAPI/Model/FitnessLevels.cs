namespace TaskOptimizer.Model
{
    public class FitnessLevels
    {
        private int m_costMultiplier;
        private int m_timeMultiplier;

        public int TimeMultiplier
        {
            get { return m_timeMultiplier; }

            set
            {
                m_timeMultiplier = value;

                if (m_timeMultiplier == 0 && CostMultiplier == 0)
                {
                    m_timeMultiplier = 1;
                    CostMultiplier = 1;
                }
            }
        }

        public int CostMultiplier
        {
            get { return m_costMultiplier; }

            set
            {
                m_costMultiplier = value;

                if (m_costMultiplier == 0 && TimeMultiplier == 0)
                {
                    m_costMultiplier = 1;
                    TimeMultiplier = 1;
                }
            }
        }
    }
}