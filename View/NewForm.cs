using System;
using System.Windows.Forms;

namespace TaskOptimizer.View
{
    public partial class NewForm : Form
    {
        private static int m_nbTasks = 100;
        private static int m_nbRobots = 15;
        private static bool m_randomTaskSizes;

        public NewForm()
        {
            InitializeComponent();
            CancelButton = cancelBtn;
            AcceptButton = okBtn;

            nbRobotBox.Minimum = 1;
            nbRobotBox.Maximum = 300;
            nbTaskBox.Minimum = 10;
            nbTaskBox.Maximum = 1000;

            NbRobots = m_nbRobots;
            NbTasks = m_nbTasks;
            RandomTaskSizes = m_randomTaskSizes;
        }

        public int NbTasks
        {
            get { return (int) nbTaskBox.Value; }

            set { nbTaskBox.Value = value; }
        }

        public int NbRobots
        {
            get { return (int) nbRobotBox.Value; }

            set { nbRobotBox.Value = value; }
        }

        public bool RandomTaskSizes
        {
            get { return randomTaskSizeBox.Checked; }
            set { randomTaskSizeBox.Checked = value; }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            m_nbTasks = NbTasks;
            m_nbRobots = NbRobots;
            m_randomTaskSizes = RandomTaskSizes;

            Close();
        }
    }
}