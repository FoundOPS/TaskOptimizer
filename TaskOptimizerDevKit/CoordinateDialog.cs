using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProblemLib.API;

namespace TaskOptimizerDevKit
{
    public partial class CoordinateDialog : Form
    {
        public CoordinateDialog(IEnumerable<Coordinate> coords)
        {
            InitializeComponent();

            listBox1.BeginUpdate();
            foreach (Coordinate c in coords)
                listBox1.Items.Add(c);
            listBox1.EndUpdate();
        }

        private void CoordinateDialog_Load(object sender, EventArgs e)
        {

        }

        public IEnumerable<Coordinate> SelectedItems
        {
            get
            {
                List<Coordinate> result = new List<Coordinate>();
                foreach (Object o in listBox1.SelectedItems)
                    result.Add((Coordinate)o);
                return result;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
