using System;
using System.Collections.Generic;
using System.Text;

namespace TaskOptimizer.Interfaces
{
    public interface WorkProgress
    {
        void onWorkProgress(string description, int percent);
       
        void onWorkEnd();
       
        bool WorkCancelled { get;}
    }
}
