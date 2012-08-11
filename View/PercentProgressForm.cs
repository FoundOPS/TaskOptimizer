using System;
using System.Windows.Forms;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.View
{
    public partial class PercentProgressForm : Form, WorkProgress
    {
        private bool m_cancelled;
        private bool m_ended;

        public PercentProgressForm()
        {
            InitializeComponent();

            cancellingLabel.Visible = false;
            m_cancelled = false;
            m_ended = false;
        }

        #region WorkProgress Members

        public void onWorkProgress(string description, int percent)
        {
            if (InvokeRequired)
            {
                WorkProgressDelegate del = onWorkProgress;
                Invoke(del, new object[] {description, percent});
                return;
            }

            if (description.Length > 0)
            {
                descriptionLabel.Text = description;
            }

            progressBar.Value = percent;
            percentLabel.Text = percent.ToString() + "%";
        }

        public void onWorkEnd()
        {
            if (InvokeRequired)
            {
                NoParamDelegate del = onWorkEnd;
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

        #endregion

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
            cancellingLabel.Visible = true;
            m_cancelled = true;
        }

        #region Nested type: NoParamDelegate

        private delegate void NoParamDelegate();

        #endregion

        #region Nested type: WorkProgressDelegate

        private delegate void WorkProgressDelegate(string description, int percent);

        #endregion
    }
}