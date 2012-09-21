namespace TaskOptimizer.Model
{
    public class FitnessLevels
    {
        private int _costMultiplier;
        public int CostMultiplier
        {
            get { return _costMultiplier; }

            set
            {
                _costMultiplier = value;

                if (_costMultiplier == 0 && TimeMultiplier == 0)
                {
                    _costMultiplier = 1;
                    TimeMultiplier = 1;
                }
            }
        }

        private int _timeMultiplier;
        public int TimeMultiplier
        {
            get { return _timeMultiplier; }

            set
            {
                _timeMultiplier = value;

                if (_timeMultiplier == 0 && CostMultiplier == 0)
                {
                    _timeMultiplier = 1;
                    CostMultiplier = 1;
                }
            }
        }
    }
}