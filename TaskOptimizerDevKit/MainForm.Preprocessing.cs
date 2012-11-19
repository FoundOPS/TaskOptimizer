using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nightingale;

namespace TaskOptimizerDevKit
{
/* This part of the main form is used for prototyping, developing
 * and testing preprocessing functions of DistributionServer.
 */

    public partial class MainForm : Form
    {
        WizardStateManager pprocStateMan = new WizardStateManager();
        PingTimer pprocRedisPing;
        PingTimer pprocOsrmPing;

        private void InitializePreprocessing()
        {
            pprocStateMan.AddState(wsPprocConfig);
            pprocStateMan.SetCurrentState("wsPprocConfig");

            pprocOsrmPing = new PingTimer(this, (int i) =>
                {
                    if (i >= 0)
                    {
                        lblPpCfgOPingResult.ForeColor = Color.DarkGreen;
                        lblPpCfgOPingResult.Text = String.Format("Delay: {0}ms", i);
                    }
					else
                    {
                        lblPpCfgOPingResult.ForeColor = Color.DarkRed;
                        lblPpCfgOPingResult.Text = "Failed!";
                    }
                });
            pprocRedisPing = new PingTimer(this, (int i) =>
            {
                if (i >= 0)
                {
                    lblPpCfgRPingResult.ForeColor = Color.DarkGreen;
                    lblPpCfgRPingResult.Text = String.Format("Delay: {0}ms", i);
                }
                else
                {
                    lblPpCfgRPingResult.ForeColor = Color.DarkRed;
                    lblPpCfgRPingResult.Text = "Failed!";
                }
            });
        }


    }
}
