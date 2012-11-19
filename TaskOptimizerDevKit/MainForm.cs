using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProblemLib;
using ProblemLib.API;
using Nightingale; 

namespace TaskOptimizerDevKit
{
    public partial class MainForm : Form
    {
        const String LOCATION_LIST_FILE = "locations.csv";

        public MainForm()
        {
            InitializeComponent();

            InitializePreprocessing();
        }

        #region Global Events/Data

        List<Coordinate> locations = new List<Coordinate>();
        List<String> osrmServers = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            // load location list
            if (File.Exists(LOCATION_LIST_FILE))
            {
                String[] lines = File.ReadAllLines(LOCATION_LIST_FILE);
                foreach (String line in lines)
                {
                    String[] split = line.Split(',');
                    Coordinate c = new Coordinate(Double.Parse(split[0]), double.Parse(split[1]));
                    locations.Add(c);
                }
            }
        }

        #endregion

        #region OSRM API Calls

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(cmbOsrmAddress.Text))
            {
                MessageBox.Show("OSRM Server Address cannot be empty!");
                cmbOsrmAddress.Focus();
                return;
            }

            Ping p = new Ping();
            PingReply reply = p.Send(cmbOsrmAddress.Text);
            if (reply.Status == IPStatus.Success)
            {
                MessageBox.Show("Ping Success!");
                if (!osrmServers.Contains(cmbOsrmAddress.Text))
                {
                    osrmServers.Add(cmbOsrmAddress.Text);
                    cmbOsrmAddress.Items.Add(cmbOsrmAddress.Text);
                }
            }
            else
            {
                MessageBox.Show("Ping Failed! Status = " + reply.Status.ToString());
            }
        }

        private void lnlOsrmInsertCoord_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CoordinateDialog dlg = new CoordinateDialog(locations);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lbOsrmArgs.BeginUpdate();
                foreach (Coordinate c in dlg.SelectedItems)
                    lbOsrmArgs.Items.Add(c);
                lbOsrmArgs.EndUpdate();
            }
        }

        private void btnOsrmSubmit_Click(object sender, EventArgs e)
        {
            if (lbOsrmMethod.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a method to call!");
                return;
            }
            
            switch (lbOsrmMethod.SelectedIndex)
            {
                case 0:
                    {
                        if (lbOsrmArgs.Items.Count < 1)
                        {
                            MessageBox.Show("You need at least 1 argument!");
                            return;
                        }

                        Coordinate result = OsrmAPI.FindNearestNode(String.Format("http://{0}:5000/", cmbOsrmAddress.Text), (Coordinate)lbOsrmArgs.Items[0]);

                        txtOsrmResponse.Text = result.ToString();
                    }
                    break;
                case 1:
                    {
                        if (lbOsrmArgs.Items.Count < 1)
                        {
                            MessageBox.Show("You need at least 2 arguments!");
                            return;
                        }

                        Pair<int, int> result = OsrmAPI.GetDistanceTime(String.Format("http://{0}:5000/", cmbOsrmAddress.Text), (Coordinate)lbOsrmArgs.Items[0], (Coordinate)lbOsrmArgs.Items[1]);

                        txtOsrmResponse.Text = String.Format("{{Distance = {0}m, Time = {1}s}}", result.First, result.Second);
                    }
                    break;
                case 2:
                    {
                        MessageBox.Show("Not implemented!");
                    }
                    break;
            }
        }

        private void llnOsrmClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbOsrmArgs.Items.Clear();
        }

        #endregion

        private void btnPpCfgGenId_Click(object sender, EventArgs e)
        {
            Guid id = Guid.NewGuid();
            txtPpCfgId.Text = id.ToString();
        }

        private void btnPpCfgRedisPing_Click(object sender, EventArgs e)
        {
            if (!Utils.IsValidIp(txtPpCfgRedisAddr.Text))
            {
                erp.Clear();
                erp.SetError(txtPpCfgRedisAddr, "Value not a valid IP!");
                return;
            }

            pprocRedisPing.Address = Utils.ToIpAddress(txtPpCfgRedisAddr.Text);
        }

        private void btnPpCfgOsrmPing_Click(object sender, EventArgs e)
        {
            if (!Utils.IsValidIp(txtPpCfgOsrmAddr.Text))
            {
                erp.Clear();
                erp.SetError(txtPpCfgOsrmAddr, "Value not a valid IP!");
                return;
            }

            pprocOsrmPing.Address = Utils.ToIpAddress(txtPpCfgOsrmAddr.Text);
        }

    }
}
