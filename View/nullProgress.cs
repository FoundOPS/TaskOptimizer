using System;
using System.Collections.Generic;
using System.Text;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.View
{
    class nullProgress:WorkProgress
    {

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
    }
}
