using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TaskOptimizer.View
{
    public partial class NewForm : Form
    {
        public NewForm()
        {
            InitializeComponent();
            this.CancelButton = this.cancelBtn;
            this.AcceptButton = this.okBtn;

            this.nbRobotBox.Minimum = 1;
            this.nbRobotBox.Maximum = 300;
            this.nbTaskBox.Minimum = 10;
            this.nbTaskBox.Maximum = 1000;

            NbRobots = m_nbRobots;
            NbTasks = m_nbTasks;
            RandomTaskSizes = m_randomTaskSizes;

        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            m_nbTasks = NbTasks;
            m_nbRobots = NbRobots;
            m_randomTaskSizes = RandomTaskSizes;

            Close();
        }

        public int NbTasks
        {
            get
            {
                return (int)this.nbTaskBox.Value;
            }

            set
            {
                this.nbTaskBox.Value = value;
            }
        }

        public int NbRobots
        {
            get
            {
                return (int)this.nbRobotBox.Value;
            }

            set
            {
                this.nbRobotBox.Value = value;
            }
        }

        public bool RandomTaskSizes
        {
            get
            {
                return this.randomTaskSizeBox.Checked;
            }
            set
            {
                this.randomTaskSizeBox.Checked = value;
            }
        }

        private static int m_nbTasks = 100;
        private static int m_nbRobots = 15;
        private static bool m_randomTaskSizes = false;
    }
}