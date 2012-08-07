using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.View
{
    public partial class PercentProgressForm : Form, WorkProgress
    {
        public PercentProgressForm()
        {
            InitializeComponent();

            this.cancellingLabel.Visible = false;
            m_cancelled = false;            
            m_ended = false;
        }

        delegate void WorkProgressDelegate(string description, int percent);
        public void onWorkProgress(string description, int percent)
        {
            if (InvokeRequired)
            {                
                WorkProgressDelegate del = new WorkProgressDelegate(onWorkProgress);
                Invoke(del, new object[] { description, percent });
                return;
            }
            
            if (description.Length > 0)
            {
                this.descriptionLabel.Text = description;
            }

            this.progressBar.Value = percent;
            this.percentLabel.Text = percent.ToString() + "%";
        }

        delegate void NoParamDelegate();
        public void onWorkEnd()
        {

            if (InvokeRequired)
            {
                NoParamDelegate del = new NoParamDelegate(onWorkEnd);
                Invoke(del);
                return;
            }

            m_ended = true;
            if (Visible)
            {
                Close();
            }            
        }

        
        public bool WorkCancelled
        {
            get { return m_cancelled; }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                if (m_ended)
                {
                    Close();
                    return;
                }
            }
            
            base.OnVisibleChanged(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {            
            base.OnFormClosing(e);
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            cancel();
        }

        private void cancel()
        {
            this.cancellingLabel.Visible = true;
            m_cancelled = true;
        }


        private bool m_cancelled;        
        private bool m_ended;        
    }
}