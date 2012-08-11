using TaskOptimizer.Interfaces;

namespace TaskOptimizer.View
{
    internal class nullProgress : WorkProgress
    {
        #region WorkProgress Members

        public void onWorkProgress(string description, int percent)
        {
        }

        public void onWorkEnd()
        {
        }

        public bool WorkCancelled
        {
            get { return false; }
        }

        #endregion
    }
}